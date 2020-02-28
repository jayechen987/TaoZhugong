using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using FluentAssertions;
using NSubstitute;
using TaoZhugong.Models.CustomerException;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.WebProfile.Enum;
using TaoZhugongTests.FakeData;

namespace TaoZhugong.Models.Tests
{
    [TestClass()]
    public class AssetRepositoryTests
    {
        private ITaoZhugongDatabaseConnection dbConnection;
        private IAssetRepository assetRepository;

        private List<Asset> returnList;
        private List<Product> productList;
        private List<TransactionRecord> holdTXList;

        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            assetRepository = new AssetRepository(dbConnection);

            productList = FakeProduct.ProductList;
            holdTXList = FakeTransactionRecord.transactionRecorList.Where(p => p.SalePrice == null).ToList();

            dbConnection.QueryableProducts.Returns(productList.AsQueryable());
            dbConnection.QueryableTransactionRecords.Returns(FakeTransactionRecord.transactionRecorList.AsQueryable());
        }

        #region AddNewAsset

        [TestMethod()]
        public void AddNewAsset_Success()
        {
            var product = FakeProduct.StockProduct1;
            assetRepository.AddNewAsset(product);

            dbConnection.Received().QueryableProducts.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().QueryableAssets.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.Received().SaveChanges();
        }

        [TestMethod()]
        public void AddNewAsset_ProductIsNull()
        {
            var product = new Product(){ProductSeq = 99};

            Action action = () => { assetRepository.AddNewAsset(product); };
            action.Should().Throw<DataNotFoundException>();

            dbConnection.Received().QueryableProducts.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().QueryableAssets.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().SaveChanges();
        }

        [TestMethod()]
        public void AddNewAsset_AssetIsNotNew()
        {
            var product = FakeProduct.StockProduct1;
            var asset = new Asset(){ProductSeq = product.ProductSeq};
            dbConnection.QueryableAssets.Returns(new List<Asset>() { asset }.AsQueryable());

            assetRepository.AddNewAsset(product);

            dbConnection.Received().QueryableProducts.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.Received().QueryableAssets.Any(p => p.ProductSeq == product.ProductSeq);
            dbConnection.DidNotReceive().Modified(Arg.Is<Asset>(p => p.ProductSeq == product.ProductSeq), EntityState.Added);
            dbConnection.DidNotReceive().SaveChanges();
        }

        #endregion AddNewAsset

        #region GetAssetListByType

        /// <summary>
        /// 取的產品類別無資產
        /// </summary>
        [TestMethod]
        public void GetAssetListByType_Empty()
        {
            returnList = new List<Asset>();
            dbConnection.QueryableAssets.Returns(FakeAsset.AssetList.AsQueryable());

            var expect = returnList;
            var actual = assetRepository.GetAssetListByType("123");

            expect.ToExpectedObject().ShouldMatch(actual);
        }

        /// <summary>
        /// 取股票的資產清單
        /// </summary>
        [TestMethod]
        public void GetAssetListByType_Stock()
        {
            returnList = new List<Asset>() { FakeAsset.StockProduct1 };
            dbConnection.QueryableAssets.Returns(returnList.AsQueryable());

            var expect = new List<Asset>()
            {
                GetExpectAssetView(FakeAsset.StockProduct1, FakeProduct.StockProduct1, holdTXList),
            }.AsQueryable();
            var actual = assetRepository.GetAssetListByType(ProductType.Stock.ToString()).AsQueryable();

            expect.ToExpectedObject().ShouldMatch(actual);
        }

        /// <summary>
        /// 有售出紀錄的資產
        /// </summary>
        [TestMethod]
        public void GetAssetListByType_Stock_WithSoldOut()
        {
            returnList = new List<Asset>() { FakeAsset.StockProduct2 };
            dbConnection.QueryableAssets.Returns(returnList.AsQueryable());

            var expect = new List<Asset>()
            {
                GetExpectAssetView(FakeAsset.StockProduct2, FakeProduct.StockProduct2, holdTXList),
            }.AsQueryable();
            var actual = assetRepository.GetAssetListByType(ProductType.Stock.ToString()).AsQueryable();

            expect.ToExpectedObject().ShouldMatch(actual);
        }



        #endregion GetAssetListByType

        #region Tool

        /// <summary>
        /// 有總價的資產取均數
        /// </summary>
        [TestMethod]
        public void GetAveragePrice_HavePrice()
        {

            var asset = SetAssetCostData(2452, 1000);

            var except = 2.45;
            var actual = assetRepository.GetAveragePrice(asset);
            Assert.AreEqual(except, actual);
        }

        /// <summary>
        /// 無總價的資產取均數(配股)
        /// </summary>
        [TestMethod]
        public void GetAveragePrice_PriceIsZero()
        {

            var asset = SetAssetCostData(0, 1000);

            var except = 0;
            var actual = assetRepository.GetAveragePrice(asset);
            Assert.AreEqual(except, actual);
        }

        /// <summary>
        /// 總數為0不做除法(防呆用)
        /// </summary>
        [TestMethod]
        public void GetAveragePrice_NumIsZero()
        {
            var asset = SetAssetCostData(1000, 0);

            var except = 0;
            var actual = assetRepository.GetAveragePrice(asset);
            Assert.AreEqual(except, actual);
        }

        /// <summary>
        /// 無手續費取損益平衡點
        /// </summary>
        [TestMethod]
        public void GetBreakevenPoint_WithNoFee()
        {

            var trans = new List<TransactionRecord>()
            {
                SetNewTXWithCostData(1000, 0, 1000),
                SetNewTXWithCostData(3000, 0, 1000),
            };
            var expect = 2;
            var actual = assetRepository.GetBreakevenPoint(trans.AsQueryable());
            Assert.AreEqual(expect, actual);

        }

        /// <summary>
        /// 有手續費取損益平衡點
        /// </summary>
        [TestMethod]
        public void GetBreakevenPoint_WithFee()
        {

            var trans = new List<TransactionRecord>()
            {
                SetNewTXWithCostData(1000, 500, 1000),
                SetNewTXWithCostData(2000, 500, 1000),
            };
            var expect = 2;
            var actual = assetRepository.GetBreakevenPoint(trans.AsQueryable());
            Assert.AreEqual(expect, actual);

        }

        /// <summary>
        /// 總價為0取損益平衡點(配股用)
        /// </summary>
        [TestMethod]
        public void GetBreakevenPoint_PriceIsZero()
        {

            var trans = new List<TransactionRecord>()
            {
                SetNewTXWithCostData(0, 250, 1000),
                SetNewTXWithCostData(0, 250, 1000),
            };
            var expect = 0.25;
            var actual = assetRepository.GetBreakevenPoint(trans.AsQueryable());
            Assert.AreEqual(expect, actual);

        }

        /// <summary>
        /// 庫存為0不做計算
        /// </summary>
        [TestMethod]
        public void GetBreakevenPoint_InstockIsZero()
        {

            var trans = new List<TransactionRecord>()
            {
                SetNewTXWithCostData(1000, 0, 0),
                SetNewTXWithCostData(2000, 0, 0),
            };
            var expect = 0;
            var actual = assetRepository.GetBreakevenPoint(trans.AsQueryable());
            Assert.AreEqual(expect, actual);

        }


        #endregion Tool


        private Asset GetExpectAssetView(Asset assetData, Product productData, List<TransactionRecord> holdTxList)
        {
            var productTXList = holdTxList.Where(p => p.ProductSeq == productData.ProductSeq);

            var expectAsset = new Asset()
            {
                Seq = assetData.Seq,
                ProductSeq = assetData.ProductSeq,
                ProductName = productData.ProductName,
                Num = assetData.Num,
                TotalPrice = assetData.TotalPrice,
                AveragePrice = assetRepository.GetAveragePrice(assetData),
                BreakevenPoint = assetRepository.GetBreakevenPoint(productTXList.AsQueryable()),
            };
            return expectAsset;
        }

        private static Asset SetAssetCostData(int totalPrice, int num)
        {
            var asset = new Asset()
            {
                TotalPrice = totalPrice,
                Num = num,
            };
            return asset;
        }

        private static TransactionRecord SetNewTXWithCostData(int totalPrice, int administractionFee, int inStock)
        {
            var tran = new TransactionRecord()
            {
                TotalPrice = totalPrice,
                AdministractionFee = administractionFee,
                InStock = inStock,
            };
            return tran;
        }
    }
}