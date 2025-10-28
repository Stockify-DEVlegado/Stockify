/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.almacen;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.Date;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import java.sql.Types;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.dao.almacen.GuiaRemisionDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import pe.edu.pucp.inf30.stockify.model.almacen.GuiaRemision;

/**
 *
 * @author DEVlegado
 */

public class GuiaRemisionDAOImpl extends TransaccionalBaseDAO<GuiaRemision> 
        implements GuiaRemisionDAO {

    @Override
    protected PreparedStatement comandoCrear(Connection conn, GuiaRemision modelo)
            throws SQLException {

        String sql
                = "{call insertarGuiaRemision(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estadoDocumento", String.valueOf(modelo.getEstadoDocumento()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idEmpresa", modelo.getCliente().getIdEmpresa());
        } else {
            cmd.setNull("p_idEmpresa", Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn,
            GuiaRemision modelo) throws SQLException {

        String sql
                = "{call modificarGuiaRemision(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", modelo.getIdGuiaRemision());
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estadoDocumento", String.valueOf(modelo.getEstadoDocumento()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if (modelo.getCliente()!= null) {
            cmd.setInt("p_idEmpresa", modelo.getCliente().getIdEmpresa());
        } else {
            cmd.setNull("p_idEmpresa", Types.INTEGER);
        }
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        String sql
                = "{call eliminarGuiaRemision(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        
        String sql = 
                "{call buscarGuiaRemisionPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        String sql = 
                "{call listarGuiasRemision()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected GuiaRemision mapearModelo(ResultSet rs) throws SQLException {
        GuiaRemision guiar = new GuiaRemision();
        guiar.setIdGuiaRemision(rs.getInt("idGuiaRemision"));
        guiar.setTotal(rs.getDouble("total"));
        guiar.setFecha(rs.getDate("fecha"));
        guiar.setEstadoDocumento(EstadoDocumento.valueOf(rs.getString("estadoDocumento")));
        int idUsuario = rs.getInt("idUsuario");
        if (!rs.wasNull()) {
            guiar.setResponsable(new UsuarioDAOImpl().leer(idUsuario));
        }
        int idEmpresa = rs.getInt("idEmpresa");
        if (!rs.wasNull()) {
            guiar.setCliente(new EmpresaDAOImpl().leer(idEmpresa));
        }
        
        return guiar;
    }
}