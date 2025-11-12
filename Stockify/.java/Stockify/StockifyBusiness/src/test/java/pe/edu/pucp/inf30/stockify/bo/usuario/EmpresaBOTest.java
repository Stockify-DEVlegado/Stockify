//package pe.edu.pucp.inf30.stockify.bo.usuario;
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
//import pe.edu.pucp.inf30.stockify.boimpl.usuario.EmpresaBOImpl;
//import pe.edu.pucp.inf30.stockify.model.Estado;
//import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
//import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;
//
//@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
//@TestInstance(TestInstance.Lifecycle.PER_CLASS)
//public class EmpresaBOTest implements GestionableProbable {
//    
//    private int testEmpresaId;
//    
//    private final int idIncorrecto = 99999;
//    
//    private final EmpresaBOImpl empresaBO = new EmpresaBOImpl();
//    
//    @BeforeAll
//    public void inicializar() {
//        
//    }
//    
//    @AfterAll
//    public void limpiar() {
////        if (this.testEmpresaId > 0) {
////            empresaBO.eliminar(this.testEmpresaId);
////        }
//    }
//    
//    @Test
//    @Order(1)
//    @Override
//    public void debeListar() {
//        List<Empresa> lista = empresaBO.listar();
//        assertNotNull(lista);
//    }
//
//    @Test
//    @Order(2)
//    @Override
//    public void debeGuardarNuevo() {
//        crearEmpresa();
//        assertTrue(this.testEmpresaId > 0, "El ID de Empresa no se pudo recuperar con el workaround.");
//    }
//
//    @Test
//    @Order(3)
//    @Override
//    public void debeObtenerSiIdExiste() {
//        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
//        assertNotNull(empresa);
//        assertEquals(this.testEmpresaId, empresa.getIdEmpresa());
//    }
//    
//    @Test
//    @Order(4)
//    @Override
//    public void noDebeObtenerSiIdNoExiste() {
//        Empresa empresa = empresaBO.obtener(this.idIncorrecto);
//        assertNull(empresa);
//    }
//    
//    @Test
//    @Order(5)
//    @Override
//    public void debeGuardarModificado() {
//        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
//        empresa.setRazonSocial("Empresa Modificada SAC");
//        empresa.setTelefono("912345678");
//        empresa.setActivo(false);
//        empresaBO.guardar(empresa, Estado.MODIFICADO);
//
//        Empresa modificada = empresaBO.obtener(this.testEmpresaId);
//        assertEquals("Empresa Modificada SAC", modificada.getRazonSocial());
//        assertEquals("912345678", modificada.getTelefono());
//        assertFalse(modificada.isActivo());
//    }
//    
//    @Test
//    @Order(6)
//    @Override
//    public void debeEliminarSiIdExiste() {
//        assertDoesNotThrow(() -> empresaBO.eliminar(this.testEmpresaId));
//        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
//        assertNull(empresa);
//    }
//    
//    @Test
//    @Order(7)
//    @Override
//    public void noDebeEliminarSiIdNoExiste() {
//        assertDoesNotThrow(() -> empresaBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(8)
//    @Override
//    public void debeHacerRollbackSiErrorEnGuardar() {
//        Empresa empresa = new Empresa();
//        empresa.setRazonSocial(null);
//        empresa.setTipoDocumento(TipoDocumento.RUC);
//        empresa.setTelefono("987654321");
//        empresa.setEmail("error@test.com");
//        empresa.setActivo(true);
//        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
//
//        assertThrows(RuntimeException.class, () -> empresaBO.guardar(empresa, Estado.NUEVO));
//    }
//    
//    @Test
//    @Order(9)
//    @Override
//    public void debeHacerRollbackSiErrorEnEliminar() {
//        assertDoesNotThrow(() -> empresaBO.eliminar(idIncorrecto));
//    }
//    
//    @Test
//    @Order(10)
//    public void debeCrearEmpresaProveedor() {
//        Empresa proveedor = new Empresa();
//        proveedor.setTipoDocumento(TipoDocumento.RUC);
//        proveedor.setRazonSocial("Proveedor Test SAC");
//        proveedor.setTelefono("998877665");
//        proveedor.setEmail("proveedor@test.com");
//        proveedor.setActivo(true);
//        proveedor.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
//        
//        empresaBO.guardar(proveedor, Estado.NUEVO);
//        
//        int idGenerado = 0;
//        List<Empresa> lista = empresaBO.listar();
//        for (Empresa e : lista) {
//            if ("proveedor@test.com".equals(e.getEmail())) {
//                idGenerado = e.getIdEmpresa();
//                break;
//            }
//        }
//        
//        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'proveedor'");
//        
//        Empresa guardado = empresaBO.obtener(idGenerado);
//        assertEquals(TipoEmpresa.PROVEEDOR, guardado.getTipoEmpresa());
//        
//        empresaBO.eliminar(idGenerado);
//    }
//    
//    @Test
//    @Order(11)
//    public void debeCrearEmpresaCliente() {
//        Empresa cliente = new Empresa();
//        cliente.setTipoDocumento(TipoDocumento.RUC);
//        cliente.setRazonSocial("Cliente Test SAC");
//        cliente.setTelefono("955443322");
//        cliente.setEmail("cliente@test.com");
//        cliente.setActivo(true);
//        cliente.setTipoEmpresa(TipoEmpresa.CLIENTE);
//        
//        empresaBO.guardar(cliente, Estado.NUEVO);
//        
//        int idGenerado = 0;
//        List<Empresa> lista = empresaBO.listar();
//        for (Empresa e : lista) {
//            if ("cliente@test.com".equals(e.getEmail())) {
//                idGenerado = e.getIdEmpresa();
//                break;
//            }
//        }
//
//        assertTrue(idGenerado > 0, "No se pudo recuperar el ID de 'cliente'");
//        
//        Empresa guardado = empresaBO.obtener(idGenerado);
//        assertEquals(TipoEmpresa.CLIENTE, guardado.getTipoEmpresa());
//        
//        empresaBO.eliminar(idGenerado);
//    }
//    
//    private void crearEmpresa() {
//        Empresa empresa = new Empresa();
//        empresa.setTipoDocumento(TipoDocumento.RUC);
//        empresa.setRazonSocial("Empresa Test SAC");
//        empresa.setTelefono("987654321");
//        empresa.setEmail("empresa@test.com");
//        empresa.setActivo(true);
//        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
//
//        empresaBO.guardar(empresa, Estado.NUEVO);
//        
//        List<Empresa> lista = empresaBO.listar();
//        for (Empresa e : lista) {
//            if ("empresa@test.com".equals(e.getEmail())) {
//                this.testEmpresaId = e.getIdEmpresa();
//                break;
//            }
//        }
//    }
//}