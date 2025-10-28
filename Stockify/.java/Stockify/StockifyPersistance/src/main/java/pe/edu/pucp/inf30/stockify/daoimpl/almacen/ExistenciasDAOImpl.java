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
import pe.edu.pucp.inf30.stockify.dao.almacen.ExistenciasDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.almacen.EstadoExistencias;
import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;

/**
 *
 * @author DEVlegado
 */

public class ExistenciasDAOImpl extends BaseDAO<Existencias> 
        implements ExistenciasDAO {
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, Existencias modelo) 
            throws SQLException {
        
        String sql = "{call insertarExistencias(?, ?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idUnico", modelo.getIdUnico());
        if (modelo.getProducto()!= null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setString("p_estado", String.valueOf(modelo.getEstado()));
        cmd.setDate("p_fechaIngreso",new java.sql.Date(modelo.getFechaIngreso().getTime()));
        cmd.setDate("p_fechaEgreso",new java.sql.Date(modelo.getFechaEgreso().getTime()));        
        if (modelo.getMovimientoIngreso() != null) {
            cmd.setInt("p_idMovimientoIngreso", modelo.getMovimientoIngreso().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimientoIngreso", Types.INTEGER);
        }
        if (modelo.getMovimientoEgreso() != null) {
            cmd.setInt("p_idMovimientoEgreso", modelo.getMovimientoEgreso().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimientoEgreso", Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            Existencias modelo) throws SQLException {
        
        String sql = "{call modificarExistencias(?, ?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idUnico", modelo.getIdUnico());
        if (modelo.getProducto()!= null) {
            cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
        } else {
            cmd.setNull("p_idProducto", Types.INTEGER);
        }
        cmd.setString("p_estado",String.valueOf(modelo.getEstado()));
        cmd.setDate("p_fechaIngreso",new java.sql.Date(modelo.getFechaIngreso().getTime()));
        cmd.setDate("p_fechaEgreso",new java.sql.Date(modelo.getFechaEgreso().getTime()));
        if (modelo.getMovimientoIngreso() != null) {
            cmd.setInt("p_idMovimientoIngreso", modelo.getMovimientoIngreso().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimientoIngreso", Types.INTEGER);
        }
        if (modelo.getMovimientoEgreso() != null) {
            cmd.setInt("p_idMovimientoEgreso", modelo.getMovimientoEgreso().getIdMovimiento());
        } else {
            cmd.setNull("p_idMovimientoEgreso", Types.INTEGER);
        }
        cmd.setInt("p_id", modelo.getIdExistencia());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call eliminarExistencias(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call buscarExistenciasPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        String sql = "{call listarExistencias()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected Existencias mapearModelo(ResultSet rs) throws SQLException {
        Existencias existencias = new Existencias();
        existencias.setIdExistencia(rs.getInt("idExistencia"));
        existencias.setIdUnico(rs.getInt("idUnico"));
        int idProducto = rs.getInt("idMovimiento");
        if (!rs.wasNull()) {
            existencias.setProducto(new ProductoDAOImpl().leer(idProducto));
        }
        existencias.setEstado(EstadoExistencias.valueOf(rs.getString("estado")));
        existencias.setFechaIngreso(rs.getTimestamp("fechaIngreso"));
        existencias.setFechaEgreso(rs.getTimestamp("fechaEgreso"));
        int idMovimientoIngreso = rs.getInt("idMovimientoIngreso");
        if (!rs.wasNull()) {
            existencias.setMovimientoIngreso(new MovimientoDAOImpl().leer(idMovimientoIngreso));
        }
        int idMovimientoEgreso = rs.getInt("idMovimientoEgreso");
        if (!rs.wasNull()) {
            existencias.setMovimientoEgreso(new MovimientoDAOImpl().leer(idMovimientoEgreso));
        }
        return existencias;
    }
}
