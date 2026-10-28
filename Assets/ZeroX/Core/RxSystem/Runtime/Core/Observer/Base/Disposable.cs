namespace ZeroX.RxSystem
{
    public interface Disposable
    {
        public void Dispose();

        
        
        /// <summary>
        /// Thêm chính nó vào disposables
        /// </summary>
        public void AddTo(Disposables disposables)
        {
            disposables.Add(this);
        }
    }
}