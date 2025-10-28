/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.dao.gestion;

import java.sql.Connection;
import java.util.List;
import pe.edu.pucp.inf30.stockify.dao.PersistibleTransaccional;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;

/**
 *
 * @author DEVlegado
 */

public interface LineaOrdenVentaDAO extends PersistibleTransaccional<LineaOrdenVenta, Integer> {
    
    List<LineaOrdenVenta> leerTodosPorOrden(int idOrden);
    List<LineaOrdenVenta> leerTodosPorOrden(int idOrden,Connection conn);
    
}
