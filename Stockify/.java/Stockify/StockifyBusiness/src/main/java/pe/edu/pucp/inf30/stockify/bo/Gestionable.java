/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo;

import java.util.List;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */

public interface Gestionable<T> {
    List<T> listar();
    T obtener(int id);
    void eliminar(int id);
    void guardar(T modelo, Estado estado);
}