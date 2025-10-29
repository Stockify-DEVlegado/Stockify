package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.boimpl.almacen.GuiaRemisionBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.GuiaRemision;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "GuiaRemisionWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class GuiaRemisionWS {
    private final Gestionable<GuiaRemision> guiaRemisionBO;
    
    public GuiaRemisionWS() {
        this.guiaRemisionBO = new GuiaRemisionBOImpl();
    }
    
    @WebMethod(operationName = "listarGuiasRemision")
    public List<GuiaRemision> listarGuiasRemision() {
        return this.guiaRemisionBO.listar();
    }
    
    @WebMethod(operationName = "obtenerGuiaRemision")
    public GuiaRemision obtenerGuiaRemision(
        @WebParam(name = "id") int id
    ) {
        return this.guiaRemisionBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarGuiaRemision")
    public void eliminarGuiaRemision(
        @WebParam(name = "id") int id
    ) {
        this.guiaRemisionBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarGuiaRemision")
    public void guardarGuiaRemision(
        @WebParam(name = "guiaRemision") GuiaRemision guiaRemision, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.guiaRemisionBO.guardar(guiaRemision, estado);
    }
}