using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watt_2_Watch;

namespace Project284
{
    internal class Menus
    {
        #region Shared UserActions instance
        // Shared UserActions instance from Program.cs
        static UserActions userActions = Program.userActions;
        #endregion

        #region Delegate definition
        // Delegate definition for handling menu navigation
        public delegate MenuHandlerDelegate MenuHandlerDelegate();
        #endregion

        #region User menu selection validation
        /// <summary>
        /// Prompts the user to enter a menu selection and returns the selection if it is within the specified range.
        /// Continuously prompts the user until a valid selection is entered.
        /// </summary>
        /// <param name="min">The minimum valid choice.</param>
        /// <param name="max">The maximum valid choice.</param>
        /// <returns>The user's valid selection as an integer.</returns>
        public static int GetValidInput(int min, int max)
        {
            int choice = 0;
            bool error = false;

            while (true)
            {
                string Value = Console.ReadLine();

                try
                {
                    choice = Convert.ToInt32(Value);
                    if (choice < min || choice > max)
                        error = true;
                    break;
                }
                catch
                {
                    error = true;
                }

                if (!error) break;
                else Console.WriteLine($"Please enter a valid option ({min}-{max}):");
            }
            return choice;
        }
        #endregion

        #region Entrance Menu
        /// <summary>
        /// Displays the entrance menu where users can log in, sign up, or exit.
        /// </summary>
        /// <returns>Next menu delegate to execute.</returns>
        public static MenuHandlerDelegate EntranceMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Watt 2 Watch!");
            Console.WriteLine("==========================\n");
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Sign Up");
            Console.WriteLine("3. Exit");

            Console.Write("Select a menu option: ");
            int choice = GetValidInput(1, 3);

            switch (choice)
            {
                case 1:
                    return userActions.Login;
                case 2:
                    return userActions.SignUp;
                case 3:
                    Console.WriteLine("Goodbye!");
                    Console.ReadKey();
                    return null;
                default:
                    return EntranceMenu;
            }
        }
        #endregion

        #region Main Menu
        /// <summary>
        /// Displays the main menu where users can choose to get recommendations, view profile, search for shows, rate shows, or log out.
        /// </summary>
        /// <returns>Next menu delegate to execute.</returns>
        public static MenuHandlerDelegate MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Main Menu:");
            Console.WriteLine("=============");
            Console.WriteLine("1. Recommend Me Something!");
            Console.WriteLine("2. View Profile");
            Console.WriteLine("3. Search for a Show");
            Console.WriteLine("4. Rate a Show");
            Console.WriteLine("5. Logout");

            Console.Write("Select a menu option: ");
            int choice = GetValidInput(1, 5);

            switch (choice)
            {
                case 1:
                    return userActions.RecommendShows;
                case 2:
                    return ProfileMenu;
                case 3:
                    return userActions.SearchShows;
                case 4:
                    return userActions.RateShow;
                case 5:
                    userActions.LoggedInUser = null; // Logout the user
                    return EntranceMenu;
                default:
                    return null;
            }
        }
        #endregion

        #region Profile Menu
        /// <summary>
        /// Displays the profile menu where users can view details, watch history, change genre preferences, or return to the main menu.
        /// </summary>
        /// <returns>Next menu delegate to execute.</returns>
        public static MenuHandlerDelegate ProfileMenu()
        {
            Console.Clear();
            Console.WriteLine("Profile Menu:");
            Console.WriteLine("===============");
            Console.WriteLine("1. Display Details");
            Console.WriteLine("2. View Watch History");
            Console.WriteLine("3. Change Genre Preferences");
            Console.WriteLine("4. Back to Main Menu");

            Console.Write("Select a menu option: ");
            int choice = GetValidInput(1, 4);

            switch (choice)
            {
                case 1:
                    return userActions.DisplayDetails;
                case 2:
                    return userActions.ViewWatchHistory;
                case 3:
                    return userActions.ChangeGenrePreferences;
                case 4:
                    return MainMenu;
                default:
                    return null;
            }
        }
        #endregion
    }
}
