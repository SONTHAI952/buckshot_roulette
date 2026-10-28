namespace ZeroX.DataTableSystem.SoTableSystem
{
    public static class SOTableConst
    {
        public const string FIELD_NAME_ROW_TYPE = "[row_type]";
        
        public static bool IsFieldNameRowType(string str)
        {
            str = str.ToLower().Replace("_", "").Replace(" ", "");
            return str == "[rowtype]";
        }
    }
}