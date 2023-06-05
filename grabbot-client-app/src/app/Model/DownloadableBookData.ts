import { BookWithPriceList } from "src/app/Model/PaginatedBooksResult";

export interface BookDataModel {
    eAN: string;
    title: string;
    productUrl: string;
    price: number;
    difference: number;
    website: string;
    currency: string;
    lastCrawled: string;
    availability: string;
}

export interface DownloadableBookData {
    bookWithPriceList: BookWithPriceList[];
    bookDataModel: BookDataModel[];
}