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
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;

/**
 *
 * @author DEVlegado
 */

public class LineaOrdenCompraDAOImpl extends TransaccionalBaseDAO<LineaOrdenCompra> 
        implements LineaOrdenCompraDAO {

    @Override
    protected PreparedStatement comandoCrear(Connection conn,
            LineaOrdenCompra modelo) throws SQLException {

        String sql = "{call insertarLineaOrdenCompra(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        cmd.setInt("p_idOrdenCompra", modelo.getOrdenCompra().getIdOrdenCompra());
        cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.registerOutParameter("p_id", Types.INTEGER);

        return cmd;
    }
    @Override
     protected PreparedStatement comandoActualizar(Connection conn, 
            LineaOrdenCompra modelo) throws SQLException {
        
        String sql = "{call modificarLineaOrdenCompra(?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenCompra", modelo.getOrdenCompra().getIdOrdenCompra());
        cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        cmd.setInt("p_id", modelo.getIdLineaOrdenCompra());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, 
            Integer id) throws SQLException {
         
        String sql = "{call eliminarLineaOrdenCompra(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
    
    @Override
      protected PreparedStatement comandoLeer(Connection conn, 
            Integer id) throws SQLException {
        
        String sql = "{call buscarLineaOrdenCompraPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }
      
    @Override
     protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        
        String sql = "{call listarLineasOrdenCompra()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

     @Override
    protected LineaOrdenCompra mapearModelo(ResultSet rs) throws SQLException {
        LineaOrdenCompra lineaOrdenCompra = new LineaOrdenCompra();
        lineaOrdenCompra.setIdLineaOrdenCompra(rs.getInt("idLineaOrdenCompra"));
        lineaOrdenCompra.setOrdenCompra(
                new OrdenCompraDAOImpl().leer(rs.getInt("idOrdenCompra")));
        lineaOrdenCompra.setProducto(
                new ProductoDAOImpl().leer(rs.getInt("idProducto")));
        lineaOrdenCompra.setCantidad(rs.getInt("cantidad"));
        lineaOrdenCompra.setSubtotal(rs.getDouble("subTotal"));
        return lineaOrdenCompra;
    }
    
    protected PreparedStatement comandoLeerTodosPorOrden(Connection conn, 
            int idOrden) throws SQLException {
        
        String sql = "{call listarLineasPorOrdenCompra(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idOrdenCompra", idOrden);
        return cmd;
    }
    
    @Override
    public List<LineaOrdenCompra> leerTodosPorOrden(int idOrden) {
        return ejecutarComando(conn -> leerTodosPorOrden(idOrden, conn));
    }
    
    @Override
    public List<LineaOrdenCompra> leerTodosPorOrden(int idOrden, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorOrden(conn, idOrden)) {
            ResultSet rs = cmd.executeQuery();

            List<LineaOrdenCompra> modelos = new ArrayList<>();
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

