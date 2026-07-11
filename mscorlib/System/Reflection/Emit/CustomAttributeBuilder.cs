using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_CustomAttributeBuilder))]
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public class CustomAttributeBuilder : _CustomAttributeBuilder
	{
		internal ConstructorInfo Ctor
		{
			get
			{
				return this.ctor;
			}
		}

		internal byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] GetBlob(Assembly asmb, ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues);

		internal object Invoke()
		{
			object obj = this.ctor.Invoke(this.args);
			for (int i = 0; i < this.namedFields.Length; i++)
			{
				this.namedFields[i].SetValue(obj, this.fieldValues[i]);
			}
			for (int j = 0; j < this.namedProperties.Length; j++)
			{
				this.namedProperties[j].SetValue(obj, this.propertyValues[j]);
			}
			return obj;
		}

		internal CustomAttributeBuilder(ConstructorInfo con, byte[] binaryAttribute)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (binaryAttribute == null)
			{
				throw new ArgumentNullException("binaryAttribute");
			}
			this.ctor = con;
			this.data = (byte[])binaryAttribute.Clone();
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs)
		{
			this.Initialize(con, constructorArgs, new PropertyInfo[0], new object[0], new FieldInfo[0], new object[0]);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, FieldInfo[] namedFields, object[] fieldValues)
		{
			this.Initialize(con, constructorArgs, new PropertyInfo[0], new object[0], namedFields, fieldValues);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues)
		{
			this.Initialize(con, constructorArgs, namedProperties, propertyValues, new FieldInfo[0], new object[0]);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
		{
			this.Initialize(con, constructorArgs, namedProperties, propertyValues, namedFields, fieldValues);
		}

		private bool IsValidType(Type t)
		{
			if (t.IsArray && t.GetArrayRank() > 1)
			{
				return false;
			}
			if (t is TypeBuilder && t.IsEnum)
			{
				Enum.GetUnderlyingType(t);
			}
			return (!t.IsClass || t.IsArray || t == typeof(object) || t == typeof(Type) || t == typeof(string) || t.Assembly.GetName().Name == "mscorlib") && (!t.IsValueType || t.IsPrimitive || t.IsEnum || (t.Assembly is AssemblyBuilder && t.Assembly.GetName().Name == "mscorlib"));
		}

		private bool IsValidParam(object o, Type paramType)
		{
			Type type = o.GetType();
			if (!this.IsValidType(type))
			{
				return false;
			}
			if (paramType == typeof(object))
			{
				if (type.IsArray && type.GetArrayRank() == 1)
				{
					return this.IsValidType(type.GetElementType());
				}
				if (!type.IsPrimitive && !typeof(Type).IsAssignableFrom(type) && type != typeof(string) && !type.IsEnum)
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsValidValue(Type type, object value)
		{
			if (type.IsValueType && value == null)
			{
				return false;
			}
			if (type.IsArray && type.GetElementType().IsValueType)
			{
				using (IEnumerator enumerator = ((Array)value).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							return false;
						}
					}
				}
				return true;
			}
			return true;
		}

		private void Initialize(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
		{
			this.ctor = con;
			this.args = constructorArgs;
			this.namedProperties = namedProperties;
			this.propertyValues = propertyValues;
			this.namedFields = namedFields;
			this.fieldValues = fieldValues;
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (constructorArgs == null)
			{
				throw new ArgumentNullException("constructorArgs");
			}
			if (namedProperties == null)
			{
				throw new ArgumentNullException("namedProperties");
			}
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if (namedFields == null)
			{
				throw new ArgumentNullException("namedFields");
			}
			if (fieldValues == null)
			{
				throw new ArgumentNullException("fieldValues");
			}
			if (con.GetParametersCount() != constructorArgs.Length)
			{
				throw new ArgumentException("Parameter count does not match passed in argument value count.");
			}
			if (namedProperties.Length != propertyValues.Length)
			{
				throw new ArgumentException("Array lengths must be the same.", "namedProperties, propertyValues");
			}
			if (namedFields.Length != fieldValues.Length)
			{
				throw new ArgumentException("Array lengths must be the same.", "namedFields, fieldValues");
			}
			if ((con.Attributes & MethodAttributes.Static) == MethodAttributes.Static || (con.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private)
			{
				throw new ArgumentException("Cannot have private or static constructor.");
			}
			Type declaringType = this.ctor.DeclaringType;
			int num = 0;
			foreach (FieldInfo fieldInfo in namedFields)
			{
				Type declaringType2 = fieldInfo.DeclaringType;
				if (declaringType != declaringType2 && !declaringType2.IsSubclassOf(declaringType) && !declaringType.IsSubclassOf(declaringType2))
				{
					throw new ArgumentException("Field '" + fieldInfo.Name + "' does not belong to the same class as the constructor");
				}
				if (!this.IsValidType(fieldInfo.FieldType))
				{
					throw new ArgumentException("Field '" + fieldInfo.Name + "' does not have a valid type.");
				}
				if (!CustomAttributeBuilder.IsValidValue(fieldInfo.FieldType, fieldValues[num]))
				{
					throw new ArgumentException("Field " + fieldInfo.Name + " is not a valid value.");
				}
				if (fieldValues[num] != null && !(fieldInfo.FieldType is TypeBuilder) && !fieldInfo.FieldType.IsEnum && !fieldInfo.FieldType.IsInstanceOfType(fieldValues[num]) && !fieldInfo.FieldType.IsArray)
				{
					throw new ArgumentException(string.Concat(new object[] { "Value of field '", fieldInfo.Name, "' does not match field type: ", fieldInfo.FieldType }));
				}
				num++;
			}
			num = 0;
			foreach (PropertyInfo propertyInfo in namedProperties)
			{
				if (!propertyInfo.CanWrite)
				{
					throw new ArgumentException("Property '" + propertyInfo.Name + "' does not have a setter.");
				}
				Type declaringType3 = propertyInfo.DeclaringType;
				if (declaringType != declaringType3 && !declaringType3.IsSubclassOf(declaringType) && !declaringType.IsSubclassOf(declaringType3))
				{
					throw new ArgumentException("Property '" + propertyInfo.Name + "' does not belong to the same class as the constructor");
				}
				if (!this.IsValidType(propertyInfo.PropertyType))
				{
					throw new ArgumentException("Property '" + propertyInfo.Name + "' does not have a valid type.");
				}
				if (!CustomAttributeBuilder.IsValidValue(propertyInfo.PropertyType, propertyValues[num]))
				{
					throw new ArgumentException("Property " + propertyInfo.Name + " is not a valid value.");
				}
				if (propertyValues[num] != null && !(propertyInfo.PropertyType is TypeBuilder) && !propertyInfo.PropertyType.IsEnum && !propertyInfo.PropertyType.IsInstanceOfType(propertyValues[num]) && !propertyInfo.PropertyType.IsArray)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"Value of property '",
						propertyInfo.Name,
						"' does not match property type: ",
						propertyInfo.PropertyType,
						" -> ",
						propertyValues[num]
					}));
				}
				num++;
			}
			num = 0;
			foreach (ParameterInfo parameterInfo in CustomAttributeBuilder.GetParameters(con))
			{
				if (parameterInfo != null)
				{
					Type parameterType = parameterInfo.ParameterType;
					if (!this.IsValidType(parameterType))
					{
						throw new ArgumentException("Parameter " + num + " does not have a valid type.");
					}
					if (!CustomAttributeBuilder.IsValidValue(parameterType, constructorArgs[num]))
					{
						throw new ArgumentException("Parameter " + num + " is not a valid value.");
					}
					if (constructorArgs[num] != null)
					{
						if (!(parameterType is TypeBuilder) && !parameterType.IsEnum && !parameterType.IsInstanceOfType(constructorArgs[num]) && !parameterType.IsArray)
						{
							throw new ArgumentException(string.Concat(new object[]
							{
								"Value of argument ",
								num,
								" does not match parameter type: ",
								parameterType,
								" -> ",
								constructorArgs[num]
							}));
						}
						if (!this.IsValidParam(constructorArgs[num], parameterType))
						{
							throw new ArgumentException("Cannot emit a CustomAttribute with argument of type " + constructorArgs[num].GetType() + ".");
						}
					}
				}
				num++;
			}
			this.data = CustomAttributeBuilder.GetBlob(declaringType.Assembly, con, constructorArgs, namedProperties, propertyValues, namedFields, fieldValues);
		}

		internal static int decode_len(byte[] data, int pos, out int rpos)
		{
			int num;
			if ((data[pos] & 128) == 0)
			{
				num = (int)(data[pos++] & 127);
			}
			else if ((data[pos] & 64) == 0)
			{
				num = ((int)(data[pos] & 63) << 8) + (int)data[pos + 1];
				pos += 2;
			}
			else
			{
				num = ((int)(data[pos] & 31) << 24) + ((int)data[pos + 1] << 16) + ((int)data[pos + 2] << 8) + (int)data[pos + 3];
				pos += 4;
			}
			rpos = pos;
			return num;
		}

		internal static string string_from_bytes(byte[] data, int pos, int len)
		{
			return Encoding.UTF8.GetString(data, pos, len);
		}

		internal string string_arg()
		{
			int num = 2;
			int num2 = CustomAttributeBuilder.decode_len(this.data, num, out num);
			return CustomAttributeBuilder.string_from_bytes(this.data, num, num2);
		}

		internal static UnmanagedMarshal get_umarshal(CustomAttributeBuilder customBuilder, bool is_field)
		{
			byte[] array = customBuilder.Data;
			UnmanagedType unmanagedType = (UnmanagedType)80;
			int num = -1;
			int num2 = -1;
			bool flag = false;
			string text = null;
			Type type = null;
			string text2 = string.Empty;
			int num3 = (int)array[2];
			num3 |= (int)array[3] << 8;
			string fullName = CustomAttributeBuilder.GetParameters(customBuilder.Ctor)[0].ParameterType.FullName;
			int num4 = 6;
			if (fullName == "System.Int16")
			{
				num4 = 4;
			}
			int num5 = (int)array[num4++];
			num5 |= (int)array[num4++] << 8;
			int i = 0;
			while (i < num5)
			{
				num4++;
				if (array[num4++] == 85)
				{
					int num6 = CustomAttributeBuilder.decode_len(array, num4, out num4);
					CustomAttributeBuilder.string_from_bytes(array, num4, num6);
					num4 += num6;
				}
				int num7 = CustomAttributeBuilder.decode_len(array, num4, out num4);
				string text3 = CustomAttributeBuilder.string_from_bytes(array, num4, num7);
				num4 += num7;
				uint num8 = <PrivateImplementationDetails>.ComputeStringHash(text3);
				if (num8 <= 2523910760U)
				{
					if (num8 <= 1554623949U)
					{
						if (num8 != 67206855U)
						{
							if (num8 != 1554623949U)
							{
								goto IL_0381;
							}
							if (!(text3 == "SafeArraySubType"))
							{
								goto IL_0381;
							}
							unmanagedType = (UnmanagedType)((int)array[num4++] | ((int)array[num4++] << 8) | ((int)array[num4++] << 16) | ((int)array[num4++] << 24));
						}
						else
						{
							if (!(text3 == "MarshalCookie"))
							{
								goto IL_0381;
							}
							num7 = CustomAttributeBuilder.decode_len(array, num4, out num4);
							text2 = CustomAttributeBuilder.string_from_bytes(array, num4, num7);
							num4 += num7;
						}
					}
					else if (num8 != 1823397059U)
					{
						if (num8 != 2523910760U)
						{
							goto IL_0381;
						}
						if (!(text3 == "IidParameterIndex"))
						{
							goto IL_0381;
						}
						num4 += 4;
					}
					else
					{
						if (!(text3 == "SizeParamIndex"))
						{
							goto IL_0381;
						}
						num2 = (int)array[num4++] | ((int)array[num4++] << 8);
						flag = true;
					}
				}
				else if (num8 <= 2658176172U)
				{
					if (num8 != 2546868066U)
					{
						if (num8 != 2658176172U)
						{
							goto IL_0381;
						}
						if (!(text3 == "ArraySubType"))
						{
							goto IL_0381;
						}
						unmanagedType = (UnmanagedType)((int)array[num4++] | ((int)array[num4++] << 8) | ((int)array[num4++] << 16) | ((int)array[num4++] << 24));
					}
					else
					{
						if (!(text3 == "MarshalTypeRef"))
						{
							goto IL_0381;
						}
						num7 = CustomAttributeBuilder.decode_len(array, num4, out num4);
						text = CustomAttributeBuilder.string_from_bytes(array, num4, num7);
						type = Type.GetType(text);
						num4 += num7;
					}
				}
				else if (num8 != 2784686469U)
				{
					if (num8 != 3888525279U)
					{
						if (num8 != 4141739223U)
						{
							goto IL_0381;
						}
						if (!(text3 == "SafeArrayUserDefinedSubType"))
						{
							goto IL_0381;
						}
						num7 = CustomAttributeBuilder.decode_len(array, num4, out num4);
						CustomAttributeBuilder.string_from_bytes(array, num4, num7);
						num4 += num7;
					}
					else
					{
						if (!(text3 == "SizeConst"))
						{
							goto IL_0381;
						}
						num = (int)array[num4++] | ((int)array[num4++] << 8) | ((int)array[num4++] << 16) | ((int)array[num4++] << 24);
						flag = true;
					}
				}
				else
				{
					if (!(text3 == "MarshalType"))
					{
						goto IL_0381;
					}
					num7 = CustomAttributeBuilder.decode_len(array, num4, out num4);
					text = CustomAttributeBuilder.string_from_bytes(array, num4, num7);
					num4 += num7;
				}
				i++;
				continue;
				IL_0381:
				throw new Exception("Unknown MarshalAsAttribute field: " + text3);
			}
			UnmanagedType unmanagedType2 = (UnmanagedType)num3;
			if (unmanagedType2 <= UnmanagedType.SafeArray)
			{
				if (unmanagedType2 == UnmanagedType.ByValTStr)
				{
					return UnmanagedMarshal.DefineByValTStr(num);
				}
				if (unmanagedType2 == UnmanagedType.SafeArray)
				{
					return UnmanagedMarshal.DefineSafeArray(unmanagedType);
				}
			}
			else if (unmanagedType2 != UnmanagedType.ByValArray)
			{
				if (unmanagedType2 != UnmanagedType.LPArray)
				{
					if (unmanagedType2 == UnmanagedType.CustomMarshaler)
					{
						return UnmanagedMarshal.DefineCustom(type, text2, text, Guid.Empty);
					}
				}
				else
				{
					if (flag)
					{
						return UnmanagedMarshal.DefineLPArrayInternal(unmanagedType, num, num2);
					}
					return UnmanagedMarshal.DefineLPArray(unmanagedType);
				}
			}
			else
			{
				if (!is_field)
				{
					throw new ArgumentException("Specified unmanaged type is only valid on fields");
				}
				return UnmanagedMarshal.DefineByValArray(num);
			}
			return UnmanagedMarshal.DefineUnmanagedMarshal((UnmanagedType)num3);
		}

		private static Type elementTypeToType(int elementType)
		{
			switch (elementType)
			{
			case 2:
				return typeof(bool);
			case 3:
				return typeof(char);
			case 4:
				return typeof(sbyte);
			case 5:
				return typeof(byte);
			case 6:
				return typeof(short);
			case 7:
				return typeof(ushort);
			case 8:
				return typeof(int);
			case 9:
				return typeof(uint);
			case 10:
				return typeof(long);
			case 11:
				return typeof(ulong);
			case 12:
				return typeof(float);
			case 13:
				return typeof(double);
			case 14:
				return typeof(string);
			default:
				throw new Exception("Unknown element type '" + elementType + "'");
			}
		}

		private static object decode_cattr_value(Type t, byte[] data, int pos, out int rpos)
		{
			TypeCode typeCode = Type.GetTypeCode(t);
			if (typeCode <= TypeCode.Boolean)
			{
				if (typeCode != TypeCode.Object)
				{
					if (typeCode == TypeCode.Boolean)
					{
						rpos = pos + 1;
						return data[pos] != 0;
					}
				}
				else
				{
					int num = (int)data[pos];
					pos++;
					if (num >= 2 && num <= 14)
					{
						return CustomAttributeBuilder.decode_cattr_value(CustomAttributeBuilder.elementTypeToType(num), data, pos, out rpos);
					}
					throw new Exception("Subtype '" + num + "' of type object not yet handled in decode_cattr_value");
				}
			}
			else
			{
				if (typeCode == TypeCode.Int32)
				{
					rpos = pos + 4;
					return (int)data[pos] + ((int)data[pos + 1] << 8) + ((int)data[pos + 2] << 16) + ((int)data[pos + 3] << 24);
				}
				if (typeCode == TypeCode.String)
				{
					if (data[pos] == 255)
					{
						rpos = pos + 1;
						return null;
					}
					int num2 = CustomAttributeBuilder.decode_len(data, pos, out pos);
					rpos = pos + num2;
					return CustomAttributeBuilder.string_from_bytes(data, pos, num2);
				}
			}
			throw new Exception("FIXME: Type " + t + " not yet handled in decode_cattr_value.");
		}

		internal static CustomAttributeBuilder.CustomAttributeInfo decode_cattr(CustomAttributeBuilder customBuilder)
		{
			byte[] array = customBuilder.Data;
			ConstructorInfo constructorInfo = customBuilder.Ctor;
			int num = 0;
			CustomAttributeBuilder.CustomAttributeInfo customAttributeInfo = default(CustomAttributeBuilder.CustomAttributeInfo);
			if (array.Length < 2)
			{
				throw new Exception("Custom attr length is only '" + array.Length + "'");
			}
			if (array[0] != 1 || array[1] != 0)
			{
				throw new Exception("Prolog invalid");
			}
			num = 2;
			ParameterInfo[] parameters = CustomAttributeBuilder.GetParameters(constructorInfo);
			customAttributeInfo.ctor = constructorInfo;
			customAttributeInfo.ctorArgs = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				customAttributeInfo.ctorArgs[i] = CustomAttributeBuilder.decode_cattr_value(parameters[i].ParameterType, array, num, out num);
			}
			int num2 = (int)array[num] + (int)array[num + 1] * 256;
			num += 2;
			customAttributeInfo.namedParamNames = new string[num2];
			customAttributeInfo.namedParamValues = new object[num2];
			for (int j = 0; j < num2; j++)
			{
				int num3 = (int)array[num++];
				int num4 = (int)array[num++];
				string text = null;
				if (num4 == 85)
				{
					int num5 = CustomAttributeBuilder.decode_len(array, num, out num);
					text = CustomAttributeBuilder.string_from_bytes(array, num, num5);
					num += num5;
				}
				int num6 = CustomAttributeBuilder.decode_len(array, num, out num);
				string text2 = CustomAttributeBuilder.string_from_bytes(array, num, num6);
				customAttributeInfo.namedParamNames[j] = text2;
				num += num6;
				if (num3 != 83)
				{
					throw new Exception("Unknown named type: " + num3);
				}
				FieldInfo field = constructorInfo.DeclaringType.GetField(text2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					throw new Exception(string.Concat(new object[] { "Custom attribute type '", constructorInfo.DeclaringType, "' doesn't contain a field named '", text2, "'" }));
				}
				object obj = CustomAttributeBuilder.decode_cattr_value(field.FieldType, array, num, out num);
				if (text != null)
				{
					obj = Enum.ToObject(Type.GetType(text), obj);
				}
				customAttributeInfo.namedParamValues[j] = obj;
			}
			return customAttributeInfo;
		}

		void _CustomAttributeBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private static ParameterInfo[] GetParameters(ConstructorInfo ctor)
		{
			ConstructorBuilder constructorBuilder = ctor as ConstructorBuilder;
			if (constructorBuilder != null)
			{
				return constructorBuilder.GetParametersInternal();
			}
			return ctor.GetParametersInternal();
		}

		private ConstructorInfo ctor;

		private byte[] data;

		private object[] args;

		private PropertyInfo[] namedProperties;

		private object[] propertyValues;

		private FieldInfo[] namedFields;

		private object[] fieldValues;

		internal struct CustomAttributeInfo
		{
			public ConstructorInfo ctor;

			public object[] ctorArgs;

			public string[] namedParamNames;

			public object[] namedParamValues;
		}
	}
}
