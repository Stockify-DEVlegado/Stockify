package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.almacen.ExistenciasBO;
import pe.edu.pucp.inf30.stockify.boimpl.almacen.ExistenciasBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;
import pe.edu.pucp.inf30.stockify.model.dto.AlertaStockDTO;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "ExistenciasWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class ExistenciasWS {
    private final ExistenciasBO existenciasBO;
    
    public ExistenciasWS() {
        this.existenciasBO = new ExistenciasBOImpl();
    }
    
    @WebMethod(operationName = "listarExistencias")
    public List<Existencias> listarExistencias() {
        return this.existenciasBO.listar();
    }
    
    @WebMethod(operationName = "obtenerExistencias")
    public Existencias obtenerExistencias(
        @WebParam(name = "id") int id
    ) {
        return this.existenciasBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarExistencias")
    public void eliminarExistencias(
        @WebParam(name = "id") int id
    ) {
        this.existenciasBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarExistencias")
    public void guardarExistencias(
        @WebParam(name = "existencias") Existencias existencias, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.existenciasBO.guardar(existencias, estado);
    }
    
    @WebMethod(operationName = "obtenerStockTotal")
    public int obtenerStockTotal() {
        try {
            return this.existenciasBO.obtenerStockTotal();
        } catch (Exception e) {
            System.err.println("Error en WS obtenerStockTotal: " + e.getMessage());
            return 0;
        }
    }

    @WebMethod(operationName = "obtenerAlertasStock")
    public List<AlertaStockDTO> obtenerAlertasStock() {
        try {
            return this.existenciasBO.obtenerProductosStockBajo();
        } catch (Exception e) {
            System.err.println("Error en WS obtenerAlertasStock: " + e.getMessage());
            return new ArrayList<>();
        }
    }
}