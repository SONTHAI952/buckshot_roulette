namespace ZeroX.SecuritySystem
{
    public class AesFloat
    {
        private byte[] keyBytes = null;
        private byte[] cipherValueBytes = null;

        public float Value
        {
            get
            {
                if (cipherValueBytes == null)
                    return 0;

                return AESEncrypt.DecryptFloat(cipherValueBytes, keyBytes);
            }

            set
            {
                if (keyBytes == null)
                    keyBytes = AESEncrypt.RandomKeyBytes();

                cipherValueBytes = AESEncrypt.EncryptFloat(value, keyBytes);
            }
        }

        public AesFloat()
        {
        }

        public AesFloat(float value)
        {
            Value = value;
        }
    }
}