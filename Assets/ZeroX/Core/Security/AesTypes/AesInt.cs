namespace ZeroX.SecuritySystem
{
    public class AesInt
    {
        private byte[] keyBytes = null;
        private byte[] cipherValueBytes = null;

        public int Value
        {
            get
            {
                if (cipherValueBytes == null)
                    return 0;

                return AESEncrypt.DecryptInt(cipherValueBytes, keyBytes);
            }

            set
            {
                if (keyBytes == null)
                    keyBytes = AESEncrypt.RandomKeyBytes();

                cipherValueBytes = AESEncrypt.EncryptInt(value, keyBytes);
            }
        }

        public AesInt()
        {
        }

        public AesInt(int value)
        {
            Value = value;
        }
    }
}