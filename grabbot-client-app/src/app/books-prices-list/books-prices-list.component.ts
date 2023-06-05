//import { Component, OnInit } from '@angular/core';

import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { BookWithPriceList, PaginatedBooksResult } from '../Model/PaginatedBooksResult';
import { AngularCsv } from 'angular-csv-ext/dist/Angular-csv';
import { ScraperService } from '../services/scraper.service';
import * as FileSaver from 'file-saver';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { DownloadableBookData } from '../Model/DownloadableBookData';
import { CrawlStatusComponent } from '../crawl-status/crawl-status.component';
import { CrawlStatus } from '../Model/CrawlStatus';
import {MatDialog, MatDialogConfig } from '@angular/material/dialog';

@Component({
  selector: 'app-books-prices-list',
  templateUrl: './books-prices-list.component.html',
  styleUrls: ['./books-prices-list.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class BooksPricesListComponent implements OnInit {
  paginatedBooksResult!: PaginatedBooksResult;
  completeBookListWithPrices!: DownloadableBookData;
  booksWithPrices: BookWithPriceList[]= [];
  displayedColumns: string[] = ['ean', 'title', 'sitePrice', 'update', 'delete'];
  dataSource = new MatTableDataSource<BookWithPriceList>(this.booksWithPrices);
  pageIndex=0
  pageSize=10
  searchText =""
  totalCount = 0
  isTableExpanded = false;
  isLoading = false;
  isDownloadDisabled = true;
  crawlStatus: CrawlStatus | undefined;
  searchTextChanged = true;
 
  constructor(private scraperservice: ScraperService, public dialog: MatDialog ) { }

  @ViewChild(MatPaginator)
  paginator!: MatPaginator;

  ngOnInit(): void {
    this.dataSource.paginator = this.paginator;
    this.isLoading = true;
    this.isDownloadDisabled = true;
    this.getBooksWithPrices();
    this.GetDownloadableBooksDataFromLastHour();    
    //this.checkCrawlStatus();
  }

  getBooksWithPrices() {
    this.scraperservice.getBooksWithPrices(this.pageIndex+1, this.pageSize, this.searchText, this.searchTextChanged).subscribe(result => {
     this.paginatedBooksResult = result;
      this.booksWithPrices = this.paginatedBooksResult.booksWithPrices;
      this.dataSource = result.booksWithPrices;
      this.setPagination()
      this.isLoading = false;
      this.searchTextChanged = false;
      console.log("Table loaded Now")
    });
  }

  GetDownloadableBooksDataFromLastHour() {
    this.scraperservice.GetDownloadableBooksDataFromLastHour().subscribe(result => {
      this.completeBookListWithPrices = result;
      this.isDownloadDisabled = false;
      console.log("Download Now")
    });
  }

  setPagination()
  {
    this.pageIndex = this.paginatedBooksResult.currentPage-1
    this.totalCount = this.searchTextChanged ?  this.paginatedBooksResult.totalCount : this.totalCount
    this.pageSize = this.paginatedBooksResult.pageSize
  }

  pageChanged(event:any)
  {
    this.isLoading = true;
    this.pageIndex = event.pageIndex
    this.pageSize = event.pageSize
    this.getBooksWithPrices()
  }

  downloadCsv()
  {
    var headers =["EAN", "Title", "Product Url", "Price", "Difference", "Website", "Currency", "Last Crawled", "Last Crawl Duration", "Availablity"]
    new AngularCsv(this.completeBookListWithPrices.bookDataModel, 'Books Prices', {headers: (headers)});
  }

  downloadJson() 
  {
    // this.scraperservice.getBooksWithPrices(1,this.totalCount, this.searchText).subscribe(result => {     
    //   this.saveAsFile(JSON.stringify(result.booksWithPrices), 'jsonfile', '')
    // });
    this.saveAsFile(JSON.stringify(this.completeBookListWithPrices.bookWithPriceList), 'jsonfile', '')
  }

 
  private saveAsFile(buffer: any, fileName: string, fileType: string): void {
    const data: Blob = new Blob([buffer], { type: fileType });
    FileSaver.saveAs(data, fileName);
  }
  
   search(event:any) {
    this.pageIndex = 0
    this.isLoading = true;
    this.searchTextChanged = true;
    this.getBooksWithPrices();
    }

    toggleTableRows() {
      this.isTableExpanded = !this.isTableExpanded;  
      this.booksWithPrices.forEach((row: any) => {
        row.isExpanded = this.isTableExpanded;
      })
    }

    checkCrawlStatus(){
      this.scraperservice.getCrawlStatus().subscribe(result => {
         console.log(result)
         this.crawlStatus = result;
         this.openDialog()
       });

    }

    openDialog(): void {
      const dialogRef = this.dialog.open(CrawlStatusComponent, {
        width: '400px',
        data: this.crawlStatus,
      });
  
      dialogRef.afterClosed().subscribe(result => {
        console.log('The dialog was closed');
        //this.animal = result;
      });

}

}





