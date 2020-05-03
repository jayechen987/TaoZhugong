using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaoZhugong.Models.Dividend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using NSubstitute;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.Transaction;
using TaoZhugong.Models.ViewModel;
using TaoZhugongTests.FakeData;

namespace TaoZhugong.Models.Tests
{
    [TestClass()]
    public class DividendsRepositoryTests
    {
        ITaoZhugongDatabaseConnection dbConnection;
        IDividendsRepository dividendsRepository;
        ITransactionRepository transactionRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            dbConnection = Substitute.For<ITaoZhugongDatabaseConnection>();
            transactionRepository = Substitute.For<ITransactionRepository>();
            dividendsRepository = new DividendsRepository(dbConnection, transactionRepository);
        }

        [TestMethod()]
        public void GetDividendList_Empty()
        {
            var returnList = new List<DividendViewModel>();

            var expect = returnList;
            var actual = dividendsRepository.GetDividendList();

            expect.ToExpectedObject().ShouldMatch(actual);
        }
    }
}