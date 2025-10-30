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

public class OrdenIngreso {
    private int idOrdenIngreso;
    private double total;
    private Date fecha;
    private EstadoDocumento estado;
    private Usuario responsable;
    private List<LineaOrdenIngreso> lineas;
    private OrdenCompra ordenCompra;

    public OrdenIngreso() {
        this.fecha = new Date();
        this.lineas = new ArrayList<>();
        this.responsable = null;
        this.ordenCompra = null;
        this.estado = EstadoDocumento.PENDIENTE;
    }

    public OrdenIngreso(int idOrdenIngreso, double total, Usuario responsable) {
        this.idOrdenIngreso = idOrdenIngreso;
        this.total = total;
        this.fecha = new Date();   
        this.responsable = responsable;
        this.lineas = new ArrayList<>();
    }

    public OrdenCompra getOrdenCompra() {
        return ordenCompra;
    }

    public void setOrdenCompra(OrdenCompra ordenCompra) {
        this.ordenCompra = ordenCompra;
    }

    public int getIdOrdenIngreso() { 
        return idOrdenIngreso; 
    }
    public void setIdOrdenIngreso(int idOrdenIngreso) { 
        this.idOrdenIngreso = idOrdenIngreso; 
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

    public List<LineaOrdenIngreso> getLineas() { 
        return lineas; 
    }
    public void setLineas(List<LineaOrdenIngreso> lineas) { 
        this.lineas = lineas; 
    }
    
}
