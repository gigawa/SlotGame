using UnityEngine;

namespace Assets.Scripts
{
    class ReelSpinState : ByTheTale.StateMachine.State
    {
        public StateMachine stateMachine { get { return (StateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("Spin");

            foreach (Reel reel in stateMachine.reels)
            {
                if(!stateMachine.seedGame)
                {
                    int newStop = Random.Range(0, reel.maxRng);
                    reel.SetTargetStopPosition(newStop);
                }

                reel.StartSpin();
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
