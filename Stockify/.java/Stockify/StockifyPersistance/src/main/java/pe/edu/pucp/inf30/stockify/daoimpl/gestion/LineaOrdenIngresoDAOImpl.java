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
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenIngresoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenIngreso;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenIngresoDAOImpl extends TransaccionalBaseDAO<LineaOrdenIngreso> 
        implements LineaOrdenIngresoDAO{
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, 
            LineaOrdenIngreso modelo) throws SQLException {
        
        String sql = "{call insertarLineaOrdenIngreso(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        if(modelo.getOrdenIngreso()!=null) {
            cmd.setInt("p_idOrdenIngreso", modelo.getOrdenIngreso().getIdOrdenIngreso());
        } else {
            cmd.setNull("p_idOrdenIngreso", Types.INTEGER);
        }
        if(modelo.getProducto()!=null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            LineaOrdenIngreso modelo) throws SQLException {
        
        String sql = "{call modificarLineaOrdenIngreso(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        if(modelo.getOrdenIngreso()!=null) {
            cmd.setInt("p_idOrdenIngreso", modelo.getOrdenIngreso().getIdOrdenIngreso());
        } else {
            cmd.setNull("p_idOrdenIngreso", Types.INTEGER);
        }
        if(modelo.getProducto()!=null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.setInt("p_id", modelo.getIdLineaOrdenIngreso());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
         
        String sql = "{call eliminarLineaOrdenIngreso(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarLineaOrdenIngresoPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarLineasOrdenIngreso()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected LineaOrdenIngreso mapearModelo(ResultSet rs) throws SQLException {
        LineaOrdenIngreso lineaOrdenIngreso = new LineaOrdenIngreso();
        lineaOrdenIngreso.setIdLineaOrdenIngreso(rs.getInt("idLineaOrdenIngreso"));
        
        int idOrdenIngreso = rs.getInt("idOrdenIngreso");
        if (!rs.wasNull()) {
            lineaOrdenIngreso.setOrdenIngreso(new OrdenIngresoDAOImpl().leer(idOrdenIngreso));
        }
        
        int idProducto = rs.getInt("idProducto");
        if(!rs.wasNull()) {
            lineaOrdenIngreso.setProducto(new ProductoDAOImpl().leer(idProducto));
        }

        lineaOrdenIngreso.setCantidad(rs.getInt("cantidad"));
        lineaOrdenIngreso.setSubtotal(rs.getDouble("subTotal"));
        return lineaOrdenIngreso;
    }

    protected PreparedStatement comandoLeerTodosPorOrden(Connection conn, 
            int idOrden) throws SQLException {
        
        String sql = "{call listarLineasPorOrdenIngreso(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenIngreso", idOrden);
        return cmd;
    }
    
    @Override
    public List<LineaOrdenIngreso> leerTodosPorOrden(int idOrden) {
        return ejecutarComando(conn -> leerTodosPorOrden(idOrden, conn));
    }
    
    @Override
    public List<LineaOrdenIngreso> leerTodosPorOrden(int idOrden, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorOrden(conn, idOrden)) {
            ResultSet rs = cmd.executeQuery();

            List<LineaOrdenIngreso> modelos = new ArrayList<>();
            while (rs.next()) {
                modelos.add(this.mapearModelo(rs));
            }

            return modelos;
        }
        catch (SQLException e) {
            System.err.println("Error SQL: " + e.getMessage());
            throw new RuntimeException(e);
        }
    }   
}