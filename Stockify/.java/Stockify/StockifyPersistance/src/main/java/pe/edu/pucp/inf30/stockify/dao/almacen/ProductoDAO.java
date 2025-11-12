/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.dao.almacen;

import java.sql.Connection;
import java.util.List;
import pe.edu.pucp.inf30.stockify.dao.Persistible;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */

public interface ProductoDAO extends Persistible<Producto, Integer> {
    
    List<Producto> leerTodosOrdenados(String criterio);
    List<Producto> leerTodosOrdenados(String criterio,Connection conn);
    
    List<Producto> leerTodosPorCategoria(int idCategoria);
    List<Producto> leerTodosPorCategoria(int idCategoria, Connection conn);
    
}
