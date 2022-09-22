const ctx = document.getElementById('myChart').getContext('2d');
const myChart = new Chart(ctx,
    {
        type: 'line',
        options: {
            animation: false,
            scales: {
                x: {
                    display: true,
                    text: 'Время'
                },
                y: {
                    beginAtZero: true,
                    display: true,
                    suggestedMax: 10,
                    text: 'Количество запросов',
                    ticks: {
                        stepSize: 1
                    }
                }
            },
        }
    }
);
function addData(chart, data) {
    chart.data = data;
    chart.update();
}

function removeData(chart) {
    chart.data.labels.pop();
    chart.data.datasets.forEach((dataset) => {
        dataset.data.pop();
    });
    chart.update();
}
function getCheckedElementName() {
    if (document.getElementById("min").checked) {
        return "min";
    }
    if (document.getElementById("hour").checked) {
        return "hour";
    }
    if (document.getElementById("day").checked) {
        return "day";
    }
}

async function SetData(datatype) {
    let apiData = (await fetch('/api/statistic', {
        method: 'GET',
        mode: 'no-cors',
        cache: 'no-cache',
        credentials: 'same-origin',
    }))
    let apiStatistic = await apiData.json();
    let data;
    switch (datatype) {
        case 'min':
            data = {
                labels: [],
                datasets: [
                    {
                        label: 'Статистика за минуту',
                        data: apiStatistic.MinutesQueueWithCurrent,
                        borderColor: 'rgba(200,200,200)',
                        backgroundColor: 'rgb(150,150,150)',
                        tension: 0.5
                    }
                ]
            }
            for (var i = 60; i >= 0; i--) {
                data.labels.push(i);
            }
            break

        case 'hour':
            data = {
                labels: [],
                datasets: [
                    {
                        label: 'Статистика за час',
                        data: apiStatistic.HourQueueWithCurrent,
                        borderColor: 'rgba(200,200,200)',
                        backgroundColor: 'rgb(150,150,150)',
                        tension: 0.5
                    }
                ]
            }
            for (var i = 60; i >= 0; i--) {
                data.labels.push(i);
            }
            break
        case 'day':
            data = {
                labels: [],
                datasets: [
                    {
                        label: 'Статистика за день',
                        data: apiStatistic.DayQueueWithCurrent,
                        borderColor: 'rgba(200,200,200)',
                        backgroundColor: 'rgb(150,150,150)',
                        tension: 0.5
                    }
                ]
            }
            for (var i = 24; i >= 0; i--) {
                data.labels.push(i);
            }
            break            
    }
    removeData(myChart);
    addData(myChart, data);
    myChart.update();
}
function SetDataInChart(dataType) {
    SetData(dataType);
}

function setDataMin() {
    SetDataInChart("min");
}
function setDataHour() {
    SetDataInChart("hour");
}
function setDataDay() {
    SetDataInChart("day");
}

SetDataInChart("min");
let timerId = window.setInterval(function () { SetDataInChart(getCheckedElementName()) }, 1000);