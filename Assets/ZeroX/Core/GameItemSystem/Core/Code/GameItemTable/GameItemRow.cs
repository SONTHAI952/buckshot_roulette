using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GameItemSystem
{
    [System.Serializable]
    public class GameItemRow
    {
        [SerializeField] private string id;
        [SerializeField] private GameItemType type = GameItemType.normal;
        [SerializeField] private AmountType amountType = AmountType.Long;
        [SerializeField] private bool amountIsTime = false;
        [SerializeField] private bool isSingleton = true;
        [SerializeField] private bool isNested;
        [SerializeField] private bool isConsumable = true;
        [SerializeField] private string keyName;
        [SerializeField] private string parentLinkIcon;
        [SerializeField] private string linkIcon;
        [SerializeField] private string dataHandlerClass;
        
        
        //Const
        private const string defaultDataHandlerClassName = nameof(GameItemSystem) + "." + nameof(GameItemDataHandler);
        private static readonly Assembly assembly = typeof(GameItemData).Assembly;
        private static Type defaultDataHandlerClassType;

        private const string defaultParentLinkIcon = "Database/Game Item/Icon";
        
        
        
        //Cache
        [NonSerialized] private Sprite iconCached;
        [NonSerialized] private GameItemDataHandler dataHandler;
        
        

        public string Id => id;

        public GameItemType Type => type;
        
        public AmountType AmountType => amountType;
        
        public bool AmountIsTime => amountIsTime;
        
        public bool IsSingleton => isSingleton;

        public bool IsNested => isNested;

        public bool IsConsumable => isConsumable;
        
        public string KeyName => keyName;

        public Sprite Icon
        {
            get
            {
                if (iconCached == null)
                {
                    string fullLink;
                    
                    if(string.IsNullOrEmpty(parentLinkIcon))
                        fullLink = Path.Combine(defaultParentLinkIcon, linkIcon);
                    else
                        fullLink = Path.Combine(parentLinkIcon, linkIcon);
                    
                    iconCached = Resources.Load<Sprite>(fullLink);
                }

                return iconCached;
            }
        }
        
        public void SetIcon(Sprite newIcon)
        {
            iconCached = newIcon;
        }
        
        private void GenerateDataHandler()
        {
            //Cache defaultDataHandlerClassType, vì đa số sẽ dùng cái này
            if (defaultDataHandlerClassType == null)
                defaultDataHandlerClassType = assembly.GetType(defaultDataHandlerClassName);

            
            
            Type dataHandlerClassType;
            
            
            
            if (string.IsNullOrEmpty(dataHandlerClass))
            {
                //Nếu không chỉ định dataHandlerClass thì dùng dataHandlerClass mặc định
                dataHandlerClassType = defaultDataHandlerClassType;
            }
            else
            {
                dataHandlerClassType = assembly.GetType(dataHandlerClass);
                if (dataHandlerClassType == null)
                {
                    throw new Exception("Cannot found GameItemDataHandler class type: " + dataHandlerClass);
                }
            }


            dataHandler = (GameItemDataHandler)Activator.CreateInstance(dataHandlerClassType);
            GameItemDataHandler.SetInfoToHandle(dataHandler, amountType, id);
        }
        
        public GameItemDataHandler DataHandler
        {
            get
            {
                if(dataHandler == null)
                    GenerateDataHandler();

                return dataHandler;
            }
        }
    }
}