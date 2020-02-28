using System.Linq;
using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProductList();
    }
}