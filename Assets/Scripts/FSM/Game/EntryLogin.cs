using System.Collections;
using System.Collections.Generic;
using ASeKi.ui;
using UnityEngine;

namespace ASeKi.fsm
{
    public class EntryLogin : State<game.GameController>
    {
        public override void Enter()
        {
            debug.PrintSystem.Log($"[Game][FSM][EntryLogin] 进入EntryLogin状态");
            ui.UiManager.instance.OpenUi<EntryCanvas>();
            debug.PrintSystem.Log($"[Game][FSM][EntryLogin] 打开UI EntryCanvas");
        }

        private void initManagers()
        {
            // 在这里进行各个管理器的初始化
        }
    }
}