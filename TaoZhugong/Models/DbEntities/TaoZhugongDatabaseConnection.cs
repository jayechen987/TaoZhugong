using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaoZhugong.Models.DbEntities
{
    public interface ITaoZhugongDatabaseConnection
    {
        IQueryable<Asset> QueryableAsset { get; }
        IQueryable<Bookkeeping> QueryableBookkeeping { get; }
        IQueryable<Dividends> QueryableDividends { get; }
        IQueryable<Product> QueryableProduct { get; }
        IQueryable<TransactionRecord> QueryableTransactionRecord { get; }
        void Modified<T>(T model, EntityState entityState) where T : class;
        int SaveChanges();
    }

    public class TaoZhugongDatabaseConnection : ITaoZhugongDatabaseConnection
    {
        TaoZhugongEntities db = new TaoZhugongEntities();

        public IQueryable<Asset> QueryableAsset => db.Asset;
        public IQueryable<Bookkeeping> QueryableBookkeeping => db.Bookkeeping;
        public IQueryable<Dividends> QueryableDividends => db.Dividends;
        public IQueryable<Product> QueryableProduct => db.Product;
        public IQueryable<TransactionRecord> QueryableTransactionRecord => db.TransactionRecord;

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