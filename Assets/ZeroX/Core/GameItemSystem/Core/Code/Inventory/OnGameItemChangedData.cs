namespace GameItemSystem
{
    public class OnGameItemChangedData
    {
        public string id;
        public string nftId;

        public GameItemChangedType changedType;
        
        public GameItemData oldGameItemData;
        public GameItemData newGameItemData;

        public InventoryParameters parameters;
        
        public long OldAmount => oldGameItemData == null ? 0 : oldGameItemData.Amount;
        public long NewAmount => newGameItemData == null ? 0 : newGameItemData.Amount;

        public string Where => parameters?.where;
        public string Reason => parameters?.reason;
    }
}