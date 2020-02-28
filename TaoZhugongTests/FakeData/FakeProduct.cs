using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugongTests.FakeData
{
    public static class FakeProduct
    {
        public static List<Product> ProductList => new List<Product>()
        {
            StockProduct1,
            StockProduct2,
        };


        public static Product StockProduct1 => new Product()
        {
            ProductSeq = 1,
            ProductName = "StockProduct1",
            ProductValue = "1001",
            Type = "Stock",
        };

        public static Product StockProduct2 => new Product()
        {
            ProductSeq = 2,
            ProductName = "StockProduct2",
            ProductValue = "1002",
            Type = "Stock",
        };
    }
}
