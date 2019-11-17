using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class GameData
    {
        public List<GameCycleData> gameHistory;
        public int credits;
        public int betLevelIndex;
        public string currentStateName;

        public GameData ()
        {
            gameHistory = new List<GameCycleData>();
        }
    }

    [Serializable]
    public class GameCycleData
    {
        public List<int> reelStops;
        public Award award;

        public int totalWin;
        public int betAmount;
        public int startingCredits;
        public int endingCredits;
    }
}
