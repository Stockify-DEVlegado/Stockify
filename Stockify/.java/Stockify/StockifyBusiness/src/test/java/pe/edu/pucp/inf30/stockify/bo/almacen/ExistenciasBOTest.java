//package pe.edu.pucp.inf30.stockify.bo.almacen;
//
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
//import pe.edu.pucp.inf30.stockify.boimpl.almacen.ExistenciasBOImpl;
//import pe.edu.pucp.inf30.stockify.boimpl.almacen.MovimientoBOImpl; // IMPORTADO
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
//import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
//import pe.edu.pucp.inf30.stockify.model.almacen.EstadoExistencias;
//import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;
//import pe.edu.pucp.inf30.stockify.model.almacen.Movimiento; // IMPORTADO
//import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
//import pe.edu.pucp.inf30.stockify.model.almacen.TipoMovimiento; // IMPORTADO
//import pe.edu.pucp.inf30.stockify.model.Estado;
//
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class ExistenciasBOTest implements GestionableProbable {
//    
//    private int testCategoriaId;
//    private int testProductoId;
//    private int testExistenciaId;
//    private int testMovimientoId; // AÑADIDO
//    
//    private final int idIncorrecto = 99999;
//    
//    private final ExistenciasBOImpl existenciasBO = new ExistenciasBOImpl();
//    private final MovimientoBOImpl movimientoBO = new MovimientoBOImpl(); // AÑADIDO
//    
//    @BeforeAll
//    public void inicializar() {
//        CategoriaDAOImpl categoriaDao = new CategoriaDAOImpl();
//        Categoria categoria = new Categoria();
//        categoria.setNombre("Categoria Test Existencias");
//        this.testCategoriaId = categoriaDao.crear(categoria);
//        
//        ProductoDAOImpl productoDao = new ProductoDAOImpl();
//        Producto producto = new Producto();
//        producto.setNombre("Producto Test Existencias");
//        producto.setDescripcion("Descripcion Test");
//        // ... (resto de la creación de producto) ...
//        producto.setPrecioUnitario(180.0);
//        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
//        this.testProductoId = productoDao.crear(producto);
//
//        // --- AÑADIDO: Crear un Movimiento de Ingreso primero ---
//        Movimiento mov = new Movimiento();
//        mov.setTipoMovimiento(TipoMovimiento.ENTRADA);
//        mov.setFecha(new GregorianCalendar(2025, Calendar.JANUARY, 9).getTime());
//        mov.setDescripcion("Movimiento (Prueba Existencias)"); // Nombre único
//        mov.setCantidad(15);
//        mov.setProducto(productoDao.leer(this.testProductoId));
//        mov.setLineaOrdenIngreso(null);
//        mov.setLineaOrdenSalida(null);
//
//        movimientoBO.guardar(mov, Estado.NUEVO);
//        
//        // Workaround para obtener el ID del Movimiento
//        List<Movimiento> listaMov = movimientoBO.listar();
//        for (Movimiento m : listaMov) {
//            if ("Movimiento (Prueba Existencias)".equals(m.getDescripcion())) {
//                this.testMovimientoId = m.getIdMovimiento();
//                break;
//            }
//        }
//        
//        if (this.testMovimientoId == 0) {
//            fail("No se pudo crear el Movimiento de requisito en @BeforeAll");
//        }
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        if (this.testMovimientoId > 0) new MovimientoBOImpl().eliminar(this.testMovimientoId); // Limpiar el movimiento
////        if (this.testProductoId > 0) new ProductoDAOImpl().eliminar(this.testProductoId);
////        if (this.testCategoriaId > 0) new CategoriaDAOImpl().eliminar(this.testCategoriaId);
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<Existencias> lista = existenciasBO.listar();
//        assertNotNull(lista);
//    }
//
//    @Test
//    @Order(2)
//    @Override
//    public void debeGuardarNuevo() {
//        crearExistencia();
//        assertTrue(this.testExistenciaId > 0, "El ID de Existencia no se pudo recuperar con el workaround.");
//    }
//    
//    @Test
//    @Order(3)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        Existencias existencia = existenciasBO.obtener(this.testExistenciaId);
//        assertNotNull(existencia);
//        assertEquals(this.testExistenciaId, existencia.getIdExistencia());
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        Existencias existencia = existenciasBO.obtener(this.idIncorrecto);
//        assertNull(existencia);
//    }
//    
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        Existencias existencia = existenciasBO.obtener(this.testExistenciaId);
//        existencia.setEstado(EstadoExistencias.VENDIDO);
//        existencia.setFechaEgreso(new GregorianCalendar(2025, Calendar.FEBRUARY, 1).getTime());
//        existenciasBO.guardar(existencia, Estado.MODIFICADO);
//
//        Existencias modificada = existenciasBO.obtener(this.testExistenciaId);
//        assertEquals(EstadoExistencias.VENDIDO, modificada.getEstado());
//        assertNotNull(modificada.getFechaEgreso());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        assertDoesNotThrow(() -> existenciasBO.eliminar(this.testExistenciaId));
//        Existencias existencia = existenciasBO.obtener(this.testExistenciaId);
//        assertNull(existencia);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertDoesNotThrow(() -> existenciasBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        Existencias existencia = new Existencias();
//        existencia.setIdUnico(12345);
//        existencia.setEstado(EstadoExistencias.DISPONIBLE);
//        existencia.setFechaIngreso(new GregorianCalendar(2025, Calendar.JANUARY, 1).getTime());
//        
//        Producto productoInvalido = new Producto();
//        productoInvalido.setIdProducto(idIncorrecto);
//        existencia.setProducto(productoInvalido);
//        
//        // Asignamos un movimiento válido para que no falle por el NOT NULL
//        existencia.setMovimientoIngreso(movimientoBO.obtener(this.testMovimientoId));
//
//        // El error de FK (producto inválido) SÍ debe lanzar RuntimeException
//        assertThrows(RuntimeException.class, () -> existenciasBO.guardar(existencia, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertDoesNotThrow(() -> existenciasBO.eliminar(idIncorrecto));
//    }
//    
//    private void crearExistencia() {
//        Existencias existencia = new Existencias();
//        existencia.setIdUnico(10001); // ID único para el workaround
//        existencia.setEstado(EstadoExistencias.DISPONIBLE);
//        existencia.setFechaIngreso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
//        existencia.setFechaEgreso(null);
//        existencia.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
//        
//        // --- CORRECCIÓN CLAVE ---
//        // Asignamos el Movimiento válido creado en @BeforeAll
//        existencia.setMovimientoIngreso(movimientoBO.obtener(this.testMovimientoId));
//        existencia.setMovimientoEgreso(null);
//
//        existenciasBO.guardar(existencia, Estado.NUEVO);
//        
//        // --- WORKAROUND ---
//        List<Existencias> lista = existenciasBO.listar();
//        for (Existencias e : lista) {
//            if (e.getIdUnico() == 10001) {
//                this.testExistenciaId = e.getIdExistencia();
//                break;
//            }
//        }
//    }
//}