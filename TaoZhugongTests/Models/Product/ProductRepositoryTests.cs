using ExpectedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentAssertions;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models.Tests
{
    [TestClass]
    public class ProductRepositoryTests
    {
        ITaoZhugongDatabaseConnection dbConnection;
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
            var returnList = new List<Product>() { new Product() { ProductSeq = 1 } };
            dbConnection.QueryableProduct.ReturnsForAnyArgs(returnList.AsQueryable());

            var expect = returnList.AsQueryable();
            var actual = productRepository.GetProductList();

            expect.ToExpectedObject().ShouldMatch(actual);
        }
        #endregion GetProcutList

        [Ignore]
        [TestMethod()]
        public void EditProduct_Exception()
        {
            var prodcut = new Product();
            //設定存檔時丟Exception
            dbConnection.SaveChanges().Throws(new Exception());

            Action action = () => { productRepository.EditProduct(prodcut); };
            action.Should().Throw<Exception>();

        }

        [TestMethod()]
        public void EditProduct_AddProduct()
        {
            var addproduct = new Product() { ProductName = "new product", ProductValue = "value", Owner = "owner" };


            var except = "Success";
            var actual = productRepository.EditProduct(addproduct);

            Assert.AreEqual(except,actual);

            dbConnection.Received(1).Modified(addproduct,EntityState.Added);
            dbConnection.Received(1).SaveChanges();
        }

        

    }
}