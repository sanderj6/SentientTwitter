import "../Chart.js/chart.min.js"

window.charts = window.charts || {}

export function RenderCharts(percentages) {
    Chart.defaults.color = '#818d96',
        Chart.defaults.scale.grid.lineWidth = 0,
        Chart.defaults.scale.beginAtZero = !0,
        Chart.defaults.datasets.bar.maxBarThickness = 45,
        Chart.defaults.elements.bar.borderRadius = 4,
        Chart.defaults.elements.bar.borderSkipped = !1,
        Chart.defaults.elements.point.radius = 0,
        Chart.defaults.elements.point.hoverRadius = 0,
        Chart.defaults.plugins.tooltip.radius = 3,
        Chart.defaults.plugins.legend.labels.boxWidth = 10;

    //PIE CHART
    window.charts["chartjs-pie"] = new Chart(jQuery("canvas[id='chartjs-pie']"), {
        type: 'pie',
        data: {
            labels: ["Positive", "Negative", "Neutral"],
            datasets: [{
                data: percentages,
                backgroundColor: [
                    '#3cbc4c', '#f46159', 'rgba(100,116,139,1)'
                ],
                borderWidth: 5,
                borderColor: "transparent",
                pointbackgroundcolor: "rgba(100,116,139,1)",
                pointbordercolor: "#fff",
                pointhoverbackgroundcolor: "#fff",
                pointhoverbordercolor: "rgba(100,116,139,1)",
                hoverOffset: 4
            }]
        },
        options: {
            responsive: true
        }
    });
}

export function DisposeCharts() {
    if (window.charts == undefined) return;

    if (window.charts["chartjs-pie"] != undefined) {
        try {
            window.charts["charpie-pie"].destroy();
            delete window.charts["charpie-pie"];
        }
        catch (error) {
            console.info("Failed to dispose of tweet chart");
        }
    }
}