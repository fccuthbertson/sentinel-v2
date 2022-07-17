import { Component, OnInit } from '@angular/core';
import {FloatLabelType} from "@angular/material/form-field";
import {FormControl} from "@angular/forms";
import {MatTableDataSource} from "@angular/material/table";
import {bufferCount, bufferTime, delay, delayWhen, interval, map, Observable, of, sampleTime, scan, take} from "rxjs";

export interface Stochastic {
  value: number
}

/*
* The Formula for the Stochastic Oscillator Is
%K=( H14−L14 / C−L14 ) × 100
where:
C = The most recent closing price
L14 = The lowest price traded of the 14 previous
trading sessions
H14 = The highest price traded during the same
14-day period
%K = The current value of the stochastic indicator
* */

export interface Technicals {
  Stochastic: Stochastic
}

export interface ExchangeSymbol {
  ticker: string
  name: string
  technicals: Technicals
}

const exchange_data: Observable<ExchangeSymbol[]> = of(
    [{
    name: 'Spy index', ticker: 'SPY', technicals: {
      Stochastic: { value: 50 }},
  },
  {
    name: 'Spx index', ticker: 'SPX', technicals: {
      Stochastic: {value: 60}
    },
  }],
  [{
    name: 'Spy index', ticker: 'SPY', technicals: {
      Stochastic: { value: 51 }},
  },
    {
      name: 'Spx index', ticker: 'SPX', technicals: {
        Stochastic: {value: 61}
      },
    }],
  [{
    name: 'Spy index', ticker: 'SPY', technicals: {
      Stochastic: { value: 52 }},
  },
    {
      name: 'Spx index', ticker: 'SPX', technicals: {
        Stochastic: {value: 62}
      },
    }]
)

const polygonData = new Observable<Array<ExchangeSymbol>>(observer => {
  fetch('')
    .then(response => response.json())
    .then(data => {
      const s = new Array<ExchangeSymbol>()
      observer.next(s)
      observer.complete()
    })
})

@Component({
  selector: 'app-watcher',
  templateUrl: './watcher.component.html',
  styleUrls: ['./watcher.component.sass']
})
export class WatcherComponent implements OnInit {

  displayedColumns: string[] = ['ticker', 'name', 'stochastic'];
  allSymbols: MatTableDataSource<ExchangeSymbol>

  constructor() {
    this.allSymbols = new MatTableDataSource<ExchangeSymbol>()
  }

  ngOnInit(): void {
    polygonData.subscribe({
      next: (symbols) => {
        console.log(symbols)
        this.allSymbols.data = symbols
      },
      error: (err: Error) => {
        console.log(err)
      },
      complete: () => {
        console.log("no more exchange data")
      }
    })
  }
}
