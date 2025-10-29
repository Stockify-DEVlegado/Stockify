package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.almacen.MovimientoBO;
import pe.edu.pucp.inf30.stockify.boimpl.almacen.MovimientoBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento;

/**
 *
 * @author DEVlegado
 */
@WebService(serviceName = "MovimientoWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class MovimientoWS {
    private final MovimientoBO movimientoBO;
    
    public MovimientoWS() {
        this.movimientoBO = new MovimientoBOImpl();
    }
    
    @WebMethod(operationName = "listarMovimientos")
    public List<Movimiento> listarMovimientos() {
        return this.movimientoBO.listar();
    }
    
    @WebMethod(operationName = "obtenerMovimiento")
    public Movimiento obtenerMovimiento(
        @WebParam(name = "id") int id
    ) {
        return this.movimientoBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarMovimiento")
    public void eliminarMovimiento(
        @WebParam(name = "id") int id
    ) {
        this.movimientoBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarMovimiento")
    public void guardarMovimiento(
        @WebParam(name = "movimiento") Movimiento movimiento, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.movimientoBO.guardar(movimiento, estado);
    }
}