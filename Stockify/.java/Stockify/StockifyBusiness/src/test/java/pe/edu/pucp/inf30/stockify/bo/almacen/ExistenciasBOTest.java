///*
// * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
// * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
// */
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
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.ProductoDAOImpl;
//import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
//import pe.edu.pucp.inf30.stockify.model.almacen.EstadoExistencias;
//import pe.edu.pucp.inf30.stockify.model.almacen.Existencias;
//import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//
///**
// *
// * @author DEVlegado
// */
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class ExistenciasBOTest implements GestionableProbable {
//    
//    private int testCategoriaId;
//    private int testProductoId;
//    private int testExistenciaId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final ExistenciasBOImpl existenciasBO = new ExistenciasBOImpl();
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
//        producto.setMarca("Marca Test");
//        producto.setStockMinimo(5);
//        producto.setStockMaximo(50);
//        producto.setPrecioUnitario(180.0);
//        producto.setCategoria(categoriaDao.leer(this.testCategoriaId));
//        this.testProductoId = productoDao.crear(producto);
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        new ProductoDAOImpl().eliminar(this.testProductoId);
////        new CategoriaDAOImpl().eliminar(this.testCategoriaId);
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
//    public void debeObtenerSiIdExiste() {
//        crearExistencia();
//        Existencias existencia = existenciasBO.obtener(this.testExistenciaId);
//        assertNotNull(existencia);
//        assertEquals(this.testExistenciaId, existencia.getIdExistencia());
//    }
//    
//    @Test
//    @Order(3)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        Existencias existencia = existenciasBO.obtener(this.idIncorrecto);
//        assertNull(existencia);
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void debeGuardarNuevo() {
//        crearExistencia();
//        assertTrue(this.testExistenciaId > 0);
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
//        existenciasBO.eliminar(this.testExistenciaId);
//        Existencias existencia = existenciasBO.obtener(this.testExistenciaId);
//        assertNull(existencia);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertThrows(RuntimeException.class, () -> existenciasBO.eliminar(idIncorrecto));
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
//        // Forzamos un error: producto con id inexistente
//        Producto productoInvalido = new Producto();
//        productoInvalido.setIdProducto(idIncorrecto);
//        existencia.setProducto(productoInvalido);
//
//        assertThrows(RuntimeException.class, () -> existenciasBO.guardar(existencia, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertThrows(RuntimeException.class, () -> existenciasBO.eliminar(idIncorrecto));
//    }
//    
//    private void crearExistencia() {
//        Existencias existencia = new Existencias();
//        existencia.setIdUnico(10001);
//        existencia.setEstado(EstadoExistencias.DISPONIBLE);
//        existencia.setFechaIngreso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
//        existencia.setProducto(new ProductoDAOImpl().leer(this.testProductoId));
//
//        existenciasBO.guardar(existencia, Estado.NUEVO);
//        this.testExistenciaId = existencia.getIdExistencia();
//    }
//}