using System.Text.RegularExpressions;
using Watt_2_Watch;
using static Project284.Menus;


namespace Project284
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MenuHandlerDelegate NextMenu = EntranceMenu;

            while (true)
            {
                Console.Clear();
                NextMenu = NextMenu();
                if (NextMenu == null) break;
            }
        }        
    }
}

