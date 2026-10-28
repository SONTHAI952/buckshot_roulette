using System.Runtime.InteropServices;
using UnityEngine;

namespace ZeroX
{
    public static class OpenAppCrossPlatform
    {
        public static void OpenSMS(string mobile_num, string message)
        {
#if UNITY_ANDROID
            //Android SMS URL - doesn't require encoding for sms call to work
            string url = string.Format("sms:{0}?body={1}", mobile_num, System.Uri.EscapeDataString(message));
#elif UNITY_IOS
        //ios SMS url - ios requires encoding for sms call to work
        //string url = string.Format("sms:{0}?&body={1}",mobile_num,WWW.EscapeURL(message)); //Method1 - Works but puts "+" for spaces
        //string url = "sms:"+mobile_num+"?&body="+WWW.EscapeURL(message); //Method2 - Works but puts "+" for spaces
        //string url = string.Format("sms:{0}?&body={1}",mobile_num,System.Uri.EscapeDataString(message)); //Method3 - Works perfect
        string url = "sms:"+mobile_num+"?&body="+ System.Uri.EscapeDataString(message); //Method4 - Works perfectly
#else
        string url = "Unknown Url";
#endif


            //Execute Text Message
            Application.OpenURL(url);
        }

        public static void OpenAppStore(string id)
        {
#if UNITY_ANDROID
            OpenAppStore_Android(id);
#elif UNITY_IOS
        OpenAppStore_IOS(id);
#else
        Debug.Log("OpenAppStore - not support this platform: " + Application.platform);
#endif
        }


        private static void OpenAppStore_Android(string appId)
        {
            string url = string.Format("https://play.google.com/store/apps/details?id={0}", appId);
            Debug.Log("Android Url: " + url);
            Application.OpenURL(url);
        }


#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void OpenAppStore_IOS(string appID);
#endif
    }
}