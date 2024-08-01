using Newtonsoft.Json;
using System;

public class TypeJsonConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Type type = (Type)value;
		writer.WriteValue(type.AssemblyQualifiedName);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		string assemblyQualifiedName = (string)reader.Value;
		return Type.GetType(assemblyQualifiedName);
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Type);
	}
}