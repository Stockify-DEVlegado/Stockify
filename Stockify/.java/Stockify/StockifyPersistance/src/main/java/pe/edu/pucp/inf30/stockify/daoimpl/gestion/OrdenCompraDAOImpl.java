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
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class OrdenCompraDAOImpl extends TransaccionalBaseDAO<OrdenCompra>
        implements OrdenCompraDAO {
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, OrdenCompra modelo) 
            throws SQLException {
        
        String sql = "{call insertarOrdenCompra(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        if (modelo.getProveedor()!= null) {
            cmd.setInt("p_idProveedor", modelo.getProveedor().getIdEmpresa());
        }
        else {
            cmd.setNull("p_idProveedor", Types.INTEGER);
        }
        
        cmd.setDate("p_fecha", new java.sql.Date(modelo.getFecha().getTime()));
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        cmd.registerOutParameter("p_id", Types.INTEGER);
                
        return cmd;

    }
    
    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            OrdenCompra modelo) throws SQLException {
        
        String sql = "{call modificarOrdenCompra(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        if (modelo.getProveedor()!= null) {
            cmd.setInt("p_idProveedor", modelo.getProveedor().getIdEmpresa());
        }
        else {
            cmd.setNull("p_idProveedor", Types.INTEGER);
        }
        
        cmd.setDate("p_fecha", new java.sql.Date(modelo.getFecha().getTime()));
        cmd.setDouble("p_total", modelo.getTotal());
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        cmd.setInt("p_id", modelo.getIdOrdenCompra());
        
        return cmd;
    }
    
    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call eliminarOrdenCompra(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
    
    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarOrdenCompraPorId(?)}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
    
    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarOrdenesCompra()}";
        
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }
    
    @Override
    protected OrdenCompra mapearModelo(ResultSet rs) throws SQLException {
        OrdenCompra ordenCompra = new OrdenCompra();
        
        ordenCompra.setIdOrdenCompra(rs.getInt("idOrdenCompra"));
        
        int idEmpresa = rs.getInt("idEmpresa");
        if (!rs.wasNull()) {
            ordenCompra.setProveedor(
                new EmpresaDAOImpl().leer(idEmpresa));
        }
        
        ordenCompra.setTotal(rs.getDouble("total"));
        ordenCompra.setEstado(EstadoDocumento.valueOf(rs.getString("estado")));
        ordenCompra.setFecha(rs.getTimestamp("fecha"));
        
        return ordenCompra;
    }
    
}