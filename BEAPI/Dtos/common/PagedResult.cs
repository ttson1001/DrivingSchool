﻿namespace BEAPI.Dtos.Common
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
