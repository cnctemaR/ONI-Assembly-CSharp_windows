using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace KSerialization
{
	public class SerializationTemplate
	{
		public SerializationTemplate(Type type)
		{
			this.serializableType = type;
			this.typeInfo = Manager.GetTypeInfo(type);
			type.GetSerializationMethods(typeof(OnSerializingAttribute), typeof(OnSerializedAttribute), out this.onSerializing, out this.onSerialized);
			MemberSerialization serializationConfig = this.GetSerializationConfig(type);
			MemberSerialization memberSerialization = serializationConfig;
			if (memberSerialization != MemberSerialization.OptOut)
			{
				if (memberSerialization == MemberSerialization.OptIn)
				{
					while (type != typeof(object))
					{
						this.AddOptInFields(type);
						this.AddOptInProperties(type);
						type = type.BaseType;
					}
				}
			}
			else
			{
				while (type != typeof(object))
				{
					this.AddPublicFields(type);
					this.AddPublicProperties(type);
					type = type.BaseType;
				}
			}
		}

		private MemberSerialization GetSerializationConfig(Type type)
		{
			MemberSerialization memberSerialization = MemberSerialization.Invalid;
			Type type2 = null;
			while (type != typeof(object))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(SerializationConfig), false);
				object[] array = customAttributes;
				int i = 0;
				while (i < array.Length)
				{
					Attribute attribute = (Attribute)array[i];
					if (attribute is SerializationConfig)
					{
						SerializationConfig serializationConfig = attribute as SerializationConfig;
						if (serializationConfig.MemberSerialization != memberSerialization && memberSerialization != MemberSerialization.Invalid)
						{
							string text = "Found conflicting serialization configurations on type " + type2.ToString() + " and " + type.ToString();
							Output.LogError(new object[] { text });
							throw new ArgumentException(text);
						}
						memberSerialization = serializationConfig.MemberSerialization;
						type2 = type.BaseType;
						break;
					}
					else
					{
						i++;
					}
				}
				type = type.BaseType;
			}
			if (memberSerialization == MemberSerialization.Invalid)
			{
				memberSerialization = MemberSerialization.OptOut;
			}
			return memberSerialization;
		}

		public override string ToString()
		{
			string text = "Template: " + this.serializableType.ToString() + "\n";
			foreach (SerializationTemplate.SerializationField serializationField in this.serializableFields)
			{
				text = text + "\t" + serializationField.ToString() + "\n";
			}
			return text;
		}

		private void AddPublicFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				this.AddValidField(fieldInfo);
			}
		}

		private void AddOptInFields(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(false);
				foreach (object obj in customAttributes)
				{
					if (obj != null && obj is Serialize)
					{
						this.AddValidField(fieldInfo);
					}
				}
			}
		}

		private void AddValidField(FieldInfo field)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(NonSerializedAttribute), false);
			if (customAttributes == null || customAttributes.Length <= 0)
			{
				this.serializableFields.Add(new SerializationTemplate.SerializationField
				{
					field = field,
					typeInfo = Manager.GetTypeInfo(field.FieldType)
				});
			}
		}

		private void AddPublicProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo propertyInfo in properties)
			{
				this.AddValidProperty(propertyInfo);
			}
		}

		private void AddOptInProperties(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(false);
				foreach (object obj in customAttributes)
				{
					if (obj != null && obj is Serialize)
					{
						this.AddValidProperty(propertyInfo);
					}
				}
			}
		}

		private void AddValidProperty(PropertyInfo property)
		{
			object[] customAttributes = property.GetCustomAttributes(typeof(NonSerializedAttribute), false);
			if (customAttributes == null || customAttributes.Length <= 0)
			{
				MethodInfo setMethod = property.GetSetMethod();
				if (setMethod != null)
				{
					this.serializableProperties.Add(new SerializationTemplate.SerializationProperty
					{
						property = property,
						typeInfo = Manager.GetTypeInfo(property.PropertyType)
					});
				}
			}
		}

		public void SerializeTemplate(BinaryWriter writer)
		{
			writer.Write(this.serializableFields.Count);
			writer.Write(this.serializableProperties.Count);
			foreach (SerializationTemplate.SerializationField serializationField in this.serializableFields)
			{
				writer.WriteKleiString(serializationField.field.Name);
				Type fieldType = serializationField.field.FieldType;
				this.WriteType(writer, fieldType);
			}
			foreach (SerializationTemplate.SerializationProperty serializationProperty in this.serializableProperties)
			{
				writer.WriteKleiString(serializationProperty.property.Name);
				Type propertyType = serializationProperty.property.PropertyType;
				this.WriteType(writer, propertyType);
			}
		}

		private void WriteType(BinaryWriter writer, Type type)
		{
			SerializationTypeInfo serializationTypeInfo = Helper.EncodeSerializationType(type);
			writer.Write((byte)serializationTypeInfo);
			if (type.IsGenericType)
			{
				if (Helper.IsUserDefinedType(serializationTypeInfo))
				{
					writer.WriteKleiString(type.GetKTypeString());
				}
				Type[] genericArguments = type.GetGenericArguments();
				writer.Write((byte)genericArguments.Length);
				for (int i = 0; i < genericArguments.Length; i++)
				{
					this.WriteType(writer, genericArguments[i]);
				}
			}
			else if (Helper.IsArray(serializationTypeInfo))
			{
				Type elementType = type.GetElementType();
				this.WriteType(writer, elementType);
			}
			else if (type.IsEnum || Helper.IsUserDefinedType(serializationTypeInfo))
			{
				writer.WriteKleiString(type.GetKTypeString());
			}
		}

		public void SerializeData(object obj, BinaryWriter writer)
		{
			if (this.onSerializing != null)
			{
				this.onSerializing.Invoke(obj, null);
			}
			foreach (SerializationTemplate.SerializationField serializationField in this.serializableFields)
			{
				try
				{
					object value = serializationField.field.GetValue(obj);
					writer.WriteValue(serializationField.typeInfo, value);
				}
				catch (Exception ex)
				{
					string text = string.Format("Error occurred while serializing field {0} on template {1}", serializationField.field.Name, this.serializableType.Name);
					Output.LogError(new object[] { text });
					throw new ArgumentException(text, ex);
				}
			}
			foreach (SerializationTemplate.SerializationProperty serializationProperty in this.serializableProperties)
			{
				try
				{
					object value2 = serializationProperty.property.GetValue(obj, null);
					writer.WriteValue(serializationProperty.typeInfo, value2);
				}
				catch (Exception ex2)
				{
					string text2 = string.Format("Error occurred while serializing property {0} on template {1}", serializationProperty.property.Name, this.serializableType.Name);
					Output.LogError(new object[] { text2 });
					throw new ArgumentException(text2, ex2);
				}
			}
			if (this.onSerialized != null)
			{
				this.onSerialized.Invoke(obj, null);
			}
		}

		public Type serializableType;

		public TypeInfo typeInfo;

		public List<SerializationTemplate.SerializationField> serializableFields = new List<SerializationTemplate.SerializationField>();

		public List<SerializationTemplate.SerializationProperty> serializableProperties = new List<SerializationTemplate.SerializationProperty>();

		public MethodInfo onSerializing;

		public MethodInfo onSerialized;

		public struct SerializationField
		{
			public FieldInfo field;

			public TypeInfo typeInfo;
		}

		public struct SerializationProperty
		{
			public PropertyInfo property;

			public TypeInfo typeInfo;
		}
	}
}
