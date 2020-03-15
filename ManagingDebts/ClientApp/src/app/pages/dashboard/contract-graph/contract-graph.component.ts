import { Component, OnInit } from '@angular/core';
import { ColorOption, TypeOption, ChartInfo } from '../../../entites/dashboard';
import { Subject } from 'rxjs';
import { DashboardService } from '../../../services/dashboard.service';
import { Supplier } from '../../../entites/supplier';
import { takeUntil } from 'rxjs/operators';
import { Contract } from '../../../entites/contract';
import { ContractService } from '../../../services/contract.service';
import Chart from 'chart.js';
import { SupplierService } from '../../../services/supplier.service';

@Component({
  selector: 'app-contract-graph',
  templateUrl: './contract-graph.component.html',
})
export class ContractGraphComponent implements OnInit {
  contractCanvas: any;
  contractCanvasContext: any;
  contractChartData;
  contracts: Contract[];
  selectedContracts: Contract[];
  isCreated: boolean;
  data: ChartInfo[];
  canvasData: number[];
  years: number[];
  selectedYear: number;
  colors: ColorOption[];
  displayTypes: TypeOption[];
  supplier: Supplier = this.supplierService.getCurrentSupplier();
  selectedDisplayType = 'bar';
  selectedColorType = 'blue';
  canvasHtml = '<canvas id="contractChart"></canvas>';
  now = new Date();
  destroyed = new Subject();
  constructor( private dashBoardService: DashboardService, private contractService: ContractService,
               private supplierService: SupplierService) { }

  ngOnInit() {
    const range = (start, step) => Array.from({ length: 7}, (_, i) => start + (i * step));
    this.years = range(this.now.getFullYear(), -1);
    this.selectedYear = new Date(this.now.setMonth(this.now.getMonth() - 1)).getUTCFullYear();
    this.colors = this.dashBoardService.getColorOptions();
    this.displayTypes = this.dashBoardService.getTypeOptions();
    this.contractService.getContractsBySupplier().
    pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.contracts = result;
      this.selectedContracts = this.selectedContracts && this.selectedContracts.length > 0 ? this.selectedContracts : result.slice(0, 10);
    });
    this.dashBoardService.getContractsInfo(this.supplier)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.data = result;
      const interval = setInterval(() => {
        if ( this.selectedContracts && this.selectedContracts.length > 0) {
          this.createChart();
          clearInterval(interval);
        }
      }, 10);
    });
  }
  createChart() {
    this.isCreated = false;
    this.contractChartData = null;
    this.canvasData = [];
    const chartLabels = [];
    this.data.forEach((info) => {
      const contract = this.selectedContracts.find(x => x.id === info.topicId);
      if (contract) {
        chartLabels.push(contract.id + ' - ' + contract.description);
        this.canvasData.push(info.amount);
      }
    });
    if (this.canvasData && this.canvasData.length > 0) {
      this.getContext();
      this.contractChartData = new Chart(this.contractCanvasContext,
        this.dashBoardService.createChartConfig(this.selectedColorType,
        this.selectedDisplayType, this.canvasData, chartLabels, this.contractCanvasContext));
      this.isCreated = true;
    }
  }
  private getContext() {
    const chartHtml = document.getElementById('contractChart');
    if (chartHtml) {
      chartHtml.remove();
    }
    document.getElementById('contractChartPlaceHolder').innerHTML += this.canvasHtml;
    this.contractCanvas = document.getElementById('contractChart');
    this.contractCanvasContext = this.contractCanvas.getContext('2d');
  }

}
