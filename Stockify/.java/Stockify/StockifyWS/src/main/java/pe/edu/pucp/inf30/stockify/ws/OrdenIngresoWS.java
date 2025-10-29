package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenIngresoBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenIngreso;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "OrdenIngresoWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class OrdenIngresoWS {
    private final Gestionable<OrdenIngreso> ordenIngresoBO;
    
    public OrdenIngresoWS() {
        this.ordenIngresoBO = new OrdenIngresoBOImpl();
    }
    
    @WebMethod(operationName = "listarOrdenesIngreso")
    public List<OrdenIngreso> listarOrdenesIngreso() {
        return this.ordenIngresoBO.listar();
    }
    
    @WebMethod(operationName = "obtenerOrdenIngreso")
    public OrdenIngreso obtenerOrdenIngreso(
        @WebParam(name = "id") int id
    ) {
        return this.ordenIngresoBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarOrdenIngreso")
    public void eliminarOrdenIngreso(
        @WebParam(name = "id") int id
    ) {
        this.ordenIngresoBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarOrdenIngreso")
    public void guardarOrdenIngreso(
        @WebParam(name = "ordenIngreso") OrdenIngreso ordenIngreso, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.ordenIngresoBO.guardar(ordenIngreso, estado);
    }
}