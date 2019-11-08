using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LogicStateMachine : ByTheTale.StateMachine.MachineBehaviour
    {
        public int credits { get; private set; }
        public int betLevelIndex { get; private set; }
        public int[] betLevels { get; private set; }
        public int minBet { get; private set; }
        public Reel[] reels;
        public WinEvaluator winEvaluator;
        public bool seedGame;

        private IdleState idleState;
        private ReelSpinState reelSpinState;
        private AwardState awardState;

        public InputMangaer inputManager;

        public Text AwardText;
        public Text CreditText;
        public Text BetText;

        public override void AddStates()
        {
            AddState<IdleState>();
            idleState = GetState<IdleState>();
            
            AddState<ReelSpinState>();
            reelSpinState = GetState<ReelSpinState>();
            
            AddState<AwardState>();
            awardState = GetState<AwardState>();

            SetInitialState<IdleState>();

            SubscribeToStateEvents();
        }

        public void Awake()
        {
            betLevels = new int[] { 1, 2, 3, 5, 10 };
            betLevelIndex = 0;
            minBet = 50;

            UpdateBetText();
            UpdateCreditText();

            inputManager.changeBet += UpdateBetText;
        }

        public void AddCredits(int cred)
        {
            credits += cred;
            CreditText.text = credits.ToString();
            UpdateCreditText();
        }

        public void SubscribeToStateEvents ()
        {
            idleState.placeBet += PlaceBet;
        }

        void UpdateBetText ()
        {
            BetText.text = (betLevels[betLevelIndex] * minBet).ToString();
        }

        void UpdateCreditText()
        {
            CreditText.text = credits.ToString();
        }

        void PlaceBet ()
        {
            if (credits >= betLevels[betLevelIndex] * minBet)
            {
                credits -= betLevels[betLevelIndex] * minBet;
                UpdateCreditText();
                SpinReels();
            }
        }

        void SpinReels()
        {
            ChangeState<ReelSpinState>();
        }
    }
}
