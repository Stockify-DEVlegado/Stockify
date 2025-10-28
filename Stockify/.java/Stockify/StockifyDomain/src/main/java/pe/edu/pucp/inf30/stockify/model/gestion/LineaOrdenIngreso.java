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

public class LineaOrdenIngreso {
    private int idLineaOrdenIngreso;
    private Producto producto;
    private int cantidad;
    private double subtotal;
    private Movimiento movimiento;
    private OrdenIngreso ordenIngreso;

    public LineaOrdenIngreso() {}

    public LineaOrdenIngreso(int idLineaOrdenIngreso, Producto producto, 
            int cantidad, double subtotal) {
        this.idLineaOrdenIngreso = idLineaOrdenIngreso;
        this.producto = producto;
        this.cantidad = cantidad;
        this.subtotal = subtotal;
    }

    public OrdenIngreso getOrdenIngreso() {
        return ordenIngreso;
    }

    public void setOrdenIngreso(OrdenIngreso ordenIngreso) {
        this.ordenIngreso = ordenIngreso;
    }
    
    public Movimiento getMovimiento() {
        return movimiento;
    }

    public void setMovimiento(Movimiento movimiento) {
        this.movimiento = movimiento;
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
