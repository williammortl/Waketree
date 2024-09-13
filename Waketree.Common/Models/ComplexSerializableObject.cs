using Newtonsoft.Json;

namespace Waketree.Common.Models
{
    public class ComplexSerializableObject
    {
        public string TypeOfValue { get; set;  }
        public string Value { get; set;  }

        public ComplexSerializableObject() 
        {
            this.TypeOfValue = string.Empty;
            this.Value = string.Empty;
        }

        public ComplexSerializableObject (object value)
        {
            var typeOfVal = value.GetType();
            this.TypeOfValue = typeOfVal.AssemblyQualifiedName.ToString();
            this.Value = JsonConvert.SerializeObject(value, typeOfVal, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public object? DeserializeObject()
        {
            var typeOfVal = Type.GetType(this.TypeOfValue);
            var retVal = JsonConvert.DeserializeObject(this.Value, typeOfVal, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            return retVal;
        }
    }
}
