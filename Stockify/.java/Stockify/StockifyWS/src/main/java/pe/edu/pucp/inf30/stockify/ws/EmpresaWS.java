package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.usuario.EmpresaBO;
import pe.edu.pucp.inf30.stockify.boimpl.usuario.EmpresaBOImpl;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "EmpresaWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class EmpresaWS {
    private final EmpresaBO empresaBO;
    
    public EmpresaWS() {
        this.empresaBO = new EmpresaBOImpl();
    }
    
    @WebMethod(operationName = "listarEmpresas")
    public List<Empresa> listarEmpresas() {
        return this.empresaBO.listar();
    }
    
    @WebMethod(operationName = "obtenerEmpresa")
    public Empresa obtenerEmpresa(
        @WebParam(name = "id") int id
    ) {
        return this.empresaBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarEmpresa")
    public void eliminarEmpresa(
        @WebParam(name = "id") int id
    ) {
        this.empresaBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarEmpresa")
    public void guardarEmpresa(
        @WebParam(name = "empresa") Empresa empresa, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.empresaBO.guardar(empresa, estado);
    }
}