using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.UI
{
    public class GrayEffectUIReplaceMaterial : GrayEffectUI
    {
        [Header("Material")]
        [SerializeField] private Material m_grayMaterial;

        private Dictionary<Graphic, Material> dictGraphicNormalMaterial;
        private Dictionary<TextMeshProUGUI, Color> dictTextNormalColor;
        
        public Material GrayMaterial
        {
            get => m_grayMaterial;
            set => m_grayMaterial = value;
        }
        
        protected override void ExecuteSetGray(bool isGray)
        {
            SetGrayForButton(isGray);
            SetGrayForGraphic(isGray);
            SetGrayForText(isGray);
        }

        void SetGrayForButton(bool isGray)
        {
            if (m_button != null)
            {
                if (isGray)
                {
                    m_button.interactable = m_buttonInteractableWhenGray;
                }
                else
                {
                    m_button.interactable = true;
                }
            }
        }
        
        void SetGrayForGraphic(bool isGray)
        {
            if(m_listGraphic == null)
                return;
            
            if(m_listGraphic.Count == 0)
                return;

            if (dictGraphicNormalMaterial == null)
            {
                dictGraphicNormalMaterial = new Dictionary<Graphic, Material>();
                foreach (var graphic in m_listGraphic)
                {
                    dictGraphicNormalMaterial.Add(graphic, graphic.material);
                }
            }

            if (isGray)
            {
                foreach (var graphic in m_listGraphic)
                {
                    graphic.material = GrayMaterial;
                }
            }
            else
            {
                foreach (var graphic in m_listGraphic)
                {
                    if (dictGraphicNormalMaterial.TryGetValue(graphic, out var normalMaterial))
                    {
                        graphic.material = normalMaterial;
                    }
                    else
                    {
                        graphic.material = null;
                    }
                }
            }
        }

        void SetGrayForText(bool isGray)
        {
            if(m_listText == null)
                return;
            
            if(m_listText.Count == 0)
                return;

            if (dictTextNormalColor == null)
            {
                dictTextNormalColor = new Dictionary<TextMeshProUGUI, Color>();
                foreach (var textMesh in m_listText)
                {
                    dictTextNormalColor.Add(textMesh, textMesh.color);
                }
            }
            
            if (isGray)
            {
                foreach (var textMesh in m_listText)
                {
                    textMesh.color = m_textColorWhenGray;
                }
            }
            else
            {
                foreach (var textMesh in m_listText)
                {
                    if (dictTextNormalColor.TryGetValue(textMesh, out var normalColor))
                    {
                        textMesh.color = normalColor;
                    }
                    else
                    {
                        textMesh.color = Color.black;
                    }
                }
            }
        }
    }
}