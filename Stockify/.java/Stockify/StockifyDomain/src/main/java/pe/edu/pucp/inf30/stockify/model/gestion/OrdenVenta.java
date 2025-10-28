/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.gestion;

import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;

/**
 *
 * @author DEVlegado
 */

public class OrdenVenta {
    private int idOrdenVenta;
    private double total;
    private Date fecha;
    private EstadoDocumento estado;
    private Empresa cliente;
    private List<LineaOrdenVenta> lineas;
    
    public OrdenVenta() {
        this.fecha = new Date();
        this.lineas = new ArrayList<>();
    }

    public OrdenVenta(int idOrdenVenta, double total, EstadoDocumento estado,
                      Empresa cliente) {
        this.idOrdenVenta = idOrdenVenta;
        this.total = total;
        this.fecha = new Date();
        this.estado = estado;
        this.cliente = cliente;
        this.lineas = new ArrayList<>();
    }

    public int getIdOrdenVenta() { 
        return idOrdenVenta; 
    }
    public void setIdOrdenVenta(int idOrdenVenta) { 
        this.idOrdenVenta = idOrdenVenta; 
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

    public Empresa getCliente() { 
        return cliente; 
    }
    public void setCliente(Empresa cliente) { 
        this.cliente = cliente; 
    }

    public List<LineaOrdenVenta> getLineas() { 
        return lineas; 
    }
    public void setLineas(List<LineaOrdenVenta> lineas) { 
        this.lineas = lineas; 
    }
    
}
