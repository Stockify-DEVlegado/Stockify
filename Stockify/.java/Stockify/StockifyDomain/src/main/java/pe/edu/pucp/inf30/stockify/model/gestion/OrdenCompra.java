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

public class OrdenCompra {
    private int idOrdenCompra;
    private double total;
    private Date fecha;
    private EstadoDocumento estado;
    private Empresa proveedor;
    private List<LineaOrdenCompra> lineas;

    public OrdenCompra() {
        this.fecha = new Date();
        this.estado = EstadoDocumento.PENDIENTE;
        this.proveedor = new Empresa();
        this.lineas = new ArrayList<>();
    }

    public OrdenCompra(int idOrdenCompra, double total, EstadoDocumento estado,
                       Empresa proveedor, Date fecha) {
        this.idOrdenCompra = idOrdenCompra;
        this.total = total;
        this.fecha = fecha;   
        this.estado = estado;
        this.proveedor = proveedor;
        this.lineas = new ArrayList<>();
    }

    public int getIdOrdenCompra() { 
        return idOrdenCompra; 
    }
    public void setIdOrdenCompra(int idOrdenCompra) { 
        this.idOrdenCompra = idOrdenCompra; 
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

    public Empresa getProveedor() { 
        return proveedor; 
    }
    public void setProveedor(Empresa proveedor) { 
        this.proveedor = proveedor; 
    }

    public List<LineaOrdenCompra> getLineas() { 
        return lineas; 
    }
    public void setLineas(List<LineaOrdenCompra> lineas) { 
        this.lineas = lineas; 
    }
    
}
