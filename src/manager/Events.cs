using Godot;

namespace Manager
{
    public class Events : Singleton<Events>
    {
        [Signal]
        private delegate void SceneUnloaded();
        [Signal]
        private delegate void SceneLoaded();

        public override void _EnterTree()
        {
            SetSingleton();
        }
    }
}
