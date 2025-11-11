/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;

/**
 *
 * @author DEVlegado
 */

public interface ProductoBO extends Gestionable<Producto> {
    
    List<Producto> listarOrdenadoPorCodigo();
    List<Producto> listarOrdenadoPorNombre();
    List<Producto> listarProductosPorCategoria(int idCategoria);
    
}
