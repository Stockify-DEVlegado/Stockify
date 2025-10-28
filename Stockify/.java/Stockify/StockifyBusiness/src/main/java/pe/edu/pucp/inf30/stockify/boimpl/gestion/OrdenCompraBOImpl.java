/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.boimpl.gestion;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import pe.edu.pucp.inf30.stockify.bo.Gestionable;
import pe.edu.pucp.inf30.stockify.dao.gestion.OrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.dao.gestion.LineaOrdenCompraDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.LineaOrdenCompraDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.gestion.OrdenCompraDAOImpl;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;
import pe.edu.pucp.inf30.stockify.db.DBManager;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;
import pe.edu.pucp.inf30.stockify.model.Estado;

/**
 *
 * @author DEVlegado
 */
public class OrdenCompraBOImpl implements Gestionable<OrdenCompra>{
    private final OrdenCompraDAO ordenCompraDao;
    private final LineaOrdenCompraDAO lineaOrdenCompraDao;
    
    public OrdenCompraBOImpl() {
        this.ordenCompraDao = new OrdenCompraDAOImpl();
        this.lineaOrdenCompraDao = new LineaOrdenCompraDAOImpl();
    }
    
    @Override
    public List<OrdenCompra> listar() {
        return this.ordenCompraDao.leerTodos();
    }
    
    @Override
    public OrdenCompra obtener(int id) {
        OrdenCompra orden = ordenCompraDao.leer(id);
        if(orden == null) return null;
        
        List<LineaOrdenCompra> lineas =
                lineaOrdenCompraDao.leerTodosPorOrden(id);
        orden.setLineas(lineas);
        return orden;
    }
    
    @Override
    public void eliminar(int id) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try(Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);
            
            try {
                List<LineaOrdenCompra> lineas = 
                        lineaOrdenCompraDao.leerTodosPorOrden(id, conn);
                for(LineaOrdenCompra linea : lineas) {
                    if(linea.getOrdenCompra().getIdOrdenCompra() == id) {
                        lineaOrdenCompraDao.eliminar(linea.getIdLineaOrdenCompra(), conn);
                    }
                }
                
                if(!ordenCompraDao.eliminar(id,conn)) {
                    throw new RuntimeException("La Orden: " + id + ", "
                            + "no se pudo eliminar");
                }
                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error eliminando OrdenCompra"
                        + "con id=" + id, ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al eliminar "
                    + "OrdenCompra", e);
        }
    }
    
    @Override
    public void guardar(OrdenCompra modelo, Estado estado) {
        DBManager dbManager = DBFactoryProvider.getManager();
        try (Connection conn = dbManager.getConnection()) {
            conn.setAutoCommit(false);

            try {
                switch (estado) {
                    case NUEVO -> {
                        int idOrden = this.ordenCompraDao.crear(modelo, conn);
                        modelo.setIdOrdenCompra(idOrden);
                        for (LineaOrdenCompra linea : modelo.getLineas()) {
                            linea.setOrdenCompra(modelo);
                            lineaOrdenCompraDao.crear(linea, conn);
                        }
                    }
                    case MODIFICADO -> {
                        ordenCompraDao.actualizar(modelo, conn);
                        for (LineaOrdenCompra linea : modelo.getLineas()) {
                            if (linea.getIdLineaOrdenCompra()== 0) {
                                linea.setOrdenCompra(modelo);
                                lineaOrdenCompraDao.crear(linea, conn);
                            } else {
                                lineaOrdenCompraDao.actualizar(linea, conn);
                            }
                        }
                    }
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw new RuntimeException("Error guardando OrdenCompra", ex);
            }
        } catch (ClassNotFoundException | SQLException e) {
            throw new RuntimeException("Error de conexión al guardar OrdenCompra"
                    + "", e);
        }
    
    }
}
