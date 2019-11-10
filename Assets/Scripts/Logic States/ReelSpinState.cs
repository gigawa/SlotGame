using UnityEngine;

namespace Assets.Scripts
{
    class ReelSpinState : ByTheTale.StateMachine.State
    {
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

            stateMachine.inputManager.DisableInputs();

            // Set reel stops for each reel
            // Random number if not seeding game
            float spinTime = 1.75f;
            float increment = 0.5f;
            for(int i = 0; i < stateMachine.reels.Length; i++)
            {
                if (stateMachine.reels[i].isActiveAndEnabled)
                {
                    if (!stateMachine.seedGame)
                    {
                        int newStop = Random.Range(0, stateMachine.reels[i].maxRng);
                        stateMachine.reels[i].SetTargetStopPosition(newStop);
                    }

                    stateMachine.reels[i].StartSpin(spinTime);
                    spinTime += increment;
                }
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
