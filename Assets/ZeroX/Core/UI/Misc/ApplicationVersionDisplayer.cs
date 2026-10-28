using System;
using TMPro;
using UnityEngine;

namespace ZeroX.UI
{
    public class ApplicationVersionDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text textVersion;
        [SerializeField] private string displayFormat = "{0}";


        private void Start()
        {
            textVersion.text = string.Format(displayFormat, Application.version);
        }


        private void Reset()
        {
            textVersion = GetComponent<TMP_Text>();
        }
    }
}