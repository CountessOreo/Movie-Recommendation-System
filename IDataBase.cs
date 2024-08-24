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
        /// <summary>
        /// Retrieves a list of shows that aired between the specified start and end years.
        /// </summary>
        /// <param name="startYear">The earliest year in the range.</param>
        /// <param name="endYear">The latest year in the range.</param>
        /// <returns>A list of shows that aired between the specified years.</returns>
        List<DatabaseRecord> FilterByYearRange(int startYear, int endYear);

        /// <summary>
        /// Retrieves a list of shows from the provided list that aired between the specified start and end years.
        /// </summary>
        /// <param name="recordList">The list of show records to filter.</param>
        /// <param name="startYear">The earliest year in the range.</param>
        /// <param name="endYear">The latest year in the range.</param>
        /// <returns>A filtered list of shows that aired between the specified years.</returns>
        List<DatabaseRecord> FilterByYearRange(List<DatabaseRecord> recordList, int startYear, int endYear);

        /// <summary>
        /// Retrieves a list of shows that fully or partially match the specified genres.
        /// </summary>
        /// <param name="genres">A list of genres to filter the shows by.</param>
        /// <returns>A list of shows that match or partially match the specified genres.</returns>
        List<DatabaseRecord> FilterByGenre(List<string> genres);

        /// <summary>
        /// Retrieves a list of shows from the provided list that fully or partially match the specified genres.
        /// </summary>
        /// <param name="recordList">The list of show records to filter.</param>
        /// <param name="genres">A list of genres to filter the shows by.</param>
        /// <returns>A filtered list of shows that match or partially match the specified genres.</returns>
        List<DatabaseRecord> FilterByGenre(List<DatabaseRecord> recordList, List<string> genres);

        /// <summary>
        /// Retrieves a list of shows that fully or partially match the specified title.
        /// </summary>
        /// <param name="title">The title or partial title to filter the shows by.</param>
        /// <returns>A list of shows that match or partially match the specified title.</returns>
        List<DatabaseRecord> FilterByTitle(string title);

        /// <summary>
        /// Retrieves a list of shows from the provided list that fully or partially match the specified title.
        /// </summary>
        /// <param name="recordList">The list of show records to filter.</param>
        /// <param name="title">The title or partial title to filter the shows by.</param>
        /// <returns>A filtered list of shows that match or partially match the specified title.</returns>
        List<DatabaseRecord> FilterByTitle(List<DatabaseRecord> recordList, string title);

        /// <summary>
        /// Retrieves a list of shows with a runtime duration between the specified minimum and maximum values.
        /// </summary>
        /// <param name="minDuration">The minimum runtime duration, in minutes.</param>
        /// <param name="maxDuration">The maximum runtime duration, in minutes.</param>
        /// <returns>A list of shows with a runtime duration within the specified range.</returns>
        List<DatabaseRecord> FilterByDuration(int minDuration, int maxDuration);

        /// <summary>
        /// Retrieves a list of shows from the provided list with a runtime duration between the specified minimum and maximum values.
        /// </summary>
        /// <param name="recordList">The list of show records to filter.</param>
        /// <param name="minDuration">The minimum runtime duration, in minutes.</param>
        /// <param name="maxDuration">The maximum runtime duration, in minutes.</param>
        /// <returns>A filtered list of shows with a runtime duration within the specified range.</returns>
        List<DatabaseRecord> FilterByDuration(List<DatabaseRecord> recordList, int minDuration, int maxDuration);

        /// <summary>
        /// Retrieves a list of shows that match the specified show type.
        /// </summary>
        /// <param name="showType">The type of show to filter by (e.g., series, movie, documentary).</param>
        /// <returns>A list of shows that match the specified show type.</returns>
        List<DatabaseRecord> FilterByType(string showType);

        /// <summary>
        /// Retrieves a list of shows from a provided list that match the specified show type.
        /// </summary>
        /// <param name="recordList">The list of show records to filter.</param>
        /// <param name="showType">The type of show to filter by (e.g., series, movie, documentary).</param>
        /// <returns>A filtered list of shows that match the specified show type.</returns>
        List<DatabaseRecord> FilterByType(List<DatabaseRecord> recordList, string showType);
    }
}
