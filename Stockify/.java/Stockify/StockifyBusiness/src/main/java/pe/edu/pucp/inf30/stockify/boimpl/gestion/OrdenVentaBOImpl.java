/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenVentaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenVentaDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenVentaDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */
public class OrdenVentaBOImpl implements Gestionable<OrdenVenta>{
    private final OrdenVentaDAO ordenVentaDao;
    private final LineaOrdenVentaDAO lineaOrdenVentaDao;
    
    public OrdenVentaBOImpl() {
        this.ordenVentaDao = new OrdenVentaDAOImpl();
        this.lineaOrdenVentaDao = new LineaOrdenVentaDAOImpl();
    }
    
    @Override
    public List<OrdenVenta> listar() {
        return this.ordenVentaDao.leerTodos();
    }
    
    @Override
    public OrdenVenta obtener(int id) {
        OrdenVenta orden = ordenVentaDao.leer(id);
        if(orden == null) return null;
        
        List<LineaOrdenVenta> lineas =
                lineaOrdenVentaDao.leerTodosPorOrden(id);
        orden.setLineas(lineas);
        return orden;
    }
    
    @Override
    public void eliminar(int id) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try(Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);
            
            try {
                List<LineaOrdenVenta> lineas = 
                        lineaOrdenVentaDao.leerTodosPorOrden(id, conn);
                for(LineaOrdenVenta linea : lineas) {
                    if(linea.getOrdenVenta().getIdOrdenVenta() == id) {
                        lineaOrdenVentaDao.eliminar(linea.getIdLineaOrdenVenta(), conn);
                    }
                }
                
                if(!ordenVentaDao.eliminar(id,conn)) {
                    throw new RuntimeException("La Orden: " + id + ", "
                            + "no se pudo eliminar");
                }
                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error eliminando OrdenVenta"
                        + "con id=" + id, ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al eliminar "
                    + "OrdenVenta", e);
        }
    }
    
    @Override
    public void guardar(OrdenVenta modelo, Estado estado) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);

            try {
                switch (estado) {
                    case NUEVO -> {
                        int idOrden = this.ordenVentaDao.crear(modelo, conn);
                        modelo.setIdOrdenVenta(idOrden);
                        for (LineaOrdenVenta linea : modelo.getLineas()) {
                            linea.setOrdenVenta(modelo);
                            lineaOrdenVentaDao.crear(linea, conn);
                        }
                    }
                    case MODIFICADO -> {
                        ordenVentaDao.actualizar(modelo, conn);
                        for (LineaOrdenVenta linea : modelo.getLineas()) {
                            if (linea.getIdLineaOrdenVenta()== 0) {
                                linea.setOrdenVenta(modelo);
                                lineaOrdenVentaDao.crear(linea, conn);
                            } else {
                                lineaOrdenVentaDao.actualizar(linea, conn);
                            }
                        }
                    }
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error guardando OrdenVenta", ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al guardar OrdenVenta"
                    + "", e);
        }
    
    }
    
}
