/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo;

/**
 *
 * @author DEVlegado
 */

public interface GestionableProbable {
    void debeListar();
    void debeObtenerSiIdExiste();
    void noDebeObtenerSiIdNoExiste();
    void debeGuardarNuevo();
    void debeGuardarModificado();
    void debeEliminarSiIdExiste();
    void noDebeEliminarSiIdNoExiste();
    void debeHacerRollbackSiErrorEnGuardar();
    void debeHacerRollbackSiErrorEnEliminar();
}
