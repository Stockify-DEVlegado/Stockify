/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.personal;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import pe.edu.pucp.inf30.stockify.dao.personal.CuentaUsuarioDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;

/**
 *
 * @author DEVlegado
 */

public class CuentaUsuarioDAOImpl extends BaseDAO<CuentaUsuario> implements 
        CuentaUsuarioDAO{
    @Override
    protected PreparedStatement comandoCrear(Connection conn, 
            CuentaUsuario modelo) throws SQLException {
        
        String sql = "{call insertarCuentaUsuario(?, ?, ?, ?)";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_username", modelo.getUsername());
        cmd.setString("p_password", modelo.getPassword());
        cmd.setDate("p_ultimoAcceso", new java.sql.Date(modelo.getUltimoAcceso().getTime()));
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
        
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            CuentaUsuario modelo) throws SQLException {
        
        String sql = "{call modificarCuentaUsuario(?, ?, ?, ?)";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", modelo.getIdCuentaUsuario());
        cmd.setString("p_username", modelo.getUsername());
        cmd.setString("p_password", modelo.getPassword());
        cmd.setDate("p_ultimoAcceso", new java.sql.Date(modelo.getUltimoAcceso().getTime()));
        return cmd;
        
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call eliminarCuentaUsuario(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarCuentaUsuarioPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarCuentasUsuarios()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected CuentaUsuario mapearModelo(ResultSet rs) throws SQLException {
        CuentaUsuario cuentaUsuario = new CuentaUsuario();
        
        cuentaUsuario.setIdCuentaUsuario(rs.getInt("idCuentaUsuario"));
        cuentaUsuario.setUsername(rs.getString("username"));
        cuentaUsuario.setPassword(rs.getString("password"));
        cuentaUsuario.setUltimoAcceso(rs.getTimestamp("ultimoAcceso"));
        
        return cuentaUsuario;
    }     
}
