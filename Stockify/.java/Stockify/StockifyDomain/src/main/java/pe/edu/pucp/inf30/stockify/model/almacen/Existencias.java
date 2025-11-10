/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.almacen;

import java.util.Date;

/**
 *
 * @author DEVlegado
 */

public class Existencias {
    
    private int idExistencia;
    private int idUnico;
    private Producto producto;
    private EstadoExistencias estado;
    private Date fechaIngreso;
    private Date fechaEgreso;
    private Movimiento movimientoIngreso;
    private Movimiento movimientoEgreso;
    
    public Existencias() {
        this.fechaIngreso= new Date();
        this.fechaEgreso= null;
        this.estado = EstadoExistencias.DISPONIBLE;
        this.producto = new Producto();
    }

    public Existencias(int idExistencia, int idUnico, EstadoExistencias estado, 
            Producto producto, Date fechaIngreso) {
        this.idExistencia = idExistencia;
        this.idUnico = idUnico;
        this.producto = producto;
        this.estado = estado;
        this.fechaIngreso = fechaIngreso;
        this.fechaEgreso = null;
        
    }

    public int getIdExistencia() {
        return idExistencia;
    }

    public void setIdExistencia(int idExistencia) {
        this.idExistencia = idExistencia;
    }

    public int getIdUnico() {
        return idUnico;
    }

    public void setIdUnico(int idUnico) {
        this.idUnico = idUnico;
    }

    public Date getFechaIngreso() {
        return fechaIngreso;
    }

    public void setFechaIngreso(Date fechaIngreso) {
        this.fechaIngreso = fechaIngreso;
    }

    public Date getFechaEgreso() {
        return fechaEgreso;
    }

    public void setFechaEgreso(Date fechaEgreso) {
        this.fechaEgreso = fechaEgreso;
    }

    public EstadoExistencias getEstado() {
        return estado;
    }

    public void setEstado(EstadoExistencias estado) {
        this.estado = estado;
    }

    public Producto getProducto() {
        return producto;
    }

    public void setProducto(Producto producto) {
        this.producto = producto;
    }

    public Movimiento getMovimientoIngreso() {
        return movimientoIngreso;
    }

    public void setMovimientoIngreso(Movimiento movimientoIngreso) {
        this.movimientoIngreso = movimientoIngreso;
    }

    public Movimiento getMovimientoEgreso() {
        return movimientoEgreso;
    }

    public void setMovimientoEgreso(Movimiento movimientoEgreso) {
        this.movimientoEgreso = movimientoEgreso;
    }
    
}
