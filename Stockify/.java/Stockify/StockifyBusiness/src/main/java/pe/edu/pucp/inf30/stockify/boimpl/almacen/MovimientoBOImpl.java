/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.almacen.MovimientoBO;
import pe.edu.pucp.inf30.stockify.dao.almacen.MovimientoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.MovimientoDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento;

/**
 *
 * @author DEVlegado
 */

public class MovimientoBOImpl implements MovimientoBO{
    private final MovimientoDAO movimientoDao;
    
    public MovimientoBOImpl() {
        this.movimientoDao = new MovimientoDAOImpl();
    }
    
    @Override
    public List<Movimiento> listar() {
        return this.movimientoDao.leerTodos();
    }
    
    @Override
    public Movimiento obtener(int id) {
        return this.movimientoDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.movimientoDao.eliminar(id);
    }
    
    @Override
    public void guardar(Movimiento modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.movimientoDao.crear(modelo);
        }
        else {
            this.movimientoDao.actualizar(modelo);
        }
    }
}
