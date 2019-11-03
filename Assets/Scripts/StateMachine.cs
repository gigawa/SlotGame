using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class StateMachine : ByTheTale.StateMachine.MachineBehaviour
    {
        [SerializeField]
        public int credits { get; private set; }
        public int currentBetLevel { get; private set; }
        public int[] betLevels { get; private set; }
        public int minBet { get; private set; }
        public Reel[] reels;
        public bool seedGame;

        public override void AddStates()
        {
            AddState<IdleState>();
            AddState<ReelSpinState>();

            SetInitialState<IdleState>();
        }

        public void AddCredits(int credits)
        {
            credits += credits;
        }
    }
}
