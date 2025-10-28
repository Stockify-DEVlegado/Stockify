/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.almacen.CategoriaBO;
import pe.edu.pucp.inf30.stockify.dao.almacen.CategoriaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;

/**
 *
 * @author DEVlegado
 */

public class CategoriaBOImpl implements CategoriaBO{
    private final CategoriaDAO categoriaDao;
    
    public CategoriaBOImpl() {
        this.categoriaDao = new CategoriaDAOImpl();
    }
    
    @Override
    public List<Categoria> listar() {
        return this.categoriaDao.leerTodos();
    }
    
    @Override
    public Categoria obtener(int id) {
        return this.categoriaDao.leer(id);
    }
    
    @Override
    public void eliminar(int id) {
        this.categoriaDao.eliminar(id);
    }
    
    @Override
    public void guardar(Categoria modelo,Estado estado) {
        if(estado == Estado.NUEVO) {
            this.categoriaDao.crear(modelo);
        }
        else {
            this.categoriaDao.actualizar(modelo);
        }
    }
}