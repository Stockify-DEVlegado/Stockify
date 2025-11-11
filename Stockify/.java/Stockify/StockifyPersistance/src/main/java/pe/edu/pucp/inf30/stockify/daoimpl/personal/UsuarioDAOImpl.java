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
import pe.edu.pucp.inf30.stockify.dao.personal.UsuarioDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;
import pe.edu.pucp.inf30.stockify.model.personal.TipoUsuario;

/**
 *
 * @author DEVlegado
 */

public class UsuarioDAOImpl extends BaseDAO<Usuario> implements 
        UsuarioDAO{
    @Override
    protected PreparedStatement comandoCrear(Connection conn, 
            Usuario modelo) throws SQLException {
        
        String sql = "{call insertarUsuario(?, ?, ?, ?, ?, ?, ?, ?)";
        
        CallableStatement cmd = conn.prepareCall(sql);
        if (modelo.getCuenta()!= null) {
            cmd.setInt("p_idCuentaUsuario", modelo.getCuenta().getIdCuentaUsuario());
        }
        else {
            cmd.setNull("p_idCuentaUsuario", Types.INTEGER);
        }
        cmd.setString("p_nombres", modelo.getNombres());
        cmd.setString("p_apellidos", modelo.getApellidos());
        cmd.setString("p_email", modelo.getEmail());
        cmd.setString("p_telefono", modelo.getTelefono());
        cmd.setBoolean("p_activo", modelo.isActivo());
        cmd.setString("p_tipoUsuario", String.valueOf(modelo.getTipoUsuario()));
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            Usuario modelo) throws SQLException {
        
        String sql = "{call modificarUsuario(?, ?, ?, ?, ?, ?, ?, ?)";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", modelo.getIdUsuario());
        if (modelo.getCuenta()!= null) {
            cmd.setInt("p_idCuentaUsuario", modelo.getCuenta().getIdCuentaUsuario());
        }
        else {
            cmd.setNull("p_idCuentaUsuario", Types.INTEGER);
        }
        cmd.setString("p_nombres", modelo.getNombres());
        cmd.setString("p_apellidos", modelo.getApellidos());
        cmd.setString("p_email", modelo.getEmail());
        cmd.setString("p_telefono", modelo.getTelefono());
        cmd.setBoolean("p_activo", modelo.isActivo());
        cmd.setString("p_tipoUsuario", String.valueOf(modelo.getTipoUsuario()));
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call eliminarUsuario(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarUsuarioPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarUsuarios()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected Usuario mapearModelo(ResultSet rs) throws SQLException {
        Usuario usuario = new Usuario();
        
        int idCuentaUsuario = rs.getInt("idCuentaUsuario");
        if (!rs.wasNull()) {
            usuario.setCuenta(
                    new CuentaUsuarioDAOImpl().leer(idCuentaUsuario));
        }
        
        usuario.setIdUsuario(rs.getInt("idUsuario"));
        usuario.setNombres(rs.getString("userName"));
        usuario.setApellidos(rs.getString("password"));
        usuario.setEmail(rs.getString("email"));
        usuario.setTelefono(rs.getString("telefono"));
        usuario.setActivo(rs.getBoolean("activo"));
        usuario.setTipoUsuario(TipoUsuario.valueOf(rs.getString("tipoUsuario")));
        
        return usuario;
    }    
}