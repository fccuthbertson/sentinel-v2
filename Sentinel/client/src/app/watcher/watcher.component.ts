import { Component, OnInit } from '@angular/core';
import {MatTable} from "@angular/material/table";

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

export interface Stochastic {
  periods: number
  K: number
  D: number
}

export interface Technicals {
  Stochastic: Stochastic
}

export interface ExchangeSymbol {
  ticker: string
  name: string
  technicals: Technicals
}

const exchange_data: ExchangeSymbol[] = [
  {
    name: 'Spy index', ticker: 'SPY', technicals: {
      Stochastic: {
        periods: 14,
        K: 20,
        D: 50
      }
    },
  },
  {
    name: 'Spx index', ticker: 'SPX', technicals: {
      Stochastic: {
        periods: 14,
        K: 21,
        D: 45
      }
    },
  }
];

@Component({
  selector: 'app-watcher',
  templateUrl: './watcher.component.html',
  styleUrls: ['./watcher.component.sass']
})
export class WatcherComponent implements OnInit {

  displayedColumns: string[] = ['ticker', 'name', '%K', '%D'];
  dataSource = exchange_data;
  constructor() { }

  ngOnInit(): void {
  }

}
