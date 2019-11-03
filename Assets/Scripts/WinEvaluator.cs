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
        public StateMachine stateMachine;
        public GameObject[,] symbolWindow;
        public Reel[] reels;
        public int windowHeight;
        public int windowWidth;

        public void Start()
        {
            reels = stateMachine.reels;
            symbolWindow = new GameObject[windowWidth, windowHeight];
        }

        public void EvaluateWin()
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
        }
    }
}
