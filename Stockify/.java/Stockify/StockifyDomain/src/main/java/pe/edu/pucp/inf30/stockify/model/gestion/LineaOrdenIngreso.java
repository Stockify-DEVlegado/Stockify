/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.gestion;

import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenIngreso {
    private int idLineaOrdenIngreso;
    private Producto producto;
    private int cantidad;
    private double subtotal;
    private OrdenIngreso ordenIngreso;

    public LineaOrdenIngreso() {
        this.producto = new Producto();
        this.ordenIngreso = new OrdenIngreso();
    }

    public LineaOrdenIngreso(int idLineaOrdenIngreso, Producto producto, 
            int cantidad, double subtotal, OrdenIngreso ordenIngreso) {
        this.idLineaOrdenIngreso = idLineaOrdenIngreso;
        this.producto = producto;
        this.ordenIngreso = ordenIngreso;
        this.cantidad = cantidad;
        this.subtotal = subtotal;
    }

    public OrdenIngreso getOrdenIngreso() {
        return ordenIngreso;
    }

    public void setOrdenIngreso(OrdenIngreso ordenIngreso) {
        this.ordenIngreso = ordenIngreso;
    }
    
    public int getIdLineaOrdenIngreso() { 
        return idLineaOrdenIngreso; 
    }
    public void setIdLineaOrdenIngreso(int idLineaOrdenIngreso) { 
        this.idLineaOrdenIngreso = idLineaOrdenIngreso; 
    }

    public Producto getProducto() { 
        return producto; 
    }
    public void setProducto(Producto producto) { 
        this.producto = producto; 
    }

    public int getCantidad() { 
        return cantidad; 
    }
    public void setCantidad(int cantidad) { 
        this.cantidad = cantidad; 
    }

    public double getSubtotal() { 
        return subtotal; 
    }
    public void setSubtotal(double subtotal) { 
        this.subtotal = subtotal; 
    }

  }
