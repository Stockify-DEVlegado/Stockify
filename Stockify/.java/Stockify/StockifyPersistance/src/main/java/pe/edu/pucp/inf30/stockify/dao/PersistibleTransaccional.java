/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.dao;

import java.sql.Connection;

/**
 *
 * @author DEVlegado
 */

public interface PersistibleTransaccional<T, I> extends Persistible<T, I> {
    I crear(T modelo, Connection conexion);
    boolean actualizar(T modelo, Connection conexion);
    boolean eliminar(I id, Connection conexion);
}
