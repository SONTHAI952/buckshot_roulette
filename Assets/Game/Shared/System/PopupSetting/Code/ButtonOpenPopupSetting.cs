using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ButtonOpenPopupSetting : MonoBehaviour
    {
        [SerializeField] private Button button;


        private void Awake()
        {
            button.onClick.AddListener(Button_OnClick);
        }

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Button_OnClick()
        {
            PopupSetting.Instance.Open();
        }
    }
}