using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum ComboType { line, way, scatter };

    /// <summary>
    /// Win Combos - the symbols used and the win amount
    /// </summary>
    [Serializable]
    public class WinCombo
    {
        public string symbol;
        public int[] wins;
        public int minLength;
        public int maxLength;
        public ComboType comboType;
        public bool bonus;
    };

    [Serializable]
    public class Award
    {
        public List<Line> lines;
        public List<Way> ways;
        public List<Scatter> scatters;

        public bool bonusTriggered;

        public Award()
        {
            lines = new List<Line>();
            ways = new List<Way>();
            scatters = new List<Scatter>();
            bonusTriggered = false;
        }
    }

    public class WinEvaluator : MonoBehaviour
    {

        public StateMachine stateMachine;
        public Symbol[,] symbolWindow;
        private Reel[] reels;
        public int windowHeight;
        public int windowWidth;

        public List<WinCombo> winCombos;
        public List<Line> lines;

        public Award currAward;

        private bool playingWinCycle = false;
        public bool shouldPlayCycle = false;

        public Coroutine winCycleInstance;

        private GameObject[,] winIndicators;
        public GameObject winIndicatorPrefab;

        public void Start()
        {
            
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

        public void Initialize()
        {
            reels = stateMachine.reels;
            symbolWindow = new Symbol[windowWidth, windowHeight];
            SetupWinIndicators();
        }

        public void SetupWinIndicators()
        {
            winIndicators = new GameObject[windowWidth, windowHeight];
            for(int i = 0; i < windowWidth; i++)
            {
                for (int j = 0; j < windowHeight; j++)
                {
                    winIndicators[i, j] = Instantiate(winIndicatorPrefab, stateMachine.reels[i].symbols[j].transform.position, transform.rotation);
                    winIndicators[i, j].SetActive(false);
                }
            }
        }

        public void UpdateSymbolWindow ()
        {
            for (int i = 0; i < windowWidth; i++)
            {
                int position = reels[i].targetStopPos - 1 > -1 ? reels[i].targetStopPos - 1 : reels[i].symbols.Count - 1;
                for (int j = 0; j < windowHeight; j++)
                {
                    symbolWindow[i, j] = reels[i].symbols[position];
                    position = position + 1 < reels[i].symbols.Count ? position + 1 : 0;
                }
            }
        }

        /// <summary>
        /// Evaluates win amount for symbols in window
        /// </summary>
        /// <returns></returns>
        public Award EvaluateWin()
        {
            Debug.Log("Evaluate Win");

            // Resets win amounts to 0
            foreach(Line line in lines)
            {
                line.winAmount = 0;
                line.winLength = 0;
            }

            // gets all symbols in window now that spin is done
            UpdateSymbolWindow();

            // gets win amount based on possible combos
            Award award = new Award();
            foreach (WinCombo combo in winCombos)
            {
                switch(combo.comboType)
                {
                    case ComboType.line:
                        var line = CheckLineCombo(combo);
                        
                        if(line.Count > 0)
                        {
                            foreach(var l in line)
                            {
                                l.bonusTriggered = combo.bonus ? true : false;
                                award.bonusTriggered |= l.bonusTriggered;
                            }

                            award.lines.AddRange(line);
                        }

                        break;

                    case ComboType.scatter:
                        var scatter = CheckScatterCombo(combo);

                        if (scatter != null)
                        {
                            award.bonusTriggered |= combo.bonus ? true : false;
                            award.scatters.Add(scatter);
                        }

                        break;

                    default:
                        Debug.Log("Unknown Combo Type");
                        break;
                }
            }

            currAward = award;

            return award;
        }

        /// <summary>
        /// Checks combo for total line win and saves win amount and length
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public List<Line> CheckLineCombo (WinCombo combo)
        {
            List<Line> lineWins = new List<Line>();
            // Iterate through all lines looking for combos
            for (int i = 0; i < lines.Count; i++)
            {
                // Check line only if bigger win possible
                if (lines[i].winAmount < combo.wins[combo.wins.Length - 1])
                {
                    int length = 0;
                    for (int j = 0; j < windowWidth; j++)
                    {
                        int position = lines[i].position[j];
                        if (symbolWindow[j, position].gameObject.name == combo.symbol)
                        {
                            length++;
                        }else
                        {
                            break;
                        }
                    }

                    // set combo win and win for line
                    if (length >= combo.minLength)
                    {
                        lines[i].winAmount = combo.wins[length - combo.minLength];
                        lines[i].winLength = length;
                        lineWins.Add(new Line(lines[i]));
                        Debug.Log("Line " + i + " Combo Win: " + combo.symbol);
                    }
                }
            }

            return lineWins;
        }

        /// <summary>
        /// Checks reels for combo as scatter pattern
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public Scatter CheckScatterCombo (WinCombo combo)
        {
            int count = 0;
            Scatter scatter = new Scatter();
            for (int i = 0; i < windowWidth && count < combo.maxLength; i++)
            {
                for(int j = 0; j < windowHeight; j++)
                {
                    if (symbolWindow[i, j].gameObject.name == combo.symbol)
                    {
                        count++;
                        var position = new Scatter.Position
                        {
                            x = i,
                            y = j
                        };
                        scatter.positions.Add(position);
                    }
                }
            }

            if(count >= combo.minLength)
            {
                scatter.winAmount = combo.wins[count - combo.minLength];
                scatter.winLength = count;

                return scatter;
            }

            return null;
        }

        public void StartWinCycle ()
        {
            if(currAward != null)
                shouldPlayCycle = true;
        }

        public void StartWinCycle(Award award)
        {
            currAward = award;
            UpdateSymbolWindow();
            shouldPlayCycle = true;
        }

        public void StopWinCycle ()
        {
            if(winCycleInstance != null)
            {
                StopCoroutine(winCycleInstance);
            }
            playingWinCycle = false;
            shouldPlayCycle = false;

            // turn off win effect on all symbols
            for (int i = 0; i < windowWidth; i++)
            {
                for (int j = 0; j < windowHeight; j++)
                {
                    symbolWindow[i, j].StopWin();
                    winIndicators[i, j].SetActive(false);
                }
            }
        }

        /// <summary>
        /// plays win cycle on last evaluated game
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayWinCycle()
        {
            // Cycle through line wins
            for (int i = 0; i < currAward.lines.Count; i++)
            {
                for (int j = 0; j < currAward.lines[i].winLength; j++)
                {
                    //Debug.Log("y: " + lines[i].position[j]);
                    int k = currAward.lines[i].position[j];
                    symbolWindow[j, k].PlayWin();
                    winIndicators[j, k].SetActive(true);
                }

                yield return new WaitForSeconds(1.5f);

                for (int j = 0; j < currAward.lines[i].winLength; j++)
                {
                    //Debug.Log("y: " + lines[i].position[j]);
                    int k = currAward.lines[i].position[j];
                    symbolWindow[j, k].StopWin();
                    winIndicators[j, k].SetActive(false);
                }
            }

            // Cycle through scatter wins
            for (int i = 0; i < currAward.scatters.Count; i++)
            {
                foreach (var scatter in currAward.scatters)
                {
                    foreach (var position in scatter.positions)
                    {
                        symbolWindow[position.x, position.y].PlayWin();
                        winIndicators[position.x, position.y].SetActive(true);
                    }
                }

                yield return new WaitForSeconds(1.5f);

                foreach (var scatter in currAward.scatters)
                {
                    foreach (var position in scatter.positions)
                    {
                        symbolWindow[position.x, position.y].StopWin();
                        winIndicators[position.x, position.y].SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(1.5f);

            playingWinCycle = false;
        }
    }
}
