/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.dto;

/**
 *
 * @author Personal
 */

public class AlertaStockDTO {
    private int idProducto;
    private String nombreProducto;
    private int stockActual;
    private int stockMinimo;
    private String estado;
    
    // Constructor vacío (REQUERIDO para JAXB)
    public AlertaStockDTO() {
    }
    
    // Constructor con parámetros
    public AlertaStockDTO(int idProducto, String nombreProducto, int stockActual, 
                          int stockMinimo, String estado) {
        this.idProducto = idProducto;
        this.nombreProducto = nombreProducto;
        this.stockActual = stockActual;
        this.stockMinimo = stockMinimo;
        this.estado = estado;
    }
    
    // Getters y Setters
    public int getIdProducto() {
        return idProducto;
    }
    
    public void setIdProducto(int idProducto) {
        this.idProducto = idProducto;
    }
    
    public String getNombreProducto() {
        return nombreProducto;
    }
    
    public void setNombreProducto(String nombreProducto) {
        this.nombreProducto = nombreProducto;
    }
    
    public int getStockActual() {
        return stockActual;
    }
    
    public void setStockActual(int stockActual) {
        this.stockActual = stockActual;
    }
    
    public int getStockMinimo() {
        return stockMinimo;
    }
    
    public void setStockMinimo(int stockMinimo) {
        this.stockMinimo = stockMinimo;
    }
    
    public String getEstado() {
        return estado;
    }
    
    public void setEstado(String estado) {
        this.estado = estado;
    }
}
