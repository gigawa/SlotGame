using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class IdleState : ByTheTale.StateMachine.State
    {
        public StateMachine stateMachine { get { return (StateMachine)machine; } }

        public delegate void PlaceBet();
        public event PlaceBet placeBet;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.inputManager.repeatBet += AttemptBet;
            stateMachine.inputManager.addCredits += AddCredits;
            stateMachine.inputManager.increaseBet += IncreaseBet;
            stateMachine.inputManager.decreaseBet += DecreaseBet;
            stateMachine.winEvaluator.StartWinCycle();

            stateMachine.RollupCreditText();

            stateMachine.inputManager.EnableInputs();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.inputManager.repeatBet -= AttemptBet;
            stateMachine.inputManager.addCredits -= AddCredits;
            stateMachine.inputManager.increaseBet -= IncreaseBet;
            stateMachine.inputManager.decreaseBet -= DecreaseBet;
            stateMachine.winEvaluator.StopWinCycle();
        }

        public override void Execute()
        {
            base.Execute();

            if (Input.GetKey(KeyCode.H))
            {
                stateMachine.ChangeState<HistoryState>();
            }
        }

        public void AttemptBet()
        {
            if (stateMachine.PlaceBet())
            {
                CommitBet();
            }
        }

        public void IncreaseBet()
        {
            if(stateMachine.betLevelIndex < stateMachine.betLevels.Length - 1)
            {
                stateMachine.betLevelIndex++;
            }
        }

        public void DecreaseBet()
        {
            if (stateMachine.betLevelIndex > 0)
            {
                stateMachine.betLevelIndex--;
            }
        }

        public void AddCredits()
        {
            stateMachine.AddCredits(1000);
            stateMachine.RollupCreditText();
        }

        public void CommitBet()
        {
            stateMachine.inputManager.DisableInputs();

            var nextCycle = new GameCycleData();
            nextCycle.betAmount = stateMachine.betLevels[stateMachine.betLevelIndex] * stateMachine.minBet;
            nextCycle.startingCredits = stateMachine.credits;
            nextCycle.reelStops = new List<int>();

            // Set reel stops for each reel
            // Random number if not seeding game  
            for (int i = 0; i < stateMachine.reels.Length; i++)
            {
                if (stateMachine.reels[i].isActiveAndEnabled)
                {
                    if (!stateMachine.seedGame)
                    {
                        int newStop = UnityEngine.Random.Range(0, stateMachine.reels[i].symbolList.Length);
                        stateMachine.reels[i].SetTargetStopPosition(newStop);
                    }

                    nextCycle.reelStops.Add(stateMachine.reels[i].targetStopPos);
                }
            }

            Award award = stateMachine.winEvaluator.EvaluateWin();
            
            int totalWin = 0;
            foreach(var combo in award.lines)
            {
                if (!combo.bonusTriggered)
                {
                    totalWin += combo.winAmount;
                }
            }
            foreach (var combo in award.ways)
            {
                if (!combo.bonusTriggered)
                {
                    totalWin += combo.winAmount;
                }
            }
            foreach (var combo in award.scatters)
            {
                if (!combo.bonusTriggered)
                {
                    totalWin += combo.winAmount;
                }
            }

            totalWin *= stateMachine.betLevels[stateMachine.betLevelIndex];

            nextCycle.award = award;

            nextCycle.totalWin = totalWin;
            stateMachine.AddCredits(totalWin);
            nextCycle.endingCredits = stateMachine.credits;

            stateMachine.dataManager.CommitCycle(nextCycle);
            stateMachine.currentCycleData = nextCycle;

            stateMachine.ChangeState<ReelSpinState>(); 
        }
    }
}
