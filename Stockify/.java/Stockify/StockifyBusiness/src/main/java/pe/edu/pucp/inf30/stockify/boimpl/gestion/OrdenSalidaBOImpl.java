/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenSalidaDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenSalidaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenSalidaDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenSalidaDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenSalida;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenSalida;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */
public class OrdenSalidaBOImpl implements Gestionable<OrdenSalida>{
    private final OrdenSalidaDAO ordenSalidaDao;
    private final LineaOrdenSalidaDAO lineaOrdenSalidaDao;
    
    public OrdenSalidaBOImpl() {
        this.ordenSalidaDao = new OrdenSalidaDAOImpl();
        this.lineaOrdenSalidaDao = new LineaOrdenSalidaDAOImpl();
    }
    
    @Override
    public List<OrdenSalida> listar() {
        return this.ordenSalidaDao.leerTodos();
    }
    
    @Override
    public OrdenSalida obtener(int id) {
        OrdenSalida orden = ordenSalidaDao.leer(id);
        if(orden == null) return null;
        
        List<LineaOrdenSalida> lineas =
                lineaOrdenSalidaDao.leerTodosPorOrden(id);
        orden.setLineas(lineas);
        return orden;
    }
    
    @Override
    public void eliminar(int id) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try(Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);
            
            try {
                List<LineaOrdenSalida> lineas = 
                        lineaOrdenSalidaDao.leerTodosPorOrden(id, conn);
                for(LineaOrdenSalida linea : lineas) {
                    if(linea.getOrdenSalida().getIdOrdenSalida() == id) {
                        lineaOrdenSalidaDao.eliminar(linea.getIdLineaOrdenSalida(), conn);
                    }
                }
                
                if(!ordenSalidaDao.eliminar(id,conn)) {
                    throw new RuntimeException("La Orden: " + id + ", "
                            + "no se pudo eliminar");
                }
                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error eliminando OrdenSalida"
                        + "con id=" + id, ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al eliminar "
                    + "OrdenSalida", e);
        }
    }
    
    @Override
    public void guardar(OrdenSalida modelo, Estado estado) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);

            try {
                switch (estado) {
                    case NUEVO -> {
                        int idOrden = this.ordenSalidaDao.crear(modelo, conn);
                        modelo.setIdOrdenSalida(idOrden);
                        for (LineaOrdenSalida linea : modelo.getLineas()) {
                            linea.setOrdenSalida(modelo);
                            lineaOrdenSalidaDao.crear(linea, conn);
                        }
                    }
                    case MODIFICADO -> {
                        ordenSalidaDao.actualizar(modelo, conn);
                        for (LineaOrdenSalida linea : modelo.getLineas()) {
                            if (linea.getIdLineaOrdenSalida()== 0) {
                                linea.setOrdenSalida(modelo);
                                lineaOrdenSalidaDao.crear(linea, conn);
                            } else {
                                lineaOrdenSalidaDao.actualizar(linea, conn);
                            }
                        }
                    }
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error guardando OrdenSalida", ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al guardar OrdenSalida"
                    + "", e);
        }
    
    }
}
