package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenVentaBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "OrdenVentaWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class OrdenVentaWS {
    private final Gestionable<OrdenVenta> ordenVentaBO;
    
    public OrdenVentaWS() {
        this.ordenVentaBO = new OrdenVentaBOImpl();
    }
    
    @WebMethod(operationName = "listarOrdenesVenta")
    public List<OrdenVenta> listarOrdenesVenta() {
        return this.ordenVentaBO.listar();
    }
    
    @WebMethod(operationName = "obtenerOrdenVenta")
    public OrdenVenta obtenerOrdenVenta(
        @WebParam(name = "id") int id
    ) {
        return this.ordenVentaBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarOrdenVenta")
    public void eliminarOrdenVenta(
        @WebParam(name = "id") int id
    ) {
        this.ordenVentaBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarOrdenVenta")
    public void guardarOrdenVenta(
        @WebParam(name = "ordenVenta") OrdenVenta ordenVenta, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.ordenVentaBO.guardar(ordenVenta, estado);
    }
}