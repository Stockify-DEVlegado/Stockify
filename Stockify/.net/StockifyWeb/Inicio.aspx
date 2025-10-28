<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="StockifyWeb.Inicio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_Contenido" runat="server">
  
  <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

  <style>
    .dbx{ --bg:#0b0c0f; --card:#21252d; --card2:#2a2f39; --stroke:#323844; --text:#e7eaf0; --muted:#a9b3c7; --accent:#8aa2ff; --accent2:#f0b75d; --radius:16px; --shadow:0 10px 24px rgba(0,0,0,.35); }
    .dbx{color:var(--text); background:var(--bg); padding:16px 20px; border-radius:12px}
    .dbx .topbar{display:flex; gap:12px; align-items:center; margin-bottom:12px}
    .dbx .search{flex:1; display:flex; gap:8px; align-items:center; background:#121419; border:1px solid var(--stroke); border-radius:40px; padding:8px 14px}
    .dbx .search input{flex:1; background:transparent; border:0; outline:none; color:var(--text); font-size:14px}
    .dbx .kpis{display:grid; grid-template-columns:repeat(5,minmax(140px,1fr)); gap:12px; margin-top:12px}
    .dbx .card{background:var(--card); border:1px solid var(--stroke); border-radius:var(--radius); padding:14px; box-shadow:var(--shadow)}
    .dbx .kpi .title{color:var(--muted); font-size:11px; margin-bottom:4px}
    .dbx .kpi .value{font-size:28px; font-weight:800; color:#ffffff !important}
    .dbx .kpi .hint{color:#a0a9bb; font-size:11px}
    .dbx .content{display:grid; grid-template-columns:1fr 320px; gap:16px; margin-top:12px}
    .dbx h2{margin:0 0 6px 0; font-size:16px; color:#ffffff !important}
    .dbx .chart{height:320px; position:relative}
    .dbx .legend{position:absolute; bottom:10px; left:50%; transform:translateX(-50%); display:flex; gap:12px; align-items:center}
    .dbx .dot{width:8px; height:8px; border-radius:50%}
    .dbx .dot.red{background:#ff4444} /* COMPRAS */
    .dbx .dot.green{background:#b6ff00} /* VENTAS */
    .dbx .two-col{display:grid; grid-template-columns:1fr 1fr; gap:8px}
    .dbx .mini{background:var(--card2); border:1px solid var(--stroke); border-radius:10px; padding:12px; text-align:center}
    .dbx .mini .big{font-weight:800; font-size:18px; color:#ffffff !important}
    .dbx .alerts .item{display:flex; align-items:center; justify-content:space-between; background:var(--card2); border:1px solid var(--stroke); border-radius:10px; padding:10px 12px; margin-top:8px}
    .dbx .alerts .name{font-weight:600; color:#ffffff !important; font-size:14px}
    .dbx .badge{background:#331b1b; border:1px solid #5a2a2a; color:#ffb0b0; padding:3px 8px; border-radius:999px; font-size:11px}
    .dbx .orders{display:grid; grid-template-columns:repeat(4, 1fr); gap:10px; margin-top:8px}
    .dbx .pill{padding:12px; background:var(--card); border:1px solid var(--stroke); border-radius:12px; text-align:center; min-height:60px; display:flex; flex-direction:column; justify-content:center}
    .dbx .pill .type{font-weight:700; color:#ffffff !important; margin-bottom:2px; font-size:14px}
    .dbx .pill .date{color:var(--muted); font-size:11px}
    
    .dbx .chart-container {
      width: 100%;
      height: 220px;
      margin-top: 8px;
      border-radius: 10px;
      background: #171a20;
      border: 1px solid var(--stroke);
      padding: 8px;
    }
    
    .dbx .mini > div:first-child { 
      color: #ffffff !important; 
      font-size: 13px;
    }
    .dbx .alerts .item > div:first-child > div:last-child { 
      color: #e0e0e0 !important; 
      font-size: 11px;
    }
    
    @media (max-width:1200px){ 
      .dbx .kpis{grid-template-columns:repeat(3,minmax(130px,1fr))} 
      .dbx .content{grid-template-columns:1fr}
      .dbx .orders{grid-template-columns:repeat(2, 1fr)}
    }
    @media (max-width:760px){ 
      .dbx{padding:12px} 
      .dbx .kpis{grid-template-columns:repeat(2,minmax(120px,1fr))} 
      .dbx .orders{grid-template-columns:1fr}
    }
  </style>

  <section class="dbx">
 
    <div class="topbar">
      <div class="search" role="search">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="7"/><path d="M21 21l-3.4-3.4"/></svg>
        <input type="text" placeholder="Search product, supplier, order" aria-label="Buscar" />
      </div>
    </div>

    <section class="kpis">
      <article class="card kpi"><div class="title">Total Productos</div><div class="value">50</div><div class="hint">Registrados</div></article>
      <article class="card kpi"><div class="title">En Stock</div><div class="value">50</div><div class="hint">Unidades</div></article>
      <article class="card kpi"><div class="title">Por recibir</div><div class="value">14</div><div class="hint">Unidades</div></article>
      <article class="card kpi"><div class="title">Entradas (7d)</div><div class="value">5</div><div class="hint">Movimientos de entrada</div></article>
      <article class="card kpi"><div class="title">Salidas (7d)</div><div class="value">21</div><div class="hint">Movimientos de salida</div></article>
    </section>

    <section class="content">
      <div>
        <article class="card chart" aria-label="Resúmenes de movimientos">
          <h2>Resúmenes de movimientos</h2>
          <div class="chart-container">
            <canvas id="movementChart"></canvas>
          </div>
          <div class="legend">
            <span class="dot red"></span><span style="color:#ffffff; font-size:13px">Compras</span>
            <span class="dot green"></span><span style="color:#ffffff; font-size:13px">Ventas</span>
          </div>
        </article>


        <section style="margin-top:14px">
          <h2>Órdenes recientes</h2>
          <div class="orders">
            <div class="pill">
              <div class="type">Compra</div>
              <div class="date">Fecha: 21/07/2025</div>
            </div>
            <div class="pill">
              <div class="type">Venta</div>
              <div class="date">Fecha: 30/07/2025</div>
            </div>
            <div class="pill">
              <div class="type">Venta</div>
              <div class="date">Fecha: 15/08/2025</div>
            </div>
            <div class="pill">
              <div class="type">Compra</div>
              <div class="date">Fecha: 15/08/2025</div>
            </div>
          </div>
        </section>
      </div>

      <div>
        <article class="card">
          <h2>Resumen productos</h2>
          <div class="two-col" style="margin-top:8px">
            <div class="mini"><div style="opacity:1; color:#ffffff !important">Número de Proveedores</div><div class="big">31</div></div>
            <div class="mini"><div style="opacity:1; color:#ffffff !important">Número de categorías</div><div class="big">21</div></div>
          </div>
        </article>
        <article class="card alerts" style="margin-top:14px; height:180px; display:flex; flex-direction:column; justify-content:space-between;">
          <h2>Alertas de Stock</h2>
          <div>
            <div class="item">
              <div>
                <div class="name">Teclado en ruso</div>
                <div style="color:#e0e0e0 !important;font-size:11px">Stock actual: 2 | Mínimo: 5</div>
              </div>
              <span class="badge">Low</span>
            </div>
            <div class="item">
              <div>
                <div class="name">Monitor ShangChun</div>
                <div style="color:#e0e0e0 !important;font-size:11px">Stock actual: 2 | Mínimo: 5</div>
              </div>
              <span class="badge">Low</span>
            </div>
          </div>
        </article>
      </div>
    </section>
  </section>

  <script>
      document.addEventListener('DOMContentLoaded', function () {
          const ctx = document.getElementById('movementChart').getContext('2d');

          const labels = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul'];
          const comprasData = [65, 59, 80, 81, 56, 55, 40];
          const ventasData = [28, 48, 40, 19, 86, 27, 90];

          const movementChart = new Chart(ctx, {
              type: 'line',
              data: {
                  labels: labels,
                  datasets: [
                      {
                          label: 'Compras',
                          data: comprasData,
                          borderColor: '#ff4444', // ROJO BRILLANTE
                          backgroundColor: 'transparent',
                          tension: 0.4,
                          borderWidth: 3,
                          pointBackgroundColor: '#ff4444',
                          pointBorderColor: '#171a20',
                          pointBorderWidth: 2,
                          pointRadius: 3,
                      },
                      {
                          label: 'Ventas',
                          data: ventasData,
                          borderColor: '#b6ff00', // VERDE ESPECÍFICO
                          backgroundColor: 'transparent',
                          tension: 0.4,
                          borderWidth: 3,
                          pointBackgroundColor: '#b6ff00',
                          pointBorderColor: '#171a20',
                          pointBorderWidth: 2,
                          pointRadius: 3,
                      }
                  ]
              },
              options: {
                  responsive: true,
                  maintainAspectRatio: false,
                  plugins: {
                      legend: {
                          display: false
                      },
                      tooltip: {
                          backgroundColor: '#21252d',
                          titleColor: '#ffffff',
                          bodyColor: '#ffffff',
                          borderColor: '#323844',
                          borderWidth: 1
                      }
                  },
                  scales: {
                      x: {
                          grid: {
                              color: '#2b3140',
                              drawBorder: false
                          },
                          ticks: {
                              color: '#ffffff',
                              font: {
                                  size: 11
                              }
                          }
                      },
                      y: {
                          grid: {
                              color: '#2b3140',
                              drawBorder: false
                          },
                          ticks: {
                              color: '#ffffff',
                              font: {
                                  size: 11
                              }
                          }
                      }
                  }
              }
          });
      });
  </script>
</asp:Content>