/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.personal;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.personal.UsuarioBO;
import pe.edu.pucp.inf30.stockify.dao.personal.UsuarioDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;

/**
 *
 * @author DEVlegado
 */

public class UsuarioBOImpl implements UsuarioBO{
    private final UsuarioDAO usuarioDao;
    
    public UsuarioBOImpl() {
        this.usuarioDao = new UsuarioDAOImpl();
    }
    
    @Override
    public List<Usuario> listar() {
        return this.usuarioDao.leerTodos();
    }
    
    @Override
    public Usuario obtener(int id) {
        return this.usuarioDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.usuarioDao.eliminar(id);
    }
    
    @Override
    public void guardar(Usuario modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.usuarioDao.crear(modelo);
        }
        else {
            this.usuarioDao.actualizar(modelo);
        }
    }
}
