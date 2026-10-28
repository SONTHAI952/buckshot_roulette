using System;
using System.Collections;
using System.Numerics;
using DataSystem.Money;
using GameItemSystem;
using TMPro;
using UnityEngine;
using ZeroX.EmissionAndFlyEffectSystem;
using ZeroX.Extensions;
using ZeroX.UI;
using Vector3 = UnityEngine.Vector3;

namespace MoneyDisplayerSystem
{
    public class MoneyDisplayer : MonoBehaviour
    {
        [SerializeField] private MoneyType moneyType = MoneyType.Coin;
        [SerializeField] private TextMeshProUGUI textAmount;
        [SerializeField] private bool useAbbreviated;
        
        [Header("Plus Effect")]
        [SerializeField] private RectTransform destFlyPoint;

        private decimal currentAnimateAmount = 0;
        private bool needShowPlusEffect;
        private int countPlusEffectPlaying = 0;

        public bool IsPlusEffectPlaying => countPlusEffectPlaying > 0;
        public RectTransform DestFlyPoint => destFlyPoint;
        
        
        
        private void Start()
        {
            MoneyDI.onTransaction.SubscribeUntilDestroy(OnTransaction, this);
            InventoryDI.OnGameItemChanged.SubscribeUntilDestroy(OnGameItemAmountChanged, this);
            
            if(countPlusEffectPlaying == 0)
                SetAnimateAmount(MoneyDI.GetAmount(moneyType));
        }

        public void SetAnimateAmount(decimal amount)
        {
            currentAnimateAmount = amount;
            if (useAbbreviated)
                textAmount.text = BigIntegerExtension.Abbreviated((BigInteger) currentAnimateAmount);
            else
                textAmount.text = MoneyFormat.Format(currentAnimateAmount);
        }

        public void PlusAnimationAmount(decimal amount)
        {
            SetAnimateAmount(currentAnimateAmount + amount);
        }
        
        void OnTransaction(MoneyTransactionData transactionData)
        {
            if(transactionData.moneyType != moneyType)
                return;

            OnMoneyAmountChanged();
        }

        void OnGameItemAmountChanged(OnGameItemChangedData data)
        {
            if(MoneyDI.IsMoneyId(data.id) == false)
                return;

            MoneyType moneyType = MoneyExtension.FromString(data.id);
            if(moneyType != this.moneyType)
                return;
            
            OnMoneyAmountChanged();
        }
        
        

        void OnMoneyAmountChanged()
        {
            if (gameObject.activeInHierarchy == false)
            {
                SetAnimateAmount(MoneyDI.GetAmount(moneyType));
                return;
            }

            StartCoroutine(Timeline());
                
                
                
            IEnumerator Timeline()
            {
                needShowPlusEffect = false;
                yield return null;
                
                if(needShowPlusEffect == false)
                {
                    SetAnimateAmount(MoneyDI.GetAmount(moneyType));
                    yield break;
                }
            }
        }

        public void BeginCustomPlusEffect()
        {
            needShowPlusEffect = true;
            countPlusEffectPlaying++;
        }

        public void EndCustomPlusEffect()
        {
            countPlusEffectPlaying--;
            if (countPlusEffectPlaying < 0)
            {
                Debug.LogError("countPlusEffectPlaying nhỏ hơn 0");
                countPlusEffectPlaying = 0;
            }
            
            if(countPlusEffectPlaying == 0)
                SetAnimateAmount(MoneyDI.GetAmount(moneyType));
        }
        
        
        
        public void ShowPlusEffect(Transform emissionPoint, decimal amountPlussed, Action onFinished)
        {
            if(amountPlussed <= 0)
            {
                onFinished?.Invoke();
                return;
            }
            
            needShowPlusEffect = true;
            countPlusEffectPlaying++;
            
            
            
            EmissionAndFlyEffectData emissionAndFlyEffectData = EmissionAndFlyEffectData.Create((RectTransform)emissionPoint, destFlyPoint, (long)amountPlussed, GameItemTable.GetIcon(GameItemIdDefine.coin));
            emissionAndFlyEffectData.onCellFinished = cellValue =>
            {
                SetAnimateAmount(currentAnimateAmount + cellValue);
            };
            
            emissionAndFlyEffectData.onFinished = () =>
            {
                countPlusEffectPlaying--;
                if (countPlusEffectPlaying < 0)
                    countPlusEffectPlaying = 0;

                if (countPlusEffectPlaying == 0)
                    SetAnimateAmount(MoneyDI.GetAmount(moneyType));

                onFinished?.Invoke();
            };
            
            EmissionAndFlyEffect.Instance.Play(emissionAndFlyEffectData);
        }

        public void ShowPlusEffect(Transform emissionPoint, decimal amountPlussed)
        {
            ShowPlusEffect(emissionPoint, amountPlussed, null);
        }
        
        public void ShowPlusEffect(Transform emissionPoint, Action onFinished)
        {
            decimal realAmount = MoneyDI.GetAmount(moneyType);
            if(realAmount <= currentAnimateAmount)
            {
                onFinished?.Invoke();
                return;
            }
            
            needShowPlusEffect = true;
            decimal amountPlussed = realAmount - currentAnimateAmount;
            ShowPlusEffect(emissionPoint, amountPlussed, onFinished);
        }
        
        public void ShowPlusEffect(Transform emissionPoint)
        {
            decimal realAmount = MoneyDI.GetAmount(moneyType);
            if(realAmount <= currentAnimateAmount)
                return;
            
            needShowPlusEffect = true;
            decimal amountPlussed = realAmount - currentAnimateAmount;
            ShowPlusEffect(emissionPoint, amountPlussed);
        }
        
        
        //Sử dụng camera
        public void ShowPlusEffect(Camera cameraOfEmissionPoint, Vector3 emissionPoint, decimal amountPlussed, Action onFinished)
        {
            if(amountPlussed <= 0)
            {
                onFinished?.Invoke();
                return;
            }
            
            needShowPlusEffect = true;
            countPlusEffectPlaying++;
            
            
            
            EmissionAndFlyEffectData emissionAndFlyEffectData = EmissionAndFlyEffectData.Create(cameraOfEmissionPoint, emissionPoint, destFlyPoint, (long)amountPlussed, GameItemTable.GetIcon(GameItemIdDefine.coin));
            emissionAndFlyEffectData.onCellFinished = cellValue =>
            {
                SetAnimateAmount(currentAnimateAmount + cellValue);
            };

            emissionAndFlyEffectData.onFinished = () =>
            {
                countPlusEffectPlaying--;
                if (countPlusEffectPlaying < 0)
                    countPlusEffectPlaying = 0;

                if (countPlusEffectPlaying == 0)
                    SetAnimateAmount(MoneyDI.GetAmount(moneyType));

                onFinished?.Invoke();
            };


            EmissionAndFlyEffect.Instance.Play(emissionAndFlyEffectData);
        }

        public void ShowPlusEffect(Camera cameraOfEmissionPoint, Vector3 emissionPoint, decimal amountPlussed)
        {
            ShowPlusEffect(cameraOfEmissionPoint, emissionPoint, amountPlussed, null);
        }

        public void ShowPlusEffect(Camera cameraOfEmissionPoint, Vector3 emissionPoint, Action onFinished)
        {
            decimal realAmount = MoneyDI.GetAmount(moneyType);
            if(realAmount <= currentAnimateAmount)
            {
                onFinished?.Invoke();
                return;
            }
            
            needShowPlusEffect = true;
            decimal amountPlussed = realAmount - currentAnimateAmount;
            ShowPlusEffect(cameraOfEmissionPoint, emissionPoint, amountPlussed, onFinished);
        }
        
        public void ShowPlusEffect(Camera cameraOfEmissionPoint, Vector3 emissionPoint)
        {
            decimal realAmount = MoneyDI.GetAmount(moneyType);
            if(realAmount <= currentAnimateAmount)
                return;
            
            needShowPlusEffect = true;
            decimal amountPlussed = realAmount - currentAnimateAmount;
            ShowPlusEffect(cameraOfEmissionPoint, emissionPoint, amountPlussed, null);
        }

        public WaitToken ShowPlusEffectOverlayAll(Transform emissionPoint, long amount)
        {
            var data = EmissionAndFlyEffectData.Create((RectTransform)emissionPoint, destFlyPoint, amount, GameItemTable.GetIcon(GameItemIdDefine.coin));
            return EmissionAndFlyEffect.Instance.Play(data);
        }
    }
}