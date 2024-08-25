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
        #endregion

        #region Public delegate functions
        /// <summary>
        /// Handles the user sign-up process, including validating credentials, selecting favourite genres, and creating a new user account.
        /// </summary>
        /// <returns>A delegate that points to the main menu handler after a successful sign-up.</returns>
        public Menus.MenuHandlerDelegate SignUp()
        {
            Console.Clear();
            Console.WriteLine("SIGN UP");
            Console.WriteLine("==========");

            // Validates user credentials
            string username = SignupGetValidUsername();
            string password = SignupGetValidPassword();
            string email = SignupGetValidEmail();

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email
            };

            // Retrieves all possible genres from the database
            List<string> validGenres = db.GetValidGenres();

            string[] genres;
            List<string> invalidGenres;

            do
            {
                Console.WriteLine("Enter your favourite genres (e.g., Action, Comedy, Drama):");
                string inputGenres = Console.ReadLine();

                // Splits the users inputs at a comma and removes excess spacing
                genres = inputGenres.Split(',').Select(g => g.Trim()).ToArray();

                invalidGenres = new List<string>();

                foreach (string genre in genres)
                {
                    // Checks if the user input genres match genres in the database (case-insensitive)
                    bool isValid = validGenres.Contains(genre, StringComparer.OrdinalIgnoreCase);

                    if (!isValid)
                    {
                        invalidGenres.Add(genre);
                    }
                }

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

        /// <summary>
        /// Handles the user login process and validating credentials.
        /// </summary>
        /// <returns>A delegate that points to the main menu handler after a successful login.</returns>
        public Menus.MenuHandlerDelegate Login()
        {
            Console.Clear();
            Console.WriteLine("LOGIN");
            Console.WriteLine("========");

            // Validates user credentials
            string username = LoginGetValidUsername();
            string password = LoginGetValidPassword();

            foreach (var user in Users)
            {
                if (user.Username == username && user.Password == password)
                {
                    LoggedInUser = user;
                    Console.WriteLine($"Login successful! Welcome back {LoggedInUser.Username}. Press any key to continue...");
                    Console.ReadKey();
                    return Menus.MainMenu;
                }
            }

            Console.WriteLine("Login failed. Try again.");
            Console.ReadKey();
            return Menus.EntranceMenu;
        }

        /// <summary>
        /// Allows user to search for a show in a database based on criteria
        /// </summary>
        /// <returns>A delegate that points to the main menu handler after a successful login</returns>

        #region Enums for SearchShows
        enum CriteriaMenu
        {
            Genre = 1,
            Title,
            Duration,
            Air_Dates,
            Type
        }
        #endregion

        public Menus.MenuHandlerDelegate SearchShows(List<DatabaseRecord> shows = null)
        {
            /*Console.Clear();
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
            Console.ReadLine();*/

            int menuCount = 1, minDuration, maxDuration, startYear, endYear;
            string genresInput, title, type;
            bool looping = true;

            Console.Clear();
            Console.WriteLine("What criteria do you want to base your search on:");
            foreach (string option in Enum.GetNames(typeof(CriteriaMenu)))
            {
                Console.WriteLine("{0}. {1}", menuCount, option.Replace("_", " "));
                menuCount++;
            }

            int critOption = Menus.GetValidInput(1, 5);

            //Initializes the results list with either a provided list or an empty one if not provided a list
            List<DatabaseRecord> results = shows ?? new List<DatabaseRecord>();

            switch ((CriteriaMenu)critOption)
            {
                case CriteriaMenu.Genre:

                    while (looping)
                    {
                        Console.Clear();

                        try
                        {
                            Console.WriteLine("Enter genres, separated by a comma:");
                            genresInput = Console.ReadLine();
                            List<string> genres = new List<string>(genresInput.Split(", "));

                            if (genresInput.Trim() == "" || genresInput == null)
                            {
                                throw new ArgumentException("Error: Please dont leave this criteria empty.");
                            }
                            else if (shows == null)
                            {
                                results = db.FilterByGenre(genres);
                                looping = false;
                            }
                            else
                            {
                                results = db.FilterByGenre(shows, genres);
                                looping = false;
                            }

                        }
                        catch (ArgumentException NullOrEmpt)
                        {
                            Console.WriteLine("{0}\nPress any key to retry...", NullOrEmpt.Message);
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;

                case CriteriaMenu.Title:

                    while (looping)
                    {
                        Console.Clear();

                        try
                        {
                            Console.WriteLine("Enter title:");
                            title = Console.ReadLine();

                            if (title.Trim() == "")
                            {
                                throw new ArgumentException("Error: Please don't leave this criteria empty.");
                            }
                            else if (shows == null)
                            {
                                results = db.FilterByTitle(title);
                                looping = false;
                            }
                            else
                            {
                                results = db.FilterByTitle(shows, title);
                                looping = false;
                            }

                        }
                        catch (ArgumentException NullOrEmpt)
                        {
                            Console.WriteLine("{0} \nPress any key to retry...", NullOrEmpt.Message);
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;

                case CriteriaMenu.Duration:
                    while (looping)
                    {
                        Console.Clear();

                        try
                        {
                            Console.WriteLine("Enter minimum runtime in minutes:");
                            minDuration = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter maximum runtime in minutes:");
                            maxDuration = int.Parse(Console.ReadLine());

                            if (maxDuration < minDuration)
                            {
                                throw new ArgumentException("Error: maximum duration cannot be lower than minimum duration.");
                            }
                            else if (shows == null)
                            {
                                results = db.FilterByDuration(minDuration, maxDuration);
                                looping = false;
                            }
                            else
                            {
                                results = db.FilterByDuration(shows, minDuration, maxDuration);
                                looping = false;
                            }

                        }
                        catch (ArgumentException lessThan)
                        {
                            Console.WriteLine("{0} \nPress any key to retry...", lessThan.Message);
                            Console.ReadKey();
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: Please enter whole numbers for the durations. \nPress any key to retry...");
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;

                case CriteriaMenu.Air_Dates:

                    while (looping)
                    {
                        Console.Clear();

                        try
                        {
                            Console.WriteLine("Between which two years would you like to search?");
                            Console.WriteLine("Enter start year:");
                            startYear = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter end year:");
                            endYear = int.Parse(Console.ReadLine());

                            if (endYear < startYear)
                            {
                                throw new ArgumentException("Error: end year cannot be lower than start year.");
                            }
                            else if (shows == null)
                            {
                                results = db.FilterByYearRange(startYear, endYear);
                                looping = false;
                            }
                            else
                            {
                                results = db.FilterByYearRange(shows, startYear, endYear);
                                looping = false;
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: Please enter whole numbers for the years\nPress any key to retry...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException lessThan)
                        {
                            Console.WriteLine("{0}\nPress any key to retry...", lessThan.Message);
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;

                case CriteriaMenu.Type:
                    while (looping)
                    {
                        Console.Clear();

                        try
                        {
                            Console.WriteLine("Enter type (Movie, TvSeries, Short, etc):");
                            type = Console.ReadLine();

                            if (type.Trim() == "" || type == null)
                            {
                                throw new ArgumentException("Error: Please dont leave this criteria empty.");
                            }
                            else if (shows == null)
                            {
                                results = db.FilterByType(type);
                                looping = false;
                            }
                            else
                            {
                                results = db.FilterByType(shows, type);
                                looping = false;
                            }
                        }
                        catch (ArgumentException NullOrEmpt)
                        {
                            Console.WriteLine("{0}\nPress any key to retry...", NullOrEmpt.Message);
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;
            }
            return Menus.MainMenu;
        }

        #region Methods For SearchShows
        private void Random50(List<DatabaseRecord> results)
        {
            Random random = new Random();
            var randomShows = results.OrderBy(x => random.Next()).Take(50).ToList();

            Thread srcThr = new Thread(() => DisplayResults(randomShows));
            Stopwatch srcSW = new Stopwatch();
            srcSW.Start();
            srcThr.Start();

            do
            {
                Console.Clear();
                Console.WriteLine("System is processing your request...{0} seconds", Math.Round(srcSW.Elapsed.TotalSeconds));
                Thread.Sleep(1000);
            }
            while (srcThr.IsAlive);

            srcThr.Join();
            srcSW.Stop();

            if (randomShows.Count == 0)
            {
                Console.WriteLine("No shows found matching the criteria.\nPress any key to return...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nWould you like to filter through the provided list for a more advanced search? Y/N");
                string response = Console.ReadLine().ToUpper();

                if (response == "Y")
                {
                    SearchShows(randomShows);
                }
            }
        }

        private void DisplayResults(List<DatabaseRecord> randomShows)
        {
            Thread.Sleep(3000);

            Console.WriteLine("\nShows that meet the User's search criteria:\n===========================================\n");
            foreach (var show in randomShows)
            {
                Console.WriteLine($"Title: {show.PrimaryTitle}, Type: {show.TitleType}, Year: {show.StartYear}, Duration: {show.RuntimeMinutes} minutes, Genres: {string.Join(", ", show.Genres)}");
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public Menus.MenuHandlerDelegate DisplaySearchResults(List<DatabaseRecord> results)
        {
            if (results.Count > 0)
            {
                Console.WriteLine("SEARCH RESULTS");
                Console.WriteLine("================");

                foreach (var show in results)
                {
                    Console.WriteLine($"{show.TitleType} {show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
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

        #region Recommend method has isssues...Not
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
