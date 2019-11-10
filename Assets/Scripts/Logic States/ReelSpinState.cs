﻿using UnityEngine;

namespace Assets.Scripts
{
    class ReelSpinState : ByTheTale.StateMachine.State
    {
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

        }

        public override void PostExecute()
        {
            base.PostExecute();

            // Check if reels are done spinning
            bool complete = true;
            foreach (Reel reel in stateMachine.reels)
            {
                complete &= !reel.spinning;
            }

            // if done spinning finish state and go to award
            if(complete)
            {
                CompleteSpin();
            }
        }

        /// <summary>
        /// Finish spinning state and go to award state
        /// </summary>
        public void CompleteSpin()
        {
            stateMachine.ChangeState<AwardState>();
        }
    }
}
