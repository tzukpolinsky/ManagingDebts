import { Injectable } from '@angular/core';
import { ChartColorOption, ColorOption, TypeOption, ChartInfo } from '../entites/dashboard';
import { HttpClient } from '@angular/common/http';
import { Supplier } from '../entites/supplier';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  purpleChart: ChartColorOption = {
    ticksFontColor: '#5603ad',
    XaxesColor: '#E14ECA',
    borderColor: '#8965e0'
  };
  blueChart: ChartColorOption = {
    ticksFontColor: '#5e72e4',
    XaxesColor: '#1D8CF8',
    borderColor: '#1f8ef1'
  };
  redChart: ChartColorOption = {
    ticksFontColor: '#e26a6a',
    XaxesColor: '#e08283',
    borderColor: '#ec250d'
  };
  orangeChart: ChartColorOption = {
    ticksFontColor: '#ff8a76',
    XaxesColor: '#DC3545',
    borderColor: '#fb6340'
  };
  greenChart: ChartColorOption = {
    ticksFontColor: '#2dce89',
    XaxesColor: '#00F2C3',
    borderColor: '#2dce89'
  };
  gradientChartOptionsConfiguration = {
    maintainAspectRatio: false,
    legend: {
      display: false
    },

    tooltips: {
      backgroundColor: '#f5f5f5',
      titleFontColor: '#333',
      bodyFontColor: '#666',
      bodySpacing: 4,
      xPadding: 12,
      mode: 'nearest',
      intersect: 0,
      position: 'nearest'
    },
    responsive: true,
    scales: {
      yAxes: [{
        barPercentage: 1.6,
        gridLines: {
          drawBorder: false,
          color: 'rgba(29,140,248,0.0)',
          zeroLineColor: 'transparent',
        },
        ticks: {
          suggestedMin: 60,
          suggestedMax: 125,
          padding: 20,
          fontColor: ''
        }
      }],

      xAxes: [{
        barPercentage: 1.6,
        gridLines: {
          drawBorder: false,
          color: '',
          zeroLineColor: 'transparent',
        },
        ticks: {
          padding: 20,
          fontColor: ''
        }
      }]
    }
  };
  gradientBarChartConfiguration: any = {
    maintainAspectRatio: false,
    legend: {
      display: false
    },

    tooltips: {
      backgroundColor: '#f5f5f5',
      titleFontColor: '#333',
      bodyFontColor: '#666',
      bodySpacing: 4,
      xPadding: 12,
      mode: 'nearest',
      intersect: 0,
      position: 'nearest'
    },
    responsive: true,
    scales: {
      yAxes: [{

        gridLines: {
          drawBorder: false,
          color: 'rgba(29,140,248,0.1)',
          zeroLineColor: 'transparent',
        },
        ticks: {
          suggestedMin: 60,
          suggestedMax: 120,
          padding: 20,
          fontColor: '#9e9e9e'
        }
      }],

      xAxes: [{

        gridLines: {
          drawBorder: false,
          color: 'rgba(29,140,248,0.1)',
          zeroLineColor: 'transparent',
        },
        ticks: {
          padding: 20,
          fontColor: '#9e9e9e'
        }
      }]
    }
  };

  constructor(private httpClient: HttpClient) { }
  createChartConfig(color: string, type: string , data: any, chartLabels: any, canvasContext: any) {
    const gradientStroke = canvasContext.createLinearGradient(0, 230, 0, 50);
    let gradientConfiguration = null;
    let config = null;
    switch (type) {
      case 'line':
        gradientConfiguration = this.gradientChartOptionsConfiguration;
        config = {
          type: 'line',
          data: {
            labels: chartLabels,
            datasets: [{
              label: 'סכום',
              fill: true,
              backgroundColor: gradientStroke,
              borderColor: '',
              borderWidth: 2,
              borderDash: [],
              borderDashOffset: 0.0,
              data,
            }]
          },
          options: gradientConfiguration
        };
        break;
        case 'bar':
          gradientConfiguration = this.gradientBarChartConfiguration;
          config = {
            type: 'bar',
            responsive: true,
            legend: {
              display: false
            },
            data: {
              labels: chartLabels,
              datasets: [{
                label: 'סכום',
                fill: true,
                backgroundColor: gradientStroke,
                hoverBackgroundColor: gradientStroke,
                borderColor: '',
                borderWidth: 2,
                borderDash: [],
                borderDashOffset: 0.0,
                data,
              }]
            },
            options: gradientConfiguration
          };
          break;
      default:
        break;
    }
    switch (color) {
      case 'red':
        gradientConfiguration.scales.yAxes[0].ticks.fontColor = this.redChart.ticksFontColor;
        gradientConfiguration.scales.xAxes[0].ticks.fontColor = this.redChart.ticksFontColor;
        config.data.datasets[0].borderColor = this.redChart.borderColor;
        gradientStroke.addColorStop(1, 'rgba(233,32,16,0.2)');
        gradientStroke.addColorStop(0.4, 'rgba(233,32,16,0.0)');
        gradientStroke.addColorStop(0, 'rgba(233,32,16,0)'); // red colors
        break;
      case 'blue':
        gradientConfiguration.scales.yAxes[0].ticks.fontColor = this.blueChart.ticksFontColor;
        gradientConfiguration.scales.xAxes[0].ticks.fontColor = this.blueChart.ticksFontColor;
        config.data.datasets[0].borderColor = this.blueChart.borderColor;
        gradientStroke.addColorStop(1, 'rgba(29,140,248,0.2)');
        gradientStroke.addColorStop(0.4, 'rgba(29,140,248,0.0)');
        gradientStroke.addColorStop(0, 'rgba(29,140,248,0)'); // blue colors
        break;
      case 'green':
        gradientConfiguration.scales.yAxes[0].ticks.fontColor = this.greenChart.ticksFontColor;
        gradientConfiguration.scales.xAxes[0].ticks.fontColor = this.greenChart.ticksFontColor;
        config.data.datasets[0].borderColor = this.greenChart.borderColor;
        gradientStroke.addColorStop(1, 'rgba(66,134,121,0.15)');
        gradientStroke.addColorStop(0.4, 'rgba(66,134,121,0.0)');
        gradientStroke.addColorStop(0, 'rgba(66,134,121,0)'); // green colors
        break;
      case 'purple':
        gradientConfiguration.scales.yAxes[0].ticks.fontColor = this.purpleChart.ticksFontColor;
        gradientConfiguration.scales.xAxes[0].ticks.fontColor = this.purpleChart.ticksFontColor;
        config.data.datasets[0].borderColor = this.purpleChart.borderColor;
        gradientStroke.addColorStop(1, 'rgba(190, 144, 212,1)');
        gradientStroke.addColorStop(0.4, 'rgba(169, 109, 173, 1)');
        gradientStroke.addColorStop(0, 'rgba(155, 89, 182, 1)'); // purple colors
        break;
      case 'orange':
        gradientConfiguration.scales.yAxes[0].ticks.fontColor = this.orangeChart.ticksFontColor;
        gradientConfiguration.scales.xAxes[0].ticks.fontColor = this.orangeChart.ticksFontColor;
        config.data.datasets[0].borderColor = this.orangeChart.borderColor;
        gradientStroke.addColorStop(1, 'rgba(253, 227, 167, 1)');
        gradientStroke.addColorStop(0.4, 'rgba(252, 214, 112, 1)');
        gradientStroke.addColorStop(0, 'rgba(249, 191, 59, 1)'); // orange colors
        break;
      default:
        break;
    }
    return config;
  }
  getColorOptions(): ColorOption[] {
    return [
      {
        text: 'אדום',
        type: 'red',
      },
      {
        text: 'כחול',
        type: 'blue',
      },
      {
        text: 'כתום',
        type: 'orange',
      },
      {
        text: 'ירוק',
        type: 'green',
      },
      {
        text: 'סגול',
        type: 'purple',
      },
    ];
  }
  getTypeOptions(): TypeOption[] {
    return [
      {
        text: 'גרף קוי',
        type: 'line'
      },
      {
        text: 'גרף עמודות',
        type: 'bar'
      },
    ];
  }
  getContractsInfo(supplier: Supplier) {
    const url = environment.serverUrl + 'dashboard/getContractsInfo';
    return this.httpClient.post<ChartInfo[]>(url, supplier);
  }
  getBudgetsInfo(supplier: Supplier) {
    const url = environment.serverUrl + 'dashboard/getBudgetsInfo';
    return this.httpClient.post<ChartInfo[]>(url, supplier);
  }
}


