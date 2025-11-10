/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.personal;

/**
 *
 * @author DEVlegado
 */

public class Usuario {
    private int idUsuario;
    private String nombres;
    private String apellidos;
    private String email;
    private String telefono;
    private boolean activo;
    private CuentaUsuario cuenta;
    private TipoUsuario tipoUsuario;

    public Usuario() {
        this.cuenta = new CuentaUsuario();
    }

    public Usuario(int idUsuario, String nombres, String apellidos, String email, 
                   String telefono, boolean activo, CuentaUsuario cuenta, 
                   TipoUsuario tipoUsuario) {
        this.idUsuario = idUsuario;
        this.nombres = nombres;
        this.apellidos = apellidos;
        this.email = email;
        this.telefono = telefono;
        this.activo = activo;
        this.cuenta = cuenta;
        this.tipoUsuario = tipoUsuario;
    }

    public int getIdUsuario() {
        return idUsuario;
    }

    public void setIdUsuario(int idUsuario) {
        this.idUsuario = idUsuario;
    }

    public String getNombres() {
        return nombres;
    }

    public void setNombres(String nombres) {
        this.nombres = nombres;
    }

    public String getApellidos() {
        return apellidos;
    }

    public void setApellidos(String apellidos) {
        this.apellidos = apellidos;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getTelefono() {
        return telefono;
    }

    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    public boolean isActivo() {
        return activo;
    }

    public void setActivo(boolean activo) {
        this.activo = activo;
    }

    public CuentaUsuario getCuenta() {
        return cuenta;
    }

    public void setCuenta(CuentaUsuario cuenta) {
        this.cuenta = cuenta;
    }

    public TipoUsuario getTipoUsuario() {
        return tipoUsuario;
    }

    public void setTipoUsuario(TipoUsuario tipoUsuario) {
        this.tipoUsuario = tipoUsuario;
    }
}

