using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    public class Reset : State<game.GameController>
    {
        public override void Enter()
        {
            debug.PrintSystem.Log($"[Game][FSM][Reset] 进入reset状态");
            resetManagers();
            m_fsm.SwitchToState((int)GameFsmState.ENTRY_LOGIN);
        }

        private void resetManagers()
        {
            // 在这里进行各个管理器的重置
        }
    }
}