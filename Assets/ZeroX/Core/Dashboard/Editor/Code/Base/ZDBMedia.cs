using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public class ZDBMedia
    {
        /// <summary>
        /// Load từ resource
        /// </summary>
        public class Texture2DRes
        {
            private string path;
            private Texture2D texture2D;


            public Texture2DRes(string path)
            {
                this.path = path;
            }
            
            public Texture2D Texture2D
            {
                get
                {
                    if (texture2D == null)
                    {
                        string fullPath = ZDBConst.ResourcesPath.RootFolder + "Media/" + path;
                        texture2D = Resources.Load<Texture2D>(fullPath);
                        if (texture2D == null)
                        {
                            Debug.LogError("ZDB Media: texture not exist at path: " + fullPath);
                        }
                    }

                    return texture2D;
                }
            }
            
            public static implicit operator Texture2D(Texture2DRes texture2DResource)
            {
                return texture2DResource.Texture2D;
            }
        }
        
        
        
        
        
        public static readonly Texture2DSolidColor SystemLine = new Texture2DSolidColor(Color.black);
        
        public static readonly Texture2DRes Back = new Texture2DRes("Back");
        public static readonly Texture2DRes Next = new Texture2DRes("Next");
        public static readonly Texture2DRes EmptySystemIcon = new Texture2DRes("Empty System Icon");
        
        public static readonly Texture2DRes ToggleOff = new Texture2DRes("Toggle Off");
        public static readonly Texture2DRes ToggleOn = new Texture2DRes("Toggle On");
        public static readonly Texture2DRes Tick = new Texture2DRes("Tick");
        public static readonly Texture2DRes Download = new Texture2DRes("Download");
        

        // public Texture2DRes circleTick;
        // public Texture2D setting;
        // public Texture2D back;
        //
        // public Texture2D toggleOff;
        // public Texture2D toggleOn;
    }
}