/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.model.dto;

/**
 *
 * @author Personal
 */

public class MovimientoMesDTO {
    private String mes;
    private int numeroMes;
    private int entradas;
    private int salidas;
    
    // Constructor vacío (REQUERIDO para JAXB)
    public MovimientoMesDTO() {
    }
    
    // Constructor con parámetros
    public MovimientoMesDTO(String mes, int numeroMes, int entradas, int salidas) {
        this.mes = mes;
        this.numeroMes = numeroMes;
        this.entradas = entradas;
        this.salidas = salidas;
    }
    
    // Getters y Setters
    public String getMes() {
        return mes;
    }
    
    public void setMes(String mes) {
        this.mes = mes;
    }
    
    public int getNumeroMes() {
        return numeroMes;
    }
    
    public void setNumeroMes(int numeroMes) {
        this.numeroMes = numeroMes;
    }
    
    public int getEntradas() {
        return entradas;
    }
    
    public void setEntradas(int entradas) {
        this.entradas = entradas;
    }
    
    public int getSalidas() {
        return salidas;
    }
    
    public void setSalidas(int salidas) {
        this.salidas = salidas;
    }
}
