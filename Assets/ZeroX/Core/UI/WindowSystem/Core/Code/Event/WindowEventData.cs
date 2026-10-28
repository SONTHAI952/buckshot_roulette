using System.Collections.Generic;

namespace ZeroX.WindowSystem
{
    public class WindowEventData
    {
        public Window window { get; private set; }
        public WindowParameters parameters { get; private set; }

        private WindowEventData()
        {
        }
        
        public WindowEventData(Window window, WindowParameters parameters)
        {
            this.window = window;
            this.parameters = parameters;
        }

        public static WindowEventData CloneEventData(WindowEventData eventData, Window newWindow)
        {
            WindowEventData newEventData = new WindowEventData();
            
            newEventData.window = newWindow;
            newEventData.parameters = eventData.parameters;
            
            return eventData;
        }
    }
}