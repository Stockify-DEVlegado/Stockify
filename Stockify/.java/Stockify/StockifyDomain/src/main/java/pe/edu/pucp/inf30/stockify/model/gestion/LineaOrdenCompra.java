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

public class LineaOrdenCompra {
    private int idLineaOrdenCompra;
    private Producto producto;
    private int cantidad;
    private double subtotal;
    private OrdenCompra ordenCompra;

    public LineaOrdenCompra() {}

    public LineaOrdenCompra(int idLineaOrdenCompra, Producto producto, 
            int cantidad, double subtotal) {
        this.idLineaOrdenCompra = idLineaOrdenCompra;
        this.producto = producto;
        this.cantidad = cantidad;
        this.subtotal = subtotal;
    }

    public OrdenCompra getOrdenCompra() {
        return ordenCompra;
    }

    public void setOrdenCompra(OrdenCompra ordenCompra) {
        this.ordenCompra = ordenCompra;
    }

    public int getIdLineaOrdenCompra() { 
        return idLineaOrdenCompra; 
    }
    public void setIdLineaOrdenCompra(int idLineaOrdenCompra) { 
        this.idLineaOrdenCompra = idLineaOrdenCompra; 
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
