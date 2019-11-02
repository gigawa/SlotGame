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
                int newStop = Random.Range(0, reel.symbols.Length - 1);
                reel.SetTargetStopPosition(newStop);
            }
        }
    }
}
