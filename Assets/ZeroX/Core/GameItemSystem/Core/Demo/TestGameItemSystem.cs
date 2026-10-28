using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GameItemSystem.Demo
{
    public class TestGameItemSystem : MonoBehaviour
    {
        [SerializeField] private string gameItemDataJson;
        [SerializeField] private Image imageIcon;
        [SerializeField] private Image imageOverrideIcon;
        
        
        private void Start()
        {
            InventoryDI.OnGameItemChanged.SubscribeUntilDestroy(OnGameItemChanged, this);
            LoadIcon();
        }

        [ContextMenu("Add")]
        public void Add()
        {
            GameItemData coin = GameItemTable.CreateDataInstance("coin");
            coin.id = "coin";
            coin.SetAmount(100);
            

            InventoryDI.Add(coin, "test", "test");
            
        }

        [ContextMenu("TestPerformance")]
        public void TestPerformance()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 1000; i++)
            {
                InventoryDI.Add("coin_long", 1, "test", "test");
            }
            st.Stop();
            Debug.Log("Duration Long: " + st.ElapsedMilliseconds);
            
            
            st.Restart();
            for (int i = 0; i < 1000; i++)
            {
                InventoryDI.Add("coin_decimal", 1.5M, "test", "test");
            }
            st.Stop();
            Debug.Log("Duration Decimal: " + st.ElapsedMilliseconds);
        }

        
        [ContextMenu("Test Phep Tinh")]
        public void TestPhepTinh()
        {
            GameItemData gameItemData = new GameItemData("coin");
            gameItemData.SetAmount(5.5m);
            Debug.Log(gameItemData.AmountDecimal);
            gameItemData.PlusAmount(15.5m);
            Debug.Log(gameItemData.AmountDecimal);
        }

        public void LoadIcon()
        {
            GameItemData gameItemData = GameItemTable.CreateDataInstanceFromJson(gameItemDataJson);
            imageIcon.sprite = gameItemData.Icon;
            imageOverrideIcon.sprite = gameItemData.OverrideIcon;
        }

        [ContextMenu("Test Init Singleton")]
        public void TestInitSingleton()
        {
            InventoryDI.Add("coin", 100, "test", "test");
            Debug.Log("Coin Amount: " + InventoryDI.GetAmount("coin"));
        }

        private void OnGameItemChanged(OnGameItemChangedData data)
        {
            Debug.Log("OnGameItemChangedData: " + data.changedType);
            Debug.Log("OldAmount: " + data.OldAmount);
            Debug.Log("NewAmount: " + data.NewAmount);
        }
    }
}