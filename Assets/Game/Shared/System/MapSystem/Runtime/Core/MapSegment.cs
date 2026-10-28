using UnityEngine;

namespace MapSystem
{
    public class MapSegment : MonoBehaviour
    {
        [SerializeField] protected MapSegmentData data;
        [SerializeField] protected Transform objectContainer;

        public MapSegmentData Data => data;
        public Transform ObjectContainer => objectContainer;
        
        

        #region Unity Method

        protected virtual void Reset()
        {
            objectContainer = transform;
        }

        #endregion
        


        protected void DestroyAllObject()
        {
            foreach (Transform child in objectContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        public virtual void LoadFromData(MapSegmentData segmentData)
        {
            DestroyAllObject();
            
            this.data = segmentData;
            MapSegmentUtility.LoadFromData_WithGroup(this, segmentData);
            
            ExecuteLoadFromData(segmentData);
        }

        protected virtual void ExecuteLoadFromData(MapSegmentData segmentData)
        {
            
        }

        
        

        #region Editor
        
        protected void Editor_DestroyAllObject()
        {
            MapSegmentUtility.Editor_DestroyAllObject(this);
        }
        
        public void Editor_LoadFromData(MapSegmentData segmentData)
        {
            Editor_DestroyAllObject();
            
            this.data = segmentData;
            MapSegmentUtility.Editor_LoadFromData(this, segmentData);
            
            Editor_ExecuteLoadFromData(segmentData);
        }

        public void Editor_SaveToData(MapSegmentData segmentData)
        {
            MapSegmentUtility.Editor_SaveToData(this, segmentData);
            
            Editor_ExecuteSaveToData(segmentData);
            
            UnityEditor.EditorUtility.SetDirty(segmentData);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        
        protected virtual void Editor_ExecuteLoadFromData(MapSegmentData segmentData)
        {
        }
        
        protected virtual void Editor_ExecuteSaveToData(MapSegmentData segmentData)
        {
            
        }

        
        [ContextMenu("Load From Data - Editor")]
        public void Editor_LoadFromData()
        {
            if (data == null)
            {
                Debug.LogError("Cannot load because data is empty");
                return;
            }
            
            Editor_LoadFromData(data);
        }

        [ContextMenu("Save To Data - Editor")]
        public void Editor_SaveToData()
        {
            if (data == null)
            {
                Debug.LogError("Cannot save because data is empty");
                return;
            }
            
            Editor_SaveToData(data);
        }
        
        #endregion
    }
}