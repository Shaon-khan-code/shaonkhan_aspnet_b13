using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.DTOs
{
    public class PagedProductListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageName { get; set; }
    }
}
