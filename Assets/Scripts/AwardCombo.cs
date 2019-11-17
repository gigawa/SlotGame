using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Basic Award Combo
    /// </summary>
    public class AwardCombo
    {
        public int winAmount = 0;
        public int winLength = 0;
    }

    /// <summary>
    /// Inherits from award combo
    /// Line win
    /// </summary>
    [Serializable]
    public class Line : AwardCombo
    {
        public int[] position;

        public Line(Line line)
        {
            position = new int[line.position.Length];
            for (int i = 0; i < line.position.Length; i++)
            {
                position[i] = line.position[i];
            }
            winAmount = line.winAmount;
            winLength = line.winLength;
        }
    }

    /// <summary>
    /// Inherits from award combo
    /// scatter win positions
    /// </summary>
    [Serializable]
    public class Scatter : AwardCombo
    {
        [Serializable]
        public struct Position
        {
            public int x;
            public int y;
        };

        public List<Position> positions;

        public Scatter()
        {
            positions = new List<Position>();
        }
    }

    [Serializable]
    public class Way : AwardCombo
    {
        public int[] position;

        public Way(Way way)
        {
            position = new int[way.position.Length];
            for (int i = 0; i < way.position.Length; i++)
            {
                position[i] = way.position[i];
            }
            winAmount = way.winAmount;
            winLength = way.winLength;
        }
    }
}
