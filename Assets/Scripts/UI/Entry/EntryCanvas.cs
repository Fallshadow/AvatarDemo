using act.UIRes;
using System.Collections.Generic;
using ASeKi.fsm;
using ASeKi.game;
using UnityEngine;
    
namespace ASeKi.ui
{
    [BindingResource(UiAssetIndex.EntryCanvas)]
    public class EntryCanvas : FullScreenCanvasBase
    {
        protected override void onOpen()
        {
            
        }

        protected override void onClose()
        {
            
        }

        public override void Initialize()
        {
            
        }

        public override void Release()
        {
            
        }

        public override void Refresh()
        {
            
        }
        
        public void OpenMainTown()
        {
            GameController.instance.FSM.SwitchToState(new LoadMainTown());
        }
    }
}