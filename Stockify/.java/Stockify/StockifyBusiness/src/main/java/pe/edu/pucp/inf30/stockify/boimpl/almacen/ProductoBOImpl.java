/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.almacen.ProductoBO;
import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */

public class ProductoBOImpl implements ProductoBO{
    private final ProductoDAO productoDao;
    
    public ProductoBOImpl() {
        this.productoDao = new ProductoDAOImpl();
    }
    
    @Override
    public List<Producto> listar() {
        return this.productoDao.leerTodos();
    }
    
    @Override
    public Producto obtener(int id) {
        return this.productoDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.productoDao.eliminar(id);
    }
    
    @Override
    public void guardar(Producto modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.productoDao.crear(modelo);
        }
        else {
            this.productoDao.actualizar(modelo);
        }
    }
    
    @Override
    public List<Producto> listarOrdenadoPorCodigo() {
        return this.productoDao.leerTodosOrdenados("codigo");
    }
    
    @Override
    public List<Producto> listarOrdenadoPorNombre() {
        return this.productoDao.leerTodosOrdenados("nombre");
    }
    
    @Override
    public List<Producto> listarProductosPorCategoria(int idCategoria) {
        return this.productoDao.leerTodosPorCategoria(idCategoria);
    }
    
}
