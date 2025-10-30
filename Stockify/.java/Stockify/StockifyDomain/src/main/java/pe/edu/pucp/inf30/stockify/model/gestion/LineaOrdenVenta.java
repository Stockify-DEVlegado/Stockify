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

public class LineaOrdenVenta {
    private int idLineaOrdenVenta;
    private Producto producto;
    private int cantidad;
    private double subtotal;
    private OrdenVenta ordenVenta;

    public LineaOrdenVenta() {
        this.producto = null;
        this.ordenVenta = null;
    }

    public LineaOrdenVenta(int idLineaOrdenVenta, Producto producto, 
            int cantidad, double subtotal) {
        this.idLineaOrdenVenta = idLineaOrdenVenta;
        this.producto = producto;
        this.cantidad = cantidad;
        this.subtotal = subtotal;
    }
    
    public OrdenVenta getOrdenVenta() {
        return ordenVenta;
    }

    public void setOrdenVenta(OrdenVenta ordenVenta) {
        this.ordenVenta = ordenVenta;
    }

    public int getIdLineaOrdenVenta() { 
        return idLineaOrdenVenta; 
    }
    public void setIdLineaOrdenVenta(int idLineaOrdenVenta) { 
        this.idLineaOrdenVenta = idLineaOrdenVenta; 
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
