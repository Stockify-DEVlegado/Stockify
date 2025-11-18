/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.almacen.ProductoBO;
import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;

/**
 *
 * @author DEVlegado
 */
public class ProductoBOImpl implements ProductoBO{
    private final ProductoDAO productoDao;
    
    public ProductoBOImpl() {
        this.productoDao = new ProductoDAOImpl();
    }
    
    @Override
    public List<Producto> listar() {
        return this.productoDao.leerTodos();
    }
    
    @Override
    public Producto obtener(int id) {
        return this.productoDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.productoDao.eliminar(id);
    }
    
    @Override
    public void guardar(Producto modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.productoDao.crear(modelo);
        }
        else {
            this.productoDao.actualizar(modelo);
        }
    }
    
    @Override
    public List<Producto> listarOrdenadoPorCodigo() {
        return this.productoDao.leerTodosOrdenados("codigo");
    }
    
    @Override
    public List<Producto> listarOrdenadoPorNombre() {
        return this.productoDao.leerTodosOrdenados("nombre");
    }
    
    @Override
    public List<Producto> listarProductosPorCategoria(int idCategoria) {
        return this.productoDao.leerTodosPorCategoria(idCategoria);
    }
    
    @Override
    public int importarDesdeInputStream(InputStream inputStream, String nombreArchivo) {
        List<Producto> productos = leerProductosDesdeInputStream(inputStream);
        
        if (productos.isEmpty()) {
            System.out.println("No se encontraron productos para importar.");
            return 0;
        }
        
        System.out.println("Procesando archivo: " + nombreArchivo);
        return this.productoDao.insertarBloque(productos);
    }
    
    private List<Producto> leerProductosDesdeInputStream(InputStream inputStream) {
        List<Producto> productos = new ArrayList<>();
        
        try (BufferedReader br = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"))) {
            String linea;
            boolean primeraLinea = true;
            int numeroLinea = 0;
            
            while ((linea = br.readLine()) != null) {
                numeroLinea++;
                
                // Saltar encabezado
                if (primeraLinea) {
                    primeraLinea = false;
                    continue;
                }
                
                // Saltar líneas vacías
                if (linea.trim().isEmpty()) {
                    continue;
                }
                
                // Parsear CSV considerando valores con comas dentro de comillas
                String[] valores = parsearLineaCSV(linea);
                
                if (valores.length < 7) {
                    System.err.println("Línea " + numeroLinea + " con formato incorrecto (se esperan 7 columnas): " + linea);
                    continue;
                }
                
                try {
                    Producto producto = crearProductoDesdeValores(valores, numeroLinea);
                    productos.add(producto);
                } catch (NumberFormatException e) {
                    System.err.println("Error en línea " + numeroLinea + ": " + e.getMessage());
                }
            }
            
            System.out.println("Se leyeron " + productos.size() + " productos del archivo CSV.");
            
        } catch (IOException e) {
            System.err.println("Error al leer el InputStream: " + e.getMessage());
            throw new RuntimeException("Error al leer archivo CSV desde InputStream", e);
        }
        
        return productos;
    }
    
    private Producto crearProductoDesdeValores(String[] valores, int numeroLinea) {
        Producto producto = new Producto();
        producto.setNombre(valores[0].trim());
        producto.setDescripcion(valores[1].trim());
        producto.setMarca(valores[2].trim());
        
        try {
            producto.setStockMinimo(Integer.parseInt(valores[3].trim()));
        } catch (NumberFormatException e) {
            throw new NumberFormatException("Stock mínimo inválido: " + valores[3]);
        }
        
        try {
            producto.setStockMaximo(Integer.parseInt(valores[4].trim()));
        } catch (NumberFormatException e) {
            throw new NumberFormatException("Stock máximo inválido: " + valores[4]);
        }
        
        try {
            producto.setPrecioUnitario(Double.parseDouble(valores[5].trim()));
        } catch (NumberFormatException e) {
            throw new NumberFormatException("Precio unitario inválido: " + valores[5]);
        }
        
        // Si idCategoria no está vacío, crear la categoría
        if (!valores[6].trim().isEmpty()) {
            try {
                Categoria categoria = new Categoria();
                categoria.setIdCategoria(Integer.parseInt(valores[6].trim()));
                producto.setCategoria(categoria);
            } catch (NumberFormatException e) {
                throw new NumberFormatException("ID de categoría inválido: " + valores[6]);
            }
        }
        
        return producto;
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
    
}