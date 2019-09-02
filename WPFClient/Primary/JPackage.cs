using System;
using Newtonsoft.Json.Linq;
namespace Primary
{
    class JPackage
    {
        public Type Type { get; set; }
        public JToken Value { get; set; }

        public static JPackage FromValue<T>(T value)
        {
            return new JPackage { Type = typeof(T), Value = JToken.FromObject(value) };
        }

        public static string Serialize(JPackage message)
        {
            return JToken.FromObject(message).ToString();
        }

        public static JPackage Deserialize(string data)
        {
            return JToken.Parse(data).ToObject<JPackage>();
        }
    }
}
