/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.almacen;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.dao.almacen.GuiaRemisionDAO;
import pe.edu.pucp.inf30.stockify.dao.almacen.LineaGuiaRemisionDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.LineaGuiaRemisionDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.GuiaRemisionDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.almacen.GuiaRemision;
import pe.edu.pucp.inf30.stockify.model.almacen.LineaGuiaRemision;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */
public class GuiaRemisionBOImpl implements Gestionable<GuiaRemision>{
    private final GuiaRemisionDAO guiaRemisionDao;
    private final LineaGuiaRemisionDAO lineaGuiaRemisionDao;
    
    public GuiaRemisionBOImpl() {
        this.guiaRemisionDao = new GuiaRemisionDAOImpl();
        this.lineaGuiaRemisionDao = new LineaGuiaRemisionDAOImpl();
    }
    
    @Override
    public List<GuiaRemision> listar() {
        return this.guiaRemisionDao.leerTodos();
    }
    
    @Override
    public GuiaRemision obtener(int id) {
        GuiaRemision orden = guiaRemisionDao.leer(id);
        if(orden == null) return null;
        
        List<LineaGuiaRemision> lineas =
                lineaGuiaRemisionDao.leerTodosPorOrden(id);
        orden.setLineas(lineas);
        return orden;
    }
    
    @Override
    public void eliminar(int id) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try(Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);
            
            try {
                List<LineaGuiaRemision> lineas = 
                        lineaGuiaRemisionDao.leerTodosPorOrden(id, conn);
                for(LineaGuiaRemision linea : lineas) {
                    if(linea.getGuiaRemision().getIdGuiaRemision() == id) {
                        lineaGuiaRemisionDao.eliminar(linea.getIdLineaGuiaRemision(), conn);
                    }
                }
                
                if(!guiaRemisionDao.eliminar(id,conn)) {
                    throw new RuntimeException("La Orden: " + id + ", "
                            + "no se pudo eliminar");
                }
                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error eliminando GuiaRemision"
                        + "con id=" + id, ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al eliminar "
                    + "GuiaRemision", e);
        }
    }
    
    @Override
    public void guardar(GuiaRemision modelo, Estado estado) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);

            try {
                switch (estado) {
                    case NUEVO -> {
                        int idOrden = this.guiaRemisionDao.crear(modelo, conn);
                        modelo.setIdGuiaRemision(idOrden);
                        for (LineaGuiaRemision linea : modelo.getLineas()) {
                            linea.setGuiaRemision(modelo);
                            lineaGuiaRemisionDao.crear(linea, conn);
                        }
                    }
                    case MODIFICADO -> {
                        guiaRemisionDao.actualizar(modelo, conn);
                        for (LineaGuiaRemision linea : modelo.getLineas()) {
                            if (linea.getIdLineaGuiaRemision()== 0) {
                                linea.setGuiaRemision(modelo);
                                lineaGuiaRemisionDao.crear(linea, conn);
                            } else {
                                lineaGuiaRemisionDao.actualizar(linea, conn);
                            }
                        }
                    }
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error guardando GuiaRemision", ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al guardar GuiaRemision"
                    + "", e);
        }
    
    }
}
