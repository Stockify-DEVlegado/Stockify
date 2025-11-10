/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.usuario;

/**
 *
 * @author DEVlegado
 */

public class Empresa {
    private int idEmpresa;
    private TipoDocumento tipoDocumento;
    private String razonSocial;
    private String telefono;
    private String email;
    private boolean activo;
    private TipoEmpresa tipoEmpresa;

    public Empresa() {
        
    }

    public Empresa(int idEmpresa, TipoDocumento tipoDocumento, String razonSocial,
                   String telefono, String email, boolean activo, TipoEmpresa tipoEmpresa) {
        this.idEmpresa = idEmpresa;
        this.tipoDocumento = tipoDocumento;
        this.razonSocial = razonSocial;
        this.telefono = telefono;
        this.email = email;
        this.activo = activo;
        this.tipoEmpresa = tipoEmpresa;
    }

    public int getIdEmpresa() {
        return idEmpresa;
    }

    public void setIdEmpresa(int idEmpresa) {
        this.idEmpresa = idEmpresa;
    }

    public TipoDocumento getTipoDocumento() {
        return tipoDocumento;
    }

    public void setTipoDocumento(TipoDocumento tipoDocumento) {
        this.tipoDocumento = tipoDocumento;
    }

    public String getRazonSocial() {
        return razonSocial;
    }

    public void setRazonSocial(String razonSocial) {
        this.razonSocial = razonSocial;
    }

    public String getTelefono() {
        return telefono;
    }

    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public boolean isActivo() {
        return activo;
    }

    public void setActivo(boolean activo) {
        this.activo = activo;
    }

    public TipoEmpresa getTipoEmpresa() {
        return tipoEmpresa;
    }

    public void setTipoEmpresa(TipoEmpresa tipoEmpresa) {
        this.tipoEmpresa = tipoEmpresa;
    }
}
