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
import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;

/**
 *
 * @author DEVlegado
 */

public class ProductoDAOImpl extends BaseDAO<Producto> 
        implements ProductoDAO {
    
    @Override
    protected PreparedStatement comandoCrear(Connection conn, Producto modelo) 
            throws SQLException {
        
        String sql = "{call insertarProducto(?, ?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_nombre", modelo.getNombre());
        cmd.setString("p_descripcion", modelo.getDescripcion());
        cmd.setString("p_marca", modelo.getMarca());
        cmd.setInt("p_stockMinimo", modelo.getStockMinimo());
        cmd.setInt("p_stockMaximo", modelo.getStockMaximo());
        cmd.setDouble("p_precioUnitario", modelo.getPrecioUnitario());
        if(modelo.getCategoria()!=null){
            cmd.setInt("p_idCategoria",modelo.getCategoria().getIdCategoria());
        }else{
            cmd.setNull("p_idCategoria",Types.INTEGER);
        }
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn, 
            Producto modelo) throws SQLException {
        
        String sql = "{call modificarProducto(?, ?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_nombre", modelo.getNombre());
        cmd.setString("p_descripcion", modelo.getDescripcion());
        cmd.setString("p_marca", modelo.getMarca());
        cmd.setInt("p_stockMinimo", modelo.getStockMinimo());
        cmd.setInt("p_stockMaximo", modelo.getStockMaximo());
        cmd.setDouble("p_precioUnitario", modelo.getPrecioUnitario());
        if(modelo.getCategoria()!=null){
            cmd.setInt("p_idCategoria", modelo.getCategoria().getIdCategoria());
        }else{
            cmd.setNull("p_idCategoria", Types.INTEGER);
        }
        cmd.setInt("p_id", modelo.getIdProducto());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call eliminarProducto(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer id) 
            throws SQLException {
        String sql = "{call buscarProductoPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", id);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) 
            throws SQLException {
        String sql = "{call listarProductos()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected Producto mapearModelo(ResultSet rs) throws SQLException {
        Producto producto = new Producto();
        producto.setIdProducto(rs.getInt("idProducto"));
        producto.setNombre(rs.getString("nombre"));
        producto.setDescripcion(rs.getString("descripcion"));
        producto.setMarca(rs.getString("marca"));
        producto.setStockMinimo(rs.getInt("stockMinimo"));
        producto.setStockMaximo(rs.getInt("stockMaximo"));
        producto.setPrecioUnitario(rs.getDouble("precioUnitario"));
        int idCategoria = rs.getInt("idCategoria");
        if(!rs.wasNull()){
            producto.setCategoria(new CategoriaDAOImpl().leer(idCategoria));
        }
        return producto;
    }
}
