using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Watt_2_Watch.Database;

namespace Project284
{
    internal interface IDataBase
    {
        List<DatabaseRecord> FilterByYearRange(int startYear, int endYear);
        List<DatabaseRecord> FilterByYearRange(List<DatabaseRecord> recordList, int startYear, int endYear);
        List<DatabaseRecord> FilterByGenre(List<string> genres);
        List<DatabaseRecord> FilterByGenre(List<DatabaseRecord> recordList, List<string> genres);
        List<DatabaseRecord> FilterByTitle(string title);
        List<DatabaseRecord> FilterByTitle(List<DatabaseRecord> recordList, string title);
        List<DatabaseRecord> FilterByDuration(int minDuration, int maxDuration);
        List<DatabaseRecord> FilterByDuration(List<DatabaseRecord> recordList, int minDuration, int maxDuration);
        List<DatabaseRecord> FilterByType(string showType);
        List<DatabaseRecord> FilterByType(List<DatabaseRecord> recordList, string showType);
    }
}
