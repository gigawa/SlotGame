using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class HistoryState : ByTheTale.StateMachine.State
    {
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

        public int gameIndex;

        public override void Enter()
        {
            base.Enter();

            stateMachine.inputManager.DisableInputs();
            stateMachine.winEvaluator.StopWinCycle();
            gameIndex = stateMachine.dataManager.gameData.gameHistory.Count - 1;
            UpdateReels();
            stateMachine.historyUI.canvas.SetActive(true);
        }

        public override void Exit()
        {
            base.Exit();

            gameIndex = stateMachine.dataManager.gameData.gameHistory.Count - 1;
            UpdateReels();
            stateMachine.winEvaluator.StopWinCycle();
            stateMachine.inputManager.EnableInputs();
            stateMachine.historyUI.canvas.SetActive(false);
        }

        public override void Execute()
        {
            base.Execute();

            if(Input.GetButtonDown("Decrease Bet"))
            {
                if(gameIndex > 0)
                {
                    gameIndex--;
                    UpdateReels();
                }
            }

            if(Input.GetButtonDown("Increase Bet"))
            {
                if(gameIndex < stateMachine.dataManager.gameData.gameHistory.Count - 1)
                {
                    gameIndex++;
                    UpdateReels();
                }
            }

            if(Input.GetButtonDown("Cancel"))
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public void UpdateReels()
        {
            Debug.Log("Game Index: " + gameIndex);
            GameCycleData data = stateMachine.dataManager.gameData.gameHistory[gameIndex];
            //stateMachine.winEvaluator.StopWinCycle();
            for (int i = 0; i < stateMachine.reels.Length; i++)
            {
                stateMachine.reels[i].SetStop(data.reelStops[i]);
            }
            stateMachine.winEvaluator.StartWinCycle(data.lines);

            UpdateUI(data);
        }

        public void UpdateUI(GameCycleData data)
        {
            stateMachine.BetText.text = data.betAmount.ToString();
            stateMachine.CreditText.text = data.endingCredits.ToString();
            stateMachine.AwardText.text = data.totalWin.ToString();

            stateMachine.historyUI.bet.text = data.betAmount.ToString();
            stateMachine.historyUI.win.text = data.totalWin.ToString();
            stateMachine.historyUI.startingText.text = data.startingCredits.ToString();
            stateMachine.historyUI.endingText.text = data.endingCredits.ToString();
        }
    }
}
