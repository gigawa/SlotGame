﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        }

        public override void Exit()
        {
            base.Exit();

            gameIndex = stateMachine.dataManager.gameData.gameHistory.Count - 1;
            UpdateReels();
            stateMachine.inputManager.EnableInputs();
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
            //stateMachine.winEvaluator.StartWinCycle(data.lines);
            for (int i = 0; i < stateMachine.reels.Length; i++)
            {
                stateMachine.reels[i].SetStop(data.reelStops[i]);
                stateMachine.winEvaluator.StartWinCycle(data.lines);
            }
        }
    }
}