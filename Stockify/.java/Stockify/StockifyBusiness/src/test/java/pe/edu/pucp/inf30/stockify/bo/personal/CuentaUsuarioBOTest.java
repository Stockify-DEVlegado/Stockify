//package pe.edu.pucp.inf30.stockify.bo.personal;
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
//import pe.edu.pucp.inf30.stockify.boimpl.personal.CuentaUsuarioBOImpl;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//import pe.edu.pucp.inf30.stockify.model.personal.CuentaUsuario;
//
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class CuentaUsuarioBOTest implements GestionableProbable {
//    
//    private int testCuentaUsuarioId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final CuentaUsuarioBOImpl cuentaUsuarioBO = new CuentaUsuarioBOImpl();
//    
//    @BeforeAll
//    public void inicializar() {
//        
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        if (this.testCuentaUsuarioId > 0) {
////            cuentaUsuarioBO.eliminar(this.testCuentaUsuarioId);
////        }
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<CuentaUsuario> lista = cuentaUsuarioBO.listar();
//        assertNotNull(lista);
//    }
//    
//    @Test
//    @Order(2)
//    @Override
//    public void debeGuardarNuevo() {
//        crearCuentaUsuario();
//        assertTrue(this.testCuentaUsuarioId > 0, "El ID no se pudo recuperar con el workaround.");
//    }
//
//    @Test
//    @Order(3)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        CuentaUsuario cuenta = cuentaUsuarioBO.obtener(this.testCuentaUsuarioId);
//        assertNotNull(cuenta);
//        assertEquals(this.testCuentaUsuarioId, cuenta.getIdCuentaUsuario());
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        CuentaUsuario cuenta = cuentaUsuarioBO.obtener(this.idIncorrecto);
//        assertNull(cuenta);
//    }
//
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        CuentaUsuario cuenta = cuentaUsuarioBO.obtener(this.testCuentaUsuarioId);
//        cuenta.setPassword("nuevoPassword123");
//        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.FEBRUARY, 1).getTime());
//        cuentaUsuarioBO.guardar(cuenta, Estado.MODIFICADO);
//
//        CuentaUsuario modificada = cuentaUsuarioBO.obtener(this.testCuentaUsuarioId);
//        assertEquals("nuevoPassword123", modificada.getPassword());
//        assertNotNull(modificada.getUltimoAcceso());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        assertDoesNotThrow(() -> cuentaUsuarioBO.eliminar(this.testCuentaUsuarioId));
//        CuentaUsuario cuenta = cuentaUsuarioBO.obtener(this.testCuentaUsuarioId);
//        assertNull(cuenta);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertDoesNotThrow(() -> cuentaUsuarioBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        CuentaUsuario cuenta = new CuentaUsuario();
//        cuenta.setUsername(null);
//        cuenta.setPassword("password123");
//        cuenta.setUltimoAcceso(null);
//
//        assertThrows(RuntimeException.class, () -> cuentaUsuarioBO.guardar(cuenta, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertDoesNotThrow(() -> cuentaUsuarioBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(10)
//    public void debeCrearCuentaSinUltimoAcceso() {
//        CuentaUsuario cuenta = new CuentaUsuario();
//        cuenta.setUsername("usuariosinacceso");
//        cuenta.setPassword("password456");
//        cuenta.setUltimoAcceso(null);
//        
//        cuentaUsuarioBO.guardar(cuenta, Estado.NUEVO);
//        
//        int idGenerado = 0;
//        List<CuentaUsuario> lista = cuentaUsuarioBO.listar();
//        for (CuentaUsuario cu : lista) {
//            if (cu.getUsername().equals("usuariosinacceso")) {
//                idGenerado = cu.getIdCuentaUsuario();
//                cuenta.setIdCuentaUsuario(idGenerado);
//                break;
//            }
//        }
//        
//        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'usuariosinacceso'");
//        assertNull(cuenta.getUltimoAcceso());
//        
//        if (idGenerado > 0) {
//            cuentaUsuarioBO.eliminar(idGenerado);
//        }
//    }
//    
//    @Test
//    @Order(11)
//    public void debeActualizarUltimoAcceso() {
//        CuentaUsuario cuenta = new CuentaUsuario();
//        cuenta.setUsername("usuarioacceso");
//        cuenta.setPassword("password789");
//        cuenta.setUltimoAcceso(null);
//        
//        cuentaUsuarioBO.guardar(cuenta, Estado.NUEVO);
//        
//        int idGenerado = 0;
//        List<CuentaUsuario> lista = cuentaUsuarioBO.listar();
//        for (CuentaUsuario cu : lista) {
//            if (cu.getUsername().equals("usuarioacceso")) {
//                idGenerado = cu.getIdCuentaUsuario();
//                cuenta.setIdCuentaUsuario(idGenerado);
//                break;
//            }
//        }
//
//        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'usuarioacceso'");
//        assertNull(cuenta.getUltimoAcceso());
//        
//        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.JANUARY, 15).getTime());
//        cuentaUsuarioBO.guardar(cuenta, Estado.MODIFICADO);
//        
//        CuentaUsuario actualizada = cuentaUsuarioBO.obtener(cuenta.getIdCuentaUsuario());
//        assertNotNull(actualizada.getUltimoAcceso());
//        
//        if (idGenerado > 0) {
//            cuentaUsuarioBO.eliminar(idGenerado);
//        }
//    }
//    
//    private void crearCuentaUsuario() {
//        CuentaUsuario cuenta = new CuentaUsuario();
//        cuenta.setUsername("usuariotest");
//        cuenta.setPassword("password123");
//        cuenta.setUltimoAcceso(new GregorianCalendar(2025, Calendar.JANUARY, 10).getTime());
//
//        cuentaUsuarioBO.guardar(cuenta, Estado.NUEVO);
//        
//        List<CuentaUsuario> lista = cuentaUsuarioBO.listar();
//        for (CuentaUsuario cu : lista) {
//            if (cu.getUsername().equals("usuariotest")) {
//                this.testCuentaUsuarioId = cu.getIdCuentaUsuario();
//                break;
//            }
//        }
//    }
//}