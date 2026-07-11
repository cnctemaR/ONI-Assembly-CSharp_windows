using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	internal static class MonoCustomAttrs
	{
		private static bool IsUserCattrProvider(object obj)
		{
			Type type = obj as Type;
			if (type is RuntimeType || type is TypeBuilder)
			{
				return false;
			}
			if (obj is Type)
			{
				return true;
			}
			if (MonoCustomAttrs.corlib == null)
			{
				MonoCustomAttrs.corlib = typeof(int).Assembly;
			}
			return obj.GetType().Assembly != MonoCustomAttrs.corlib;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object[] GetCustomAttributesInternal(ICustomAttributeProvider obj, Type attributeType, bool pseudoAttrs);

		internal static object[] GetPseudoCustomAttributes(ICustomAttributeProvider obj, Type attributeType)
		{
			object[] array = null;
			if (obj is MonoMethod)
			{
				array = ((MonoMethod)obj).GetPseudoCustomAttributes();
			}
			else if (obj is FieldInfo)
			{
				array = ((FieldInfo)obj).GetPseudoCustomAttributes();
			}
			else if (obj is ParameterInfo)
			{
				array = ((ParameterInfo)obj).GetPseudoCustomAttributes();
			}
			else if (obj is Type)
			{
				array = MonoCustomAttrs.GetPseudoCustomAttributes((Type)obj);
			}
			if (attributeType != null && array != null)
			{
				int i = 0;
				while (i < array.Length)
				{
					if (attributeType.IsAssignableFrom(array[i].GetType()))
					{
						if (array.Length == 1)
						{
							return array;
						}
						return new object[] { array[i] };
					}
					else
					{
						i++;
					}
				}
				return EmptyArray<object>.Value;
			}
			return array;
		}

		private static object[] GetPseudoCustomAttributes(Type type)
		{
			int num = 0;
			TypeAttributes attributes = type.Attributes;
			if ((attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if ((attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if ((attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				array[num++] = new SerializableAttribute();
			}
			if ((attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				array[num++] = new ComImportAttribute();
			}
			return array;
		}

		internal static object[] GetCustomAttributesBase(ICustomAttributeProvider obj, Type attributeType, bool inheritedOnly)
		{
			object[] array;
			if (MonoCustomAttrs.IsUserCattrProvider(obj))
			{
				array = obj.GetCustomAttributes(attributeType, true);
			}
			else
			{
				array = MonoCustomAttrs.GetCustomAttributesInternal(obj, attributeType, false);
			}
			if (!inheritedOnly)
			{
				object[] pseudoCustomAttributes = MonoCustomAttrs.GetPseudoCustomAttributes(obj, attributeType);
				if (pseudoCustomAttributes != null)
				{
					object[] array2 = new object[array.Length + pseudoCustomAttributes.Length];
					Array.Copy(array, array2, array.Length);
					Array.Copy(pseudoCustomAttributes, 0, array2, array.Length, pseudoCustomAttributes.Length);
					return array2;
				}
			}
			return array;
		}

		internal static object[] GetCustomAttributes(ICustomAttributeProvider obj, Type attributeType, bool inherit)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			if (attributeType == typeof(MonoCustomAttrs))
			{
				attributeType = null;
			}
			object[] array = MonoCustomAttrs.GetCustomAttributesBase(obj, attributeType, false);
			if (!inherit && array.Length == 1)
			{
				if (array[0] == null)
				{
					throw new CustomAttributeFormatException("Invalid custom attribute format");
				}
				object[] array2;
				if (attributeType != null)
				{
					if (attributeType.IsAssignableFrom(array[0].GetType()))
					{
						array2 = (object[])Array.CreateInstance(attributeType, 1);
						array2[0] = array[0];
					}
					else
					{
						array2 = (object[])Array.CreateInstance(attributeType, 0);
					}
				}
				else
				{
					array2 = (object[])Array.CreateInstance(array[0].GetType(), 1);
					array2[0] = array[0];
				}
				return array2;
			}
			else
			{
				if (inherit && MonoCustomAttrs.GetBase(obj) == null)
				{
					inherit = false;
				}
				if (attributeType != null && attributeType.IsSealed && inherit && !MonoCustomAttrs.RetrieveAttributeUsage(attributeType).Inherited)
				{
					inherit = false;
				}
				int num = Math.Max(array.Length, 16);
				ICustomAttributeProvider customAttributeProvider = obj;
				List<object> list;
				object[] array4;
				if (inherit)
				{
					Dictionary<Type, MonoCustomAttrs.AttributeInfo> dictionary = new Dictionary<Type, MonoCustomAttrs.AttributeInfo>(num);
					int num2 = 0;
					list = new List<object>(num);
					for (;;)
					{
						foreach (object obj2 in array)
						{
							if (obj2 == null)
							{
								goto Block_22;
							}
							Type type = obj2.GetType();
							if (!(attributeType != null) || attributeType.IsAssignableFrom(type))
							{
								MonoCustomAttrs.AttributeInfo attributeInfo;
								AttributeUsageAttribute attributeUsageAttribute;
								if (dictionary.TryGetValue(type, out attributeInfo))
								{
									attributeUsageAttribute = attributeInfo.Usage;
								}
								else
								{
									attributeUsageAttribute = MonoCustomAttrs.RetrieveAttributeUsage(type);
								}
								if ((num2 == 0 || attributeUsageAttribute.Inherited) && (attributeUsageAttribute.AllowMultiple || attributeInfo == null || (attributeInfo != null && attributeInfo.InheritanceLevel == num2)))
								{
									list.Add(obj2);
								}
								if (attributeInfo == null)
								{
									dictionary.Add(type, new MonoCustomAttrs.AttributeInfo(attributeUsageAttribute, num2));
								}
							}
						}
						if ((customAttributeProvider = MonoCustomAttrs.GetBase(customAttributeProvider)) != null)
						{
							num2++;
							array = MonoCustomAttrs.GetCustomAttributesBase(customAttributeProvider, attributeType, true);
						}
						if (!inherit || customAttributeProvider == null)
						{
							goto IL_02C7;
						}
					}
					Block_22:
					throw new CustomAttributeFormatException("Invalid custom attribute format");
					IL_02C7:
					if (attributeType == null || attributeType.IsValueType)
					{
						array4 = new Attribute[list.Count];
					}
					else
					{
						array4 = Array.CreateInstance(attributeType, list.Count) as object[];
					}
					list.CopyTo(array4, 0);
					return array4;
				}
				if (attributeType == null)
				{
					object[] array3 = array;
					for (int i = 0; i < array3.Length; i++)
					{
						if (array3[i] == null)
						{
							throw new CustomAttributeFormatException("Invalid custom attribute format");
						}
					}
					Attribute[] array5 = new Attribute[array.Length];
					array.CopyTo(array5, 0);
					return array5;
				}
				list = new List<object>(num);
				foreach (object obj3 in array)
				{
					if (obj3 == null)
					{
						throw new CustomAttributeFormatException("Invalid custom attribute format");
					}
					Type type2 = obj3.GetType();
					if (!(attributeType != null) || attributeType.IsAssignableFrom(type2))
					{
						list.Add(obj3);
					}
				}
				if (attributeType == null || attributeType.IsValueType)
				{
					array4 = new Attribute[list.Count];
				}
				else
				{
					array4 = Array.CreateInstance(attributeType, list.Count) as object[];
				}
				list.CopyTo(array4, 0);
				return array4;
			}
		}

		internal static object[] GetCustomAttributes(ICustomAttributeProvider obj, bool inherit)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!inherit)
			{
				return (object[])MonoCustomAttrs.GetCustomAttributesBase(obj, null, false).Clone();
			}
			return MonoCustomAttrs.GetCustomAttributes(obj, typeof(MonoCustomAttrs), inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomAttributeData[] GetCustomAttributesDataInternal(ICustomAttributeProvider obj);

		internal static IList<CustomAttributeData> GetCustomAttributesData(ICustomAttributeProvider obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return Array.AsReadOnly<CustomAttributeData>(MonoCustomAttrs.GetCustomAttributesDataInternal(obj));
		}

		internal static bool IsDefined(ICustomAttributeProvider obj, Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			AttributeUsageAttribute attributeUsageAttribute = null;
			while (!MonoCustomAttrs.IsUserCattrProvider(obj))
			{
				if (MonoCustomAttrs.IsDefinedInternal(obj, attributeType))
				{
					return true;
				}
				object[] pseudoCustomAttributes = MonoCustomAttrs.GetPseudoCustomAttributes(obj, attributeType);
				if (pseudoCustomAttributes != null)
				{
					for (int i = 0; i < pseudoCustomAttributes.Length; i++)
					{
						if (attributeType.IsAssignableFrom(pseudoCustomAttributes[i].GetType()))
						{
							return true;
						}
					}
				}
				if (attributeUsageAttribute == null)
				{
					if (!inherit)
					{
						return false;
					}
					attributeUsageAttribute = MonoCustomAttrs.RetrieveAttributeUsage(attributeType);
					if (!attributeUsageAttribute.Inherited)
					{
						return false;
					}
				}
				obj = MonoCustomAttrs.GetBase(obj);
				if (obj == null)
				{
					return false;
				}
			}
			return obj.IsDefined(attributeType, inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsDefinedInternal(ICustomAttributeProvider obj, Type AttributeType);

		private static PropertyInfo GetBasePropertyDefinition(MonoProperty property)
		{
			MethodInfo methodInfo = property.GetGetMethod(true);
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				methodInfo = property.GetSetMethod(true);
			}
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				return null;
			}
			MethodInfo baseMethod = methodInfo.GetBaseMethod();
			if (!(baseMethod != null) || !(baseMethod != methodInfo))
			{
				return null;
			}
			ParameterInfo[] indexParameters = property.GetIndexParameters();
			if (indexParameters != null && indexParameters.Length != 0)
			{
				Type[] array = new Type[indexParameters.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = indexParameters[i].ParameterType;
				}
				return baseMethod.DeclaringType.GetProperty(property.Name, property.PropertyType, array);
			}
			return baseMethod.DeclaringType.GetProperty(property.Name, property.PropertyType);
		}

		private static EventInfo GetBaseEventDefinition(MonoEvent evt)
		{
			MethodInfo methodInfo = evt.GetAddMethod(true);
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				methodInfo = evt.GetRaiseMethod(true);
			}
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				methodInfo = evt.GetRemoveMethod(true);
			}
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				return null;
			}
			MethodInfo baseMethod = methodInfo.GetBaseMethod();
			if (baseMethod != null && baseMethod != methodInfo)
			{
				BindingFlags bindingFlags = (methodInfo.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);
				bindingFlags |= (methodInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
				return baseMethod.DeclaringType.GetEvent(evt.Name, bindingFlags);
			}
			return null;
		}

		private static ICustomAttributeProvider GetBase(ICustomAttributeProvider obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is Type)
			{
				return ((Type)obj).BaseType;
			}
			MethodInfo methodInfo = null;
			if (obj is MonoProperty)
			{
				return MonoCustomAttrs.GetBasePropertyDefinition((MonoProperty)obj);
			}
			if (obj is MonoEvent)
			{
				return MonoCustomAttrs.GetBaseEventDefinition((MonoEvent)obj);
			}
			if (obj is MonoMethod)
			{
				methodInfo = (MethodInfo)obj;
			}
			if (methodInfo == null || !methodInfo.IsVirtual)
			{
				return null;
			}
			MethodInfo baseMethod = methodInfo.GetBaseMethod();
			if (baseMethod == methodInfo)
			{
				return null;
			}
			return baseMethod;
		}

		private static AttributeUsageAttribute RetrieveAttributeUsageNoCache(Type attributeType)
		{
			if (attributeType == typeof(AttributeUsageAttribute))
			{
				return new AttributeUsageAttribute(AttributeTargets.Class);
			}
			AttributeUsageAttribute attributeUsageAttribute = null;
			object[] customAttributes = MonoCustomAttrs.GetCustomAttributes(attributeType, typeof(AttributeUsageAttribute), false);
			if (customAttributes.Length == 0)
			{
				if (attributeType.BaseType != null)
				{
					attributeUsageAttribute = MonoCustomAttrs.RetrieveAttributeUsage(attributeType.BaseType);
				}
				if (attributeUsageAttribute != null)
				{
					return attributeUsageAttribute;
				}
				return MonoCustomAttrs.DefaultAttributeUsage;
			}
			else
			{
				if (customAttributes.Length > 1)
				{
					throw new FormatException("Duplicate AttributeUsageAttribute cannot be specified on an attribute type.");
				}
				return (AttributeUsageAttribute)customAttributes[0];
			}
		}

		private static AttributeUsageAttribute RetrieveAttributeUsage(Type attributeType)
		{
			AttributeUsageAttribute attributeUsageAttribute = null;
			if (MonoCustomAttrs.usage_cache == null)
			{
				MonoCustomAttrs.usage_cache = new Dictionary<Type, AttributeUsageAttribute>();
			}
			if (MonoCustomAttrs.usage_cache.TryGetValue(attributeType, out attributeUsageAttribute))
			{
				return attributeUsageAttribute;
			}
			attributeUsageAttribute = MonoCustomAttrs.RetrieveAttributeUsageNoCache(attributeType);
			MonoCustomAttrs.usage_cache[attributeType] = attributeUsageAttribute;
			return attributeUsageAttribute;
		}

		private static Assembly corlib;

		[ThreadStatic]
		private static Dictionary<Type, AttributeUsageAttribute> usage_cache;

		private static readonly AttributeUsageAttribute DefaultAttributeUsage = new AttributeUsageAttribute(AttributeTargets.All);

		private class AttributeInfo
		{
			public AttributeInfo(AttributeUsageAttribute usage, int inheritanceLevel)
			{
				this._usage = usage;
				this._inheritanceLevel = inheritanceLevel;
			}

			public AttributeUsageAttribute Usage
			{
				get
				{
					return this._usage;
				}
			}

			public int InheritanceLevel
			{
				get
				{
					return this._inheritanceLevel;
				}
			}

			private AttributeUsageAttribute _usage;

			private int _inheritanceLevel;
		}
	}
}
