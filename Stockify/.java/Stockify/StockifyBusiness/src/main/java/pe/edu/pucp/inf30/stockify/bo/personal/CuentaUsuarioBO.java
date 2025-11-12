/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.personal;

import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;

/**
 *
 * @author DEVlegado
 */

public interface CuentaUsuarioBO extends Gestionable<CuentaUsuario> {
    boolean login(String userName, String password);
}
