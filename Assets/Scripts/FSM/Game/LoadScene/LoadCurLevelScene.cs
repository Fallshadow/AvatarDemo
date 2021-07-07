using System.Collections.Generic;

namespace ASeKi.fsm
{
    public class LoadCurLevelScene : LoadScene
    {
        
        public LoadCurLevelScene() : base()
        {
            scene_Name = "Level_001";
        }

        protected override void onEnter()
        {

        }

        protected override void onExit()
        {

        }

        protected override void onLoading()
        {

        }

        protected override void onLoadingEnd()
        {

        }

        protected override int levelId()
        {
            return 0;
        }

        public void SetSceneName(string name)
        {
            scene_Name = name;
        }
        
    }
}