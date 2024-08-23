

namespace Watt_2_Watch
{
    public class User
    {
        public User() { }


        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Dictionary<string, int> PreferredGenres { get; private set; } = new Dictionary<string, int>();
        public List<Database.DatabaseRecord> WatchHistory { get; private set; } = new List<Database.DatabaseRecord>();


        public void AddGenrePreference(string genre, int weight)
        {
            if (PreferredGenres.ContainsKey(genre))
                PreferredGenres[genre] += weight;
            else
                PreferredGenres[genre] = weight;
        }

        public void UpdateGenrePreferences(Dictionary<string, int> newPreferences)
        {
            PreferredGenres = newPreferences;
        }
    }
}
