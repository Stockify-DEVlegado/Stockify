package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.personal.CuentaUsuarioBO;
import pe.edu.pucp.inf30.stockify.boimpl.personal.CuentaUsuarioBOImpl;
import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "CuentaUsuarioWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class CuentaUsuarioWS {
    private final CuentaUsuarioBO cuentaUsuarioBO;
    
    public CuentaUsuarioWS() {
        this.cuentaUsuarioBO = new CuentaUsuarioBOImpl();
    }
    
    @WebMethod(operationName = "listarCuentasUsuario")
    public List<CuentaUsuario> listarCuentasUsuario() {
        return this.cuentaUsuarioBO.listar();
    }
    
    @WebMethod(operationName = "obtenerCuentaUsuario")
    public CuentaUsuario obtenerCuentaUsuario(
        @WebParam(name = "id") int id
    ) {
        return this.cuentaUsuarioBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarCuentaUsuario")
    public void eliminarCuentaUsuario(
        @WebParam(name = "id") int id
    ) {
        this.cuentaUsuarioBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarCuentaUsuario")
    public void guardarCuentaUsuario(
        @WebParam(name = "cuentaUsuario") CuentaUsuario cuentaUsuario, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.cuentaUsuarioBO.guardar(cuentaUsuario, estado);
    }
}