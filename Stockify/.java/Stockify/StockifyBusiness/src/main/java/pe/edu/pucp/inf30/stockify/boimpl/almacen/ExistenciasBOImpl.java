/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.almacen.ExistenciasBO;
import pe.edu.pucp.inf30.stockify.dao.almacen.ExistenciasDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ExistenciasDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;

/**
 *
 * @author DEVlegado
 */

public class ExistenciasBOImpl implements ExistenciasBO{
    private final ExistenciasDAO existenciasDao;
    
    public ExistenciasBOImpl() {
        this.existenciasDao = new ExistenciasDAOImpl();
    }
    
    @Override
    public List<Existencias> listar() {
        return this.existenciasDao.leerTodos();
    }
    
    @Override
    public Existencias obtener(int id) {
        return this.existenciasDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.existenciasDao.eliminar(id);
    }
    
    @Override
    public void guardar(Existencias modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.existenciasDao.crear(modelo);
        }
        else {
            this.existenciasDao.actualizar(modelo);
        }
    }
}
