using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ButtonSystem.Editors
{
    public static class ZButtonCreator
    {
        [MenuItem("GameObject/UI/ZButton")]
        public static void CreateTemplate1()
        {
            Transform parent = null;
            if (Selection.activeTransform == null) //Nếu không chọn object nào thì cần kiểm tra nếu đang trong prefab mode thì parent sẽ là prefab đó
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                    parent = prefabStage.prefabContentsRoot.transform;
            }
            else
            {
                parent = Selection.activeTransform;
            }
            
            
            //Button
            GameObject buttonObj = new GameObject("Button", typeof(CanvasRenderer), typeof(Image), typeof(ZButton));
            buttonObj.transform.SetParent(parent);
            RectTransform buttonRectTransform = (RectTransform) buttonObj.transform;
            buttonRectTransform.localScale = Vector3.one;
            buttonRectTransform.localPosition = Vector3.zero;
            buttonRectTransform.sizeDelta = new Vector2(350, 140);




            //Text
            GameObject textMeshObj = new GameObject("Text (TMP)", typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textMeshObj.transform.SetParent(buttonObj.transform);
            RectTransform textRectTransform = (RectTransform)textMeshObj.transform;
            textRectTransform.localScale = Vector3.one;
            textRectTransform.localPosition = Vector3.zero;
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.sizeDelta = new Vector2(-36, 0);
            
            //Text Comp
            TextMeshProUGUI textMeshComp = textMeshObj.GetComponent<TextMeshProUGUI>();
            textMeshComp.enableAutoSizing = true;
            textMeshComp.fontSizeMax = 60;
            textMeshComp.enableWordWrapping = false;
            textMeshComp.raycastTarget = false;
            textMeshComp.color = Color.black;
            textMeshComp.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshComp.verticalAlignment = VerticalAlignmentOptions.Middle;
            textMeshComp.text = "New Text";
            



            Undo.RegisterCreatedObjectUndo(buttonObj, "Create ZButton Template");
            Selection.SetActiveObjectWithContext(buttonObj, buttonObj);
        }
    }
}