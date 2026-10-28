using System;

namespace ZeroX.JsonSystem
{
    public static class ZJsonConst
    {
        public const string EM_JsonNodeIsNotObject = "JsonNode is not object";
        public const string EM_JsonNodeIsNotList = "JsonNode is not list";
        public const string EM_JsonNodeIsNotScalar = "JsonNode is not scalar";
        public const string EM_IncorrectJsonStringFormat = "Incorrect Json string format";
        
        public static readonly Exception EX_JsonNodeIsNotScalar = new Exception(EM_JsonNodeIsNotScalar);
    }
}