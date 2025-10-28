/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.db;

/**
 *
 * @author DEVlegado
 */
public abstract class DBManagerFactory {
    public abstract DBManager crearDBManager(String host, int puerto, 
                                             String esquema, String usuario, 
                                             String password);
}
