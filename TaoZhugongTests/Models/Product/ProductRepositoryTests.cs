﻿using ExpectedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentAssertions;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models.Tests
{
    [TestClass]
    public class ProductRepositoryTests
    {
        ITaoZhugongDatabaseConnection dbConnection;
        IAssetRepository assetRepository ;

        private IProductRepository productRepository;
        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            assetRepository = Substitute.For<IAssetRepository>();
            productRepository = new ProductRepository(dbConnection, assetRepository);
        }

        #region GetProcutList

        [TestMethod()]
        public void GetProductList_NoData()
        {
            //設定環境
            dbConnection.QueryableProducts.ReturnsForAnyArgs(new List<Product>().AsQueryable());

            var expect = new List<Product>().AsQueryable();
            var actual = productRepository.GetProductList();

            expect.ToExpectedObject().ShouldMatch(actual);

        }

        [TestMethod()]
        public void GetProductList_HaveData()
        {
            var returnList = new List<Product>() { new Product() { ProductSeq = 1 } };
            dbConnection.QueryableProducts.ReturnsForAnyArgs(returnList.AsQueryable());

            var expect = returnList.AsQueryable();
            var actual = productRepository.GetProductList();

            expect.ToExpectedObject().ShouldMatch(actual);
        }
        #endregion GetProcutList

        #region EditProduct

        [TestMethod()]
        public void EditProduct_AddProduct()
        {
            var addproduct = new Product() { ProductName = "new product", ProductValue = "value", Owner = "owner" };


            var except = "Success";
            var actual = productRepository.EditProduct(addproduct);

            Assert.AreEqual(except,actual);

            dbConnection.DidNotReceive().QueryableProducts.FirstOrDefault(p => p.ProductSeq == addproduct.ProductSeq);
            dbConnection.DidNotReceive().Modified(addproduct, EntityState.Modified);
            dbConnection.Received().Modified(addproduct, EntityState.Added);
            dbConnection.Received(1).SaveChanges();
            assetRepository.Received(1).AddNewAsset(addproduct);
        }

        [TestMethod()]
        public void EditProduct_EditProduct()
        {
            var editproduct = new Product() { ProductSeq = 1, ProductName = "edit product", ProductValue = "value", Owner = "owner" };
            var dbData = new Product() { ProductSeq = 1, ProductName = "old product", ProductValue = "value", Owner = "owner" };
            dbConnection.QueryableProducts.ReturnsForAnyArgs(new List<Product>() { dbData }.AsQueryable());


            var except = "Success";
            var actual = productRepository.EditProduct(editproduct);

            Assert.AreEqual(except, actual);
            dbConnection.Received(1).QueryableProducts.FirstOrDefault(p => p.ProductSeq == editproduct.ProductSeq);
            dbConnection.Received(1).Modified(dbData, EntityState.Modified);
            dbConnection.DidNotReceive().Modified(editproduct, EntityState.Added);
            dbConnection.Received(1).SaveChanges();
            assetRepository.DidNotReceive().AddNewAsset(editproduct);
        }
        [TestMethod()]
        public void EditProduct_OldProductNotFound()
        {
            var editproduct = new Product() { ProductSeq = 1, ProductName = "edit product", ProductValue = "value", Owner = "owner" };

            Action action = () => { productRepository.EditProduct(editproduct); };
            action.Should().Throw<DataNotFoundException>();

            dbConnection.Received(1).QueryableProducts.FirstOrDefault(p => p.ProductSeq == editproduct.ProductSeq);
            dbConnection.DidNotReceive().Modified(editproduct, EntityState.Modified);
            dbConnection.DidNotReceive().Modified(editproduct, EntityState.Added);
            dbConnection.DidNotReceive().SaveChanges();
            assetRepository.DidNotReceive().AddNewAsset(editproduct);

        }
        #endregion EditProduct

    }
}