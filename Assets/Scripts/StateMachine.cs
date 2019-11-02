using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class StateMachine : ByTheTale.StateMachine.MachineBehaviour
    {
        public int credits { get; private set; }
        public int currentBetLevel { get; private set; }
        public int[] betLevels { get; private set; }
        public int minBet { get; private set; }

        public override void AddStates()
        {
            AddState<IdleState>();

            SetInitialState<IdleState>();
        }
    }
}
