using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugongTests.FakeData
{
    public class FakeDividends
    {
        public static List<Dividends> DividendList => new List<Dividends>()
        {
            StockProduct1Div1,
            StockProduct2Div1
        };

        public static Dividends StockProduct1Div1 => new Dividends()
        {
            Seq = 1,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            ExRightDate = DateTime.Now.AddDays(30),
            StockDividend = 1,
            CashDividends = 1,
            DividendDate = DateTime.Now.AddDays(40)
        };
        public static Dividends StockProduct1Div2 => new Dividends()
        {
            Seq = 1,
            ProductSeq = FakeProduct.StockProduct1.ProductSeq,
            ExRightDate = DateTime.Now.AddDays(-30),
            StockDividend = 0.5,
            CashDividends = 0.5,
            DividendDate = DateTime.Now.AddDays(-20)
        };
        public static Dividends StockProduct2Div1 => new Dividends()
        {
            Seq = 1,
            ProductSeq = FakeProduct.StockProduct2.ProductSeq,
            ExRightDate = DateTime.Now.AddDays(20),
            StockDividend = 0.5,
            CashDividends = 0,
            DividendDate = DateTime.Now.AddDays(40)
        };
    }
}
