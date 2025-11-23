package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.gestion.LineaOrdenVentaBO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenVentaDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenVentaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

public class LineaOrdenVentaBOImpl implements LineaOrdenVentaBO {
    
    private final LineaOrdenVentaDAO lineaOrdenVentaDao;
    private final OrdenVentaDAO ordenVentaDao;
    
    public LineaOrdenVentaBOImpl() {
        this.lineaOrdenVentaDao = new LineaOrdenVentaDAOImpl();
        this.ordenVentaDao = new OrdenVentaDAOImpl();
    }
    
    @Override
    public int importarDesdeInputStream(InputStream inputStream, String nombreArchivo, int idOrdenVenta) {
        // 1. Verificar que la orden existe
        OrdenVenta orden = ordenVentaDao.leer(idOrdenVenta);
        if (orden == null) {
            throw new RuntimeException("La orden de venta con ID " + idOrdenVenta + " no existe");
        }
        
        // 2. Leer líneas del CSV
        List<LineaOrdenVenta> lineas = leerLineasDesdeInputStream(inputStream, idOrdenVenta);
        
        if (lineas.isEmpty()) {
            System.out.println("No se encontraron líneas para importar.");
            return 0;
        }
        
        System.out.println("Procesando archivo: " + nombreArchivo + " para orden: " + idOrdenVenta);
        return this.lineaOrdenVentaDao.insertarBloque(lineas, idOrdenVenta);
    }
    
    private List<LineaOrdenVenta> leerLineasDesdeInputStream(InputStream inputStream, int idOrdenVenta) {
        List<LineaOrdenVenta> lineas = new ArrayList<>();
        
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
                    LineaOrdenVenta lineaOrden = crearLineaDesdeValores(valores, numeroLinea, idOrdenVenta);
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
    
    private LineaOrdenVenta crearLineaDesdeValores(String[] valores, int numeroLinea, int idOrdenVenta) {
        LineaOrdenVenta linea = new LineaOrdenVenta();
        
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
        
        // Asociar a la orden de venta
        OrdenVenta orden = new OrdenVenta();
        orden.setIdOrdenVenta(idOrdenVenta);
        linea.setOrdenVenta(orden);
        
        return linea;
    }
    
    private String[] parsearLineaCSV(String linea) {
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
    public List<LineaOrdenVenta> listarPorOrden(int idOrdenVenta) {
        return this.lineaOrdenVentaDao.leerTodosPorOrden(idOrdenVenta);
    }
    
    @Override
    public void eliminar(int idLinea) {
        this.lineaOrdenVentaDao.eliminar(idLinea);
    }
    
    @Override
    public void guardar(LineaOrdenVenta linea, int idOrdenVenta) {
        // Asociar la línea a la orden
        OrdenVenta orden = new OrdenVenta();
        orden.setIdOrdenVenta(idOrdenVenta);
        linea.setOrdenVenta(orden);
        
        // Guardar (si es nuevo id = 0, si es modificación id > 0)
        if (linea.getIdLineaOrdenVenta() == 0) {
            this.lineaOrdenVentaDao.crear(linea);
        } else {
            this.lineaOrdenVentaDao.actualizar(linea);
        }
    }
}