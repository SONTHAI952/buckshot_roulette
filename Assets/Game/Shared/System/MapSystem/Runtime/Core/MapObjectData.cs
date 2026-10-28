using UnityEngine;

namespace MapSystem
{
    [System.Serializable]
    public class MapObjectData
    {
        public string ExportToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void ImportFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}