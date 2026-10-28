using ZeroX.SaveGameSystem.StorageServices;

namespace ZeroX.SaveGameSystem
{
    public static class StorageServiceProvider
    {
        public static StorageService Generate()
        {
#if UNITY_EDITOR
            return new StorageService_FileSplit();
            
#elif ZEROX_SDK_TIKTOK
            return new StorageService_Tiktok();
            
#else
            return new StorageService_FileSplit();
#endif
        }
    }
}