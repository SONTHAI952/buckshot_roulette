namespace GameItemSystem
{
    public class InventoryParameters
    {
        public string where = "";
        public string reason = "";

        
        
        public InventoryParameters()
        {
        }

        public InventoryParameters(string where, string reason)
        {
            this.where = where;
            this.reason = reason;
        }
    }
}