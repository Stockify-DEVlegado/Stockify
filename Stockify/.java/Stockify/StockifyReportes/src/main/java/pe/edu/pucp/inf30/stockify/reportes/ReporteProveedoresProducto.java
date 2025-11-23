
package pe.edu.pucp.inf30.stockify.reportes;

import java.io.IOException;
import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import java.awt.Image;
import java.io.FileNotFoundException;
import java.io.InputStream;
import java.sql.Connection;
import java.sql.SQLException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import javax.imageio.ImageIO;
import net.sf.jasperreports.engine.JRException;
import net.sf.jasperreports.engine.JasperExportManager;
import net.sf.jasperreports.engine.JasperFillManager;
import net.sf.jasperreports.engine.JasperPrint;
import pe.edu.pucp.inf30.stockify.db.DBFactoryProvider;


/**
 *
 * @author carlo
 */

@WebServlet(name = "ReporteProveedoresProducto", urlPatterns = {"/reportes/proveedoresProducto"})
public class ReporteProveedoresProducto extends HttpServlet {

    private final String NOMBRE_REPORTE = "reportes/Reporte_ProveedoresPorProducto.jasper";
    private final String NOMBRE_LOGO = "imagenes/LogoLetrasEnNegro.png";
    
    protected void processRequest(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        
        response.setContentType("application/pdf");
        
        InputStream reporte = getClass().getClassLoader().getResourceAsStream(NOMBRE_REPORTE);
        if (reporte == null) {
            throw new FileNotFoundException("No se encontro el reporte: " + NOMBRE_REPORTE);
        }
        
        Map<String, Object> parametros = new HashMap<>();
        
        // 1. Parámetro: logo
        InputStream logo = getClass().getClassLoader().getResourceAsStream(NOMBRE_LOGO);
        if (logo == null) {
            throw new FileNotFoundException("No se encontro el logo: " + NOMBRE_LOGO);
        }
        Image imagen = ImageIO.read(logo);
        parametros.put("logo", imagen);
        
        // 2. Parámetro: idProducto (REQUERIDO)
        String idProductoStr = request.getParameter("idProducto");
        if (idProductoStr == null || idProductoStr.isEmpty()) {
            throw new RuntimeException("El parametro idProducto es requerido.");
        }
        Integer idProducto = Integer.parseInt(idProductoStr);
        parametros.put("idProducto", idProducto);
        
        String fechaDesdeStr = request.getParameter("fechaDesde");
        if (fechaDesdeStr == null || fechaDesdeStr.isEmpty()) {
            throw new RuntimeException("El parametro fechaDesde es requerido.");
        }

        String fechaHastaStr = request.getParameter("fechaHasta");
        if (fechaHastaStr == null || fechaHastaStr.isEmpty()) {
            throw new RuntimeException("El parametro fechaHasta es requerido.");
        }
        
        try (Connection conn = DBFactoryProvider.getManager().getConnection()) {
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
            Date fechaDesde = sdf.parse(fechaDesdeStr);
            Date fechaHasta = sdf.parse(fechaHastaStr);

            parametros.put("fechaDesde", fechaDesde);
            parametros.put("fechaHasta", fechaHasta);

            JasperPrint jp = JasperFillManager.fillReport(reporte, parametros, conn);
            JasperExportManager.exportReportToPdfStream(jp, response.getOutputStream());
        }
        catch (SQLException | ClassNotFoundException | JRException | ParseException ex) {
            response.sendError(HttpServletResponse.SC_INTERNAL_SERVER_ERROR, 
                    "Error al generar el reporte: " + ex.getMessage());
        }
    }

    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        processRequest(request, response);
    }

    @Override
    public String getServletInfo() {
        return "Short description";
    }

}
