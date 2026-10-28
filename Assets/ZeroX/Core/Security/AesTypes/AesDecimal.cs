namespace ZeroX.SecuritySystem
{
    public class AesDecimal
    {
        private byte[] keyBytes = null;
        private byte[] cipherValueBytes = null;

        public decimal Value
        {
            get
            {
                if (cipherValueBytes == null)
                    return 0;

                return AESEncrypt.DecryptDecimal(cipherValueBytes, keyBytes);
            }

            set
            {
                if (keyBytes == null)
                    keyBytes = AESEncrypt.RandomKeyBytes();

                cipherValueBytes = AESEncrypt.EncryptDecimal(value, keyBytes);
            }
        }

        public AesDecimal()
        {
        }

        public AesDecimal(decimal value)
        {
            Value = value;
        }
    }
}