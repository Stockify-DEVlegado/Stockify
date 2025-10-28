/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.dao.gestion;

import java.sql.Connection;
import java.util.List;
import pe.edu.pucp.inf30.stockify.dao.PersistibleTransaccional;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenSalida;

/**
 *
 * @author DEVlegado
 */

public interface LineaOrdenSalidaDAO extends PersistibleTransaccional<LineaOrdenSalida, Integer> {
    
    List<LineaOrdenSalida> leerTodosPorOrden(int idOrden);
    List<LineaOrdenSalida> leerTodosPorOrden(int idOrden, Connection conn);
    
}
