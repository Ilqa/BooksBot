import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { CrawlStatus } from '../Model/CrawlStatus';

@Component({
  selector: 'app-crawl-status',
  templateUrl: './crawl-status.component.html',
  styleUrls: ['./crawl-status.component.css']
})
export class CrawlStatusComponent {

  constructor(
    public dialogRef: MatDialogRef<CrawlStatusComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CrawlStatus,
  ) {}

  onOkClick(): void {
    this.dialogRef.close();
  }

}


