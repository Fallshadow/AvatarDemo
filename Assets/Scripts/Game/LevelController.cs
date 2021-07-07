namespace ASeKi.game
{
    public class LevelController : SingletonMonoBehaviorNoDestroy<LevelController>
    {
        private string curLevel = "";

        public void SetCurLevel(string name)
        {
            curLevel = name;
        }
        
        public string GetCurLevel()
        {
            return curLevel;
        }
    }
}