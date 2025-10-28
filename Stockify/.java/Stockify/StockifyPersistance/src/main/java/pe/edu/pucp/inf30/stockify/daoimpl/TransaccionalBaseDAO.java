/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl;

import java.sql.Connection;
import pe.edu.pucp.inf30.stockify.dao.PersistibleTransaccional;

/**
 *
 * @author DEVlegado
 */

public abstract class TransaccionalBaseDAO<T> extends BaseDAO<T>
        implements PersistibleTransaccional<T, Integer>{
    
    @Override
    public Integer crear(T modelo, Connection conexion) {
        return ejecutarComandoCrear(conexion, modelo);
    }
    
    @Override
    public boolean actualizar(T modelo, Connection conexion) {
        return ejecutarComandoActualizar(conexion, modelo);
    }
    
    @Override
    public boolean eliminar(Integer id, Connection conexion) {
        return ejecutarComandoEliminar(conexion, id);
    }
    
    
    
}