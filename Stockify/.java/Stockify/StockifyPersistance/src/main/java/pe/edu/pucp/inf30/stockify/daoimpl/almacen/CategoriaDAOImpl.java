/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.almacen;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import pe.edu.pucp.inf30.stockify.dao.almacen.CategoriaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;

/**
 *
 * @author DEVlegado
 */

public class CategoriaDAOImpl extends BaseDAO<Categoria> 
        implements CategoriaDAO {
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, Categoria modelo) 
            throws SQLException {
        
        String sql = "{call insertarCategoria(?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
     
        cmd.setString("p_nombre", modelo.getNombre());
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            Categoria modelo) throws SQLException {
        
        String sql = "{call modificarCategoria(?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
          cmd.setString("p_nombre", modelo.getNombre());
        cmd.setInt("p_id", modelo.getIdCategoria());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call eliminarCategoria(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id)
            throws SQLException {
        String sql = "{call buscarCategoriaPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        String sql = "{call listarCategoria()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected Categoria mapearModelo(ResultSet rs) throws SQLException {
        Categoria categoria = new Categoria();
      
        categoria.setIdCategoria(rs.getInt("idCategoria"));
        categoria.setNombre(rs.getString("nombre"));
 
        return categoria;
    }
}
