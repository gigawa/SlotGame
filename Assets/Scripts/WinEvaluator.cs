using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinEvaluator : MonoBehaviour
    {
        [Serializable]
        public struct WinCombo
        {
            public string[] symbols;
            public int win;
        };

        [Serializable]
        public struct Line
        {
            public int [] position;
        };

        public StateMachine stateMachine;
        public GameObject[,] symbolWindow;
        public Reel[] reels;
        public int windowHeight;
        public int windowWidth;

        public List<WinCombo> winCombos;
        public List<Line> lines;

        public void Start()
        {
            reels = stateMachine.reels;
            symbolWindow = new GameObject[windowWidth, windowHeight];
        }

        public int EvaluateWin()
        {
            Debug.Log("Evaluate Win");
            for (int i = 0; i < windowWidth; i++)
            {
                int position = reels[i].stopPosition - 1 > -1 ? reels[i].stopPosition - 1 : reels[i].symbols.Length - 1;
                for (int j = 0; j < windowHeight; j++)
                {
                    symbolWindow[i,j] = reels[i].symbols[position];
                    position = position + 1 < reels[i].symbols.Length ? position + 1 : 0;
                }
            }

            for(int i = 0; i < windowWidth; i++)
            {
                for(int j = 0; j < windowHeight; j++)
                Debug.Log(symbolWindow[i,j].name);
            }

            int win = 0;

            foreach(WinCombo combo in winCombos)
            {
                win += CheckCombo(combo);
            }

            Debug.Log("Total Win: " + win);

            return win;
        }

        public int CheckCombo(WinCombo combo)
        {
            int comboWin = 0;
            for(int i = 0; i < lines.Count; i++)
            {
                bool win = true;
                for (int j = 0; j < combo.symbols.Length; j++)
                {
                    int position = lines[i].position[j];
                    if (symbolWindow[j, position].name != combo.symbols[j])
                    {
                        win = false;
                    }
                }

                if(win)
                {
                    comboWin += combo.win;
                    Debug.Log("Line " + i + " Win: " + combo.win);
                }
            }

            return comboWin;
        }
    }
}
