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
                    int newStop = Random.Range(0, reel.symbols.Length - 1);
                    reel.SetTargetStopPosition(newStop);
                }

                reel.StartSpin();
            }
        }

        public override void PostExecute()
        {
            base.PostExecute();

            bool complete = true;
            foreach (Reel reel in stateMachine.reels)
            {
                complete &= reel.spinning;
            }

            if(complete)
            {
                CompleteSpin();
            }
        }

        public void CompleteSpin()
        {
            stateMachine.ChangeState<IdleState>();
        }
    }
}
