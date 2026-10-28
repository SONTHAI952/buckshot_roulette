namespace ZeroX.RxSystem
{
    public class DisposableSingle : Disposable
    {
        public SubjectBase subject;
        public ObserverBase observer;


        public DisposableSingle(SubjectBase subject, ObserverBase observer)
        {
            this.subject = subject;
            this.observer = observer;
        }


        public void Dispose()
        {
            subject.UnSubscribe(observer);
        }
    }
}