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
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenVentaDAOImpl extends TransaccionalBaseDAO<LineaOrdenVenta> 
        implements LineaOrdenVentaDAO {

    @Override
    protected PreparedStatement comandoCrear(Connection conn,
            LineaOrdenVenta modelo) throws SQLException {

        String sql = "{call insertarLineaOrdenVenta(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        if(modelo.getOrdenVenta()!=null) {
            cmd.setInt("p_idOrdenVenta", modelo.getOrdenVenta().getIdOrdenVenta());
        } else {
            cmd.setNull("p_idOrdenVenta", Types.INTEGER);
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
            LineaOrdenVenta modelo) throws SQLException {
        
        String sql = "{call modificarLineaOrdenVenta(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        if(modelo.getOrdenVenta()!=null) {
            cmd.setInt("p_idOrdenVenta", modelo.getOrdenVenta().getIdOrdenVenta());
        } else {
            cmd.setNull("p_idOrdenVenta", Types.INTEGER);
        }
        if(modelo.getProducto()!=null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.setInt("p_id", modelo.getIdLineaOrdenVenta());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
         
        String sql = "{call eliminarLineaOrdenVenta(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
    
    @Override
      protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarLineaOrdenVentaPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
      
    @Override
     protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarLineasOrdenVenta()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

     @Override
    protected LineaOrdenVenta mapearModelo(ResultSet rs) throws SQLException {
        LineaOrdenVenta lineaOrdenVenta = new LineaOrdenVenta();
        lineaOrdenVenta.setIdLineaOrdenVenta(rs.getInt("idLineaOrdenVenta"));
        int idOrdenVenta = rs.getInt("idOrdenVenta");
        if(!rs.wasNull()) {
            lineaOrdenVenta.setOrdenVenta(new OrdenVentaDAOImpl().leer(idOrdenVenta));
        }
        int idProducto = rs.getInt("idProducto");
        if(!rs.wasNull()) {
            lineaOrdenVenta.setProducto(new ProductoDAOImpl().leer(idProducto));
        }
        lineaOrdenVenta.setCantidad(rs.getInt("cantidad"));
        lineaOrdenVenta.setSubtotal(rs.getDouble("subTotal"));
        return lineaOrdenVenta;
    }
    
    protected PreparedStatement comandoLeerTodosPorOrden(Connection conn, 
            int idOrden) throws SQLException {
        
        String sql = "{call listarLineasPorOrdenVenta(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenVenta", idOrden);
        return cmd;
    }
    
    @Override
    public List<LineaOrdenVenta> leerTodosPorOrden(int idOrden) {
        return ejecutarComando(conn -> leerTodosPorOrden(idOrden, conn));
    }
    
    @Override
    public List<LineaOrdenVenta> leerTodosPorOrden(int idOrden, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorOrden(conn, idOrden)) {
            ResultSet rs = cmd.executeQuery();

            List<LineaOrdenVenta> modelos = new ArrayList<>();
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