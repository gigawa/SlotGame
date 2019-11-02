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

        public override void Execute()
        {
            base.Execute();

            if(Input.GetKeyDown(KeyCode.Space))
            {
                AttemptBet();
            }
        }

        public void AttemptBet()
        {
            if(stateMachine.credits > stateMachine.currentBetLevel * stateMachine.minBet)
            {
                stateMachine.ChangeState<ReelSpinState>();
            }
        }
    }
}
