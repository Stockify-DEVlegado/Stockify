/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package pe.edu.pucp.inf30.stockify.daoimpl.usuario;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;

import pe.edu.pucp.inf30.stockify.dao.usuario.EmpresaDAO;
import pe.edu.pucp.inf30.stockify.daoimpl.BaseDAO;
import pe.edu.pucp.inf30.stockify.model.usuario.Empresa;

/**
 *
 * @author DEVlegado
 */

public class EmpresaDAOImpl extends BaseDAO<Empresa> implements EmpresaDAO {

    @Override
    protected PreparedStatement comandoCrear(Connection conn, Empresa modelo)
            throws SQLException {
        String sql = "{call insertarEmpresa(?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_tipoDocumento", String.valueOf(modelo.getTipoDocumento()));
        cmd.setString("p_razonSocial", modelo.getRazonSocial());
        cmd.setString("p_telefono", modelo.getTelefono());
        cmd.setString("p_email", modelo.getEmail());
        cmd.setBoolean("p_activo", modelo.isActivo());
        cmd.setString("p_tipoEmpresa", String.valueOf(modelo.getTipoEmpresa()));
        cmd.registerOutParameter("p_id", Types.INTEGER);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoActualizar(Connection conn,
            Empresa modelo) throws SQLException {

        String sql = "{call modificarEmpresa(?, ?, ?, ?, ?, ?, ?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setString("p_tipoDocumento", String.valueOf(modelo.getTipoDocumento()));
        cmd.setString("p_razonSocial", modelo.getRazonSocial());
        cmd.setString("p_telefono", modelo.getTelefono());
        cmd.setString("p_email", modelo.getEmail());
        cmd.setBoolean("p_activo", modelo.isActivo());
        cmd.setString("p_tipoEmpresa", String.valueOf(modelo.getTipoEmpresa()));
        cmd.setInt("p_id", modelo.getIdEmpresa());
        return cmd;
    }

    @Override
    protected PreparedStatement comandoEliminar(Connection conn, Integer idEmpresa) throws SQLException {
        String sql = "{call eliminarEmpresa(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", idEmpresa);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeer(Connection conn, Integer idEmpresa) throws SQLException {
        String sql = "{call buscarEmpresaPorId(?)}";
        CallableStatement cmd = conn.prepareCall(sql);
        cmd.setInt("p_id", idEmpresa);
        return cmd;
    }

    @Override
    protected PreparedStatement comandoLeerTodos(Connection conn) throws SQLException {
        String sql = "{call listarEmpresas()}";
        CallableStatement cmd = conn.prepareCall(sql);
        return cmd;
    }

    @Override
    protected Empresa mapearModelo(ResultSet rs) throws SQLException {
        Empresa empresa = new Empresa();
        empresa.setIdEmpresa(rs.getInt("idEmpresa"));
        empresa.setRazonSocial(rs.getString("razonSocial"));
        empresa.setTelefono(rs.getString("telefono"));
        empresa.setEmail(rs.getString("email"));
        empresa.setActivo(rs.getBoolean("activo"));
        return empresa;
    }
}