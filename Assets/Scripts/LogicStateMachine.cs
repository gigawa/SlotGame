﻿using System.Collections;
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
            SetCreditText();
            UpdateAwardText(0);

            inputManager.changeBet += UpdateBetText;
        }

        public void AddCredits(int cred)
        {
            credits += cred;
            RollupCreditText();
        }

        public void SubscribeToStateEvents ()
        {
            idleState.placeBet += PlaceBet;
        }

        void UpdateBetText ()
        {
            BetText.text = (betLevels[betLevelIndex] * minBet).ToString();
        }

        void SetCreditText()
        {
            CreditText.text = credits.ToString();
        }

        void RollupCreditText ()
        {
            RollupText rollupText = new RollupText(CreditText, credits, 0.75);
            StartCoroutine("NumberRollUp", rollupText);
        }

        public void UpdateAwardText(int award)
        {
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
            Debug.Log("Roll Up");
            Text theText = rollupText.textObject;
            int targetNumber = rollupText.target;
            double length = rollupText.length;

            int currentNumber = int.Parse(theText.text);
            float currentFloat = currentNumber;

            float diff = targetNumber - currentNumber;

            // text increments up while less than target
            Debug.Log("Current: " + currentNumber + " Target: " + targetNumber);
            while (currentNumber < targetNumber)
            {
                Debug.Log("Roll Up");
                currentFloat += Time.deltaTime * (diff / (float)length);
                currentNumber = (int)currentFloat;
                theText.text = currentNumber.ToString();

                yield return null;
            }
            theText.text = targetNumber.ToString();

            yield return null;
        }

        void PlaceBet ()
        {
            if (credits >= betLevels[betLevelIndex] * minBet)
            {
                credits -= betLevels[betLevelIndex] * minBet;
                SetCreditText();
                UpdateAwardText(0);
                SpinReels();
            }
        }

        void SpinReels()
        {
            ChangeState<ReelSpinState>();
        }
    }
}
