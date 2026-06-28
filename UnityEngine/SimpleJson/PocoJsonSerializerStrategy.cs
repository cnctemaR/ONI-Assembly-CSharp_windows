using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SimpleJson.Reflection;

namespace SimpleJson
{
	[GeneratedCode("simple-json", "1.0.0")]
	internal class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		public PocoJsonSerializerStrategy()
		{
			this.ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
			this.GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(this.GetterValueFactory));
			this.SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(this.SetterValueFactory));
		}

		protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
		{
			return clrPropertyName;
		}

		internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
		{
			return ReflectionUtils.GetContructor(key, (!key.IsArray) ? PocoJsonSerializerStrategy.EmptyTypes : PocoJsonSerializerStrategy.ArrayConstructorParameterTypes);
		}

		internal virtual IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
		{
			IDictionary<string, ReflectionUtils.GetDelegate> dictionary = new Dictionary<string, ReflectionUtils.GetDelegate>();
			foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
			{
				if (propertyInfo.CanRead)
				{
					MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
					if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
					{
						dictionary[this.MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = ReflectionUtils.GetGetMethod(propertyInfo);
					}
				}
			}
			foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
			{
				if (!fieldInfo.IsStatic && fieldInfo.IsPublic)
				{
					dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = ReflectionUtils.GetGetMethod(fieldInfo);
				}
			}
			return dictionary;
		}

		internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
		{
			IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
			foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
			{
				if (propertyInfo.CanWrite)
				{
					MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
					if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
					{
						dictionary[this.MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, ReflectionUtils.GetSetMethod(propertyInfo));
					}
				}
			}
			foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
			{
				if (!fieldInfo.IsInitOnly && !fieldInfo.IsStatic && fieldInfo.IsPublic)
				{
					dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
				}
			}
			return dictionary;
		}

		public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
		{
			return this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output);
		}

		public virtual object DeserializeObject(object value, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			string text = value as string;
			object obj;
			if (type == typeof(Guid) && string.IsNullOrEmpty(text))
			{
				obj = default(Guid);
			}
			else if (value == null)
			{
				obj = null;
			}
			else
			{
				object obj2 = null;
				if (text != null)
				{
					if (text.Length != 0)
					{
						if (type == typeof(DateTime) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
						{
							return DateTime.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
						}
						if (type == typeof(DateTimeOffset) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset)))
						{
							return DateTimeOffset.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
						}
						if (type == typeof(Guid) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
						{
							return new Guid(text);
						}
						return text;
					}
					else
					{
						if (type == typeof(Guid))
						{
							obj2 = default(Guid);
						}
						else if (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
						{
							obj2 = null;
						}
						else
						{
							obj2 = text;
						}
						if (!ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
						{
							return text;
						}
					}
				}
				else if (value is bool)
				{
					return value;
				}
				bool flag = value is long;
				bool flag2 = value is double;
				if ((flag && type == typeof(long)) || (flag2 && type == typeof(double)))
				{
					obj = value;
				}
				else if ((flag2 && type != typeof(double)) || (flag && type != typeof(long)))
				{
					obj2 = ((!typeof(IConvertible).IsAssignableFrom(type)) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
					if (ReflectionUtils.IsNullableType(type))
					{
						obj = ReflectionUtils.ToNullableType(obj2, type);
					}
					else
					{
						obj = obj2;
					}
				}
				else
				{
					IDictionary<string, object> dictionary = value as IDictionary<string, object>;
					if (dictionary != null)
					{
						IDictionary<string, object> dictionary2 = dictionary;
						if (ReflectionUtils.IsTypeDictionary(type))
						{
							Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
							Type type2 = genericTypeArguments[0];
							Type type3 = genericTypeArguments[1];
							Type type4 = typeof(Dictionary<, >).MakeGenericType(new Type[] { type2, type3 });
							IDictionary dictionary3 = (IDictionary)this.ConstructorCache[type4](null);
							foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
							{
								dictionary3.Add(keyValuePair.Key, this.DeserializeObject(keyValuePair.Value, type3));
							}
							obj2 = dictionary3;
						}
						else if (type == typeof(object))
						{
							obj2 = value;
						}
						else
						{
							obj2 = this.ConstructorCache[type](null);
							foreach (KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> keyValuePair2 in this.SetCache[type])
							{
								object obj3;
								if (dictionary2.TryGetValue(keyValuePair2.Key, out obj3))
								{
									obj3 = this.DeserializeObject(obj3, keyValuePair2.Value.Key);
									keyValuePair2.Value.Value(obj2, obj3);
								}
							}
						}
					}
					else
					{
						IList<object> list = value as IList<object>;
						if (list != null)
						{
							IList<object> list2 = list;
							IList list3 = null;
							if (type.IsArray)
							{
								list3 = (IList)this.ConstructorCache[type](new object[] { list2.Count });
								int num = 0;
								foreach (object obj4 in list2)
								{
									list3[num++] = this.DeserializeObject(obj4, type.GetElementType());
								}
							}
							else if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type))
							{
								Type type5 = ReflectionUtils.GetGenericTypeArguments(type)[0];
								Type type6 = typeof(List<>).MakeGenericType(new Type[] { type5 });
								list3 = (IList)this.ConstructorCache[type6](new object[] { list2.Count });
								foreach (object obj5 in list2)
								{
									list3.Add(this.DeserializeObject(obj5, type5));
								}
							}
							obj2 = list3;
						}
					}
					obj = obj2;
				}
			}
			return obj;
		}

		protected virtual object SerializeEnum(Enum p)
		{
			return Convert.ToDouble(p, CultureInfo.InvariantCulture);
		}

		protected virtual bool TrySerializeKnownTypes(object input, out object output)
		{
			bool flag = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is DateTimeOffset)
			{
				output = ((DateTimeOffset)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else
			{
				Enum @enum = input as Enum;
				if (@enum != null)
				{
					output = this.SerializeEnum(@enum);
				}
				else
				{
					flag = false;
					output = null;
				}
			}
			return flag;
		}

		protected virtual bool TrySerializeUnknownTypes(object input, out object output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			output = null;
			Type type = input.GetType();
			bool flag;
			if (type.FullName == null)
			{
				flag = false;
			}
			else
			{
				IDictionary<string, object> dictionary = new JsonObject();
				IDictionary<string, ReflectionUtils.GetDelegate> dictionary2 = this.GetCache[type];
				foreach (KeyValuePair<string, ReflectionUtils.GetDelegate> keyValuePair in dictionary2)
				{
					if (keyValuePair.Value != null)
					{
						dictionary.Add(this.MapClrMemberNameToJsonFieldName(keyValuePair.Key), keyValuePair.Value(input));
					}
				}
				output = dictionary;
				flag = true;
			}
			return flag;
		}

		internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;

		internal IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>> GetCache;

		internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;

		internal static readonly Type[] EmptyTypes = new Type[0];

		internal static readonly Type[] ArrayConstructorParameterTypes = new Type[] { typeof(int) };

		private static readonly string[] Iso8601Format = new string[] { "yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z", "yyyy-MM-dd\\THH:mm:ss\\Z", "yyyy-MM-dd\\THH:mm:ssK" };
	}
}
