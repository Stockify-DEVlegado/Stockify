/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
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

/**
 *
 * @author DEVlegado
 */
@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class CategoriaBOTest implements GestionableProbable {
    
    private int testCategoriaPadreId;
    private int testCategoriaId;
    
    private final int idIncorrecto = 99999;
    
    private final CategoriaBOImpl categoriaBO = new CategoriaBOImpl();
    
    @BeforeAll
    public void inicializar() {
        // Crear una categoría padre para probar la jerarquía
        Categoria categoriaPadre = new Categoria();
        categoriaPadre.setNombre("Categoria Padre Test");
        categoriaBO.guardar(categoriaPadre, Estado.NUEVO);
        this.testCategoriaPadreId = categoriaPadre.getIdCategoria();
    }
    
    @AfterAll
    public void limpiar() {
//        if (this.testCategoriaPadreId > 0) {
//            categoriaBO.eliminar(this.testCategoriaPadreId);
//        }
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
    public void debeObtenerSiIdExiste() {
        crearCategoria();
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        assertNotNull(categoria);
        assertEquals(this.testCategoriaId, categoria.getIdCategoria());
    }
    
    @Test
    @Order(3)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        Categoria categoria = categoriaBO.obtener(this.idIncorrecto);
        assertNull(categoria);
    }
    
    @Test
    @Order(4)
    @Override
    public void debeGuardarNuevo() {
        crearCategoria();
        assertTrue(this.testCategoriaId > 0);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        categoria.setNombre("Categoria Modificada");
        categoriaBO.guardar(categoria, Estado.MODIFICADO);

        Categoria modificada = categoriaBO.obtener(this.testCategoriaId);
        assertEquals("Categoria Modificada", modificada.getNombre());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        categoriaBO.eliminar(this.testCategoriaId);
        Categoria categoria = categoriaBO.obtener(this.testCategoriaId);
        assertNull(categoria);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        assertThrows(RuntimeException.class, () -> categoriaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        Categoria categoria = new Categoria();
        // Forzamos un error: nombre nulo o muy largo (dependiendo de validaciones)
        categoria.setNombre(null);
        
        assertThrows(RuntimeException.class, () -> categoriaBO.guardar(categoria, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertThrows(RuntimeException.class, () -> categoriaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(10)
    public void debeCrearCategoriaConPadre() {
        Categoria categoriaHija = new Categoria();
        categoriaHija.setNombre("Subcategoria Test");
        categoriaHija.setCategoria(categoriaBO.obtener(this.testCategoriaPadreId));
        
        categoriaBO.guardar(categoriaHija, Estado.NUEVO);
        
        assertTrue(categoriaHija.getIdCategoria() > 0);
        assertNotNull(categoriaHija.getCategoria());
        assertEquals(this.testCategoriaPadreId, categoriaHija.getCategoria().getIdCategoria());
        
        // Limpiar
        categoriaBO.eliminar(categoriaHija.getIdCategoria());
    }
    
    @Test
    @Order(11)
    public void debeCrearCategoriaSinPadre() {
        Categoria categoriaRaiz = new Categoria();
        categoriaRaiz.setNombre("Categoria Raiz Test");
        categoriaRaiz.setCategoria(null);
        
        categoriaBO.guardar(categoriaRaiz, Estado.NUEVO);
        
        assertTrue(categoriaRaiz.getIdCategoria() > 0);
        assertNull(categoriaRaiz.getCategoria());
        
        // Limpiar
        categoriaBO.eliminar(categoriaRaiz.getIdCategoria());
    }
    
    private void crearCategoria() {
        Categoria categoria = new Categoria();
        categoria.setNombre("Categoria Test");
        categoria.setCategoria(categoriaBO.obtener(this.testCategoriaPadreId));

        categoriaBO.guardar(categoria, Estado.NUEVO);
        this.testCategoriaId = categoria.getIdCategoria();
    }
}