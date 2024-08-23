using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static Watt_2_Watch.Database;
using Watt_2_Watch;

namespace Watt_2_Watch
{
    internal class RecommendationEngine
    {
        //public Dictionary<string, object> user;
        //private Database db;

        //public RecommendationEngine(Dictionary<string, object> user, Database db)
        //{
        //    this.user = user;
        //    this.db = db;
        //}

        //public void RecommendShows()
        //{
        //    var genreRankings = (Dictionary<string, int>)user["PreferredGenres"];
        //    //Orders the list in acceding order
        //    var sortedGenres = genreRankings.OrderBy(gr => gr.Value).Select(gr => gr.Key).ToList();
        //    //Reverses order of list to display the most ranked genre first
        //    sortedGenres.Reverse();

        //    Random random = new Random();

        //    foreach (var genre in sortedGenres)
        //    {
        //        List<DatabaseRecord> Shows = db.FilterByGenre(new List<string> { genre });
        //        //Shuffles the Shows list and takes the first 10
        //        var randomShows = Shows.OrderBy(x => random.Next()).Take(10).ToList();

        //        Console.WriteLine($"Shows found for {genre}:\r\n");
        //        foreach (DatabaseRecord rec in randomShows)
        //        {
        //            Console.WriteLine($"Title: {rec.OriginalTitle}.");
        //        }
        //        Console.WriteLine();
        //    }

        //    Console.ReadKey();
        //    Console.Clear();
        //}
    }
}
