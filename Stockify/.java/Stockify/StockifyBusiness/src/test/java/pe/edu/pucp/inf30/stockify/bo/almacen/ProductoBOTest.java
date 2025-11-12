//package pe.edu.pucp.inf30.stockify.bo.almacen;
//
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
//import pe.edu.pucp.inf30.stockify.boimpl.almacen.ProductoBOImpl;
//import pe.edu.pucp.inf30.stockify.daoimpl.almacen.CategoriaDAOImpl;
//import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
//import pe.edu.pucp.inf30.stockify.model.almacen.Producto;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class ProductoBOTest implements GestionableProbable {
//    
//    private int testCategoriaId;
//    private int testProductoId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final ProductoBOImpl productoBO = new ProductoBOImpl();
//    
//    @BeforeAll
//    public void inicializar() {
//        CategoriaDAOImpl categoriaDao = new CategoriaDAOImpl();
//        Categoria categoria = new Categoria();
//        categoria.setNombre("Categoria Test Producto");
//        
//        List<Categoria> lista = categoriaDao.leerTodos();
//        for(Categoria c : lista){
//            if("Categoria Test Producto".equals(c.getNombre())){
//                this.testCategoriaId = c.getIdCategoria();
//                return;
//            }
//        }
//        
//        this.testCategoriaId = categoriaDao.crear(categoria);
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        if (this.testCategoriaId > 0) {
////            new CategoriaDAOImpl().eliminar(this.testCategoriaId);
////        }
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<Producto> lista = productoBO.listar();
//        assertNotNull(lista);
//    }
//
//    @Test
//    @Order(2)
//    @Override
//    public void debeGuardarNuevo() {
//        crearProducto();
//        assertTrue(this.testProductoId > 0, "El ID de Producto no se pudo recuperar.");
//    }
//
//    @Test
//    @Order(3)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        Producto producto = productoBO.obtener(this.testProductoId);
//        assertNotNull(producto);
//        assertEquals(this.testProductoId, producto.getIdProducto());
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        Producto producto = productoBO.obtener(this.idIncorrecto);
//        assertNull(producto);
//    }
//    
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        Producto producto = productoBO.obtener(this.testProductoId);
//        producto.setPrecioUnitario(350.0);
//        producto.setMarca("Marca Modificada");
//        productoBO.guardar(producto, Estado.MODIFICADO);
//
//        Producto modificado = productoBO.obtener(this.testProductoId);
//        assertEquals(350.0, modificado.getPrecioUnitario());
//        assertEquals("Marca Modificada", modificado.getMarca());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        assertDoesNotThrow(() -> productoBO.eliminar(this.testProductoId));
//        Producto producto = productoBO.obtener(this.testProductoId);
//        assertNull(producto);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertDoesNotThrow(() -> productoBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        Producto producto = new Producto();
//        producto.setNombre("Producto Invalido");
//        producto.setDescripcion("Descripcion Test");
//        producto.setMarca("Marca Test");
//        producto.setStockMinimo(10);
//        producto.setStockMaximo(100);
//        producto.setPrecioUnitario(200.0);
//        
//        Categoria categoriaInvalida = new Categoria();
//        categoriaInvalida.setIdCategoria(idIncorrecto);
//        producto.setCategoria(categoriaInvalida);
//
//        assertThrows(RuntimeException.class, () -> productoBO.guardar(producto, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertDoesNotThrow(() -> productoBO.eliminar(idIncorrecto));
//    }
//    
//    private void crearProducto() {
//        Producto producto = new Producto();
//        producto.setNombre("Producto Test");
//        producto.setDescripcion("Descripcion Test");
//        producto.setMarca("Marca Test");
//        producto.setStockMinimo(5);
//        producto.setStockMaximo(50);
//        producto.setPrecioUnitario(250.0);
//        producto.setCategoria(new CategoriaDAOImpl().leer(this.testCategoriaId));
//
//        productoBO.guardar(producto, Estado.NUEVO);
//        
//        List<Producto> lista = productoBO.listar();
//        for (Producto p : lista) {
//            if ("Producto Test".equals(p.getNombre())) {
//                this.testProductoId = p.getIdProducto();
//                break;
//            }
//        }
//    }
//}