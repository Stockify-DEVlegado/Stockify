/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.almacen;

import java.util.Date;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenSalida;

/**
 *
 * @author DEVlegado
 */
public class Movimiento {
    private int idMovimiento;
    private TipoMovimiento tipoMovimiento;
    private Date fecha;
    private String descripcion;
    private LineaOrdenIngreso lineaOrdenIngreso;
    private LineaOrdenSalida lineaOrdenSalida;
    private Producto producto;
    private int cantidad;

    public Movimiento() {
        this.fecha = new Date();
        this.producto = new Producto();
    }

    public Movimiento(int idMovimiento, TipoMovimiento tipoMovimiento, 
             String descripcion, int cantidad, Date fecha, Producto producto) {
        this.idMovimiento = idMovimiento;
        this.tipoMovimiento = tipoMovimiento;
        this.fecha = fecha;
        this.descripcion = descripcion;
        this.producto = producto;
        this.cantidad = cantidad;
    }

    public int getIdMovimiento() {
        return idMovimiento;
    }

    public void setIdMovimiento(int idMovimiento) {
        this.idMovimiento = idMovimiento;
    }

    public TipoMovimiento getTipoMovimiento() {
        return tipoMovimiento;
    }

    public void setTipoMovimiento(TipoMovimiento tipoMovimiento) {
        this.tipoMovimiento = tipoMovimiento;
    }

    public Date getFecha() {
        return fecha;
    }

    public void setFecha(Date fecha) {
        this.fecha = fecha;
    }

    public String getDescripcion() {
        return descripcion;
    }

    public void setDescripcion(String descripcion) {
        this.descripcion = descripcion;
    }

    public LineaOrdenIngreso getLineaOrdenIngreso() {
        return lineaOrdenIngreso;
    }

    public void setLineaOrdenIngreso(LineaOrdenIngreso lineaOrdenIngreso) {
        this.lineaOrdenIngreso = lineaOrdenIngreso;
    }

    public LineaOrdenSalida getLineaOrdenSalida() {
        return lineaOrdenSalida;
    }

    public void setLineaOrdenSalida(LineaOrdenSalida lineaOrdenSalida) {
        this.lineaOrdenSalida = lineaOrdenSalida;
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
    
}
