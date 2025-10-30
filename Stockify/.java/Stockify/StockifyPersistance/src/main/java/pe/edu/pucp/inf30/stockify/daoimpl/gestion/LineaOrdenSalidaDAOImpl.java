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
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenSalidaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenSalida;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenSalidaDAOImpl extends TransaccionalBaseDAO<LineaOrdenSalida> 
        implements LineaOrdenSalidaDAO{
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, 
            LineaOrdenSalida modelo) throws SQLException {
        
        String sql = "{call insertarLineaOrdenSalida(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        if(modelo.getOrdenSalida()!=null) {
            cmd.setInt("p_idOrdenSalida", modelo.getOrdenSalida().getIdOrdenSalida());
        } else {
            cmd.setNull("p_idOrdenSalida", Types.INTEGER);
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
            LineaOrdenSalida modelo) throws SQLException {
        
        String sql = "{call modificarLineaOrdenSalida(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        if(modelo.getOrdenSalida()!=null) {
            cmd.setInt("p_idOrdenSalida", modelo.getOrdenSalida().getIdOrdenSalida());
        } else {
            cmd.setNull("p_idOrdenSalida", Types.INTEGER);
        }
        if(modelo.getProducto()!=null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.setInt("p_id", modelo.getIdLineaOrdenSalida());
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
         
        String sql = "{call eliminarLineaOrdenSalida(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarLineaOrdenSalidaPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarLineasOrdenSalida()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected LineaOrdenSalida mapearModelo(ResultSet rs) throws SQLException {
        LineaOrdenSalida lineaOrdenSalida = new LineaOrdenSalida();
        lineaOrdenSalida.setIdLineaOrdenSalida(rs.getInt("idLineaOrdenSalida"));
        
        int idOrdenSalida = rs.getInt("idOrdenSalida");
        if(!rs.wasNull()) {
            lineaOrdenSalida.setOrdenSalida(new OrdenSalidaDAOImpl().leer(idOrdenSalida));
        }
        
        int idProducto = rs.getInt("idProducto");
        if(!rs.wasNull()) {
            lineaOrdenSalida.setProducto(new ProductoDAOImpl().leer(idProducto));
        }

        lineaOrdenSalida.setCantidad(rs.getInt("cantidad"));
        lineaOrdenSalida.setSubtotal(rs.getDouble("subTotal"));
        return lineaOrdenSalida;
    }

    protected PreparedStatement comandoLeerTodosPorOrden(Connection conn, 
            int idOrden) throws SQLException {
        
        String sql = "{call listarLineasPorOrdenSalida(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenSalida", idOrden);
        return cmd;
    }
    
    @Override
    public List<LineaOrdenSalida> leerTodosPorOrden(int idOrden) {
        return ejecutarComando(conn -> leerTodosPorOrden(idOrden, conn));
    }
    
    @Override
    public List<LineaOrdenSalida> leerTodosPorOrden(int idOrden, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorOrden(conn, idOrden)) {
            ResultSet rs = cmd.executeQuery();

            List<LineaOrdenSalida> modelos = new ArrayList<>();
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
