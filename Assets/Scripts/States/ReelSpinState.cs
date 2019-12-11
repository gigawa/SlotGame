using UnityEngine;

namespace Assets.Scripts
{
    class ReelSpinState : ByTheTale.StateMachine.State
    {
        public StateMachine stateMachine { get { return (StateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

            float spinTime = 1.75f;
            float increment = 0.25f;
            for (int i = 0; i < stateMachine.reels.Length; i++)
            {
                stateMachine.reels[i].StartSpin(spinTime);
                spinTime += increment;
            }
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
