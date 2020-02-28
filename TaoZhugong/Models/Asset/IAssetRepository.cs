using TaoZhugong.Models.DbEntities;

namespace TaoZhugong.Models
{
    public interface IAssetRepository
    {
        /// <summary>
        /// 建立完產品後會新增該產品的資產資料供日後計算
        /// </summary>
        /// <param name="product"></param>
        void AddNewAsset(Product product);
    }
}