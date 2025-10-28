/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.usuario;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.usuario.EmpresaBO;
import pe.edu.pucp.inf30.stockify.dao.usuario.EmpresaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;

/**
 *
 * @author DEVlegado
 */

public class EmpresaBOImpl implements EmpresaBO{
    private final EmpresaDAO empresaDao;
    
    public EmpresaBOImpl() {
        this.empresaDao = new EmpresaDAOImpl();
    }
    
    @Override
    public List<Empresa> listar() {
        return this.empresaDao.leerTodos();
    }
    
    @Override
    public Empresa obtener(int id) {
        return this.empresaDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.empresaDao.eliminar(id);
    }
    
    @Override
    public void guardar(Empresa modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.empresaDao.crear(modelo);
        }
        else {
            this.empresaDao.actualizar(modelo);
        }
    }
}
