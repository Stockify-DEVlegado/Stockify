/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.gestion;

import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;

/**
 *
 * @author DEVlegado
 */

public class OrdenSalida {
    
    private int idOrdenSalida;
    private double total;
    private Date fecha;
    private EstadoDocumento estado;
    private Usuario responsable;
    private List<LineaOrdenSalida> lineas;
    private OrdenVenta ordenVenta;
    
    public OrdenSalida() {
        this.fecha = new Date();
        this.responsable = new Usuario();
        this.estado = EstadoDocumento.PENDIENTE;
        this.ordenVenta = new OrdenVenta();
        this.lineas = new ArrayList<>();
    }
    
    public OrdenSalida(int idOrdenSalida, double total, Usuario responsable,
            EstadoDocumento estado, Date fecha, OrdenVenta ordenVenta) {
        this.idOrdenSalida = idOrdenSalida;
        this.total = total;
        this.fecha = fecha;
        this.estado = estado;
        this.ordenVenta = ordenVenta;
        this.responsable = responsable;
        this.lineas = new ArrayList<>();
    }

    public OrdenVenta getOrdenVenta() {
        return ordenVenta;
    }

    public void setOrdenVenta(OrdenVenta ordenVenta) {
        this.ordenVenta = ordenVenta;
    }
    
    public int getIdOrdenSalida() {
        return idOrdenSalida;
    }

    public void setIdOrdenSalida(int idOrdenSalida) {
        this.idOrdenSalida = idOrdenSalida;
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

    public EstadoDocumento getEstado() {
        return estado;
    }

    public void setEstado(EstadoDocumento estado) {
        this.estado = estado;
    }

    public Usuario getResponsable() {
        return responsable;
    }

    public void setResponsable(Usuario responsable) {
        this.responsable = responsable;
    }

    public List<LineaOrdenSalida> getLineas() {
        return lineas;
    }

    public void setLineas(List<LineaOrdenSalida> lineas) {
        this.lineas = lineas;
    } 
    
}
