namespace ZeroX.JsonSystem
{
    public interface IZJsonCustomSerializable
    {
        /// <summary>
        /// Phải trả về ZJsonObject hoặc null (không được trả ZJsonList hay ZJsonScalar, vì khi đọc lại sẽ không khớp).
        /// <para>Trả null thì field sẽ được ghi thành null trong json</para>
        /// </summary>
        public ZJsonNode ToJsonNode();

        public void FromJsonNode(ZJsonNode jsonNode);
    }
}