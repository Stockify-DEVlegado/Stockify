package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.gestion.LineaOrdenCompraBO;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.LineaOrdenCompraBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;

/**
 * Web Service para gestión de Líneas de Orden de Compra
 */
@WebService(serviceName = "LineaOrdenCompraWS",
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class LineaOrdenCompraWS {
    
    private final LineaOrdenCompraBO lineaOrdenCompraBO;
    
    public LineaOrdenCompraWS() {
        this.lineaOrdenCompraBO = new LineaOrdenCompraBOImpl();
    }
    
    @WebMethod(operationName = "importarLineasCompraDesdeCSV")
    public int importarLineasCompraDesdeCSV(
        @WebParam(name = "archivoCSV") byte[] archivoCSV,
        @WebParam(name = "idOrdenCompra") int idOrdenCompra
    ) {
        try {
            java.io.ByteArrayInputStream inputStream = 
                new java.io.ByteArrayInputStream(archivoCSV);
            
            int lineasImportadas = this.lineaOrdenCompraBO.importarDesdeInputStream(
                inputStream, 
                "lineas_orden_" + idOrdenCompra + ".csv",
                idOrdenCompra  // ← Parámetro adicional para asociar a orden específica
            );
            
            inputStream.close();
            
            System.out.println("✓ Web Service: Se importaron " + lineasImportadas + 
                             " líneas para la orden " + idOrdenCompra);
            return lineasImportadas;
            
        } catch (Exception e) {
            System.err.println("✗ Error en Web Service al importar líneas: " + e.getMessage());
            e.printStackTrace();
            throw new RuntimeException("Error al importar líneas: " + e.getMessage());
        }
    }
    
    @WebMethod(operationName = "listarLineasCompraPorOrden")
    public List<LineaOrdenCompra> listarLineasCompraPorOrden(
        @WebParam(name = "idOrdenCompra") int idOrdenCompra
    ) {
        return this.lineaOrdenCompraBO.listarPorOrden(idOrdenCompra);
    }
    
    @WebMethod(operationName = "eliminarLineaCompra")
    public void eliminarLineaCompra(@WebParam(name = "idLinea") int idLinea) {
        this.lineaOrdenCompraBO.eliminar(idLinea);
    }
    
    @WebMethod(operationName = "guardarLineaCompra")
    public void guardarLineaCompra(
        @WebParam(name = "linea") LineaOrdenCompra linea,
        @WebParam(name = "idOrdenCompra") int idOrdenCompra
    ) {
        // Asociar la línea a la orden antes de guardar
        // (esto se haría en el BO)
        this.lineaOrdenCompraBO.guardar(linea, idOrdenCompra);
    }
}