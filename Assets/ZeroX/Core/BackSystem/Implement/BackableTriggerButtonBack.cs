using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.BackSystem
{
    public class BackableTriggerButtonBack : BackableMonoBehaviour
    {
        [SerializeField] private Button buttonBack;
        

        
        protected override void ExecuteBack()
        {
            if(BackManager.Instance.IsButtonClickable(buttonBack) == false)
                return;
            
            buttonBack.onClick.Invoke();
        }
    }
}