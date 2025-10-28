/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl;

import java.sql.Connection;
import java.sql.SQLException;

/**
 *
 * @author DEVlegado
 */

public interface ComandoDAO<R> {
    R ejecutar(Connection conn) throws SQLException;
}