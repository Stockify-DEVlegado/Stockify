package pe.edu.pucp.inf30.stockify.bo.gestion;

import java.io.InputStream;
import java.util.List;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;

public interface LineaOrdenCompraBO {
    int importarDesdeInputStream(InputStream inputStream, String nombreArchivo, int idOrdenCompra);
    List<LineaOrdenCompra> listarPorOrden(int idOrdenCompra);
    void eliminar(int idLinea);
    void guardar(LineaOrdenCompra linea, int idOrdenCompra);
}