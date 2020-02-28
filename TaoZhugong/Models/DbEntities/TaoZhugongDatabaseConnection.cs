using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    public interface ITaoZhugongDatabaseConnection
    {
        IQueryable<Asset> QueryableAssets { get; }
        IQueryable<Bookkeeping> QueryableBookkeepings { get; }
        IQueryable<Dividends> QueryableDividends { get; }
        IQueryable<Product> QueryableProducts { get; }
        IQueryable<TransactionRecord> QueryableTransactionRecords { get; }
        void Modified<T>(T model, EntityState entityState) where T : class;
        int SaveChanges();
    }

    public class TaoZhugongDatabaseConnection : ITaoZhugongDatabaseConnection
    {
        TaoZhugongEntities db = new TaoZhugongEntities();

        public IQueryable<Asset> QueryableAssets => db.Asset;
        public IQueryable<Bookkeeping> QueryableBookkeepings => db.Bookkeeping;
        public IQueryable<Dividends> QueryableDividends => db.Dividends;
        public IQueryable<Product> QueryableProducts => db.Product;
        public IQueryable<TransactionRecord> QueryableTransactionRecords => db.TransactionRecord;

        public void Modified<T>(T model, EntityState entityState) where T : class
        {
            db.Entry(model).State = entityState;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}