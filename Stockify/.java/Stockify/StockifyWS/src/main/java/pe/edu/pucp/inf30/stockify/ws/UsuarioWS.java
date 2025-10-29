package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.personal.UsuarioBO;
import pe.edu.pucp.inf30.stockify.boimpl.personal.UsuarioBOImpl;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "UsuarioWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class UsuarioWS {
    private final UsuarioBO usuarioBO;
    
    public UsuarioWS() {
        this.usuarioBO = new UsuarioBOImpl();
    }
    
    @WebMethod(operationName = "listarUsuarios")
    public List<Usuario> listarUsuarios() {
        return this.usuarioBO.listar();
    }
    
    @WebMethod(operationName = "obtenerUsuario")
    public Usuario obtenerUsuario(
        @WebParam(name = "id") int id
    ) {
        return this.usuarioBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarUsuario")
    public void eliminarUsuario(
        @WebParam(name = "id") int id
    ) {
        this.usuarioBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarUsuario")
    public void guardarUsuario(
        @WebParam(name = "usuario") Usuario usuario, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.usuarioBO.guardar(usuario, estado);
    }
}