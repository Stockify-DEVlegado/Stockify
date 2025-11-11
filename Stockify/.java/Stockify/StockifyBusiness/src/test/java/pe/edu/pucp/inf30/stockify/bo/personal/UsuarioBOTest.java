/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.personal;

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
import pe.edu.pucp.inf30.stockify.boimpl.personal.UsuarioBOImpl;
import pe.edu.pucp.inf30.stockify.daoimpl.personal.CuentaUsuarioDAOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;
import pe.edu.pucp.inf30.stockify.model.personal.TipoUsuario;
import pe.edu.pucp.inf30.stockify.model.personal.Usuario;

/**
 *
 * @author DEVlegado
 */
@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class UsuarioBOTest implements GestionableProbable {
    
    private int testCuentaUsuarioId;
    private int testUsuarioId;
    
    private final int idIncorrecto = 99999;
    
    private final UsuarioBOImpl usuarioBO = new UsuarioBOImpl();
    
    @BeforeAll
    public void inicializar() {
        CuentaUsuarioDAOImpl cuentaDao = new CuentaUsuarioDAOImpl();
        CuentaUsuario cuenta = new CuentaUsuario();
        cuenta.setUsername("cuentatest");
        cuenta.setPassword("password123");
        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
        this.testCuentaUsuarioId = cuentaDao.crear(cuenta);
    }
    
    @AfterAll
    public void limpiar() {
//        new CuentaUsuarioDAOImpl().eliminar(this.testCuentaUsuarioId);
    }
    
    @Test
    @Order(1)
    @Override
    public void debeListar() {
        List<Usuario> lista = usuarioBO.listar();
        assertNotNull(lista);
    }
    
    @Test
    @Order(2)
    @Override
    public void debeObtenerSiIdExiste() {
        crearUsuario();
        Usuario usuario = usuarioBO.obtener(this.testUsuarioId);
        assertNotNull(usuario);
        assertEquals(this.testUsuarioId, usuario.getIdUsuario());
    }
    
    @Test
    @Order(3)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        Usuario usuario = usuarioBO.obtener(this.idIncorrecto);
        assertNull(usuario);
    }
    
    @Test
    @Order(4)
    @Override
    public void debeGuardarNuevo() {
        crearUsuario();
        assertTrue(this.testUsuarioId > 0);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        Usuario usuario = usuarioBO.obtener(this.testUsuarioId);
        usuario.setNombres("Nombres Modificados");
        usuario.setTelefono("912345678");
        usuario.setActivo(false);
        usuarioBO.guardar(usuario, Estado.MODIFICADO);

        Usuario modificado = usuarioBO.obtener(this.testUsuarioId);
        assertEquals("Nombres Modificados", modificado.getNombres());
        assertEquals("912345678", modificado.getTelefono());
        assertFalse(modificado.isActivo());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        usuarioBO.eliminar(this.testUsuarioId);
        Usuario usuario = usuarioBO.obtener(this.testUsuarioId);
        assertNull(usuario);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        assertThrows(RuntimeException.class, () -> usuarioBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        Usuario usuario = new Usuario();
        usuario.setNombres("Test");
        usuario.setApellidos("Usuario");
        usuario.setEmail("test@usuario.com");
        usuario.setTelefono("987654321");
        usuario.setActivo(true);
        usuario.setTipoUsuario(TipoUsuario.OPERARIO);
        // Forzamos un error: cuenta con id inexistente
        CuentaUsuario cuentaInvalida = new CuentaUsuario();
        cuentaInvalida.setIdCuentaUsuario(idIncorrecto);
        usuario.setCuenta(cuentaInvalida);

        assertThrows(RuntimeException.class, () -> usuarioBO.guardar(usuario, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertThrows(RuntimeException.class, () -> usuarioBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(10)
    public void debeCrearUsuarioAlmacenero() {
        Usuario almacenero = new Usuario();
        almacenero.setNombres("Juan");
        almacenero.setApellidos("Perez");
        almacenero.setEmail("almacenero@test.com");
        almacenero.setTelefono("998877665");
        almacenero.setActivo(true);
        almacenero.setTipoUsuario(TipoUsuario.OPERARIO);
        almacenero.setCuenta(new CuentaUsuarioDAOImpl().leer(this.testCuentaUsuarioId));
        
        usuarioBO.guardar(almacenero, Estado.NUEVO);
        
        assertTrue(almacenero.getIdUsuario() > 0);
        assertEquals(TipoUsuario.OPERARIO, almacenero.getTipoUsuario());
        
        // Limpiar
        usuarioBO.eliminar(almacenero.getIdUsuario());
    }
    
    @Test
    @Order(11)
    public void debeCrearUsuarioAdministrador() {
        Usuario admin = new Usuario();
        admin.setNombres("Maria");
        admin.setApellidos("Lopez");
        admin.setEmail("admin@test.com");
        admin.setTelefono("955443322");
        admin.setActivo(true);
        admin.setTipoUsuario(TipoUsuario.ADMINISTRADOR);
        admin.setCuenta(new CuentaUsuarioDAOImpl().leer(this.testCuentaUsuarioId));
        
        usuarioBO.guardar(admin, Estado.NUEVO);
        
        assertTrue(admin.getIdUsuario() > 0);
        assertEquals(TipoUsuario.ADMINISTRADOR, admin.getTipoUsuario());
        
        // Limpiar
        usuarioBO.eliminar(admin.getIdUsuario());
    }
    
    private void crearUsuario() {
        Usuario usuario = new Usuario();
        usuario.setNombres("Carlos");
        usuario.setApellidos("Garcia");
        usuario.setEmail("usuario@test.com");
        usuario.setTelefono("987654321");
        usuario.setActivo(true);
        usuario.setTipoUsuario(TipoUsuario.OPERARIO);
        usuario.setCuenta(new CuentaUsuarioDAOImpl().leer(this.testCuentaUsuarioId));

        usuarioBO.guardar(usuario, Estado.NUEVO);
        this.testUsuarioId = usuario.getIdUsuario();
    }
}