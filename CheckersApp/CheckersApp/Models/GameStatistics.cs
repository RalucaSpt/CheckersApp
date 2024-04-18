using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CheckersApp.Models
{
    namespace CheckersApp.Models
    {
        using Newtonsoft.Json;
        using System.IO;

        public static class GameStatistics
        {
            public static int BlackWins { get; set; }
            public static int RedWins { get; set; }
            public static int Draws { get; set; }
            public static int MaxPiecesRemaining { get; set; }

            public static void SaveStatistics(string filePath)
            {
                var stats = new
                {
                    BlackWins,
                    RedWins,
                    Draws,
                    MaxPiecesRemaining
                };
                string json = JsonConvert.SerializeObject(stats);
                File.WriteAllText(filePath, json);
            }

            public static void LoadStatistics(string filePath)
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var stats = JsonConvert.DeserializeObject<dynamic>(json);
                    BlackWins = (int)stats.BlackWins;
                    RedWins = (int)stats.RedWins;
                    Draws = (int)stats.Draws;
                    MaxPiecesRemaining = (int)stats.MaxPiecesRemaining;
                }
            }

            public static void EndGame(Color winner, int piecesRemaining)
            {
                LoadStatistics("Checkers_WPF_App\\CheckersApp\\CheckersApp\\Json\\statistics.json");
                if (winner == Color.Red)
                {
                    RedWins++;
                }
                else if (winner == Color.Black)
                {
                    BlackWins++;
                }
                else
                {
                    Draws++;
                }


                if (piecesRemaining > MaxPiecesRemaining)
                {
                    MaxPiecesRemaining = piecesRemaining;
                }
                SaveStatistics("Checkers_WPF_App\\CheckersApp\\CheckersApp\\Json\\statistics.json");
            }
        }
    }

}
