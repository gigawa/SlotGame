using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class ReelSpinState : ByTheTale.StateMachine.State
    {
        public StateMachine stateMachine { get { return (StateMachine)machine; } }
    }
}
