/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.bo.usuario;

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
import pe.edu.pucp.inf30.stockify.boimpl.usuario.EmpresaBOImpl;
import pe.edu.pucp.inf30.stockify.model.Estado;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoDocumento;
import pe.edu.pucp.inf30.stockify.model.usuario.TipoEmpresa;

/**
 *
 * @author DEVlegado
 */
@TestMethodOrder(MethodOrderer.OrderAnnotation.class)
@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class EmpresaBOTest implements GestionableProbable {
    
    private int testEmpresaId;
    
    private final int idIncorrecto = 99999;
    
    private final EmpresaBOImpl empresaBO = new EmpresaBOImpl();
    
    @BeforeAll
    public void inicializar() {
        // No se requiere inicialización previa
    }
    
    @AfterAll
    public void limpiar() {
//        if (this.testEmpresaId > 0) {
//            empresaBO.eliminar(this.testEmpresaId);
//        }
    }
    
    @Test
    @Order(1)
    @Override
    public void debeListar() {
        List<Empresa> lista = empresaBO.listar();
        assertNotNull(lista);
    }
    
    @Test
    @Order(2)
    @Override
    public void debeObtenerSiIdExiste() {
        crearEmpresa();
        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
        assertNotNull(empresa);
        assertEquals(this.testEmpresaId, empresa.getIdEmpresa());
    }
    
    @Test
    @Order(3)
    @Override
    public void noDebeObtenerSiIdNoExiste() {
        Empresa empresa = empresaBO.obtener(this.idIncorrecto);
        assertNull(empresa);
    }
    
    @Test
    @Order(4)
    @Override
    public void debeGuardarNuevo() {
        crearEmpresa();
        assertTrue(this.testEmpresaId > 0);
    }
    
    @Test
    @Order(5)
    @Override
    public void debeGuardarModificado() {
        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
        empresa.setRazonSocial("Empresa Modificada SAC");
        empresa.setTelefono("912345678");
        empresa.setActivo(false);
        empresaBO.guardar(empresa, Estado.MODIFICADO);

        Empresa modificada = empresaBO.obtener(this.testEmpresaId);
        assertEquals("Empresa Modificada SAC", modificada.getRazonSocial());
        assertEquals("912345678", modificada.getTelefono());
        assertFalse(modificada.isActivo());
    }
    
    @Test
    @Order(6)
    @Override
    public void debeEliminarSiIdExiste() {
        empresaBO.eliminar(this.testEmpresaId);
        Empresa empresa = empresaBO.obtener(this.testEmpresaId);
        assertNull(empresa);
    }
    
    @Test
    @Order(7)
    @Override
    public void noDebeEliminarSiIdNoExiste() {
        assertThrows(RuntimeException.class, () -> empresaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(8)
    @Override
    public void debeHacerRollbackSiErrorEnGuardar() {
        Empresa empresa = new Empresa();
        // Forzamos un error: razon social nula
        empresa.setRazonSocial(null);
        empresa.setTipoDocumento(TipoDocumento.RUC);
        empresa.setTelefono("987654321");
        empresa.setEmail("error@test.com");
        empresa.setActivo(true);
        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);

        assertThrows(RuntimeException.class, () -> empresaBO.guardar(empresa, Estado.NUEVO));
    }
    
    @Test
    @Order(9)
    @Override
    public void debeHacerRollbackSiErrorEnEliminar() {
        assertThrows(RuntimeException.class, () -> empresaBO.eliminar(idIncorrecto));
    }
    
    @Test
    @Order(10)
    public void debeCrearEmpresaProveedor() {
        Empresa proveedor = new Empresa();
        proveedor.setTipoDocumento(TipoDocumento.RUC);
        proveedor.setRazonSocial("Proveedor Test SAC");
        proveedor.setTelefono("998877665");
        proveedor.setEmail("proveedor@test.com");
        proveedor.setActivo(true);
        proveedor.setTipoEmpresa(TipoEmpresa.PROVEEDOR);
        
        empresaBO.guardar(proveedor, Estado.NUEVO);
        
        assertTrue(proveedor.getIdEmpresa() > 0);
        assertEquals(TipoEmpresa.PROVEEDOR, proveedor.getTipoEmpresa());
        
        // Limpiar
        empresaBO.eliminar(proveedor.getIdEmpresa());
    }
    
    @Test
    @Order(11)
    public void debeCrearEmpresaCliente() {
        Empresa cliente = new Empresa();
        cliente.setTipoDocumento(TipoDocumento.RUC);
        cliente.setRazonSocial("Cliente Test SAC");
        cliente.setTelefono("955443322");
        cliente.setEmail("cliente@test.com");
        cliente.setActivo(true);
        cliente.setTipoEmpresa(TipoEmpresa.CLIENTE);
        
        empresaBO.guardar(cliente, Estado.NUEVO);
        
        assertTrue(cliente.getIdEmpresa() > 0);
        assertEquals(TipoEmpresa.CLIENTE, cliente.getTipoEmpresa());
        
        // Limpiar
        empresaBO.eliminar(cliente.getIdEmpresa());
    }
    
    private void crearEmpresa() {
        Empresa empresa = new Empresa();
        empresa.setTipoDocumento(TipoDocumento.RUC);
        empresa.setRazonSocial("Empresa Test SAC");
        empresa.setTelefono("987654321");
        empresa.setEmail("empresa@test.com");
        empresa.setActivo(true);
        empresa.setTipoEmpresa(TipoEmpresa.PROVEEDOR);

        empresaBO.guardar(empresa, Estado.NUEVO);
        this.testEmpresaId = empresa.getIdEmpresa();
    }
}