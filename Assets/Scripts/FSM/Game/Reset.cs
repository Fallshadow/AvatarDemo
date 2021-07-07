using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    public class Reset : State<game.GameController>
    {
        public override void Enter()
        {
            resetManagers();
            m_fsm.SwitchToState((int)GameFsmState.RESET);
        }

        private void resetManagers()
        {
            // 在这里进行各个管理器的重置
        }
    }
}