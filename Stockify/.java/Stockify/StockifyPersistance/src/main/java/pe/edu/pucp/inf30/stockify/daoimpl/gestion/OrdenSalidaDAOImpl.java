/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.gestion;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.Date;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenSalidaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenSalida;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;

/**
 *
 * @author DEVlegado
 */

public class OrdenSalidaDAOImpl extends TransaccionalBaseDAO<OrdenSalida> 
        implements OrdenSalidaDAO{
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, OrdenSalida modelo) 
            throws SQLException {
        
        String sql = 
                "{call insertarOrdenSalida(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if (modelo.getOrdenVenta()!= null) {
            cmd.setInt("p_idOrdenVenta", modelo.getOrdenVenta().getIdOrdenVenta());
        } else {
            cmd.setNull("p_idOrdenVenta", Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, OrdenSalida modelo) 
            throws SQLException {
        
        String sql = 
                "{call modificarOrdenSalida(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", modelo.getIdOrdenSalida());
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if (modelo.getOrdenVenta()!= null) {
            cmd.setInt("p_idOrdenVenta", modelo.getOrdenVenta().getIdOrdenVenta());
        } else {
            cmd.setNull("p_idOrdenVenta", Types.INTEGER);
        }
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        
        String sql = "{call eliminarOrdenSalida(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        
        String sql = "{call buscarOrdenSalidaPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) throws SQLException {
        
        String sql = "{call listarOrdenesSalida()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected OrdenSalida mapearModelo(ResultSet rs) throws SQLException {
        OrdenSalida ordensalida = new OrdenSalida();
        
        ordensalida.setIdOrdenSalida(rs.getInt("idOrdenSalida"));
        ordensalida.setTotal(rs.getDouble("total"));
        ordensalida.setFecha(rs.getDate("fecha"));
        ordensalida.setEstado(EstadoDocumento.valueOf(rs.getString("estado")));
        
        int idUsuario = rs.getInt("idUsuario");
        if (!rs.wasNull()) {
            ordensalida.setResponsable(new UsuarioDAOImpl().leer(idUsuario));
        }
        int idOrdenVenta = rs.getInt("idOrdenVenta");
        if (!rs.wasNull()) {
            ordensalida.setOrdenVenta(new OrdenVentaDAOImpl().leer(idOrdenVenta));
        }
        
        return ordensalida;
    }
}
