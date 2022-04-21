export interface Pagination {
  // needs to be the same as in header json string
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  result: T; // array of memebrs Members[]
  pagination: Pagination;
}
