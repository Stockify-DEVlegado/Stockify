/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.almacen;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;

/**
 *
 * @author DEVlegado
 */

public class GuiaRemision {
    private int idGuiaRemision;
    private double total;
    private Date fecha;
    private EstadoDocumento estadoDocumento;
    private Usuario responsable;
    private Empresa cliente;
    private List<LineaGuiaRemision> lineas;

    public GuiaRemision() {
        this.fecha = new Date();
        this.lineas = new ArrayList<>();
    }
    
    public GuiaRemision(int idGuiaRemision, double total, EstadoDocumento estadoDocumento, 
            Usuario responsable, Empresa cliente) {
        this.idGuiaRemision = idGuiaRemision;
        this.total = total;
        this.estadoDocumento = estadoDocumento;
        this.responsable = responsable;
        this.cliente = cliente;
        this.fecha = new Date();
        this.lineas = new ArrayList<>();
    }
    
    public int getIdGuiaRemision() {
        return this.idGuiaRemision;
    }

    public void setIdGuiaRemision(int idGuiaRemision) {
        this.idGuiaRemision = idGuiaRemision;
    }

    public double getTotal() {
        return total;
    }

    public void setTotal(double total) {
        this.total = total;
    }

    public Date getFecha() {
        return fecha;
    }

    public void setFecha(Date fecha) {
        this.fecha = fecha;
    }

    public EstadoDocumento getEstadoDocumento() {
        return estadoDocumento;
    }

    public void setEstadoDocumento(EstadoDocumento estadoDocumento) {
        this.estadoDocumento = estadoDocumento;
    }

    public Usuario getResponsable() {
        return responsable;
    }

    public void setResponsable(Usuario responsable) {
        this.responsable = responsable;
    }

    public Empresa getCliente() {
        return cliente;
    }

    public void setCliente(Empresa cliente) {
        this.cliente = cliente;
    }

    public List<LineaGuiaRemision> getLineas() {
        return lineas;
    }

    public void setLineas(List<LineaGuiaRemision> lineas) {
        this.lineas = lineas;
    }
    
}
