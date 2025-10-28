/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.personal;

import java.util.Date;

/**
 *
 * @author DEVlegado
 */

public class CuentaUsuario {
    private int idCuentaUsuario;
    private String username;
    private String password; 
    private Date ultimoAcceso;

    public CuentaUsuario() {
        this.ultimoAcceso = new Date();
    }

    public CuentaUsuario(int idCuentaUsuario, String username, String password) {
        this.idCuentaUsuario = idCuentaUsuario;
        this.username = username;
        this.password = password;
        this.ultimoAcceso = new Date();
    }

    public int getIdCuentaUsuario() {
        return idCuentaUsuario;
    }

    public void setIdCuentaUsuario(int idCuentaUsuario) {
        this.idCuentaUsuario = idCuentaUsuario;
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public Date getUltimoAcceso() {
        return ultimoAcceso;
    }

    public void setUltimoAcceso(Date ultimoAcceso) {
        this.ultimoAcceso = ultimoAcceso;
    }
}
