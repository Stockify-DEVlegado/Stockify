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
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenIngresoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;

/**
 *
 * @author DEVlegado
 */

public class OrdenIngresoDAOImpl extends TransaccionalBaseDAO<OrdenIngreso> 
        implements OrdenIngresoDAO{
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, OrdenIngreso modelo) 
            throws SQLException {
        
        String sql = 
                "{call insertarOrdenIngreso(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if(modelo.getOrdenCompra()!=null) {
            cmd.setInt("p_idOrdenCompra", modelo.getOrdenCompra().getIdOrdenCompra());
        } else {
            cmd.setInt("p_idOrdenCompra", Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, OrdenIngreso modelo) 
            throws SQLException {
        
        String sql = 
                "{call modificarOrdenIngreso(?, ?, ?, ?, ?, ?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenIngreso", modelo.getIdOrdenIngreso());
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setDate("p_fecha", new Date(modelo.getFecha().getTime()));
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        if (modelo.getResponsable()!= null) {
            cmd.setInt("p_idUsuario", modelo.getResponsable().getIdUsuario());
        } else {
            cmd.setNull("p_idUsuario", Types.INTEGER);
        }
        if (modelo.getOrdenCompra()!= null) {
            cmd.setInt("p_idOrdenCompra", modelo.getOrdenCompra().getIdOrdenCompra());
        } else {
            cmd.setNull("p_id", Types.INTEGER);
        }
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        
        String sql = "{call eliminarOrdenIngreso(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        
        String sql = "{call buscarOrdenIngresoPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarOrdenesIngreso()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected OrdenIngreso mapearModelo(ResultSet rs) throws SQLException {
        OrdenIngreso ordeningreso = new OrdenIngreso();
        
        ordeningreso.setIdOrdenIngreso(rs.getInt("idOrdenIngreso"));
        ordeningreso.setTotal(rs.getDouble("total"));
        ordeningreso.setFecha(rs.getDate("fecha"));
        ordeningreso.setEstado(EstadoDocumento.valueOf(rs.getString("estado")));
        
        int idUsuario = rs.getInt("idUsuario");
        if (!rs.wasNull()) {
            ordeningreso.setResponsable(new UsuarioDAOImpl().leer(idUsuario));
        }
        int idOrdenCompra = rs.getInt("idOrdenCompra");
        if (!rs.wasNull()) {
            ordeningreso.setOrdenCompra(new OrdenCompraDAOImpl().leer(idOrdenCompra));
        }
        
        return ordeningreso;
    }
}