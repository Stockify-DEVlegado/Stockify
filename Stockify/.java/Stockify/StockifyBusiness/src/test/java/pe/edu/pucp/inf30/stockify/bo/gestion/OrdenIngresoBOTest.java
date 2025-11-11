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
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenCompraBOImpl;
import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenIngresoBOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.CuentaUsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenCompra;
import pe.edu.pucp.inf30.stockify.model.gestion.OrdenIngreso;
import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;
import pe.edu.pucp.inf30.stockify.model.personal.TipoUsuario;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;

@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class OrdenIngresoBOTest implements GestionableProbable {
    
    private int testCategoriaId;
    private int testProductoId;
    private int testEmpresaId;
    private int testCuentaUsuarioId;
    private int testUsuarioId;
    private int testOrdenCompraId;
    private int testOrdenIngresoId;
    
    private final int idIncorrecto = 99999;
    
    private final OrdenIngresoBOImpl ordenIngresoBO = new OrdenIngresoBOImpl();
    private final OrdenCompraBOImpl ordenCompraBO = new OrdenCompraBOImpl();
    
    @BeforeAll
    public void inicializar() {
        CategoriaDAOImpl categoriaDao = new CategoriaDAOImpl();
        Categoria categoria = new Categoria();
        categoria.setNombre("Categoria Test OrdenIngreso");
        this.testCategoriaId = categoriaDao.crear(categoria);
        
        ProductoDAOImpl productoDao = new ProductoDAOImpl();
        Producto producto = new Producto();
        producto.setNombre("Producto Test OrdenIngreso");
        producto.setDescripcion("Descripcion Test");
        producto.setMarca("Marca Test");
        producto.setStockMinimo(10);
        producto.setStockMaximo(100);
        producto.setPrecioUnitario(180.0);
        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
        this.testProductoId = productoDao.crear(producto);
        
        EmpresaDAOImpl empresaDao = new EmpresaDAOImpl();
        Empresa empresa = new Empresa();
        empresa.setTipoDocumento(TipoDocumento.RUC);
        empresa.setRazonSocial("Proveedor Test OrdenIngreso");
        empresa.setTelefono("987654321");
        empresa.setEmail("proveedoringreso@test.com");
        empresa.setActivo(true);
        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
        this.testEmpresaId = empresaDao.crear(empresa);
        
        CuentaUsuarioDAOImpl cuentaDao = new CuentaUsuarioDAOImpl();
        CuentaUsuario cuenta = new CuentaUsuario();
        cuenta.setUsername("usuarioingreso");
        cuenta.setPassword("password123");
        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
        this.testCuentaUsuarioId = cuentaDao.crear(cuenta);
        
        UsuarioDAOImpl usuarioDao = new UsuarioDAOImpl();
        Usuario usuario = new Usuario();
        usuario.setNombres("Usuario");
        usuario.setApellidos("Test Ingreso");
        usuario.setEmail("usuarioingreso@test.com");
        usuario.setTelefono("987654321");
        usuario.setActivo(true);
        usuario.setTipoUsuario(TipoUsuario.OPERARIO);
        usuario.setCuenta(cuentaDao.leer(this.testCuentaUsuarioId));
        this.testUsuarioId = usuarioDao.crear(usuario);
        
        OrdenCompra ordenCompra = new OrdenCompra();
        ordenCompra.setProveedor(empresaDao.leer(this.testEmpresaId));
        ordenCompra.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 15).getTime());
        ordenCompra.setEstado(EstadoDocumento.PROCESADO);
        ordenCompra.setTotal(0.0);
        ordenCompra.setLineas(new ArrayList<>());
        ordenCompraBO.guardar(ordenCompra, Estado.NUEVO);
        
        List<OrdenCompra> lista = ordenCompraBO.listar();
        for (OrdenCompra o : lista) {
            if (o.getProveedor() != null &&
                o.getProveedor().getIdEmpresa() == this.testEmpresaId) {
                this.testOrdenCompraId = o.getIdOrdenCompra();
                break;
            }
        }
    }
    
    @AfterAll
    public void limpiar() {
//        if (this.testOrdenCompraId > 0) {
//            ordenCompraBO.eliminar(this.testOrdenCompraId);
//        }
//        if (this.testUsuarioId > 0) new UsuarioDAOImpl().eliminar(this.testUsuarioId);
//        if (this.testCuentaUsuarioId > 0) new CuentaUsuarioDAOImpl().eliminar(this.testCuentaUsuarioId);
//        if (this.testProductoId > 0) new ProductoDAOImpl().eliminar(this.testProductoId);
//        if (this.testCategoriaId > 0) new CategoriaDAOImpl().eliminar(this.testCategoriaId);
//        if (this.testEmpresaId > 0) new EmpresaDAOImpl().eliminar(this.testEmpresaId);
    }
    
    @Test
    @Order(1)
    @Override
    public void debeListar() {
        List<OrdenIngreso> lista = ordenIngresoBO.listar();
        assertNotNull(lista);
    }

    @Test
    @Order(2)
    @Override
    public void debeGuardarNuevo() {
        crearOrdenIngreso();
        assertTrue(this.testOrdenIngresoId > 0, "El ID de OrdenIngreso no se pudo recuperar.");
    }
    
    @Test
    @Order(3)
    @Override
    public void debeObtenerSiIdExiste() {
        OrdenIngreso orden = ordenIngresoBO.obtener(this.testOrdenIngresoId);
        assertNotNull(orden);
        assertEquals(this.testOrdenIngresoId, orden.getIdOrdenIngreso());
    }
    
    @Test
    @Order(4)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        OrdenIngreso orden = ordenIngresoBO.obtener(this.idIncorrecto);
        assertNull(orden);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        OrdenIngreso orden = ordenIngresoBO.obtener(this.testOrdenIngresoId);
        orden.setTotal(2500.0);
        orden.setEstado(EstadoDocumento.COMPLETADO);
        ordenIngresoBO.guardar(orden, Estado.MODIFICADO);

        OrdenIngreso modificada = ordenIngresoBO.obtener(this.testOrdenIngresoId);
        assertEquals(2500.0, modificada.getTotal());
        assertEquals(EstadoDocumento.COMPLETADO, modificada.getEstado());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        assertDoesNotThrow(() -> ordenIngresoBO.eliminar(this.testOrdenIngresoId));
        OrdenIngreso orden = ordenIngresoBO.obtener(this.testOrdenIngresoId);
        assertNull(orden);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        assertThrows(RuntimeException.class, () -> ordenIngresoBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        OrdenIngreso orden = new OrdenIngreso();
        orden.setResponsable(new UsuarioDAOImpl().leer(this.testUsuarioId));
        orden.setOrdenCompra(ordenCompraBO.obtener(this.testOrdenCompraId));
        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
        orden.setLineas(new ArrayList<>());

        LineaOrdenIngreso linea = new LineaOrdenIngreso();
        linea.setCantidad(1);
        linea.setSubtotal(10.0);
        orden.getLineas().add(linea);

        assertThrows(RuntimeException.class, () -> ordenIngresoBO.guardar(orden, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertThrows(RuntimeException.class, () -> ordenIngresoBO.eliminar(idIncorrecto));
    }
    
    private void crearOrdenIngreso() {
        OrdenIngreso orden = new OrdenIngreso();
        orden.setResponsable(new UsuarioDAOImpl().leer(this.testUsuarioId));
        orden.setOrdenCompra(ordenCompraBO.obtener(this.testOrdenCompraId));
        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 20).getTime());
        orden.setEstado(EstadoDocumento.PROCESADO);

        LineaOrdenIngreso linea = new LineaOrdenIngreso();
        linea.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
        linea.setCantidad(10);
        linea.setSubtotal(1800.0);

        List<LineaOrdenIngreso> lineas = new ArrayList<>();
        lineas.add(linea);
        orden.setLineas(lineas);
        orden.setTotal(1800.0);

        ordenIngresoBO.guardar(orden, Estado.NUEVO);
        
        List<OrdenIngreso> lista = ordenIngresoBO.listar();
        for (OrdenIngreso oi : lista) {
            if (oi.getResponsable() != null &&
                oi.getResponsable().getIdUsuario() == this.testUsuarioId &&
                oi.getTotal() == 1800.0) {
                
                this.testOrdenIngresoId = oi.getIdOrdenIngreso();
                break;
            }
        }
    }
}