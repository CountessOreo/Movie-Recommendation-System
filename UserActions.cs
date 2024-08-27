using System.Diagnostics;
using System.Text.RegularExpressions;
using Watt_2_Watch;
using static Watt_2_Watch.Database;



namespace Project284
{
    internal class UserActions
    {
        #region Delegates and Events
        public delegate void SignUpSuccessHandler(string message);
        public event SignUpSuccessHandler OnSignUpSuccess;
        #endregion

        #region Constructor
        public UserActions() 
        {      
            Users = new List<User>();
            Users.Add(new User { Username = "admin", Password = "@dmin123" });
            OnSignUpSuccess += message => Console.WriteLine(message);
        }
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
        /// 

        public Menus.MenuHandlerDelegate SignUp()
        {
            Console.Clear();
            Console.WriteLine("SIGN UP");
            Console.WriteLine("==========\n");

            string username = SignupGetValidUsername();
            string password = SignupGetValidPassword();
            string email = SignupGetValidEmail();

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email
            };

            List<string> validGenres = db.GetValidGenres();

            string[] genres;
            List<string> invalidGenres;
            HashSet<string> genreSet;

            do
            {
                Console.WriteLine("Enter your favourite genres (e.g., Action, Comedy, Drama):");
                string inputGenres = Console.ReadLine();
                genres = inputGenres.Split(',').Select(g => g.Trim()).ToArray();

                invalidGenres = new List<string>();
                genreSet = new HashSet<string>();

                foreach (string genre in genres)
                {
       
                    string capitalizedGenre = char.ToUpper(genre[0]) + genre.Substring(1).ToLower();
                    bool isValid = validGenres.Contains(capitalizedGenre);

                    if (!isValid)
                    {
                        invalidGenres.Add(genre);
                    }
                    else if (!genreSet.Add(capitalizedGenre))
                    {
                        Console.WriteLine($"Duplicate genre detected: {capitalizedGenre}");
                    }
                }

                if (invalidGenres.Any())
                {
                    Console.WriteLine($"\nThe following genres are not valid: {string.Join(", ", invalidGenres)}");
                    Console.WriteLine("Here are the available genres:");
                    Console.WriteLine("");
                    Console.WriteLine(string.Join(", ", validGenres));
                }
            } while (invalidGenres.Any() || genreSet.Count < genres.Length);
            var preferredGenres = genreSet.ToDictionary(g => g, g => 1);

            newUser.UpdateGenrePreferences(preferredGenres);

            Users.Add(newUser);
            LoggedInUser = newUser;
            string successMessage = $"\nYou've successfully created an account {LoggedInUser.Username}.";
            OnSignUpSuccess?.Invoke(successMessage);

            Console.WriteLine("Press any key to continue...");
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
            Console.WriteLine("========\n");

            // Validates user credentials
            string username = LoginGetValidUsername();
            string password = LoginGetValidPassword();

            foreach (var user in Users)
            {
                if (user.Username == username && user.Password == password)
                {
                    LoggedInUser = user;
                    Console.WriteLine($"\nLogin successful! Welcome back {LoggedInUser.Username}. Press any key to continue...");
                    Console.ReadKey();
                    return Menus.MainMenu;
                }
            }

            Console.WriteLine("\nLogin failed. Try again.");
            Console.ReadKey();
            return Menus.EntranceMenu;
        }

        /// <summary>
        /// Enum criteria menu 
        /// </summary>
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

        // <summary>
        /// Allows user to search for a show in a database based on criteria
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate SearchShows(List<DatabaseRecord> shows = null)
        {
            // Initialize variables
            int menuCount = 1, minDuration, maxDuration, startYear, endYear;
            string genresInput, title, type;
            bool looping = true;

            Console.Clear();
            Console.WriteLine("SEARCH");
            Console.WriteLine("=========\n");
            Console.WriteLine("What criteria do you want to base your search on:");

            // Display search criteria options
            foreach (string option in Enum.GetNames(typeof(CriteriaMenu)))
            {
                Console.WriteLine($"{menuCount}. {option.Replace("_", " ")}");
                menuCount++;
            }

            // Get valid input for the criteria selection
            int critOption = Menus.GetValidInput(1, 5);

            // Initialize the results list with the provided list or an empty one
            List<DatabaseRecord> results;

            if (shows != null)
            {
                results = shows;
            }
            else
            {
                results = new List<DatabaseRecord>();
            }

            // Handle different search criteria
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
                            List<string> genres = genresInput.Split(", ").Select(g => g.Trim()).ToList();

                            if (string.IsNullOrWhiteSpace(genresInput))
                            {
                                throw new ArgumentException("Error: Please don't leave this criteria empty.");
                            }

                            // Filter by genre
                            results = (shows == null) ? db.FilterByGenre(genres) : db.FilterByGenre(shows, genres);
                            looping = false;
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"{ex.Message}\nPress any key to retry...");
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

                            if (string.IsNullOrWhiteSpace(title))
                            {
                                throw new ArgumentException("Error: Please don't leave this criteria empty.");
                            }

                            // Filter by title
                            results = (shows == null) ? db.FilterByTitle(title) : db.FilterByTitle(shows, title);
                            looping = false;
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"{ex.Message} \nPress any key to retry...");
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
                            // Get and validate duration range
                            Console.WriteLine("Enter minimum runtime in minutes:");
                            minDuration = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter maximum runtime in minutes:");
                            maxDuration = int.Parse(Console.ReadLine());

                            if (maxDuration < minDuration)
                            {
                                throw new ArgumentException("Error: maximum duration cannot be lower than minimum duration.");
                            }

                            // Filter by duration
                            results = (shows == null) ? db.FilterByDuration(minDuration, maxDuration) : db.FilterByDuration(shows, minDuration, maxDuration);
                            looping = false;
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"{ex.Message} \nPress any key to retry...");
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
                            // Get and validate year range
                            Console.WriteLine("Between which two years would you like to search?");
                            Console.WriteLine("Enter start year:");
                            startYear = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter end year:");
                            endYear = int.Parse(Console.ReadLine());

                            if (endYear < startYear)
                            {
                                throw new ArgumentException("Error: end year cannot be lower than start year.");
                            }

                            // Filter by year range
                            results = (shows == null) ? db.FilterByYearRange(startYear, endYear) : db.FilterByYearRange(shows, startYear, endYear);
                            looping = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Error: Please enter whole numbers for the years\nPress any key to retry...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"{ex.Message}\nPress any key to retry...");
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
                            // Get and validate type input
                            Console.WriteLine("Enter type (Movie, TvSeries, Short, etc):");
                            type = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(type))
                            {
                                throw new ArgumentException("Error: Please don't leave this criteria empty.");
                            }

                            // Filter by type
                            results = (shows == null) ? db.FilterByType(type) : db.FilterByType(shows, type);
                            looping = false;
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"{ex.Message}\nPress any key to retry...");
                            Console.ReadKey();
                        }
                    }
                    Random50(results);
                    break;
            }

            return Menus.MainMenu;
        }

        #region Methods For SearchShows

        // Randomly select up to 50 shows from the results and display them
        private void Random50(List<DatabaseRecord> results)
        {
            Random random = new Random();
            var randomShows = results.OrderBy(x => random.Next()).Take(50).ToList();

            Thread srcThr = new Thread(() => DisplayResults(randomShows));
            Stopwatch srcSW = new Stopwatch();
            srcSW.Start();
            srcThr.Start();

            // Display loading message while processing
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

        /// <summary>
        /// Displays the shows that meet the criteria.
        /// </summary>
        /// <param name="randomShows">List of show records that meet the criteria</param>
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
        /// Users can rate a show and add it to their watch history list.
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate RateShow()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("RATE SHOW");
                Console.WriteLine("============\n");
                Console.WriteLine("Enter the title of the show you want to rate:");
                string title = Console.ReadLine();

                // Search for shows matching the entered title in the database
                var similarShows = db.FilterByTitle(title);

                // If no shows are found, notify the user and return to the previous menu
                if (similarShows.Count == 0)
                {
                    Console.WriteLine("\nNo shows found with that title. Press any key to go back...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("\nThe following shows match your criteria:");
                    for (int i = 0; i < similarShows.Count; i++)
                    {
                        var show = similarShows[i];
                        Console.WriteLine($"{i + 1}. {show.PrimaryTitle} ({show.StartYear}) - Genres: {string.Join(", ", show.Genres)}");
                    }
                    Console.WriteLine("\nSelect the show you want to rate:");
                    string input = Console.ReadLine();
                    bool isValidIndex = int.TryParse(input, out int selectedShowIndex);

                    // If the user's input is valid, proceed with rating the selected show
                    if (isValidIndex && selectedShowIndex > 0 && selectedShowIndex <= similarShows.Count)
                    {
                        var selectedShow = similarShows[selectedShowIndex - 1];
                        string rating;

                        // Prompt the user to rate the selected show, allowing only "like" or "dislike"
                        do
                        {
                            Console.WriteLine($"\nYou selected: {selectedShow.PrimaryTitle} ({selectedShow.StartYear}) - Genres: {string.Join(", ", selectedShow.Genres)}");
                            Console.WriteLine("\nDid you like or dislike this show? (Enter 'like' or 'dislike'):");
                            rating = Console.ReadLine().Trim().ToLower();
                        } while (rating != "like" && rating != "dislike");

                        // Determine the adjustment value based on the user's rating
                        int adjustment = (rating == "like") ? 1 : -1;

                        // Adjust the user's preference weight for each genre in the selected show
                        foreach (var genre in selectedShow.Genres)
                        {
                            if (LoggedInUser.PreferredGenres.ContainsKey(genre))
                            {
                                // Update the genre's weight in preferences if it already exists
                                LoggedInUser.PreferredGenres[genre] += adjustment;
                            }
                            else
                            {
                                // Add the genre to preferences if it doesn't already exist
                                // The weight is set to the adjustment value (positive for "like", 0 for "dislike")
                                LoggedInUser.PreferredGenres[genre] = adjustment > 0 ? adjustment : 0;
                            }
                        }

                        // Add the selected show to the user's watch history after rating
                        LoggedInUser.WatchHistory.Add(selectedShow);

                        // Inform the user that the rating has been logged and the show added to the watch history
                        Console.WriteLine("\nYour rating has been logged and the show has been added to your watch history.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        // If the user's selection is invalid, notify them and return to the previous menu
                        Console.WriteLine("\nInvalid selection. Press any key to go back...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the rating process
                Console.WriteLine($"\nAn error occurred while rating the show: {ex.Message}");
            }
            return Menus.MainMenu;
        }


        /// <summary>
        /// Displays user profile credentials.
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate DisplayDetails()
        {
            if (LoggedInUser == null)
            {
                Console.WriteLine("No user is currently logged in.");
                Console.ReadKey();
                return Menus.MainMenu;
            }

            Console.Clear();
            Console.WriteLine("USER DETAILS");
            Console.WriteLine("==============\n");
            Console.WriteLine($"Username: {LoggedInUser.Username}");
            Console.WriteLine($"Email: {LoggedInUser.Email}");

            Console.WriteLine("Preferred Genres:");
            foreach (var genre in LoggedInUser.PreferredGenres)
            {
                Console.WriteLine($"{genre.Key}: {genre.Value}");
            }

            // Prompt the user to press any key to return to the profile menu
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();

            return Menus.ProfileMenu;
        }

        /// <summary>
        /// Displays a users rated shows.
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate ViewWatchHistory()
        {
            Console.Clear();
            Console.WriteLine("WATCH HISTORY:");
            Console.WriteLine("================\n");

            if (LoggedInUser.WatchHistory.Count == 0)
            {
                Console.WriteLine("No shows have been added to your watch history.");
            }
            else
            {
                foreach (var record in LoggedInUser.WatchHistory)
                {
                    Console.WriteLine($"{record.PrimaryTitle} ({record.StartYear}) - Genres: {string.Join(", ", record.Genres)}");
                }
            }

            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
            return Menus.ProfileMenu;
        }

        /// <summary>
        /// Allows a user to change their genre preferences.
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate ChangeGenrePreferences()
        {
            Console.Clear();
            Console.WriteLine("CHANGE GENRE PREFERENCE");
            Console.WriteLine("==========================\n");
            Console.WriteLine("Enter your new favourite genres (e.g., Action, Comedy, Drama):");

            List<string> validGenres = db.GetValidGenres();
            string[] genres;
            List<string> invalidGenres;
            HashSet<string> genreSet;

            do
            {
                // Read the user's input and split it into a list of genres
                string inputGenres = Console.ReadLine();
                genres = inputGenres.Split(',').Select(g => g.Trim()).ToArray();

                invalidGenres = new List<string>();
                genreSet = new HashSet<string>();

                foreach (string genre in genres)
                {
                    // Capitalize the first letter and convert the rest to lowercase
                    string capitalizedGenre = char.ToUpper(genre[0]) + genre.Substring(1).ToLower();
                    bool isValid = validGenres.Contains(capitalizedGenre);

                    if (!isValid)
                    {
                        invalidGenres.Add(genre);
                    }
                    else if (!genreSet.Add(capitalizedGenre))
                    {
                        Console.WriteLine($"Duplicate genre detected: {capitalizedGenre}");
                    }
                }

                if (invalidGenres.Any())
                {
                    Console.WriteLine($"\nThe following genres are not valid: {string.Join(", ", invalidGenres)}");
                    Console.WriteLine("Here are the available genres:");
                    Console.WriteLine("");
                    Console.WriteLine(string.Join(", ", validGenres));
                }
            } while (invalidGenres.Any() || genreSet.Count < genres.Length);

            var newPreferences = genreSet.ToDictionary(g => g, g => 1);
            LoggedInUser.UpdateGenrePreferences(newPreferences);

            Console.WriteLine("Preferences updated! Press any key to go back...");
            Console.ReadKey();

            return Menus.ProfileMenu;
        }

        /// <summary>
        /// Recommends shows based on a users genre preference rankings.
        /// </summary>
        /// <returns>A delegate that points to the main menu.</returns>
        public Menus.MenuHandlerDelegate RecommendShows()
        {
            if (LoggedInUser == null || LoggedInUser.PreferredGenres == null)
            {
                Console.WriteLine("User is not logged in or preferences are not set.");
                Console.ReadKey();
                return Menus.MainMenu;
            }

            var preferredGenres = LoggedInUser.PreferredGenres;
            var orderedGenres = preferredGenres.OrderBy(gr => gr.Value);
            var genreNames = orderedGenres.Select(gr => gr.Key);
            var sortedGenres = genreNames.ToList();

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

        /// <summary>
        /// Displays results of the searched criteria.
        /// </summary>
        /// <param name="sortedGenres">Genres sorted by ranking.</param>
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

        #region Signup validation methods
        /// <summary>
        /// Validates username.
        /// </summary>
        /// <returns>Username</returns>
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
        /// <summary>
        /// Validates user email.
        /// </summary>
        /// <returns>Email</returns>
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

        /// <summary>
        /// Validates user password.
        /// </summary>
        /// <returns>Password</returns>
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
        /// <summary>
        /// Validates username against user dictionary.
        /// </summary>
        /// <returns>Username</returns>
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
        /// <summary>
        /// Validates user password against user dictionary.
        /// </summary>
        /// <returns>Password</returns>
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
