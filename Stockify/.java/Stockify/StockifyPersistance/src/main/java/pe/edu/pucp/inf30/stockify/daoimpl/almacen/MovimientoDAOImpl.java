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
    import pe.edu.pucp.inf30.stockify.dao.almacen.MovimientoDAO;
    import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
    import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento;
    import pe.edu.pucp.inf30.stockify.model.almacen.TipoMovimiento;
    import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenIngresoDAOImpl;
    import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenSalidaDAOImpl;
    import pe.edu.pucp.inf30.stockify.model.dto.MovimientoMesDTO;

    /**
     *
     * @author DEVlegado
     */

    public class MovimientoDAOImpl extends BaseDAO<Movimiento> 
            implements MovimientoDAO {

        @Override
        protected PreparedStatement comandoCrear(Connection conn, Movimiento modelo) 
                throws SQLException {

            String sql = "{call insertarMovimiento(?, ?, ?, ?, ?, ?, ?, ?)}";
            CallableStatement cmd = conn.prepareCall(sql);
            cmd.setString("p_tipoMovimiento", String.valueOf(modelo.getTipoMovimiento()));
            cmd.setDate("p_fecha",new java.sql.Date(modelo.getFecha().getTime()));
            cmd.setString("p_descripcion", modelo.getDescripcion());
            if(modelo.getLineaOrdenIngreso() != null) {
                cmd.setInt("p_idLineaOrdenIngreso", modelo.getLineaOrdenIngreso().getIdLineaOrdenIngreso());
            } else {
                cmd.setNull("p_idLineaOrdenIngreso", Types.INTEGER);
            }
            if(modelo.getLineaOrdenSalida() != null) {
                cmd.setInt("p_idLineaOrdenSalida", modelo.getLineaOrdenSalida().getIdLineaOrdenSalida());
            } else {
                cmd.setNull("p_idLineaOrdenSalida", Types.INTEGER);
            }
            if(modelo.getProducto() != null) {
                cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
            } else  {
                cmd.setNull("p_idProducto", Types.INTEGER);
            }
            cmd.setInt("p_cantidad", modelo.getCantidad());
            cmd.registerOutParameter("p_id", Types.INTEGER);
            return cmd;
        }

        @Override
        protected PreparedStatement comandoActualizar(Connection conn, 
                Movimiento modelo) throws SQLException {

            String sql = "{call modificarMovimiento(?, ?, ?, ?, ?, ?, ?, ?)}";
            CallableStatement cmd = conn.prepareCall(sql);
            cmd.setString("p_tipoMovimiento", String.valueOf(modelo.getTipoMovimiento()));
            cmd.setDate("p_fecha",new java.sql.Date(modelo.getFecha().getTime()));
            cmd.setString("p_descripcion", modelo.getDescripcion());
            if(modelo.getLineaOrdenIngreso() != null) {
                cmd.setInt("p_idLineaOrdenIngreso", modelo.getLineaOrdenIngreso().getIdLineaOrdenIngreso());
            } else {
                cmd.setNull("p_idLineaOrdenIngreso", Types.INTEGER);
            }
            if(modelo.getLineaOrdenSalida() != null) {
                cmd.setInt("p_idLineaOrdenSalida", modelo.getLineaOrdenSalida().getIdLineaOrdenSalida());
            } else {
                cmd.setNull("p_idLineaOrdenSalida", Types.INTEGER);
            }
            if(modelo.getProducto() != null) {
                cmd.setInt("p_idProducto", modelo.getProducto().getIdProducto());
            } else  {
                cmd.setNull("p_idProducto", Types.INTEGER);
            }
            cmd.setInt("p_cantidad",modelo.getCantidad());
            cmd.setInt("p_id", modelo.getIdMovimiento());
            return cmd;
        }

        @Override
        protected PreparedStatement comandoEliminar(Connection conn, Integer id) 
                throws SQLException {
            String sql = "{call eliminarMovimiento(?)}";
            CallableStatement cmd = conn.prepareCall(sql);
            cmd.setInt("p_id", id);
            return cmd;
        }

        @Override
        protected PreparedStatement comandoLeer(Connection conn, Integer id) 
                throws SQLException {
            String sql = "{call buscarMovimientoPorId(?)}";
            CallableStatement cmd = conn.prepareCall(sql);
            cmd.setInt("p_id", id);
            return cmd;
        }

        @Override
        protected PreparedStatement comandoLeerTodos(Connection conn) 
                throws SQLException {
            String sql = "{call listarMovimientos()}";
            CallableStatement cmd = conn.prepareCall(sql);
            return cmd;
        }

        @Override
        protected Movimiento mapearModelo(ResultSet rs) throws SQLException {
            Movimiento movimiento = new Movimiento();
            movimiento.setIdMovimiento(rs.getInt("idMovimiento"));
            movimiento.setDescripcion(rs.getString("descripcion"));
            movimiento.setTipoMovimiento(TipoMovimiento.valueOf(rs.getString("tipoMovimiento")));
            movimiento.setFecha(rs.getTimestamp("fecha"));
            int idLineaOrdenIngreso = rs.getInt("idLineaOrdenIngreso");
            if(!rs.wasNull()) {
                movimiento.setLineaOrdenIngreso(new LineaOrdenIngresoDAOImpl().leer(idLineaOrdenIngreso));
            }
            int idLineaOrdenSalida = rs.getInt("idLineaOrdenSalida");
            if(!rs.wasNull()) {
                movimiento.setLineaOrdenSalida(new LineaOrdenSalidaDAOImpl().leer(idLineaOrdenSalida));
            }
            int idProducto = rs.getInt("idProducto");
            if(!rs.wasNull()) {
                movimiento.setProducto(new ProductoDAOImpl().leer(idProducto));
            }
            movimiento.setCantidad(rs.getInt("cantidad"));
            return movimiento;
        }
        
        @Override
        public int contarPorTipo(String tipo, int dias) {
            return ejecutarComando(conn -> {
                String procedure = tipo.equals("ENTRADA") ? 
                    "{call contarMovimientosEntrada(?)}" : 
                    "{call contarMovimientosSalida(?)}";

                try (CallableStatement cmd = conn.prepareCall(procedure)) {
                    cmd.setInt(1, dias);
                    ResultSet rs = cmd.executeQuery();
                    if (rs.next()) {
                        return rs.getInt("total");
                    }
                    return 0;
                } catch (SQLException e) {
                    System.err.println("Error SQL en contarPorTipo: " + e.getMessage());
                    throw new RuntimeException(e);
                }
            });
        }

        @Override
        public List<MovimientoMesDTO> obtenerMovimientosPorMes(int meses) {
            return ejecutarComando(conn -> {
                try (CallableStatement cmd = conn.prepareCall("{call obtenerMovimientosPorMes(?)}")) {
                    cmd.setInt(1, meses);
                    ResultSet rs = cmd.executeQuery();
                    List<MovimientoMesDTO> resultados = new ArrayList<>();

                    while (rs.next()) {
                        MovimientoMesDTO dato = new MovimientoMesDTO(
                            rs.getString("mes"),
                            rs.getInt("numeroMes"),
                            rs.getInt("entradas"),
                            rs.getInt("salidas")
                        );
                        resultados.add(dato);
                    }
                    return resultados;
                } catch (SQLException e) {
                    System.err.println("Error SQL en obtenerMovimientosPorMes: " + e.getMessage());
                    throw new RuntimeException(e);
                }
            });
        }
   
    
    }