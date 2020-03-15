import Chart from 'chart.js';
import { BillingSummaryService } from '../../../services/billing-summary.service';
import { ChartColorOption, ColorOption, TypeOption } from '../../../entites/dashboard';
import { BillingSummary } from '../../../entites/billing-summary';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { DashboardService } from '../../../services/dashboard.service';

@Component({
  selector: 'app-summary-graph',
  templateUrl: './summary-graph.component.html',
})
export class SummaryGraphComponent implements OnInit, OnDestroy {
  summaryCanvas: any;
  summaryCanvasContext: any;
  summaryChartData;

  isCreated: boolean;
  data: BillingSummary[];
  canvasData: number[];
  years: number[];
  selectedYear: number;
  colors: ColorOption[];
  displayTypes: TypeOption[];
  amountOfResults = [10, 25, 50, 100];
  selectedAmountOfResults = 10;
  selectedDisplayType = 'line';
  selectedColorType = 'red';
  canvasHtml = '<canvas id="generalSummary"></canvas>';
  now = new Date();
  destroyed = new Subject();
  constructor(private summaryService: BillingSummaryService, private dashBoardService: DashboardService) {}

  ngOnInit() {
    const range = (start, step) => Array.from({ length: 7}, (_, i) => start + (i * step));
    this.years = range(this.now.getFullYear(), -1);
    this.selectedYear = new Date(this.now.setMonth(this.now.getMonth() - 1)).getUTCFullYear();
    this.colors = this.dashBoardService.getColorOptions();
    this.displayTypes = this.dashBoardService.getTypeOptions();
    this.summaryService.GetAllSummaryByCustomer()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.data = result.sort((x, y) => x.rowId < y.rowId ? 1 : -1);
      this.createSummaryChart();
      });
  }
  ngOnDestroy() {
    this.destroyed.next();
  }
  createSummaryChart() {
    this.isCreated = false;
    this.summaryChartData = null;
    let rawData = this.data.filter(x => new Date(x.dateOfValue).getFullYear() === parseInt(this.selectedYear.toString(), 10));
    if (rawData.length > 0 ) {
      this.handleData(rawData);
    } else {
      this.selectedYear = this.selectedYear - 1;
      rawData = this.data.filter(x => new Date(x.dateOfValue).getFullYear() === parseInt(this.selectedYear.toString(), 10));
      if (rawData.length > 0 ) {
        this.handleData(rawData);
      }
    }
  }
  private handleData(rawData) {
    const chartLabels = [];
    this.canvasData = [];
    rawData.slice(0, parseInt(this.selectedAmountOfResults.toString(), 10))
    .forEach((summary) => {
      chartLabels.push('תאריך ערך - ' + new Date(summary.dateOfValue).toLocaleDateString('he').replace(/\./gi, '/') +
      (summary.supplierClientNumber !== 0 ? ' מספר משלם - ' + summary.supplierClientNumber : ''));
      this.canvasData.push(summary.totalDebit);
    });
    this.getContext();
    this.summaryChartData = new Chart(this.summaryCanvasContext,
    this.dashBoardService.createChartConfig(this.selectedColorType,
    this.selectedDisplayType, this.canvasData, chartLabels, this.summaryCanvasContext));
    this.isCreated = true;
  }
  private getContext() {
    const chartHtml = document.getElementById('generalSummary');
    if (chartHtml) {
      chartHtml.remove();
    }
    document.getElementById('summaryChart').innerHTML += this.canvasHtml;
    this.summaryCanvas = document.getElementById('generalSummary');
    this.summaryCanvasContext = this.summaryCanvas.getContext('2d');
  }
}
