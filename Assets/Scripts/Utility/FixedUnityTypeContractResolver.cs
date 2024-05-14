using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utility
{
    public class FixedUnityTypeContractResolver : Newtonsoft.Json.UnityConverters.UnityTypeContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Ignored && member.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() != null) 
                jsonProperty.Ignored = true;

            return jsonProperty;
        }
    }
}