using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class DataManager : MonoBehaviour
    {

        public void Start()
        {
            gameData = new GameData();
        }

        public GameData gameData;

        public void CommitCycle (GameCycleData cycleData)
        {
            if(gameData.gameHistory.Count > 10)
            {
                gameData.gameHistory.RemoveAt(0);
            }
            gameData.gameHistory.Add(cycleData);
        }

        public void SaveGameData ()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
            bf.Serialize(file, gameData);
            file.Close();
        }

        public void LoadGameData()
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
                GameData save = (GameData)bf.Deserialize(file);
                file.Close();

                // TODO: Run some checks to make sure it's valid
                gameData = save;
            }
        }
    }
}
