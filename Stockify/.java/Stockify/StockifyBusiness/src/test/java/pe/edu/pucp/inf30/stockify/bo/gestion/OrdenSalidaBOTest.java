///*
// * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
// * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
// */
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
//import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenSalidaBOImpl;
//import pe.edu.pucp.inf30.stockify.boimpl.gestion.OrdenVentaBOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.personal.CuentaUsuarioDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.personal.UsuarioDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.usuario.EmpresaDAOImpl;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//import pe.edu.pucp.inf30.stockify.model.EstadoDocumento;
//import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
//import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
//import pe.edu.pucp.inf30.stockify.model.gestion.LineaOrdenSalida;
//import pe.edu.pucp.inf30.stockify.model.gestion.OrdenSalida;
//import pe.edu.pucp.inf30.stockify.model.gestion.OrdenVenta;
//import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;
//import pe.edu.pucp.inf30.stockify.model.personal.TipoUsuario;
//import pe.edu.pucp.inf30.stockify.model.personal.Usuario;
//import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;
//
///**
// *
// * @author DEVlegado
// */
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class OrdenSalidaBOTest implements GestionableProbable {
//    
//    private int testCategoriaId;
//    private int testProductoId;
//    private int testEmpresaId;
//    private int testCuentaUsuarioId;
//    private int testUsuarioId;
//    private int testOrdenVentaId;
//    private int testOrdenSalidaId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final OrdenSalidaBOImpl ordenSalidaBO = new OrdenSalidaBOImpl();
//    private final OrdenVentaBOImpl ordenVentaBO = new OrdenVentaBOImpl();
//    
//    @BeforeAll
//    public void inicializar() {
//        // Crear Categoria
//        CategoriaDAOImpl categoriaDao = new CategoriaDAOImpl();
//        Categoria categoria = new Categoria();
//        categoria.setNombre("Categoria Test OrdenSalida");
//        this.testCategoriaId = categoriaDao.crear(categoria);
//        
//        // Crear Producto
//        ProductoDAOImpl productoDao = new ProductoDAOImpl();
//        Producto producto = new Producto();
//        producto.setNombre("Producto Test OrdenSalida");
//        producto.setDescripcion("Descripcion Test");
//        producto.setMarca("Marca Test");
//        producto.setStockMinimo(10);
//        producto.setStockMaximo(100);
//        producto.setPrecioUnitario(250.0);
//        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
//        this.testProductoId = productoDao.crear(producto);
//        
//        // Crear Empresa Cliente
//        EmpresaDAOImpl empresaDao = new EmpresaDAOImpl();
//        Empresa empresa = new Empresa();
//        empresa.setTipoDocumento(TipoDocumento.RUC);
//        empresa.setRazonSocial("Cliente Test OrdenSalida");
//        empresa.setTelefono("987654321");
//        empresa.setEmail("clientesalida@test.com");
//        empresa.setActivo(true);
//        empresa.setTipoEmpresa(TipoEmpresa.CLIENTE);
//        this.testEmpresaId = empresaDao.crear(empresa);
//        
//        // Crear Cuenta Usuario
//        CuentaUsuarioDAOImpl cuentaDao = new CuentaUsuarioDAOImpl();
//        CuentaUsuario cuenta = new CuentaUsuario();
//        cuenta.setUsername("usuariosalida");
//        cuenta.setPassword("password123");
//        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
//        this.testCuentaUsuarioId = cuentaDao.crear(cuenta);
//        
//        // Crear Usuario
//        UsuarioDAOImpl usuarioDao = new UsuarioDAOImpl();
//        Usuario usuario = new Usuario();
//        usuario.setNombres("Usuario");
//        usuario.setApellidos("Test Salida");
//        usuario.setEmail("usuariosalida@test.com");
//        usuario.setTelefono("987654321");
//        usuario.setActivo(true);
//        usuario.setTipoUsuario(TipoUsuario.OPERARIO);
//        usuario.setCuenta(cuentaDao.leer(this.testCuentaUsuarioId));
//        this.testUsuarioId = usuarioDao.crear(usuario);
//        
//        // Crear Orden de Venta
//        OrdenVenta ordenVenta = new OrdenVenta();
//        ordenVenta.setCliente(empresaDao.leer(this.testEmpresaId));
//        ordenVenta.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 20).getTime());
//        ordenVenta.setEstado(EstadoDocumento.PROCESADO);
//        ordenVenta.setTotal(0.0);
//        ordenVenta.setLineas(new ArrayList<>());
//        ordenVentaBO.guardar(ordenVenta, Estado.NUEVO);
//        this.testOrdenVentaId = ordenVenta.getIdOrdenVenta();
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        if (this.testOrdenVentaId > 0) {
////            ordenVentaBO.eliminar(this.testOrdenVentaId);
////        }
////        new UsuarioDAOImpl().eliminar(this.testUsuarioId);
////        new CuentaUsuarioDAOImpl().eliminar(this.testCuentaUsuarioId);
////        new ProductoDAOImpl().eliminar(this.testProductoId);
////        new CategoriaDAOImpl().eliminar(this.testCategoriaId);
////        new EmpresaDAOImpl().eliminar(this.testEmpresaId);
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<OrdenSalida> lista = ordenSalidaBO.listar();
//        assertNotNull(lista);
//    }
//    
//    @Test
//    @Order(2)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        crearOrdenSalida();
//        OrdenSalida orden = ordenSalidaBO.obtener(this.testOrdenSalidaId);
//        assertNotNull(orden);
//        assertEquals(this.testOrdenSalidaId, orden.getIdOrdenSalida());
//    }
//    
//    @Test
//    @Order(3)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        OrdenSalida orden = ordenSalidaBO.obtener(this.idIncorrecto);
//        assertNull(orden);
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void debeGuardarNuevo() {
//        crearOrdenSalida();
//        assertTrue(this.testOrdenSalidaId > 0);
//    }
//    
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        OrdenSalida orden = ordenSalidaBO.obtener(this.testOrdenSalidaId);
//        orden.setTotal(2000.0);
//        orden.setEstado(EstadoDocumento.COMPLETADO);
//        ordenSalidaBO.guardar(orden, Estado.MODIFICADO);
//
//        OrdenSalida modificada = ordenSalidaBO.obtener(this.testOrdenSalidaId);
//        assertEquals(2000.0, modificada.getTotal());
//        assertEquals(EstadoDocumento.COMPLETADO, modificada.getEstado());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        ordenSalidaBO.eliminar(this.testOrdenSalidaId);
//        OrdenSalida orden = ordenSalidaBO.obtener(this.testOrdenSalidaId);
//        assertNull(orden);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertThrows(RuntimeException.class, () -> ordenSalidaBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        OrdenSalida orden = new OrdenSalida();
//        orden.setResponsable(new UsuarioDAOImpl().leer(this.testUsuarioId));
//        orden.setOrdenVenta(ordenVentaBO.obtener(this.testOrdenVentaId));
//        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
//        orden.setLineas(new ArrayList<>());
//
//        // Forzamos un error: linea sin producto
//        LineaOrdenSalida linea = new LineaOrdenSalida();
//        linea.setCantidad(1);
//        linea.setSubtotal(10.0);
//        orden.getLineas().add(linea);
//
//        assertThrows(RuntimeException.class, () -> ordenSalidaBO.guardar(orden, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertThrows(RuntimeException.class, () -> ordenSalidaBO.eliminar(idIncorrecto));
//    }
//    
//    private void crearOrdenSalida() {
//        OrdenSalida orden = new OrdenSalida();
//        orden.setResponsable(new UsuarioDAOImpl().leer(this.testUsuarioId));
//        orden.setOrdenVenta(ordenVentaBO.obtener(this.testOrdenVentaId));
//        orden.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 25).getTime());
//        orden.setEstado(EstadoDocumento.PROCESADO);
//
//        LineaOrdenSalida linea = new LineaOrdenSalida();
//        linea.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
//        linea.setCantidad(4);
//        linea.setSubtotal(1000.0);
//
//        List<LineaOrdenSalida> lineas = new ArrayList<>();
//        lineas.add(linea);
//        orden.setLineas(lineas);
//        orden.setTotal(1000.0);
//
//        ordenSalidaBO.guardar(orden, Estado.NUEVO);
//        this.testOrdenSalidaId = orden.getIdOrdenSalida();
//    }
//}