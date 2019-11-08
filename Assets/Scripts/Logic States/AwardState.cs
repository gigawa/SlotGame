using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class AwardState : ByTheTale.StateMachine.State
    {
        public LogicStateMachine stateMachine { get { return (LogicStateMachine)machine; } }

        public override void Enter()
        {
            base.Enter();

            int award = stateMachine.winEvaluator.EvaluateWin() * stateMachine.betLevels[stateMachine.betLevelIndex];
            stateMachine.AddCredits(award);
            stateMachine.ChangeState<IdleState>();
        }
    }
}
