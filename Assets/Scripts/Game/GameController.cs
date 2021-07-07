using System.Collections;
using ASeKi.fsm;
using UnityEngine;

namespace ASeKi.game
{
    public class GameController : SingletonMonoBehaviorNoDestroy<GameController>
    {
        public readonly Fsm<GameController> FSM = new Fsm<GameController>();
        
        private IEnumerator Start()
        {
            SetManagersActive(false);
            yield return new WaitForEndOfFrame();   // TODO:这里应该读取本地表格数据
            initState();
            FSM.SwitchToState((int)GameFsmState.ENTRY);
            SetManagersActive(true);
        }

        private void initState()
        {
            FSM.Initialize(this);
            FSM.AddState((int)GameFsmState.ENTRY, new Entry());
            FSM.AddState((int)GameFsmState.RESET, new Reset());
            FSM.AddState((int)GameFsmState.ENTRY_LOGIN, new EntryLogin());
            FSM.AddState((int)GameFsmState.MAIN_TOWN, new LoadMainTown());
            FSM.AddState((int)GameFsmState.LOAD_SCENE, new LoadCurLevelScene());
#if UNITY_EDITOR
            FSM.AddState((int)GameFsmState.DEBUG_ENTRY, new DebugEntry());
#endif
            
        }

        // NOTE: 初始化完成前先停止GameControll上所有MonoBehaviour的Update。
        // TODO: 所有全域的Update應該都要統一放在GameControll裡。
        private void SetManagersActive(bool isActive)
        {
            MonoBehaviour[] managers = GetComponents<MonoBehaviour>();
            for(int i = 0; i < managers.Length; ++i)
            {
                managers[i].enabled = isActive;
            }
        }
        
        private void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        private void Update()
        {
            FSM.Update();
        }
        
        private void LateUpdate()
        {
            FSM.LateUpdate();
        }

        private void OnDestroy()
        {
            FSM.Finalize();
        }
    }
}
