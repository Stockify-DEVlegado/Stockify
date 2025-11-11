package pe.edu.pucp.inf30.stockify.bo.gestion;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.GregorianCalendar;
import java.util.List;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.MethodOrderer;
import org.junit.jupiter.api.Order;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.junit.jupiter.api.TestMethodOrder;

import static org.junit.jupiter.api.Assertions.*;
import pe.edu.pucp.inf30.stockify.bo.GestionableProbable;

import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenVentaBOImpl;
import pe.edu.pucp.inf30.stockify.dao.almacen.CategoriaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
import pe.edu.pucp.inf30.stockify.dao.usuario.EmpresaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenVenta;

@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class OrdenVentaBOTest implements GestionableProbable{
    private int testCategoriaId;
    private int testProductoId;
    private int testEmpresaId;
    private int testOrdenVentaId;
    
    private final int idIncorrecto = 99999;
    
    private final OrdenVentaBOImpl ordenVentaBO = new OrdenVentaBOImpl();
    
    @BeforeAll
    public void inicializar() {
        
        CategoriaDAO categoriaDao = new CategoriaDAOImpl();
        Categoria categoria = new Categoria();
        categoria.setNombre("Categoria Test");
        this.testCategoriaId = categoriaDao.crear(categoria);
        
        ProductoDAO productoDao = new ProductoDAOImpl();
        Producto producto = new Producto();
        producto.setNombre("NombreProdTest");
        producto.setDescripcion("DescripcionProdTest");
        producto.setMarca("MarcaProdTest");
        producto.setStockMinimo(10);
        producto.setStockMaximo(20);
        producto.setPrecioUnitario(200.00);
        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
        this.testProductoId = productoDao.crear(producto);
        
        EmpresaDAO empresaDao = new EmpresaDAOImpl();
        Empresa empresa = new Empresa();
        empresa.setTipoDocumento(TipoDocumento.DNI);
        empresa.setRazonSocial("RazonSocialSACTest");
        empresa.setTelefono("999999999");
        empresa.setEmail("test@pucp.edu.pe");
        empresa.setActivo(true);
        empresa.setTipoEmpresa(TipoEmpresa.CLIENTE);
        this.testEmpresaId = empresaDao.crear(empresa);
        
    }
    
    @AfterAll
    public void limpiar() {
//        if(this.testProductoId > 0) new ProductoDAOImpl().eliminar(this.testProductoId);
//        if(this.testCategoriaId > 0) new CategoriaDAOImpl().eliminar(this.testCategoriaId);
//        if(this.testEmpresaId > 0) new EmpresaDAOImpl().eliminar(this.testEmpresaId);
    }
    
    @Test
    @Order(1)
    @Override
    public void debeListar() {
        List<OrdenVenta> lista = ordenVentaBO.listar();
        assertNotNull(lista);
    }

    @Test
    @Order(2)
    @Override
    public void debeGuardarNuevo() {
        crearOrdenVenta();
        assertTrue(this.testOrdenVentaId > 0, "El ID de OrdenVenta no se pudo recuperar.");
    }
    
    @Test
    @Order(3)
    @Override
    public void debeObtenerSiIdExiste() {
        OrdenVenta orden = ordenVentaBO.obtener(this.testOrdenVentaId);
        assertNotNull(orden);
        assertEquals(this.testOrdenVentaId, orden.getIdOrdenVenta());
    }
    
    @Test
    @Order(4)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        OrdenVenta orden = ordenVentaBO.obtener(this.idIncorrecto);
        assertNull(orden);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        OrdenVenta orden = ordenVentaBO.obtener(this.testOrdenVentaId);
        orden.setTotal(300.0); // Cambiamos el total para distinguirlo
        ordenVentaBO.guardar(orden, Estado.MODIFICADO);

        OrdenVenta modificada = ordenVentaBO.obtener(this.testOrdenVentaId);
        assertEquals(300.0, modificada.getTotal());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        // Un eliminar exitoso NO debe lanzar excepción
        assertDoesNotThrow(() -> ordenVentaBO.eliminar(this.testOrdenVentaId));
        OrdenVenta orden = ordenVentaBO.obtener(this.testOrdenVentaId);
        assertNull(orden);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        // Asumimos que tu BO SÍ lanza excepción, como OrdenCompra
        assertThrows(RuntimeException.class, () -> ordenVentaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        OrdenVenta orden = new OrdenVenta();
        orden.setCliente(new EmpresaDAOImpl().leer(this.testEmpresaId));
        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
        orden.setLineas(new ArrayList<>());

        LineaOrdenVenta linea = new LineaOrdenVenta();
        linea.setCantidad(1);
        linea.setSubtotal(10.0);
        orden.getLineas().add(linea);

        assertThrows(RuntimeException.class, () -> ordenVentaBO.guardar(orden, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertThrows(RuntimeException.class, () -> ordenVentaBO.eliminar(idIncorrecto));
    }
    
    private void crearOrdenVenta() {
        OrdenVenta orden = new OrdenVenta();
        orden.setCliente(new EmpresaDAOImpl().leer(this.testEmpresaId));
        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
        orden.setEstado(EstadoDocumento.PROCESADO);

        LineaOrdenVenta linea = new LineaOrdenVenta();
        linea.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
        linea.setCantidad(2);
        linea.setSubtotal(200.0); // Total 200.0

        List<LineaOrdenVenta> lineas = new ArrayList<>();
        lineas.add(linea);
        orden.setLineas(lineas);
        orden.setTotal(200.0);

        ordenVentaBO.guardar(orden, Estado.NUEVO);
        
        // --- WORKAROUND CORREGIDO ---
        List<OrdenVenta> lista = ordenVentaBO.listar();
        for (OrdenVenta o : lista) {
            if (o.getCliente() != null && 
                o.getCliente().getIdEmpresa() == this.testEmpresaId &&
                o.getTotal() == 200.0) {
                
                this.testOrdenVentaId = o.getIdOrdenVenta();
                break;
            }
        }
    }
    
}