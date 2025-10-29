package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenSalidaBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenSalida;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "OrdenSalidaWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class OrdenSalidaWS {
    private final Gestionable<OrdenSalida> ordenSalidaBO;
    
    public OrdenSalidaWS() {
        this.ordenSalidaBO = new OrdenSalidaBOImpl();
    }
    
    @WebMethod(operationName = "listarOrdenesSalida")
    public List<OrdenSalida> listarOrdenesSalida() {
        return this.ordenSalidaBO.listar();
    }
    
    @WebMethod(operationName = "obtenerOrdenSalida")
    public OrdenSalida obtenerOrdenSalida(
        @WebParam(name = "id") int id
    ) {
        return this.ordenSalidaBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarOrdenSalida")
    public void eliminarOrdenSalida(
        @WebParam(name = "id") int id
    ) {
        this.ordenSalidaBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarOrdenSalida")
    public void guardarOrdenSalida(
        @WebParam(name = "ordenSalida") OrdenSalida ordenSalida, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.ordenSalidaBO.guardar(ordenSalida, estado);
    }
}