using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StateMachine : ByTheTale.StateMachine.MachineBehaviour
    {
        [System.Serializable]
        public struct HistoryUI
        {
            public GameObject canvas;
            public Text startingText;
            public Text endingText;
            public Text bet;
            public Text win;
        };

        public struct RollupText
        {
            public Text textObject;
            public int target;
            public double length;

            public RollupText(Text text, int targetInt, double lengthS)
            {
                textObject = text;
                target = targetInt;
                length = lengthS;
            }
        }

        public int credits { get; private set; }
        public int betLevelIndex { get; set; }
        public int[] betLevels { get; private set; }
        public int minBet { get; private set; }
        public Reel[] reels;
        public WinEvaluator winEvaluator;
        public bool seedGame;

        private IdleState idleState;
        private ReelSpinState reelSpinState;
        public AwardState awardState;

        public InputMangaer inputManager;
        public SoundController soundController;
        public BaseGamePresentationController presentationController;

        public Text AwardText;
        public Text CreditText;
        public Text BetText;

        public GameCycleData currentCycleData;
        public DataManager dataManager;
        public HistoryUI historyUI;

        public GameObject[] SymbolList;

        public override void AddStates()
        {
            AddState<IdleState>();
            idleState = GetState<IdleState>();
            
            AddState<ReelSpinState>();
            reelSpinState = GetState<ReelSpinState>();
            
            AddState<AwardState>();
            awardState = GetState<AwardState>();

            AddState<HistoryState>();

            SetInitialState<IdleState>();
        }

        public void Awake()
        {
            betLevels = new int[] { 1, 2, 3, 5, 10 };
            betLevelIndex = 0;
            minBet = 50;

            UpdateBetText();
            SetCreditText();
            UpdateAwardText(0);

            inputManager.changeBet += UpdateBetText;
        }

        public void AddCredits(int cred)
        {
            credits += cred;
        }

        void UpdateBetText ()
        {
            BetText.text = (betLevels[betLevelIndex] * minBet).ToString();
        }

        void SetCreditText()
        {
            CreditText.text = credits.ToString();
        }

        public void RollupCreditText ()
        {
            RollupText rollupText = new RollupText(CreditText, credits, 0.5);
            StartCoroutine("NumberRollUp", rollupText);
        }

        public void UpdateAwardText(int award)
        {
            AwardText.text = "0";
            RollupText rollupText = new RollupText(AwardText, award, 2);
            StartCoroutine("NumberRollUp", rollupText);
        }

        /// <summary>
        /// Increments text value until reaches target value
        /// </summary>
        /// <param name="rollupText"></param>
        /// <returns></returns>
        public IEnumerator NumberRollUp(RollupText rollupText)
        {
            Text theText = rollupText.textObject;
            int targetNumber = rollupText.target;
            double length = rollupText.length;

            int currentNumber = int.Parse(theText.text);
            float currentFloat = currentNumber;

            float diff = targetNumber - currentNumber;

            // text increments up while less than target
            while (currentNumber < targetNumber)
            {
                currentFloat += Time.deltaTime * (diff / (float)length);
                currentNumber = (int)currentFloat;
                theText.text = currentNumber.ToString();

                yield return null;
            }
            theText.text = targetNumber.ToString();

            yield return null;
        }

        public bool PlaceBet ()
        {
            if (credits >= betLevels[betLevelIndex] * minBet)
            {
                credits -= betLevels[betLevelIndex] * minBet;
                SetCreditText();
                UpdateAwardText(0);

                return true;
            }

            return false;
        }

        public void Execute ()
        {
            currentCycleData = dataManager.gameData.gameHistory[dataManager.gameData.gameHistory.Count - 1];
        }
    }
}
