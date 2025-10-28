/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.almacen;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;
import pe.edu.pucp.inf30.stockify.dao.almacen.LineaGuiaRemisionDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.TransaccionalBaseDAO;
import pe.edu.pucp.inf30.stockify.model.almacen.LineaGuiaRemision;

/**
 *
 * @author DEVlegado
 */

public class LineaGuiaRemisionDAOImpl extends TransaccionalBaseDAO<LineaGuiaRemision> 
        implements LineaGuiaRemisionDAO {

    @Override
    protected PreparedStatement comandoCrear(Connection conn, LineaGuiaRemision modelo)
            throws SQLException {

        String sql
                = "{call insertarLineaGuiaRemision(?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        cmd.setInt("p_idGuiaRemision", modelo.getGuiaRemision().getIdGuiaRemision());
        cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        if (modelo.getMovimiento()!= null) {
            cmd.setInt("p_idMovimiento", modelo.getMovimiento().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimiento", Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn,
            LineaGuiaRemision modelo) throws SQLException {

        String sql
                = "{call modificarLineaGuiaRemision(?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);

        cmd.setInt("p_idGuiaRemision", modelo.getGuiaRemision().getIdGuiaRemision());
        cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        cmd.setInt("p_cantidad", modelo.getCantidad());
        cmd.setDouble("p_subTotal", modelo.getSubtotal());
        if (modelo.getMovimiento()!= null) {
            cmd.setInt("p_idMovimiento", modelo.getMovimiento().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimiento", Types.INTEGER);
        }
        cmd.setInt("p_id", modelo.getIdLineaGuiaRemision());
        
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call eliminarLineaGuiaRemision(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call buscarLineaGuiaRemisionPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) throws 
            SQLException {
        String sql = "{call listarLineasGuiaRemision()}";
        CallableStatement cmd = conn.prepareCall(sql);
        
        return cmd;
    }

    @Override
    protected LineaGuiaRemision mapearModelo(ResultSet rs) throws SQLException {
        LineaGuiaRemision lineaGuiaRemision = new LineaGuiaRemision();
        lineaGuiaRemision.setIdLineaGuiaRemision(rs.getInt("idLineaGuiaRemision"));
        lineaGuiaRemision.setGuiaRemision(
                new GuiaRemisionDAOImpl().leer(rs.getInt("idOrdenIngreso")));
        lineaGuiaRemision.setProducto(
                new ProductoDAOImpl().leer(rs.getInt("idProducto")));
        
        lineaGuiaRemision.setCantidad(rs.getInt("cantidad"));
        lineaGuiaRemision.setSubtotal(rs.getDouble("subtotal"));
        int idMovimiento = rs.getInt("idEmpresa");
        if (!rs.wasNull()) {
            lineaGuiaRemision.setMovimiento(new MovimientoDAOImpl().leer(idMovimiento));
        }
        return lineaGuiaRemision;
    }
    
    protected PreparedStatement comandoLeerTodosPorOrden(Connection conn, 
            int idGuia) throws SQLException {
        
        String sql = "{call listarLineasPorGuiaRemision(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idGuiaRemision", idGuia);
        return cmd;
    }
    
    @Override
    public List<LineaGuiaRemision> leerTodosPorOrden(int idGuia) {
        return ejecutarComando(conn -> leerTodosPorOrden(idGuia, conn));
    }
    
    @Override
    public List<LineaGuiaRemision> leerTodosPorOrden(int idGuia, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorOrden(conn, idGuia)) {
            ResultSet rs = cmd.executeQuery();

            List<LineaGuiaRemision> modelos = new ArrayList<>();
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
