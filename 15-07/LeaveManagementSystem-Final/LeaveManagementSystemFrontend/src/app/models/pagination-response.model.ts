export interface PaginationResponse<T> {
  data: {
    $values: T[];
  };
  pagination: {
    totalRecords: number;
    page: number;
    pageSize: number;
    totalPages: number;
  };
}
