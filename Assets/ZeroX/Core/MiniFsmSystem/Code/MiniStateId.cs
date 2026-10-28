namespace ZeroX.MiniFsmSystem
{
    public class MiniStateId<TStateId>
    {
        public TStateId value;
        public bool hasValue;

        
        
        public void SetNone()
        {
            value = default;
            hasValue = false;
        }

        public void SetValue(TStateId newValue)
        {
            value = newValue;
            hasValue = true;
        }
    }
}