/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.model.dto.AlertaStockDTO;

/**
 *
 * @author DEVlegado
 */

public interface ExistenciasBO extends Gestionable<Existencias> {
    int obtenerStockTotal();
    List<AlertaStockDTO> obtenerProductosStockBajo();
}
