using System.Diagnostics;
using System.Text.RegularExpressions;
using Watt_2_Watch;
using static Watt_2_Watch.Database;



namespace Project284
{
    internal class UserActions
    {
        #region Constructor
        public UserActions() {}
        #endregion

        #region Variables
        // Instantiate the database
        public Database MyImdBDatabase = new Database(Properties.Resources.MoviesDatabase);
        #endregion

        #region Properties
        public List<User> Users { get; set; } = new List<User>();
        public User LoggedInUser { get; set; } = null;
        public Database db { get; set; } = new Database(Properties.Resources.MoviesDatabase);

        // We need an instance of the User class
        #endregion

        #region Public delegate functions
        public Menus.MenuHandlerDelegate SignUp()
        {
            Console.Clear();
            Console.WriteLine("Sign Up");
            Console.WriteLine("==========");

            string username = SignupGetValidUsername();
            string password = SignupGetValidPassword();
            string email = SignupGetValidEmail();

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email
            };

            var validGenres = db.GetValidGenres();

            string[] genres;
            List<string> invalidGenres;

            do
            {
                Console.WriteLine("Select your favourite genres (e.g., Action, Comedy, Drama):");
                string inputGenres = Console.ReadLine();
                genres = inputGenres.Split(',').Select(g => g.Trim()).ToArray();

                invalidGenres = genres.Where(g => !validGenres.Contains(g, StringComparer.OrdinalIgnoreCase)).ToList();

                if (invalidGenres.Any())
                {
                    Console.WriteLine($"The following genres are not valid: {string.Join(", ", invalidGenres)}");
                    Console.WriteLine("Here are the available genres:");
                    Console.WriteLine(string.Join(", ", validGenres));
                }
            } while (invalidGenres.Any());

            foreach (string genre in genres)
            {
                newUser.AddGenrePreference(genre, 1);
            }

            Users.Add(newUser);
            LoggedInUser = newUser;

            Console.WriteLine("Account created successfully! Press any key to continue...");
            Console.ReadKey();
            return Menus.MainMenu;
        }
        public Menus.MenuHandlerDelegate Login()
        {
            Console.Clear();
            Console.WriteLine("Login");

            string username = LoginGetValidUsername();
            string password = LoginGetValidPassword();

            foreach (var user in Users)
            {
                if (user.Username == username && user.Password == password)
                {
                    LoggedInUser = user;
                    Console.WriteLine("Login successful! Press any key to continue...");
                    Console.WriteLine($"LoggedInUser set to: {LoggedInUser.Username}");
                    Console.ReadKey();
                    return Menus.MainMenu;
                }
            }

            Console.WriteLine("Login failed. Press any key to try again...");
            Console.ReadKey();
            return Menus.EntranceMenu;
        }

        public Menus.MenuHandlerDelegate SearchShows()
        {
            Console.Clear();
            Console.WriteLine("Search for Shows");
            Console.WriteLine("1. By Genre");
            Console.WriteLine("2. By Title");

            string option = Console.ReadLine();
            List<Database.DatabaseRecord> results = new List<Database.DatabaseRecord>();

            switch (option)
            {
                case "1":
                Console.WriteLine("Enter Genre:");
                string genre = Console.ReadLine();
                results = db.FilterByGenre(new List<string> { genre });
                break;
                case "2":
                Console.WriteLine("Enter Title:");
                string title = Console.ReadLine();
                results = db.FilterByTitle(title);
                break;
                default:
                Console.WriteLine("Invalid option.");
                Console.ReadLine();
                return SearchShows;
            }

            DisplaySearchResults(results);
            Console.ReadLine();
            return Menus.MainMenu;
        }
        public Menus.MenuHandlerDelegate DisplaySearchResults(List<Database.DatabaseRecord> results)
        {
            if (results.Count > 0)
            {
                Console.WriteLine("Search Results:");
                foreach (var show in results)
                {
                    Console.WriteLine($"{show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
                }
            }
            else
            {
                Console.WriteLine("No shows found matching the criteria.");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
            return Menus.MainMenu;
        }
        public Menus.MenuHandlerDelegate RateShow()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Rate a Show\nEnter the title of the show you want to rate:");
                string title = Console.ReadLine();

                var similarShows = db.FilterByTitle(title);
                if (similarShows.Count == 0)
                {
                    Console.WriteLine("No shows found with that title. Press any key to go back...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Select the show you want to rate:");
                    for (int i = 0; i < similarShows.Count; i++)
                    {
                        var show = similarShows[i];
                        Console.WriteLine($"{i + 1}. {show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int selectedShowIndex) && selectedShowIndex > 0 && selectedShowIndex <= similarShows.Count)
                    {
                        var selectedShow = similarShows[selectedShowIndex - 1];
                        int rating;
                        do
                        {
                            Console.WriteLine($"You selected: {selectedShow.PrimaryTitle} ({selectedShow.StartYear}) - Genres: {string.Join(", ", selectedShow.Genres)}\nRate this show (1-5 stars):");
                        } while (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5);

                        Console.WriteLine("Rating recorded. Press any key to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection. Press any key to go back...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
              Console.WriteLine($"An error occurred while rating the show: {ex.Message}");
            }

            return Menus.MainMenu;
        }



        public Menus.MenuHandlerDelegate DisplayDetails()
        {
            Console.Clear();
            Console.WriteLine("User Details");
            Console.WriteLine($"Username: {LoggedInUser.Username}");
            Console.WriteLine($"Email: {LoggedInUser.Email}");
            Console.WriteLine("Preferred Genres:");
            foreach (var genre in LoggedInUser.PreferredGenres)
            {
                Console.WriteLine($"{genre.Key}: {genre.Value}");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
            return Menus.ProfileMenu;
        }
        public Menus.MenuHandlerDelegate ViewWatchHistory()
        {
            Console.Clear();
            Console.WriteLine("Watch History:");
            foreach (var record in LoggedInUser.WatchHistory)
            {
                Console.WriteLine($"{record.PrimaryTitle} ({record.StartYear}) - Genres: {string.Join(", ", record.Genres)}");
            }
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
            return Menus.ProfileMenu;
        }
        public Menus.MenuHandlerDelegate ChangeGenrePreferences()
        {
            Console.Clear();
            Console.WriteLine("Change Genre Preferences");
            Console.WriteLine("Enter your new favourite genres (e.g., Action, Comedy, Drama):");

            string[] genres = Console.ReadLine().Split(',');
            var newPreferences = new Dictionary<string, int>();

            foreach (string genre in genres)
            {
                newPreferences[genre.Trim()] = 1;
            }

            LoggedInUser.UpdateGenrePreferences(newPreferences);
            Console.WriteLine("Preferences updated! Press any key to go back...");
            Console.ReadKey();
            return Menus.ProfileMenu;
        }

        #region Recommend method has isssues
        public Menus.MenuHandlerDelegate RecommendShows()
        {

            //Orders the list in acceding order
            var sortedGenres = LoggedInUser.PreferredGenres.OrderBy(gr => gr.Value).Select(gr => gr.Key).ToList();
            //Reverses order of list to display the most ranked genre first
            sortedGenres.Reverse();

            Thread thr = new Thread(() => PrintResults(sortedGenres));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            thr.Start();

            Console.Clear();
            Thread.Sleep(500);

            do
            {
                Console.Clear();
                Console.WriteLine("System is processing your request...{0} seconds", Math.Round(sw.Elapsed.TotalSeconds));
                Thread.Sleep(1000);
            }
            while (thr.IsAlive);

            thr.Join();
            sw.Stop();

            Console.WriteLine("Press any key to return...");

            Console.ReadKey();
            return Menus.MainMenu;
        }

        #region Recommended Show Thread

        public void PrintResults(List<string> sortedGenres)
        {
            Thread.Sleep(2000);

            Console.Clear();

            Random random = new Random();

            foreach (var genre in sortedGenres)
            {
                List<DatabaseRecord> Shows = db.FilterByGenre(new List<string> { genre });
                //Shuffles the Shows list and takes the first 10
                var randomShows = Shows.OrderBy(x => random.Next()).Take(10).ToList();

                Console.WriteLine($"Shows found for {genre}:\r\n");
                foreach (DatabaseRecord rec in randomShows)
                {
                    Console.WriteLine($"Title: {rec.OriginalTitle}. Genres - {string.Join(", ", rec.Genres)}");
                }
                Console.WriteLine();
            }
        }

        #endregion

        #endregion 

        #endregion


        #region Menu validation methods
        public string GetValidOption()
        {
            Console.WriteLine("1. Sign Up");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            string option = Console.ReadLine();

            while (option != "1" && option != "2" && option != "3")
            {
                Console.WriteLine("Invalid option. Choose an option displayed above.");
                option = Console.ReadLine();
            }

            return option;
        }
        #endregion


        #region Signup validation methods
        public string SignupGetValidUsername()
        {
            string username;
            while (true)
            {
                Console.Write("Enter username: ");
                username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username cannot be empty.");
                }
                else if (Users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Username already exists. Please choose a different one.");
                }
                else
                {
                    break;
                }
            }
            return username;
        }
        public string SignupGetValidEmail()
        {
            string email;
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

            while (true)
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Email cannot be empty.");
                }
                else if (!emailRegex.IsMatch(email))
                {
                    Console.WriteLine("Invalid email format. Please enter a valid email address.");
                }
                else if (Users.Any(u => u.Email == email))
                {
                    Console.WriteLine("Email is already in use. Please use a different one.");
                }
                else
                {
                    break;
                }
            }
            return email;
        }
        public string SignupGetValidPassword()
        {
            string password;
            var passwordRegex = new Regex(@"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]{8,}$");

            while (true)
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Password cannot be empty.");
                }
                else if (password.Length < 8)
                {
                    Console.WriteLine("Password must be at least 8 characters long.");
                }
                else if (!passwordRegex.IsMatch(password))
                {
                    Console.WriteLine("Password must contain at least one special character.");
                }
                else
                {
                    break;
                }
            }
            return password;
        }
        #endregion


        #region Login validation methods
        public string LoginGetValidUsername()
        {
            string username;
            while (true)
            {
                Console.Write("Enter username: ");
                username = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username cannot be empty.");
                }
                else
                {
                    break;
                }
            }
            return username;
        }
        public string LoginGetValidPassword()
        {
            string password;
            var passwordRegex = new Regex(@"^(?=.*[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-])[A-Za-z\d!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]{8,}$");

            while (true)
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Password cannot be empty.");
                }
                else if (password.Length < 8)
                {
                    Console.WriteLine("Password must be at least 8 characters long.");
                }
                else if (!passwordRegex.IsMatch(password))
                {
                    Console.WriteLine("Password must contain at least one special character.");
                }
                else
                {
                    break;
                }
            }
            return password;
        }
        #endregion
    }
}
