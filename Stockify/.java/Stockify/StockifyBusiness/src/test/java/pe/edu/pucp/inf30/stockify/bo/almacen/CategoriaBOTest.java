package pe.edu.pucp.inf30.stockify.bo.almacen;

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
import pe.edu.pucp.inf30.stockify.boimpl.almacen.CategoriaBOImpl;
import pe.edu.pucp.inf30.stockify.model.almacen.Categoria;
import pe.edu.pucp.inf30.stockify.model.Estado;

@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class CategoriaBOTest implements GestionableProbable {
    
    private int testCategoriaPadreId;
    private int testCategoriaId;
    
    private final int idIncorrecto = 99999;
    
    private final CategoriaBOImpl categoriaBO = new CategoriaBOImpl();
    
    // Nombres de prueba únicos
    private final String NOMBRE_PADRE = "Categoria Padre (Prueba Unica)";
    private final String NOMBRE_HIJA = "Categoria Hija (Prueba Unica)";
    private final String NOMBRE_SUB = "Subcategoria (Prueba Unica)";
    private final String NOMBRE_RAIZ = "Categoria Raiz (Prueba Unica)";
    
    @BeforeAll
    public void inicializar() {
        // Limpiamos datos de corridas anteriores si existen
        limpiarDatosPorNombre();
        
        Categoria categoriaPadre = new Categoria();
        categoriaPadre.setNombre(NOMBRE_PADRE);
        categoriaBO.guardar(categoriaPadre, Estado.NUEVO);
        
        List<Categoria> lista = categoriaBO.listar();
        for (Categoria c : lista) {
            if (NOMBRE_PADRE.equals(c.getNombre())) {
                this.testCategoriaPadreId = c.getIdCategoria();
                break;
            }
        }
    }
    
    @AfterAll
    public void limpiar() {
        // Descomentamos la limpieza para que esta clase no contamine futuras pruebas
        if (this.testCategoriaPadreId > 0) {
            categoriaBO.eliminar(this.testCategoriaPadreId);
        }
        // Limpiamos los otros por si acaso
        limpiarDatosPorNombre();
    }
    
    private void limpiarDatosPorNombre() {
        List<Categoria> lista = categoriaBO.listar();
        for (Categoria c : lista) {
            if (NOMBRE_HIJA.equals(c.getNombre()) ||
                NOMBRE_SUB.equals(c.getNombre()) ||
                NOMBRE_RAIZ.equals(c.getNombre())) {
                
                try {
                   categoriaBO.eliminar(c.getIdCategoria());
                } catch (Exception e) {
                   // Ignorar si falla (ej. si un producto aún la usa)
                }
            }
        }
    }
    
    @Test
    @Order(1)
    @Override
    public void debeListar() {
        List<Categoria> lista = categoriaBO.listar();
        assertNotNull(lista);
    }

    @Test
    @Order(2)
    @Override
    public void debeGuardarNuevo() {
        crearCategoria();
        assertTrue(this.testCategoriaId > 0, "El ID de Categoria no se pudo recuperar.");
    }

    @Test
    @Order(3)
    @Override
    public void debeObtenerSiIdExiste() {
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        assertNotNull(categoria);
        assertEquals(this.testCategoriaId, categoria.getIdCategoria());
    }
    
    @Test
    @Order(4)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        Categoria categoria = categoriaBO.obtener(this.idIncorrecto);
        assertNull(categoria);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        categoria.setNombre("Categoria Modificada (Prueba Unica)");
        categoriaBO.guardar(categoria, Estado.MODIFICADO);

        Categoria modificada = categoriaBO.obtener(this.testCategoriaId);
        assertEquals("Categoria Modificada (Prueba Unica)", modificada.getNombre());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        // Ahora no debería lanzar excepción, porque esta categoría
        // (NOMBRE_HIJA) no está siendo usada por ningún producto.
        assertDoesNotThrow(() -> categoriaBO.eliminar(this.testCategoriaId));
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        assertNull(categoria);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        assertDoesNotThrow(() -> categoriaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        Categoria categoria = new Categoria();
        categoria.setNombre(null);
        
        assertThrows(RuntimeException.class, () -> categoriaBO.guardar(categoria, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertDoesNotThrow(() -> categoriaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(10)
    public void debeCrearCategoriaConPadre() {
        Categoria categoriaHija = new Categoria();
        categoriaHija.setNombre(NOMBRE_SUB);
        categoriaHija.setCategoria(categoriaBO.obtener(this.testCategoriaPadreId));
        
        categoriaBO.guardar(categoriaHija, Estado.NUEVO);
        
        int idGenerado = 0;
        List<Categoria> lista = categoriaBO.listar();
        for (Categoria c : lista) {
            if (NOMBRE_SUB.equals(c.getNombre())) {
                idGenerado = c.getIdCategoria();
                categoriaHija.setIdCategoria(idGenerado);
                break;
            }
        }
        
        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'Subcategoria'");
        assertNotNull(categoriaHija.getCategoria());
        assertEquals(this.testCategoriaPadreId, categoriaHija.getCategoria().getIdCategoria());
        
        categoriaBO.eliminar(idGenerado);
    }
    
    @Test
    @Order(11)
    public void debeCrearCategoriaSinPadre() {
        Categoria categoriaRaiz = new Categoria();
        categoriaRaiz.setNombre(NOMBRE_RAIZ);
        categoriaRaiz.setCategoria(null);
        
        categoriaBO.guardar(categoriaRaiz, Estado.NUEVO);
        
        int idGenerado = 0;
        List<Categoria> lista = categoriaBO.listar();
        for (Categoria c : lista) {
            if (NOMBRE_RAIZ.equals(c.getNombre())) {
                idGenerado = c.getIdCategoria();
                categoriaRaiz.setIdCategoria(idGenerado);
                break;
            }
        }
        
        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'Categoria Raiz'");
        assertNull(categoriaRaiz.getCategoria());
        
        categoriaBO.eliminar(idGenerado);
    }
    
    private void crearCategoria() {
        Categoria categoria = new Categoria();
        categoria.setNombre(NOMBRE_HIJA);
        categoria.setCategoria(categoriaBO.obtener(this.testCategoriaPadreId));

        categoriaBO.guardar(categoria, Estado.NUEVO);
        
        List<Categoria> lista = categoriaBO.listar();
        for (Categoria c : lista) {
            if (NOMBRE_HIJA.equals(c.getNombre())) {
                this.testCategoriaId = c.getIdCategoria();
                break;
            }
        }
    }
}