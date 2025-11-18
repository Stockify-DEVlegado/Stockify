/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.model.dto.MovimientoMesDTO;

/**
 *
 * @author DEVlegado
 */

public interface MovimientoBO extends Gestionable<Movimiento> {
    int contarPorTipo(String tipo, int dias);
    List<MovimientoMesDTO> obtenerMovimientosPorMes(int meses);
}
