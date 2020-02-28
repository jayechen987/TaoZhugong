using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;
using TaoZhugongTests.FakeData;

namespace TaoZhugong.Models.Tests
{
    [TestClass()]
    public class AssetRepositoryTests
    {
        private ITaoZhugongDatabaseConnection dbConnection;
        private IAssetRepository assetRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            assetRepository = new AssetRepository(dbConnection);
            dbConnection.QueryableProduct.Returns(FakeProduct.ProductList.AsQueryable());
        }

        #region AddNewAsset

        [TestMethod()]
        public void AddNewAsset_Success()
        {
            var product = FakeProduct.StockProduct1;
            assetRepository.AddNewAsset(product);

            dbConnection.Received().QueryableProduct.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().QueryableAsset.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.Received().SaveChanges();
        }

        [TestMethod()]
        public void AddNewAsset_ProductIsNull()
        {
            var product = new Product(){ProductSeq = 99};

            Action action = () => { assetRepository.AddNewAsset(product); };
            action.Should().Throw<DataNotFoundException>();

            dbConnection.Received().QueryableProduct.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().QueryableAsset.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().SaveChanges();
        }

        [TestMethod()]
        public void AddNewAsset_AssetIsNotNew()
        {
            var product = FakeProduct.StockProduct1;
            var asset = new Asset(){ProductSeq = product.ProductSeq};
            dbConnection.QueryableAsset.Returns(new List<Asset>() { asset }.AsQueryable());

            assetRepository.AddNewAsset(product);

            dbConnection.Received().QueryableProduct.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().QueryableAsset.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().SaveChanges();
        }

        #endregion AddNewAsset

    }
}