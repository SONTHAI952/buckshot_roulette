namespace ZeroX.SceneTransitionerSystem
{
    [System.Serializable]
    public class SceneTransitionerPrefabData
    {
        public string id;
        public SceneTransitioner prefab;

        public SceneTransitionerPrefabData()
        {
        }

        public SceneTransitionerPrefabData(string id)
        {
            this.id = id;
        }

        public SceneTransitionerPrefabData(string id, SceneTransitioner prefab)
        {
            this.id = id;
            this.prefab = prefab;
        }
    }
}