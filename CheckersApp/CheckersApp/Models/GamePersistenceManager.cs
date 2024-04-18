using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CheckersApp.Models
{
    public class GamePersistenceManager
    {
        public void SerializeGameState(Game game, out string serializedGameState)
        {
            serializedGameState = JsonConvert.SerializeObject(game);
        }

        public void DeserializeGameState(string serializedGameState, out Game game)
        {
            game = JsonConvert.DeserializeObject<Game>(serializedGameState);
        }

        public void SaveGameToFile(string serializedGameState, string filePath)
        {
            System.IO.File.WriteAllText(filePath, serializedGameState);
        }

        public string LoadGameFromFile(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }
    }

}
