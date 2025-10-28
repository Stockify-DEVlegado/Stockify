/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.gestion;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class OrdenVentaDAOImpl extends TransaccionalBaseDAO<OrdenVenta> 
        implements OrdenVentaDAO{
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, OrdenVenta modelo) 
            throws SQLException {
        
        String sql = "{call insertarOrdenVenta(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        if (modelo.getCliente() != null) {
            cmd.setInt("p_idCliente", modelo.getCliente().getIdEmpresa());
        }
        else {
            cmd.setNull("p_idCliente", Types.INTEGER);
        }
        cmd.setDate("p_fecha", new java.sql.Date(modelo.getFecha().getTime()));
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        cmd.registerOutParameter("p_id", Types.INTEGER);
                
        return cmd;

    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            OrdenVenta modelo) throws SQLException {
        
        String sql = "{call modificarOrdenVenta(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        if (modelo.getCliente() != null) {
            cmd.setInt("p_idCliente", modelo.getCliente().getIdEmpresa());
        }
        else {
            cmd.setNull("p_idCliente", Types.INTEGER);
        }
        
        cmd.setDate("p_fecha", new java.sql.Date(modelo.getFecha().getTime()));
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        cmd.setInt("p_id", modelo.getIdOrdenVenta());
        
        return cmd;
    }

        @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call eliminarOrdenVenta(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarOrdenVentaPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarOrdenesVenta()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }


    @Override
    protected OrdenVenta mapearModelo(ResultSet rs) throws SQLException {
        OrdenVenta ordenVenta = new OrdenVenta();
        
        ordenVenta.setIdOrdenVenta(rs.getInt("idOrdenVenta"));
        
        int idEmpresa = rs.getInt("idEmpresa");
        if (!rs.wasNull()) {
            ordenVenta.setCliente(
                new EmpresaDAOImpl().leer(idEmpresa));
        }
        
        ordenVenta.setTotal(rs.getDouble("total"));
        ordenVenta.setEstado(EstadoDocumento.valueOf(rs.getString("estado")));
        ordenVenta.setFecha(rs.getTimestamp("fecha"));
        
        return ordenVenta;
    }
}
