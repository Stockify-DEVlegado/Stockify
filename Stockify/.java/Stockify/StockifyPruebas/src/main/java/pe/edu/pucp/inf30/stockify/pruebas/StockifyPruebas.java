/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 */

package pe.edu.pucp.inf30.stockify.pruebas;

import java.sql.Connection;
import java.sql.SQLException;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;

/**
 *
 * @author DEVlegado
 */

public class StockifyPruebas {
    
    public static void main(String[] args) throws SQLException,
            ClassNotFoundException {
        
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection connection = dbManager.getConnection()) {
            if (connection != null && !connection.isClosed()) {
                System.out.println("Conexion establecida correctamente.");
            } else {
                System.out.println("No se pudo establecer la conexion.");
            }
        } catch (SQLException e) {
            System.err.println("Error al conectar a la base de datos: "
                    + e.getMessage());
        }


        
    }
}
