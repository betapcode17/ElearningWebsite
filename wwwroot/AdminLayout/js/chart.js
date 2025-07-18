const chartDataElement = document.getElementById("chart-data");
const labelsAttr = chartDataElement
  ? chartDataElement.getAttribute("data-labels") || "[]"
  : "[]";
const valuesAttr = chartDataElement
  ? chartDataElement.getAttribute("data-values") || "[]"
  : "[]";

let chartLabels = [];
let chartData = [];
try {
  chartLabels = JSON.parse(labelsAttr);
  chartData = JSON.parse(valuesAttr);
} catch (error) {
  console.error("Error parsing chart data:", error);
  chartLabels = [];
  chartData = [];
}

const revenueLineChart = new Chart(
  document.getElementById("revenueLineChart"),
  {
    type: "line",
    data: {
      labels: chartLabels,
      datasets: [
        {
          label: "Doanh thu (VNĐ)",
          data: chartData,
          borderColor: "#2b6cb0",
          backgroundColor: "rgba(43, 108, 176, 0.2)",
          fill: true,
          tension: 0.4,
          pointRadius: 5,
          pointHoverRadius: 8,
          pointBackgroundColor: "#2b6cb0",
          pointBorderColor: "#ffffff",
          pointBorderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          title: {
            display: true,
            text: "Doanh thu (VNĐ)",
            font: { size: 14, weight: "bold" },
            color: "#2d3436",
          },
          grid: {
            color: "rgba(0, 0, 0, 0.05)",
          },
          ticks: {
            callback: function (value) {
              return value.toLocaleString() + " VNĐ";
            },
            font: { size: 12 },
            color: "#636e72",
          },
        },
        x: {
          title: {
            display: true,
            text: "Thời gian",
            font: { size: 14, weight: "bold" },
            color: "#2d3436",
          },
          grid: {
            display: false,
          },
          ticks: {
            font: { size: 12 },
            color: "#636e72",
          },
        },
      },
      plugins: {
        legend: {
          display: true,
          position: "top",
          labels: {
            font: { size: 14 },
            color: "#2d3436",
          },
        },
        tooltip: {
          backgroundColor: "rgba(0, 0, 0, 0.8)",
          titleFont: { size: 14 },
          bodyFont: { size: 12 },
          callbacks: {
            label: function (context) {
              return `Doanh thu: ${context.parsed.y.toLocaleString()} VNĐ`;
            },
          },
        },
      },
      interaction: {
        mode: "nearest",
        intersect: false,
      },
    },
  }
);

function exportReport() {
  alert("Chức năng xuất báo cáo đang được phát triển!");
}
