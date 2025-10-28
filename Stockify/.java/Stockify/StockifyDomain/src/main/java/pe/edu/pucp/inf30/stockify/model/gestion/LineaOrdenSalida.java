/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.gestion;

import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenSalida {
    private int idLineaOrdenSalida;
    private Producto producto;
    private int cantidad;
    private double subtotal;
    private Movimiento movimiento;
    private OrdenSalida ordenSalida;

    public LineaOrdenSalida() {}

    public LineaOrdenSalida(int idLineaOrdenSalida, Producto producto, 
            int cantidad, double subtotal) {
        this.idLineaOrdenSalida = idLineaOrdenSalida;
        this.producto = producto;
        this.cantidad = cantidad;
        this.subtotal = subtotal;
    }

    public OrdenSalida getOrdenSalida() {
        return ordenSalida;
    }

    public void setOrdenSalida(OrdenSalida ordenSalida) {
        this.ordenSalida = ordenSalida;
    }
    
    public Movimiento getMovimiento() {
        return movimiento;
    }

    public void setMovimiento(Movimiento movimiento) {
        this.movimiento = movimiento;
    }

    public int getIdLineaOrdenSalida() { 
        return idLineaOrdenSalida; 
    }
    public void setIdLineaOrdenSalida(int idLineaOrdenSalida) { 
        this.idLineaOrdenSalida = idLineaOrdenSalida; 
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
