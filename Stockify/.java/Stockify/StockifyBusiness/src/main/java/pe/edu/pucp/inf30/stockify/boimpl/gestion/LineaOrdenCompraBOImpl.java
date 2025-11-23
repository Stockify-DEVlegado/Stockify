package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.sql.Connection;
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.gestion.LineaOrdenCompraBO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenCompraDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenCompraDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

public class LineaOrdenCompraBOImpl implements LineaOrdenCompraBO {
    
    private final LineaOrdenCompraDAO lineaOrdenCompraDao;
    private final OrdenCompraDAO ordenCompraDao;
    
    public LineaOrdenCompraBOImpl() {
        this.lineaOrdenCompraDao = new LineaOrdenCompraDAOImpl();
        this.ordenCompraDao = new OrdenCompraDAOImpl();
    }
    
    @Override
    public int importarDesdeInputStream(InputStream inputStream, String nombreArchivo, int idOrdenCompra) {
        // 1. Verificar que la orden existe
        OrdenCompra orden = ordenCompraDao.leer(idOrdenCompra);
        if (orden == null) {
            throw new RuntimeException("La orden de compra con ID " + idOrdenCompra + " no existe");
        }
        
        // 2. Leer líneas del CSV
        List<LineaOrdenCompra> lineas = leerLineasDesdeInputStream(inputStream, idOrdenCompra);
        
        if (lineas.isEmpty()) {
            System.out.println("No se encontraron líneas para importar.");
            return 0;
        }
        
        System.out.println("Procesando archivo: " + nombreArchivo + " para orden: " + idOrdenCompra);
        return this.lineaOrdenCompraDao.insertarBloque(lineas, idOrdenCompra);
    }
    
    private List<LineaOrdenCompra> leerLineasDesdeInputStream(InputStream inputStream, int idOrdenCompra) {
        List<LineaOrdenCompra> lineas = new ArrayList<>();
        
        try (BufferedReader br = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"))) {
            String linea;
            int numeroLinea = 0;
            boolean primeraLinea = true;
            
            while ((linea = br.readLine()) != null) {
                numeroLinea++;
                
                // Saltar líneas vacías
                if (linea.trim().isEmpty()) continue;
                
                // Saltar encabezado (primera línea)
                if (primeraLinea) {
                    primeraLinea = false;
                    continue;
                }
                
                // Parsear CSV
                String[] valores = parsearLineaCSV(linea);
                
                if (valores.length < 3) { // idProducto, cantidad, precioUnitario
                    System.err.println("Línea " + numeroLinea + " ignorada: formato incorrecto");
                    continue;
                }
                
                try {
                    LineaOrdenCompra lineaOrden = crearLineaDesdeValores(valores, numeroLinea, idOrdenCompra);
                    lineas.add(lineaOrden);
                    
                } catch (NumberFormatException ex) {
                    System.err.println("Error en línea " + numeroLinea + ": " + ex.getMessage());
                }
            }
            
            System.out.println("Se leyeron " + lineas.size() + " líneas del archivo CSV.");
            
        } catch (IOException e) {
            System.err.println("Error al leer el InputStream: " + e.getMessage());
            throw new RuntimeException("Error al leer archivo CSV desde InputStream", e);
        }
        
        return lineas;
    }
    
    private LineaOrdenCompra crearLineaDesdeValores(String[] valores, int numeroLinea, int idOrdenCompra) {
        LineaOrdenCompra linea = new LineaOrdenCompra();
        
        // Producto
        try {
            Producto producto = new Producto();
            producto.setIdProducto(Integer.parseInt(valores[0].trim()));
            linea.setProducto(producto);
        } catch (NumberFormatException e) {
            throw new NumberFormatException("ID de producto inválido: " + valores[0]);
        }
        
        // Cantidad
        try {
            int cantidad = Integer.parseInt(valores[1].trim());
            if (cantidad <= 0) {
                throw new NumberFormatException("La cantidad debe ser mayor a 0");
            }
            linea.setCantidad(cantidad);
        } catch (NumberFormatException e) {
            throw new NumberFormatException("Cantidad inválida: " + valores[1]);
        }
        
        // Precio Unitario (para calcular subtotal)
        double precioUnitario = 0;
        try {
            precioUnitario = Double.parseDouble(valores[2].trim());
            if (precioUnitario < 0) {
                throw new NumberFormatException("El precio unitario no puede ser negativo");
            }
        } catch (NumberFormatException e) {
            throw new NumberFormatException("Precio unitario inválido: " + valores[2]);
        }
        
        // Calcular subtotal
        double subtotal = linea.getCantidad() * precioUnitario;
        linea.setSubtotal(subtotal);
        
        // Asociar a la orden de compra
        OrdenCompra orden = new OrdenCompra();
        orden.setIdOrdenCompra(idOrdenCompra);
        linea.setOrdenCompra(orden);
        
        return linea;
    }
    
    private String[] parsearLineaCSV(String linea) {
        // Mismo método que en OrdenCompraBOImpl
        List<String> valores = new ArrayList<>();
        StringBuilder valorActual = new StringBuilder();
        boolean dentroDeComillas = false;
        
        for (int i = 0; i < linea.length(); i++) {
            char c = linea.charAt(i);
            
            if (c == '"') {
                dentroDeComillas = !dentroDeComillas;
            } else if (c == ',' && !dentroDeComillas) {
                valores.add(valorActual.toString());
                valorActual = new StringBuilder();
            } else {
                valorActual.append(c);
            }
        }
        
        valores.add(valorActual.toString());
        return valores.toArray(new String[0]);
    }
    
    @Override
    public List<LineaOrdenCompra> listarPorOrden(int idOrdenCompra) {
        return this.lineaOrdenCompraDao.leerTodosPorOrden(idOrdenCompra);
    }
    
    @Override
    public void eliminar(int idLinea) {
        this.lineaOrdenCompraDao.eliminar(idLinea);
    }
    
    @Override
    public void guardar(LineaOrdenCompra linea, int idOrdenCompra) {
        // Asociar la línea a la orden
        OrdenCompra orden = new OrdenCompra();
        orden.setIdOrdenCompra(idOrdenCompra);
        linea.setOrdenCompra(orden);
        
        // Guardar (si es nuevo id = 0, si es modificación id > 0)
        if (linea.getIdLineaOrdenCompra() == 0) {
            this.lineaOrdenCompraDao.crear(linea);
        } else {
            this.lineaOrdenCompraDao.actualizar(linea);
        }
    }
}