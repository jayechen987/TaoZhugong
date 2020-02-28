using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models.Tests
{
    [TestClass]
    public class ProductRepositoryTests
    {
        ITaoZhugongDatabaseConnection dbConnection ;
        private IProductRepository productRepository;
        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            productRepository = new ProductRepository(dbConnection);
        }

        #region GetProcutList

        [TestMethod()]
        public void GetProductList_NoData()
        {
            //設定環境
            dbConnection.QueryableProduct.ReturnsForAnyArgs(new List<Product>().AsQueryable());

            var expect = new List<Product>().AsQueryable();
            var actual = productRepository.GetProductList();

            expect.ToExpectedObject().ShouldMatch(actual);

        }

        [TestMethod()]
        public void GetProductList_HaveData()
        {
            var returnList = new List<Product>() {new Product() {ProductSeq = 1}};
            dbConnection.QueryableProduct.ReturnsForAnyArgs(returnList.AsQueryable());

            var expect = returnList.AsQueryable();
            var actual = productRepository.GetProductList();

            expect.ToExpectedObject().ShouldMatch(actual);
        }
        #endregion GetProcutList

    }
}