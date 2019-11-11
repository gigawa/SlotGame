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
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

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
            stateMachine.winEvaluator.StartWinCycle();

            stateMachine.RollupCreditText();

            stateMachine.inputManager.EnableInputs();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.inputManager.repeatBet -= AttemptBet;
            stateMachine.inputManager.addCredits -= AddCredits;
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
                        int newStop = UnityEngine.Random.Range(0, stateMachine.reels[i].maxRng);
                        stateMachine.reels[i].SetTargetStopPosition(newStop);
                    }

                    nextCycle.reelStops.Add(stateMachine.reels[i].targetStopPos);
                }
            }

            int award = stateMachine.winEvaluator.EvaluateWin() * stateMachine.betLevels[stateMachine.betLevelIndex];
            
            nextCycle.lines = new List<WinEvaluator.Line>();
            foreach(var line in stateMachine.winEvaluator.lines)
            {
                nextCycle.lines.Add(new WinEvaluator.Line(line));
            }

            nextCycle.totalWin = award;
            stateMachine.AddCredits(award);
            nextCycle.endingCredits = stateMachine.credits;

            stateMachine.dataManager.CommitCycle(nextCycle);
            stateMachine.currentCycleData = nextCycle;

            float spinTime = 1.75f;
            float increment = 0.5f;
            for (int i = 0; i < stateMachine.reels.Length; i++)
            {
                stateMachine.reels[i].StartSpin(spinTime);
                spinTime += increment;
            }
        }
    }
}
