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

            stateMachine.inputManager.EnableInputs();
        }

        public override void Exit()
        {
            base.Exit();

            stateMachine.inputManager.repeatBet -= AttemptBet;
            stateMachine.inputManager.addCredits -= AddCredits;
            stateMachine.winEvaluator.StopWinCycle();
        }

        public void AttemptBet()
        {
            placeBet();
        }

        public void AddCredits()
        {
            stateMachine.AddCredits(1000);
        }
    }
}
