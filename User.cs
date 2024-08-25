

namespace Watt_2_Watch
{
    public class User
    {
        #region Constructor
        public User() 
        {
            PreferredGenres = new Dictionary<string, int>();
            WatchHistory = new List<Database.DatabaseRecord>();
        }
        #endregion

        #region Properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Dictionary<string, int> PreferredGenres { get; set; } = new Dictionary<string, int>();
        public List<Database.DatabaseRecord> WatchHistory { get; private set; } = new List<Database.DatabaseRecord>();
        #endregion

        #region User genre preference methods
        /// <summary>
        /// Adjusts the rank of a user's preferred genre by modifying its weight.
        /// </summary>
        /// <param name="genre">The genre whose preference weight will be changed.</param>
        /// <param name="weight">The amount to change the genre's weight by. Positive values increase the preference, while negative values decrease it.</param>
        public void AddGenrePreference(string genre, int weight)
        {
            if (PreferredGenres.ContainsKey(genre))
                PreferredGenres[genre] += weight;
            else
                PreferredGenres[genre] = weight;
        }

        /// <summary>
        /// Replaces the current genre preferences with a new set of preferences.
        /// </summary>
        /// <param name="newPreferences">A dictionary containing genres and their respective weights to update the user's preferences.</param>
        public void UpdateGenrePreferences(Dictionary<string, int> newPreferences)
        {
            PreferredGenres = newPreferences;
        }
        #endregion

        #region User Details Methods
        public void DisplayDetails()
        {
            Console.WriteLine($"Username: {Username}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine("Preferred Genres:");
            if (PreferredGenres.Count > 0)
            {
                foreach (var genre in PreferredGenres)
                {
                    Console.WriteLine($"{genre.Key}: {genre.Value}");
                }
            }
            else
            {
                Console.WriteLine("No preferred genres set.");
            }
        }
        #endregion
    }
}
