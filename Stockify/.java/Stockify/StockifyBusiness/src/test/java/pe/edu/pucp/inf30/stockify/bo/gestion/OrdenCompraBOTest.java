//package pe.edu.pucp.inf30.stockify.bo.gestion;
//
//import java.util.ArrayList;
//import java.util.Calendar;
//import java.util.GregorianCalendar;
//import java.util.List;
//import org.junit.jupiter.api.AfterAll;
//import org.junit.jupiter.api.BeforeAll;
//import org.junit.jupiter.api.MethodOrderer;
//import org.junit.jupiter.api.Order;
//import org.junit.jupiter.api.Test;
//import org.junit.jupiter.api.TestInstance;
//import org.junit.jupiter.api.TestMethodOrder;
//
//import static org.junit.jupiter.api.Assertions.*;
//import pe.edu.pucp.inf30.stockify.bo.GestionableProbable;
//import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenCompraBOImpl;
//import pe.edu.pucp.inf30.stockify.dao.almacen.CategoriaDAO;
//import pe.edu.pucp.inf30.stockify.dao.almacen.ProductoDAO;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
//import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
//import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
//import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;
//import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
//import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenCompra;
//
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class OrdenCompraBOTest implements GestionableProbable{
//    private int testCategoriaId;
//    private int testProductoId;
//    private int testEmpresaId;
//    private int testOrdenCompraId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final OrdenCompraBOImpl ordenCompraBO = new OrdenCompraBOImpl();
//    
//    @BeforeAll
//    public void inicializar() {
//        
//        CategoriaDAO categoriaDao = new CategoriaDAOImpl();
//        Categoria categoria = new Categoria();
//        categoria.setNombre("Categoria Test");
//        this.testCategoriaId = categoriaDao.crear(categoria);
//        
//        ProductoDAO productoDao = new ProductoDAOImpl();
//        Producto producto = new Producto();
//        producto.setNombre("NombreProdTest");
//        producto.setDescripcion("DescripcionProdTest");
//        producto.setMarca("MarcaProdTest");
//        producto.setStockMinimo(10);
//        producto.setStockMaximo(20);
//        producto.setPrecioUnitario(200.00);
//        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
//        this.testProductoId = productoDao.crear(producto);
//        
//        EmpresaDAOImpl empresaDao = new EmpresaDAOImpl();
//        Empresa empresa = new Empresa();
//        empresa.setTipoDocumento(TipoDocumento.DNI);
//        empresa.setRazonSocial("RazonSocialSACTest");
//        empresa.setTelefono("999999999");
//        empresa.setEmail("test@pucp.edu.pe");
//        empresa.setActivo(true);
//        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
//        this.testEmpresaId = empresaDao.crear(empresa);
//        
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        new CategoriaDAOImpl().eliminar(this.testCategoriaId);
////        new ProductoDAOImpl().eliminar(this.testProductoId);
////        new EmpresaDAOImpl().eliminar(this.testEmpresaId);
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<OrdenCompra> lista = ordenCompraBO.listar();
//        assertNotNull(lista);
//    }
//
//    @Test
//    @Order(2)
//    @Override
//    public void debeGuardarNuevo() {
//        crearOrdenCompra();
//        assertTrue(this.testOrdenCompraId > 0, "El ID de OrdenCompra no se pudo recuperar.");
//    }
//
//    @Test
//    @Order(3)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        OrdenCompra orden = ordenCompraBO.obtener(this.testOrdenCompraId);
//        assertNotNull(orden);
//        assertEquals(this.testOrdenCompraId, orden.getIdOrdenCompra());
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        OrdenCompra orden = ordenCompraBO.obtener(this.idIncorrecto);
//        assertNull(orden);
//    }
//    
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        OrdenCompra orden = ordenCompraBO.obtener(this.testOrdenCompraId);
//        orden.setTotal(300.0);
//        ordenCompraBO.guardar(orden, Estado.MODIFICADO);
//
//        OrdenCompra modificada = ordenCompraBO.obtener(this.testOrdenCompraId);
//        assertEquals(300.0, modificada.getTotal());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        assertDoesNotThrow(() -> ordenCompraBO.eliminar(this.testOrdenCompraId));
//        OrdenCompra orden = ordenCompraBO.obtener(this.testOrdenCompraId);
//        assertNull(orden);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertThrows(RuntimeException.class, () -> ordenCompraBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        OrdenCompra orden = new OrdenCompra();
//        orden.setProveedor(new EmpresaDAOImpl().leer(this.testEmpresaId));
//        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
//        orden.setLineas(new ArrayList<>());
//
//        LineaOrdenCompra linea = new LineaOrdenCompra();
//        linea.setCantidad(1);
//        linea.setSubtotal(10.0);
//        orden.getLineas().add(linea);
//
//        assertThrows(RuntimeException.class, () -> ordenCompraBO.guardar(orden, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertThrows(RuntimeException.class, () -> ordenCompraBO.eliminar(idIncorrecto));
//    }
//    
//    private void crearOrdenCompra() {
//        OrdenCompra orden = new OrdenCompra();
//        orden.setProveedor(new EmpresaDAOImpl().leer(this.testEmpresaId));
//        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
//        orden.setEstado(EstadoDocumento.PROCESADO);
//
//        LineaOrdenCompra linea = new LineaOrdenCompra();
//        linea.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
//        linea.setCantidad(2);
//        linea.setSubtotal(200.0); // Total es 200.0
//
//        List<LineaOrdenCompra> lineas = new ArrayList<>();
//        lineas.add(linea);
//        orden.setLineas(lineas);
//        orden.setTotal(200.0);
//
//        ordenCompraBO.guardar(orden, Estado.NUEVO);
// 
//        List<OrdenCompra> lista = ordenCompraBO.listar();
//        for (OrdenCompra o : lista) {
//            if (o.getProveedor() != null && 
//                o.getProveedor().getIdEmpresa() == this.testEmpresaId &&
//                o.getTotal() == 200.0) {
//                
//                this.testOrdenCompraId = o.getIdOrdenCompra();
//                break;
//            }
//        }
//    }
//    
//}