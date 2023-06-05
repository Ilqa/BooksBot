export interface PaginatedBooksResult {
    currentPage: number;
    totalPages: number;
    totalCount: number;
    pageSize: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
    booksWithPrices: BookWithPriceList[];
}

export interface BookWithPriceList {
    eAN: string;
    title: string;
    sitePrices: WebsitePrice[];
}

export interface WebsitePrice {
    productUrl: string;
    price: number;
    difference: number;
    website: string;
    currency: string;
    lastCrawled: string;
    availability: string;
}