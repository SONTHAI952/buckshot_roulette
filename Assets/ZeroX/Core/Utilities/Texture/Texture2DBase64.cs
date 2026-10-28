using System;
using UnityEngine;

namespace ZeroX
{
    public class Texture2DBase64
    {
        private int width;
        private int height;
        private string base64 = "";
        private byte[] bytes;
        private Texture2D texture2D;

        public Texture2DBase64(int width, int height, string base64)
        {
            this.width = width;
            this.height = height;
            this.base64 = base64;
        }

        void InitBytesIfNot()
        {
            if (bytes == null)
            {
                bytes = Convert.FromBase64String(base64);
            }
        }
        
        public Texture2D Texture2D
        {
            get
            {
                InitBytesIfNot();
                if (texture2D == null)
                {
                    texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    texture2D.LoadImage(bytes);
                }

                return texture2D;
            }
        }

        public static implicit operator Texture2D(Texture2DBase64 texture2DBase64)
        {
            return texture2DBase64.Texture2D;
        }
    }
}