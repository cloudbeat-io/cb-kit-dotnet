using Newtonsoft.Json.Serialization;
using System;

namespace CloudBeat.Kit.Common.Json
{
    /// <summary>
    /// CamelCase resolver which preserves Dictionary key casing
    /// </summary>
    public class CamelCasePreserveDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);
            contract.DictionaryKeyResolver = propertyName => propertyName;
            return contract;
        }
    }
}
