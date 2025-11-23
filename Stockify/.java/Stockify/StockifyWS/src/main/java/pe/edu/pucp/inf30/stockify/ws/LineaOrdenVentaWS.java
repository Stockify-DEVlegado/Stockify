package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.gestion.LineaOrdenVentaBO;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.LineaOrdenVentaBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;

/**
 * Web Service para gestión de Líneas de Orden de Venta
 */
@WebService(serviceName = "LineaOrdenVentaWS",
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class LineaOrdenVentaWS {
    
    private final LineaOrdenVentaBO lineaOrdenVentaBO;
    
    public LineaOrdenVentaWS() {
        this.lineaOrdenVentaBO = new LineaOrdenVentaBOImpl();
    }
    
    @WebMethod(operationName = "importarLineasVentaDesdeCSV")
    public int importarLineasVentasDesdeCSV(
        @WebParam(name = "archivoCSV") byte[] archivoCSV,
        @WebParam(name = "idOrdenVenta") int idOrdenVenta
    ) {
        try {
            java.io.ByteArrayInputStream inputStream = 
                new java.io.ByteArrayInputStream(archivoCSV);
            
            int lineasImportadas = this.lineaOrdenVentaBO.importarDesdeInputStream(
                inputStream, 
                "lineas_orden_" + idOrdenVenta + ".csv",
                idOrdenVenta
            );
            
            inputStream.close();
            
            System.out.println("✓ Web Service: Se importaron " + lineasImportadas + 
                             " líneas para la orden de venta " + idOrdenVenta);
            return lineasImportadas;
            
        } catch (Exception e) {
            System.err.println("✗ Error en Web Service al importar líneas de venta: " + e.getMessage());
            e.printStackTrace();
            throw new RuntimeException("Error al importar líneas de venta: " + e.getMessage());
        }
    }
    
    @WebMethod(operationName = "listarLineasVentaPorOrden")
    public List<LineaOrdenVenta> listarLineasVentaPorOrden(
        @WebParam(name = "idOrdenVenta") int idOrdenVenta
    ) {
        return this.lineaOrdenVentaBO.listarPorOrden(idOrdenVenta);
    }
    
    @WebMethod(operationName = "eliminarLineaVenta")
    public void eliminarLineaVenta(@WebParam(name = "idLinea") int idLinea) {
        this.lineaOrdenVentaBO.eliminar(idLinea);
    }
    
    @WebMethod(operationName = "guardarLineaVenta")
    public void guardarLineaVenta(
        @WebParam(name = "linea") LineaOrdenVenta linea,
        @WebParam(name = "idOrdenVenta") int idOrdenVenta
    ) {
        this.lineaOrdenVentaBO.guardar(linea, idOrdenVenta);
    }
}