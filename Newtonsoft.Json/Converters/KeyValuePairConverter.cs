using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	public class KeyValuePairConverter : JsonConverter
	{
		private static ReflectionObject InitializeReflectionObject(Type t)
		{
			IList<Type> genericArguments = t.GetGenericArguments();
			Type type = genericArguments[0];
			Type type2 = genericArguments[1];
			return ReflectionObject.Create(t, t.GetConstructor(new Type[] { type, type2 }), new string[] { "Key", "Value" });
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(value.GetType());
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			writer.WriteStartObject();
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Key") : "Key");
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Key"), reflectionObject.GetType("Key"));
			writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Value") : "Value");
			serializer.Serialize(writer, reflectionObject.GetValue(value, "Value"), reflectionObject.GetType("Value"));
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = ReflectionUtils.IsNullableType(objectType);
			Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
			ReflectionObject reflectionObject = KeyValuePairConverter.ReflectionObjectPerType.Get(type);
			if (reader.TokenType != JsonToken.Null)
			{
				object obj = null;
				object obj2 = null;
				KeyValuePairConverter.ReadAndAssert(reader);
				while (reader.TokenType == JsonToken.PropertyName)
				{
					string text = reader.Value.ToString();
					if (string.Equals(text, "Key", StringComparison.OrdinalIgnoreCase))
					{
						KeyValuePairConverter.ReadAndAssert(reader);
						obj = serializer.Deserialize(reader, reflectionObject.GetType("Key"));
					}
					else if (string.Equals(text, "Value", StringComparison.OrdinalIgnoreCase))
					{
						KeyValuePairConverter.ReadAndAssert(reader);
						obj2 = serializer.Deserialize(reader, reflectionObject.GetType("Value"));
					}
					else
					{
						reader.Skip();
					}
					KeyValuePairConverter.ReadAndAssert(reader);
				}
				return reflectionObject.Creator(new object[] { obj, obj2 });
			}
			if (!flag)
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			Type type = (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType);
			return type.IsValueType() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
		}

		private static void ReadAndAssert(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw JsonSerializationException.Create(reader, "Unexpected end when reading KeyValuePair.");
			}
		}

		private const string KeyName = "Key";

		private const string ValueName = "Value";

		private static readonly ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(new Func<Type, ReflectionObject>(KeyValuePairConverter.InitializeReflectionObject));
	}
}
