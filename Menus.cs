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
        public delegate MenuHandlerDelegate MenuHandlerDelegate();

        public static int GetValidInput(int min, int max)
        {
            int choice = 0;
            bool error;

            while (true)
            {
                string Value = Console.ReadLine();

                try
                {
                    choice = Convert.ToInt32(Value);
                    if ((choice < min) || (choice > max))
                        error = true;
                    break;
                }
                catch (Exception)
                {
                    error = true;
                }

                if (!error)
                    break;
                else
                    Console.WriteLine($"Please enter a valid option ({min}-{max}):");
            }
            return choice;
        }

        public static MenuHandlerDelegate EntranceMenu()
        {
            UserActions userActions = new UserActions();

            Console.Clear();
            Console.WriteLine("Welcome to Watt 2 Watch!");
            Console.WriteLine("==========================\n");
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Sign Up");
            Console.WriteLine("3. Exit");

            Console.Write("Select a menu option: ");
            int choice = Menus.GetValidInput(1, 3);

            switch (choice)
            {
                case 1:
                return userActions.Login;
                case 2:
                return userActions.SignUp;
                case 3:
                {
                    Console.WriteLine("Goodbye!");
                    Console.ReadKey();
                    return null;
                };
                default:
                return EntranceMenu;
            }
        }

        public static MenuHandlerDelegate MainMenu()
        {
            UserActions userActions = new UserActions();
            Console.Clear();
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Recommend Me Something!");
            Console.WriteLine("2. View Profile");
            Console.WriteLine("3. Search for a Show");
            Console.WriteLine("4. Rate a Show");
            Console.WriteLine("5. Logout");

            Console.Write("Select a menu option: ");
            int choice = Menus.GetValidInput(1, 5);

            switch (choice)
            {
                case 1:
               // return RecommendationEngine.RecommendShows;
                case 2:
                return ProfileMenu;
                case 3:
                return userActions.SearchShows;
                case 4:
                return userActions.RateShow;
                case 5:
                return EntranceMenu;
                default:
                return null;
            }
        }

        public static MenuHandlerDelegate ProfileMenu()
        {
            UserActions userActions = new UserActions();
            Console.Clear();
            Console.WriteLine("Profile Menu:");
            Console.WriteLine("1. Display Details");
            Console.WriteLine("2. View Watch History");
            Console.WriteLine("3. Change Genre Preferences");
            Console.WriteLine("4. Back to Main Menu");

            Console.Write("Select a menu option: ");
            int choice = Menus.GetValidInput(1, 4);

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
    }
}
