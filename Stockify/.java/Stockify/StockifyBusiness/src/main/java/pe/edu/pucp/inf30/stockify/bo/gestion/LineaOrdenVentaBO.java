package pe.edu.pucp.inf30.stockify.bo.gestion;

import java.io.InputStream;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;

public interface LineaOrdenVentaBO {
    int importarDesdeInputStream(InputStream inputStream, String nombreArchivo, int idOrdenVenta);
    List<LineaOrdenVenta> listarPorOrden(int idOrdenVenta);
    void eliminar(int idLinea);
    void guardar(LineaOrdenVenta linea, int idOrdenVenta);
}