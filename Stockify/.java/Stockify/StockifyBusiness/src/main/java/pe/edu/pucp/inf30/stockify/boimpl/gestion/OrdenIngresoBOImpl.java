/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenIngresoDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenIngresoDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenIngresoDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenIngresoDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */
public class OrdenIngresoBOImpl implements Gestionable<OrdenIngreso>{
    private final OrdenIngresoDAO ordenIngresoDao;
    private final LineaOrdenIngresoDAO lineaOrdenIngresoDao;
    
    public OrdenIngresoBOImpl() {
        this.ordenIngresoDao = new OrdenIngresoDAOImpl();
        this.lineaOrdenIngresoDao = new LineaOrdenIngresoDAOImpl();
    }
    
    @Override
    public List<OrdenIngreso> listar() {
        return this.ordenIngresoDao.leerTodos();
    }
    
    @Override
    public OrdenIngreso obtener(int id) {
        OrdenIngreso orden = ordenIngresoDao.leer(id);
        if(orden == null) return null;
        
        List<LineaOrdenIngreso> lineas =
                lineaOrdenIngresoDao.leerTodosPorOrden(id);
        orden.setLineas(lineas);
        return orden;
    }
    
    @Override
    public void eliminar(int id) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try(Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);
            
            try {
                List<LineaOrdenIngreso> lineas = 
                        lineaOrdenIngresoDao.leerTodosPorOrden(id, conn);
                for(LineaOrdenIngreso linea : lineas) {
                    if(linea.getOrdenIngreso().getIdOrdenIngreso() == id) {
                        lineaOrdenIngresoDao.eliminar(linea.getIdLineaOrdenIngreso(), conn);
                    }
                }
                
                if(!ordenIngresoDao.eliminar(id,conn)) {
                    throw new RuntimeException("La Orden: " + id + ", "
                            + "no se pudo eliminar");
                }
                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error eliminando OrdenIngreso"
                        + "con id=" + id, ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al eliminar "
                    + "OrdenIngreso", e);
        }
    }
    
    @Override
    public void guardar(OrdenIngreso modelo, Estado estado) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);

            try {
                switch (estado) {
                    case NUEVO -> {
                        int idOrden = this.ordenIngresoDao.crear(modelo, conn);
                        modelo.setIdOrdenIngreso(idOrden);
                        for (LineaOrdenIngreso linea : modelo.getLineas()) {
                            linea.setOrdenIngreso(modelo);
                            lineaOrdenIngresoDao.crear(linea, conn);
                        }
                    }
                    case MODIFICADO -> {
                        ordenIngresoDao.actualizar(modelo, conn);
                        for (LineaOrdenIngreso linea : modelo.getLineas()) {
                            if (linea.getIdLineaOrdenIngreso()== 0) {
                                linea.setOrdenIngreso(modelo);
                                lineaOrdenIngresoDao.crear(linea, conn);
                            } else {
                                lineaOrdenIngresoDao.actualizar(linea, conn);
                            }
                        }
                    }
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error guardando OrdenIngreso", ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al guardar OrdenIngreso"
                    + "", e);
        }
    
    }
}
