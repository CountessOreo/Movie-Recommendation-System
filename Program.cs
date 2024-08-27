using System;
using System;
using Watt_2_Watch;
using static Project284.Menus;

namespace Project284
{
    internal class Program
    {
        public static UserActions userActions = new UserActions();

        static void Main(string[] args)
        {
            MenuHandlerDelegate nextMenu = EntranceMenu;

            while (true)
            {
                Console.Clear();
                nextMenu = nextMenu();
                if (nextMenu == null) break;
            }
        }
    }
}
