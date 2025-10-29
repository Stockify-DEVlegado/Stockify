package pe.edu.pucp.inf30.stockify.ws;

import jakarta.jws.WebService;
import jakarta.jws.WebMethod;
import jakarta.jws.WebParam;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.bo.almacen.CategoriaBO;
import pe.edu.pucp.inf30.stockify.boimpl.almacen.CategoriaBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;

/**
 *
 * @author Alonso
 */
@WebService(serviceName = "CategoriaWS", 
        targetNamespace = "http://services.stockify.pucp.edu.pe/")
public class CategoriaWS {
    private final CategoriaBO categoriaBO;
    
    public CategoriaWS() {
        this.categoriaBO = new CategoriaBOImpl();
    }
    
    @WebMethod(operationName = "listarCategorias")
    public List<Categoria> listarCategorias() {
        return this.categoriaBO.listar();
    }
    
    @WebMethod(operationName = "obtenerCategoria")
    public Categoria obtenerCategoria(
        @WebParam(name = "id") int id
    ) {
        return this.categoriaBO.obtener(id);
    }
    
    @WebMethod(operationName = "eliminarCategoria")
    public void eliminarCategoria(
        @WebParam(name = "id") int id
    ) {
        this.categoriaBO.eliminar(id);
    }
    
    @WebMethod(operationName = "guardarCategoria")
    public void guardarCategoria(
        @WebParam(name = "categoria") Categoria categoria, 
        @WebParam(name = "estado") Estado estado
    ) {
        this.categoriaBO.guardar(categoria, estado);
    }
}