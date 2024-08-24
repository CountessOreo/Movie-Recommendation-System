using Project284;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Watt_2_Watch
{
    public class Database : IDataBase
    {
        #region Constructor
        /// <summary>
        /// Turns the Movies Database text file into accessible records.
        /// </summary>
        /// <param name="DatabaseFile"></param>
        public Database(string DatabaseFile)
        {
            string[] lines = DatabaseFile.Split('\n');
            bool firstLine = true;

            foreach (string line in lines)
            {
                if (line == null || line == "")
                    continue;

                if (!firstLine)
                {
                    string[] fields = line.Split('\t');
                    bool isAdult = false;

                    // Checks if fields exist and sets unknown values to zero
                    if (fields.Length > 4 && fields[4] == "1")
                        isAdult = true;
                    if (fields.Length > 5 && fields[5] == "\\N")
                        fields[5] = "0";
                    if (fields.Length > 6 && fields[6] == "\\N")
                        fields[6] = "0";
                    if (fields.Length > 7 && fields[7] == "\\N")
                        fields[7] = "0";


                    try
                    {
                        // Try converting the fields into integers
                        int startYear = Convert.ToInt32(fields[5]);
                        int endYear = Convert.ToInt32(fields[6]);
                        int runtimeMinutes = Convert.ToInt32(fields[7]);

                        DatabaseRecord record = new DatabaseRecord()
                        {
                            ShowId = fields[0],
                            TitleType = fields[1],
                            PrimaryTitle = fields[2],
                            OriginalTitle = fields[3],
                            IsAdult = isAdult,
                            StartYear = startYear,
                            EndYear = endYear,
                            RuntimeMinutes = runtimeMinutes,
                            Genres = fields[8].Split(',').ToList(),
                        };

                        Records.Add(record);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                    firstLine = false;
            }
        }
        #endregion

        #region Classes and records
        /// <summary>
        /// Database record definition.
        /// </summary>
        public record DatabaseRecord
        {
            public string ShowId { get; init; }
            public string TitleType { get; init; }
            public string PrimaryTitle { get; init; }
            public string OriginalTitle { get; init; }
            public bool IsAdult { get; init; }
            public int StartYear { get; init; }
            public int EndYear { get; init; }
            public int RuntimeMinutes { get; init; }
            public List<string> Genres { get; init; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Database records.
        /// </summary>
        public List<DatabaseRecord> Records { get; private set; } = new List<DatabaseRecord>();
        #endregion

        #region Private method
        /// <summary>
        /// Filters the collection of records to include only those with specified title types.
        /// </summary>
        /// <returns>An IEnumerable of DatabaseRecord where each record's TitleType is one of the following: "tvSeries", "movie", "short", "tvMiniSeries", or "tvSpecial".</returns>
        private IEnumerable<DatabaseRecord> FilterByType(IEnumerable<DatabaseRecord> records)
        {
            var filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in records)
            {

                if (rec.TitleType == "tvSeries" || rec.TitleType == "movie" || rec.TitleType == "short" || rec.TitleType == "tvMiniSeries" || rec.TitleType == "tvSpecial")
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }
        #endregion

        #region Interface Methods
        public List<DatabaseRecord> FilterByYearRange(int startYear, int endYear)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(Records);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.StartYear >= startYear && rec.StartYear <= endYear)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByYearRange(List<DatabaseRecord> recordList, int startYear, int endYear)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(recordList);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.StartYear >= startYear && rec.StartYear <= endYear)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByGenre(List<string> genres)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(Records);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                foreach (var genre in rec.Genres)
                {
                    if (genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
                    {
                        filteredRecords.Add(rec);
                        break;
                    }
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByGenre(List<DatabaseRecord> recordList, List<string> genres)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(recordList);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                foreach (var genre in rec.Genres)
                {
                    if (genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
                    {
                        filteredRecords.Add(rec);
                        break;
                    }
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByTitle(string title)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(Records);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByTitle(List<DatabaseRecord> recordList, string title)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(recordList);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.PrimaryTitle.Contains(title, StringComparison.OrdinalIgnoreCase) || rec.OriginalTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByDuration(int minDuration, int maxDuration)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(Records);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByDuration(List<DatabaseRecord> recordList, int minDuration, int maxDuration)
        {
            IEnumerable<DatabaseRecord> filteredByType = FilterByType(recordList);

            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in filteredByType)
            {
                if (rec.RuntimeMinutes >= minDuration && rec.RuntimeMinutes <= maxDuration)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByType(string showType)
        {
            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in Records)
            {
                if (rec.TitleType == showType)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }

        public List<DatabaseRecord> FilterByType(List<DatabaseRecord> recordList, string showType)
        {
            List<DatabaseRecord> filteredRecords = new List<DatabaseRecord>();

            foreach (var rec in recordList)
            {
                if (rec.TitleType == showType)
                {
                    filteredRecords.Add(rec);
                }
            }
            return filteredRecords;
        }
        #endregion

        #region Validate genres
        /// <summary>
        /// Retrieves a list of unique genres from the database records, removes any spaces, and validates them against the database.
        /// </summary>
        /// <returns>A list of valid, distinct genres.</returns>
        public List<string> GetValidGenres()
        {
            List<string> validGenres = new List<string>();

            foreach (var record in Records)
            {
                foreach (string genre in record.Genres)
                {
                    // Removes spaces
                    string trimmedGenre = genre.Trim();

                    if (trimmedGenre.Length > 0 && !validGenres.Contains(trimmedGenre))
                    {
                        validGenres.Add(trimmedGenre);
                    }
                }
            }
            return validGenres;
        }
        #endregion
    }
}
