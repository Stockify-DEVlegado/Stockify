package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.gestion.OrdenCompraBO;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenCompraBOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "OrdenCompraWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class OrdenCompraWS {
    private final OrdenCompraBO ordenCompraBO;
    
    public OrdenCompraWS() {
        this.ordenCompraBO = new OrdenCompraBOImpl();
    }
    
    @WebMethod(operationName = "listarOrdenesCompra")
    public List<OrdenCompra> listarOrdenesCompra() {
        return this.ordenCompraBO.listar();
    }
    
    @WebMethod(operationName = "obtenerOrdenCompra")
    public OrdenCompra obtenerOrdenCompra(
        @WebParam(name = "id") int id
    ) {
        return this.ordenCompraBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarOrdenCompra")
    public void eliminarOrdenCompra(
        @WebParam(name = "id") int id
    ) {
        this.ordenCompraBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarOrdenCompra")
    public void guardarOrdenCompra(
        @WebParam(name = "ordenCompra") OrdenCompra ordenCompra, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.ordenCompraBO.guardar(ordenCompra, estado);
    }
    
    @WebMethod(operationName = "obtenerProductosPorRecibir")
    public int obtenerProductosPorRecibir() {
        try {
            return this.ordenCompraBO.obtenerProductosPorRecibir();
        } catch (Exception e) {
            System.err.println("Error en WS obtenerProductosPorRecibir: " + e.getMessage());
            return 0;
        }
    }
}