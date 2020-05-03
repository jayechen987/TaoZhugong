using System.Collections.Generic;
using TaoZhugong.Models.DbEntities;
using TaoZhugong.Models.ViewModel;

namespace TaoZhugong.Models.Dividend
{
    public interface IDividendsRepository
    {
        /// <summary>
        /// 新增配股配息紀錄
        /// </summary>
        /// <param name="dividends"></param>
        /// <returns></returns>
        string AddDividends(Dividends dividends);

        /// <summary>
        /// 配股日到後根據資料表對交易紀錄跟資產做加減
        /// </summary>
        /// <param name="dividends"></param>
        void SetDividendsSchedule(Dividends dividends);

        void DividendSchedule();
        List<DividendViewModel> GetDividendList();
    }
}