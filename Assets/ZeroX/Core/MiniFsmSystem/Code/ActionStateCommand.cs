using System;

namespace ZeroX.MiniFsmSystem
{
    public class ActionStateCommand : IStateCommand
    {
        public Action action;


        
        public ActionStateCommand()
        {
        }

        public ActionStateCommand(Action action)
        {
            this.action = action;
        }


        
        public void Execute()
        {
            action?.Invoke();
        }
    }
}