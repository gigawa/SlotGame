using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class AwardState : ByTheTale.StateMachine.State
    {
        public StateMachine stateMachine { get { return (StateMachine)machine; } }

        public int maxSmallWin = 200;
        public int maxBigWin = 400;

        public override void Enter()
        {
            base.Enter();

            int award = stateMachine.dataManager.GetLastGame().totalWin;

            if (award > 0)
            {
                if (award < maxSmallWin)
                {
                    stateMachine.soundController.PlaySmallWinSound();
                    stateMachine.presentationController.SmallWinCelebration();
                }
                else if (award < maxBigWin)
                {
                    stateMachine.presentationController.BigWinCelebration();
                }
                else
                {

                }
            }

            stateMachine.UpdateAwardText(award);

            stateMachine.ChangeState<IdleState>();
        }
    }
}
