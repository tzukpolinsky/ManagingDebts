import { Component, OnInit } from '@angular/core';
import { ChartInfo, ColorOption, TypeOption } from '../../../entites/dashboard';
import { Supplier } from '../../../entites/supplier';
import { Subject } from 'rxjs';
import { DashboardService } from '../../../services/dashboard.service';
import { BudgetService } from '../../../services/budget.service';
import { takeUntil } from 'rxjs/operators';
import Chart from 'chart.js';
import { Budget } from '../../../entites/budget';
import { SupplierService } from '../../../services/supplier.service';

@Component({
  selector: 'app-budget-graph',
  templateUrl: './budget-graph.component.html',
})
export class BudgetGraphComponent implements OnInit {
  budgetCanvas: any;
  budgetCanvasContext: any;
  budgetChartData;
  budgets: Budget[];
  selectedBudgets: Budget[];
  isCreated: boolean;
  data: ChartInfo[];
  canvasData: number[];
  years: number[];
  selectedYear: number;
  colors: ColorOption[];
  displayTypes: TypeOption[];
  supplier: Supplier = this.supplierService.getCurrentSupplier();
  selectedDisplayType = 'line';
  selectedColorType = 'green';
  canvasHtml = '<canvas id="budgetChart"></canvas>';
  now = new Date();
  destroyed = new Subject();
  constructor( private dashBoardService: DashboardService, private budgetService: BudgetService,
               private supplierService: SupplierService) { }

  ngOnInit() {
    const range = (start, step) => Array.from({ length: 7}, (_, i) => start + (i * step));
    this.years = range(this.now.getFullYear(), -1);
    this.selectedYear = new Date(this.now.setMonth(this.now.getMonth() - 1)).getUTCFullYear();
    this.colors = this.dashBoardService.getColorOptions();
    this.displayTypes = this.dashBoardService.getTypeOptions();
    this.budgetService.getBySupplier().
    pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.budgets = result;
      this.selectedBudgets = result.slice(0, 10);
    });
    this.dashBoardService.getBudgetsInfo(this.supplier)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.data = result;
      const interval = setInterval(() => {
        if ( this.selectedBudgets && this.selectedBudgets.length > 0) {
          this.createChart();
          clearInterval(interval);
        }
      }, 10);
    });
  }
  createChart() {
    this.isCreated = false;
    this.budgetChartData = null;
    this.canvasData = [];
    const chartLabels = [];
    this.data.forEach((info) => {
      const budget = this.selectedBudgets.find(x => x.id === info.topicId);
      if (budget) {
        chartLabels.push(budget.name);
        this.canvasData.push(info.amount);
      }
    });
    if (this.canvasData && this.canvasData.length > 0) {
      this.getContext();
      this.budgetChartData = new Chart(this.budgetCanvasContext,
        this.dashBoardService.createChartConfig(this.selectedColorType,
        this.selectedDisplayType, this.canvasData, chartLabels, this.budgetCanvasContext));
      this.isCreated = true;
    }
  }
  private getContext() {
    const chartHtml = document.getElementById('budgetChart');
    if (chartHtml) {
      chartHtml.remove();
    }
    document.getElementById('budegtChartPlaceHolder').innerHTML += this.canvasHtml;
    this.budgetCanvas = document.getElementById('budgetChart');
    this.budgetCanvasContext = this.budgetCanvas.getContext('2d');
  }
}
