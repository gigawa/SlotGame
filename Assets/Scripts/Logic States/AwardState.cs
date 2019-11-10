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

            stateMachine.UpdateAwardText(stateMachine.dataManager.GetLastGame().totalWin);
            stateMachine.ChangeState<IdleState>();
        }
    }
}
