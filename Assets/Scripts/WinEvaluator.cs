﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinEvaluator : MonoBehaviour
    {
        /// <summary>
        /// Win Combos - the symbols used and the win amount
        /// </summary>
        [Serializable]
        public class WinCombo
        {
            public List<string> symbols;
            public int win;

            public WinCombo()
            {
                symbols = new List<string>();
            }
        };

        [Serializable]
        public class SimpleComboTemplate
        {
            public string symbol;
            public int[] wins;
            public int minLength;
            public int maxLength;
        }

        /// <summary>
        /// Line - position is y position with index being the reel
        /// Also holds win amount when evaluated
        /// </summary>
        [Serializable]
        public class Line
        {
            public int [] position;
            public int winAmount = 0;
            public int winLength = 0;
        };

        public LogicStateMachine stateMachine;
        public Symbol[,] symbolWindow;
        private Reel[] reels;
        public int windowHeight;
        public int windowWidth;

        public List<WinCombo> winCombos;
        public List<Line> lines;

        private bool playingWinCycle = false;
        private bool shouldPlayCycle = false;

        public SimpleComboTemplate[] comboTemplates;

        public Coroutine winCycleInstance;

        public void Start()
        {
            reels = stateMachine.reels;
            symbolWindow = new Symbol[windowWidth, windowHeight];
            SetupSimpleCombos();
        }

        public void FixedUpdate()
        {
            if(shouldPlayCycle)
            {
                if (!playingWinCycle)
                {
                    playingWinCycle = true;
                    winCycleInstance = StartCoroutine(PlayWinCycle());
                }
            }
        }

        /// <summary>
        /// Setup basic combos - 3, 4, 5 in a row type combos
        /// </summary>
        public void SetupSimpleCombos()
        {
            if (winCombos == null)
            {
                winCombos = new List<WinCombo>();
            }

            foreach(var combo in comboTemplates)
            {
                for(int i = 0; i <= combo.maxLength - combo.minLength; i++)
                {
                    WinCombo win = new WinCombo();
                    for(int j = 0; j < combo.minLength + i; j++)
                    {
                        win.symbols.Add(combo.symbol);
                    }
                    win.win = combo.wins[i];
                    winCombos.Add(win);
                }
            }
        }

        /// <summary>
        /// Evaluates win amount for symbols in window
        /// </summary>
        /// <returns></returns>
        public int EvaluateWin()
        {
            Debug.Log("Evaluate Win");

            // Resets win amounts to 0
            foreach(Line line in lines)
            {
                line.winAmount = 0;
                line.winLength = 0;
            }

            // gets all symbols in window now that spin is done
            for (int i = 0; i < windowWidth; i++)
            {
                int position = reels[i].stopPosition - 1 > -1 ? reels[i].stopPosition - 1 : reels[i].symbols.Length - 1;
                for (int j = 0; j < windowHeight; j++)
                {
                    symbolWindow[i,j] = reels[i].symbols[position];
                    position = position + 1 < reels[i].symbols.Length ? position + 1 : 0;
                }
            }

            // gets win amount based on possible combos
            int win = 0;
            int count = 0;
            foreach(WinCombo combo in winCombos)
            {
                win += CheckCombo(combo);
                count++;
            }

            Debug.Log("Combo Count: " + count);

            // logging
            for(int i = 0; i < lines.Count; i++)
            {
                Debug.Log("Line " + i + " Win: " + lines[i].winAmount);
            }

            Debug.Log("Total Win: " + win);

            return win;
        }

        /// <summary>
        /// Checks combo for total win and saves win amount and length
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public int CheckCombo(WinCombo combo)
        {
            int comboWin = 0;
            Debug.Log("Combo: " + combo.symbols[0]);

            // Iterate through all lines looking for combos
            for(int i = 0; i < lines.Count; i++)
            {
                // Check line only if bigger win possible
                if (lines[i].winAmount < combo.win)
                {
                    Debug.Log("First Loop");
                    bool win = true;
                    for (int j = 0; j < combo.symbols.Count; j++)
                    {
                        Debug.Log("Second Loop");
                        int position = lines[i].position[j];
                        if (symbolWindow[j, position].gameObject.name != combo.symbols[j])
                        {
                            Debug.Log(symbolWindow[j, position].gameObject.name + " != " + combo.symbols[j]);
                            Debug.Log("Line " + i + " No Match: " + j + ", " + position);
                            win = false;
                        }
                    }

                    // set combo win and win for line
                    if (win)
                    {
                        Debug.Log("Win");
                        comboWin += combo.win;
                        lines[i].winAmount = combo.win;
                        lines[i].winLength = combo.symbols.Count;
                        //Debug.Log("Line " + i + " Combo Win: " + combo.win);
                    }
                }
            }

            return comboWin;
        }

        public void StartWinCycle ()
        {
            shouldPlayCycle = true;
        }

        public void StopWinCycle ()
        {
            StopCoroutine(winCycleInstance);
            playingWinCycle = false;
            shouldPlayCycle = false;
        }

        IEnumerator PlayWinCycle()
        {
            //Debug.Log("Play Win Cycle");
            for(int i = 0; i < lines.Count; i++)
            {
                if(lines[i].winLength > 0)
                {
                    for (int j = 0; j < lines[i].winLength; j++)
                    {
                        //Debug.Log("y: " + lines[i].position[j]);
                        symbolWindow[j, lines[i].position[j]].winEffect.SetActive(true);
                    }

                    yield return new WaitForSeconds(1.5f);

                    for (int j = 0; j < lines[i].winLength; j++)
                    {
                        //Debug.Log("y: " + lines[i].position[j]);
                        symbolWindow[j, lines[i].position[j]].winEffect.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(1.5f);

            playingWinCycle = false;
        }
    }
}
