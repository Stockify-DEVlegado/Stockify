/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.dao;

import java.util.List;

/**
 *
 * @author DEVlegado
 */

public interface Persistible<T, I> {
    I crear(T modelo);
    boolean actualizar(T modelo);
    boolean eliminar(I id);
    T leer(I id);
    List<T> leerTodos();
}
