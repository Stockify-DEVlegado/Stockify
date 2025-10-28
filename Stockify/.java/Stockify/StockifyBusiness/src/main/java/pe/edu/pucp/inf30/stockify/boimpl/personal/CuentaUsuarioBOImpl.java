/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.personal;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.personal.CuentaUsuarioBO;
import pe.edu.pucp.inf30.stockify.dao.personal.CuentaUsuarioDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.CuentaUsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;

/**
 *
 * @author DEVlegado
 */

public class CuentaUsuarioBOImpl implements CuentaUsuarioBO{
    private final CuentaUsuarioDAO cuentaUsuarioDao;
    
    public CuentaUsuarioBOImpl() {
        this.cuentaUsuarioDao = new CuentaUsuarioDAOImpl();
    }
    
    @Override
    public List<CuentaUsuario> listar() {
        return this.cuentaUsuarioDao.leerTodos();
    }
    
    @Override
    public CuentaUsuario obtener(int id) {
        return this.cuentaUsuarioDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.cuentaUsuarioDao.eliminar(id);
    }
    
    @Override
    public void guardar(CuentaUsuario modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.cuentaUsuarioDao.crear(modelo);
        }
        else {
            this.cuentaUsuarioDao.actualizar(modelo);
        }
    }
}
