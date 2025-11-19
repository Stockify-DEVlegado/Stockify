package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.almacen.ProductoBO;
import pe.edu.pucp.inf30.stockify.boimpl.almacen.ProductoBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "ProductoWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class ProductoWS {
    private final ProductoBO productoBO;
    
    public ProductoWS() {
        this.productoBO = new ProductoBOImpl();
    }
    
    @WebMethod(operationName = "listarProductos")
    public List<Producto> listarProductos() {
        return this.productoBO.listar();
    }
    
    @WebMethod(operationName = "obtenerProducto")
    public Producto obtenerProducto(
        @WebParam(name = "id") int id
    ) {
        return this.productoBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarProducto")
    public void eliminarProducto(
        @WebParam(name = "id") int id
    ) {
        this.productoBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarProducto")
    public void guardarProducto(
        @WebParam(name = "producto") Producto producto, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.productoBO.guardar(producto, estado);
    }
    
    @WebMethod(operationName = "listarProductosOrdenadoPorCodigo")
    public List<Producto> listarProductosOrdenadoPorCodigo() {
        return this.productoBO.listarOrdenadoPorCodigo();
    }
    
    @WebMethod(operationName = "listarProductosOrdenadoPorNombre")
    public List<Producto> listarProductosOrdenadoPorNombre() {
        return this.productoBO.listarOrdenadoPorNombre();
    }
    
    @WebMethod(operationName = "listarProductosPorCategoria")
    public List<Producto> listarProductosPorCategoria(
        @WebParam(name = "idCategoria") int idCategoria
    ) {
        return this.productoBO.listarProductosPorCategoria(idCategoria);
    }
    
    /**
     * Importa productos desde un archivo CSV enviado como byte[]
     * @param archivoCSV El archivo CSV con los datos de los productos como byte[]
     * @return Número de productos importados exitosamente
     */
    @WebMethod(operationName = "importarProductosDesdeCSV")
    public int importarProductosDesdeCSV(
        @WebParam(name = "archivoCSV") byte[] archivoCSV
    ) {
        try {
            // Convertir byte[] a InputStream
            java.io.ByteArrayInputStream inputStream = new java.io.ByteArrayInputStream(archivoCSV);
            
            // Llamar al método del BO que procesa el InputStream
            int productosImportados = this.productoBO.importarDesdeInputStream(
                inputStream, 
                "productos_importados.csv"
            );
            
            inputStream.close();
            
            System.out.println("✓ Web Service: Se importaron " + productosImportados + " productos.");
            return productosImportados;
            
        } catch (Exception e) {
            System.err.println("✗ Error en Web Service al importar productos: " + e.getMessage());
            e.printStackTrace();
            throw new RuntimeException("Error al importar productos: " + e.getMessage());
        }
    }
    
    @WebMethod(operationName = "contarProductos")
    public int contarProductos() {
        try {
            return this.productoBO.contarTotal();
        } catch (Exception e) {
            System.err.println("Error en WS contarProductos: " + e.getMessage());
            return 0;
        }
    }
    
    @WebMethod(operationName = "obtenerStockActual")
    public int obtenerStockActual(
        @WebParam(name = "idProducto") int idProducto
    ) {
        try {
            return this.productoBO.obtenerStockActual(idProducto);
        } catch (Exception e) {
            System.err.println("Error en WS obtenerStockActual: " + e.getMessage());
            return 0;
        }
    }
}