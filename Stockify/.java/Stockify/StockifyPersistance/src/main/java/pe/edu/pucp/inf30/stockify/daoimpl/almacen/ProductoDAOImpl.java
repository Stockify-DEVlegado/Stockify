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
import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;

/**
 *
 * @author DEVlegado
 */
public class ProductoDAOImpl extends BaseDAO<Producto> implements ProductoDAO {
    
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
    
    protected PreparedStatement comandoLeerTodosOrdenados(Connection conn, 
            String criterio) throws SQLException {
        
        String sql = "{call listarProductosOrdenados(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_criterioOrden", criterio);
        return cmd;
    }
    
    @Override
    public List<Producto> leerTodosOrdenados(String criterio) {
        return ejecutarComando(conn -> leerTodosOrdenados(criterio, conn));
    }
    
    @Override
    public List<Producto> leerTodosOrdenados(String criterio, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosOrdenados(conn, criterio)) {
            ResultSet rs = cmd.executeQuery();

            List<Producto> modelos = new ArrayList<>();
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
    
    protected PreparedStatement comandoLeerTodosPorCategoria(Connection conn, 
            int idCategoria) throws SQLException {
        
        String sql = "{call listarProductosPorCategoria(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_idCategoria", idCategoria);
        return cmd;
    }
    
    @Override
    public List<Producto> leerTodosPorCategoria(int idCategoria) {
        return ejecutarComando(conn -> leerTodosPorCategoria(idCategoria, conn));
    }
    
    @Override
    public List<Producto> leerTodosPorCategoria(int idCategoria, Connection conn) {
        try (PreparedStatement cmd = this.comandoLeerTodosPorCategoria(conn, idCategoria)) {
            ResultSet rs = cmd.executeQuery();

            List<Producto> modelos = new ArrayList<>();
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
    
    @Override
    public int insertarBloque(List<Producto> productos) {
        Connection conn = null;
        int insertados = 0;
        
        try {
            // Obtener el DBManager configurado
            DBManager dbManager = DBFactoryProvider.getManager();
            conn = dbManager.getConnection();
            conn.setAutoCommit(false); // Iniciar transacción
            
            for (Producto producto : productos) {
                try (CallableStatement cmd = (CallableStatement) comandoCrear(conn, producto)) {
                    cmd.execute();
                    
                    // Obtener el ID generado
                    int idGenerado = cmd.getInt("p_id");
                    producto.setIdProducto(idGenerado);
                    insertados++;
                }
            }
            
            conn.commit(); // Si todo salió bien, confirmar transacción
            System.out.println("✓ Se insertaron " + insertados + " productos exitosamente.");
            
        } catch (SQLException | ClassNotFoundException e) {
            System.err.println("✗ Error al insertar bloque de productos: " + e.getMessage());
            if (conn != null) {
                try {
                    conn.rollback(); // Revertir todos los cambios
                    System.err.println("✗ Transacción revertida. No se insertó ningún producto.");
                } catch (SQLException ex) {
                    System.err.println("✗ Error al hacer rollback: " + ex.getMessage());
                }
            }
            insertados = 0; // Ninguno fue insertado
            throw new RuntimeException("Error en la inserción masiva", e);
        } finally {
            if (conn != null) {
                try {
                    conn.setAutoCommit(true); // Restaurar el modo auto-commit
                    conn.close();
                } catch (SQLException e) {
                    System.err.println("✗ Error al cerrar conexión: " + e.getMessage());
                }
            }
        }
        
        return insertados;
    }
    @Override
    public int contarTotal() {
        return ejecutarComando(conn -> {
            try (CallableStatement cmd = conn.prepareCall("{call contarProductos()}")) {
                ResultSet rs = cmd.executeQuery();
                if (rs.next()) {
                    return rs.getInt("total");
                }
                return 0;
            } catch (SQLException e) {
                System.err.println("Error SQL en contarTotal: " + e.getMessage());
                throw new RuntimeException(e);
            }
        });
    }
    
    @Override
    public int obtenerStockActual(int idProducto) {
        return ejecutarComando(conn -> {
            try (CallableStatement cmd = conn.prepareCall("{call obtenerStockActualPorIdProducto(?)}")) {
                cmd.setInt(1, idProducto);
                ResultSet rs = cmd.executeQuery();
                if (rs.next()) {
                    return rs.getInt("stockActual");
                }
                return 0;
            } catch (SQLException e) {
                System.err.println("Error SQL en obtenerStockActual: " + e.getMessage());
                throw new RuntimeException(e);
            }
        });
    }
}