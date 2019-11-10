using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class AwardState : ByTheTale.StateMachine.State
    {
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("History Count: " + stateMachine.dataManager.gameData.gameHistory.Count.ToString());

            stateMachine.UpdateAwardText(stateMachine.dataManager.gameData.gameHistory[stateMachine.dataManager.gameData.gameHistory.Count - 1].totalWin);
            stateMachine.ChangeState<IdleState>();
        }
    }
}
