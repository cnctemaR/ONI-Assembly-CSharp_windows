using System;
using System.Globalization;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Properties
{
	public static class TypeConversion
	{
		static TypeConversion()
		{
			TypeConversion.PrimitiveConverters.Register();
		}

		public static void Register<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
		{
			TypeConversion.s_GlobalConverters.Register(typeof(TSource), typeof(TDestination), converter);
		}

		public static TDestination Convert<TSource, TDestination>(ref TSource value)
		{
			TDestination tdestination;
			bool flag = !TypeConversion.TryConvert<TSource, TDestination>(ref value, out tdestination);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("TypeConversion no converter has been registered for SrcType=[{0}] to DstType=[{1}]", typeof(TSource), typeof(TDestination)));
			}
			return tdestination;
		}

		public unsafe static bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
		{
			Delegate @delegate;
			bool flag = TypeConversion.s_GlobalConverters.TryGetConverter(typeof(TSource), typeof(TDestination), out @delegate);
			bool flag2;
			if (flag)
			{
				destination = ((TypeConverter<TSource, TDestination>)@delegate)(ref source);
				flag2 = true;
			}
			else
			{
				bool flag3 = typeof(TSource).IsValueType && typeof(TSource) == typeof(TDestination);
				if (flag3)
				{
					destination = *UnsafeUtility.As<TSource, TDestination>(ref source);
					flag2 = true;
				}
				else
				{
					bool isNullable = TypeTraits<TDestination>.IsNullable;
					if (isNullable)
					{
						bool flag4 = TypeTraits<TSource>.IsNullable && Nullable.GetUnderlyingType(typeof(TDestination)) != Nullable.GetUnderlyingType(typeof(TSource));
						if (flag4)
						{
							destination = default(TDestination);
							flag2 = false;
						}
						else
						{
							Type underlyingType = Nullable.GetUnderlyingType(typeof(TDestination));
							bool isEnum = underlyingType.IsEnum;
							if (isEnum)
							{
								Type underlyingType2 = Enum.GetUnderlyingType(underlyingType);
								object obj = global::System.Convert.ChangeType(source, underlyingType2);
								destination = (TDestination)((object)Enum.ToObject(underlyingType, obj));
								flag2 = true;
							}
							else
							{
								bool flag5 = source == null;
								if (flag5)
								{
									destination = default(TDestination);
									flag2 = true;
								}
								else
								{
									destination = (TDestination)((object)global::System.Convert.ChangeType(source, underlyingType));
									flag2 = true;
								}
							}
						}
					}
					else
					{
						bool flag6 = TypeTraits<TSource>.IsNullable && typeof(TDestination) == Nullable.GetUnderlyingType(typeof(TSource));
						if (flag6)
						{
							bool flag7 = source == null;
							if (flag7)
							{
								destination = default(TDestination);
								flag2 = false;
							}
							else
							{
								destination = (TDestination)((object)source);
								flag2 = true;
							}
						}
						else
						{
							bool isUnityObject = TypeTraits<TDestination>.IsUnityObject;
							if (isUnityObject)
							{
								bool flag8 = TypeConversion.TryConvertToUnityEngineObject<TSource, TDestination>(source, out destination);
								if (flag8)
								{
									return true;
								}
							}
							bool isEnum2 = TypeTraits<TDestination>.IsEnum;
							if (isEnum2)
							{
								bool flag9 = typeof(TSource) == typeof(string);
								if (flag9)
								{
									try
									{
										destination = (TDestination)((object)Enum.Parse(typeof(TDestination), (string)((object)source)));
									}
									catch (ArgumentException)
									{
										destination = default(TDestination);
										return false;
									}
									return true;
								}
								bool flag10 = TypeConversion.IsNumericType(typeof(TSource));
								if (flag10)
								{
									destination = *UnsafeUtility.As<TSource, TDestination>(ref source);
									return true;
								}
							}
							TSource tsource = source;
							TDestination tdestination;
							bool flag11;
							if (tsource is TDestination)
							{
								tdestination = tsource as TDestination;
								flag11 = true;
							}
							else
							{
								flag11 = false;
							}
							bool flag12 = flag11;
							if (flag12)
							{
								destination = tdestination;
								flag2 = true;
							}
							else
							{
								bool flag13 = typeof(TDestination).IsAssignableFrom(typeof(TSource));
								if (flag13)
								{
									destination = (TDestination)((object)source);
									flag2 = true;
								}
								else
								{
									destination = default(TDestination);
									flag2 = false;
								}
							}
						}
					}
				}
			}
			return flag2;
		}

		private static bool TryConvertToUnityEngineObject<TSource, TDestination>(TSource source, out TDestination destination)
		{
			bool flag = !typeof(Object).IsAssignableFrom(typeof(TDestination));
			bool flag2;
			if (flag)
			{
				destination = default(TDestination);
				flag2 = false;
			}
			else
			{
				bool flag3 = typeof(Object).IsAssignableFrom(typeof(TSource)) || source is Object;
				if (flag3)
				{
					bool flag4 = source == null;
					if (flag4)
					{
						destination = default(TDestination);
						return true;
					}
					bool flag5 = typeof(TDestination) == typeof(Object);
					if (flag5)
					{
						destination = (TDestination)((object)source);
						return true;
					}
				}
				Delegate @delegate;
				bool flag6 = TypeConversion.s_GlobalConverters.TryGetConverter(typeof(TSource), typeof(Object), out @delegate);
				if (flag6)
				{
					Object @object = ((TypeConverter<TSource, Object>)@delegate)(ref source);
					destination = (TDestination)((object)@object);
					flag2 = @object;
				}
				else
				{
					destination = default(TDestination);
					flag2 = false;
				}
			}
			return flag2;
		}

		private static bool IsNumericType(Type t)
		{
			TypeCode typeCode = Type.GetTypeCode(t);
			TypeCode typeCode2 = typeCode;
			return typeCode2 - TypeCode.SByte <= 10;
		}

		private static readonly ConversionRegistry s_GlobalConverters = ConversionRegistry.Create();

		private static class PrimitiveConverters
		{
			public static void Register()
			{
				TypeConversion.PrimitiveConverters.RegisterInt8Converters();
				TypeConversion.PrimitiveConverters.RegisterInt16Converters();
				TypeConversion.PrimitiveConverters.RegisterInt32Converters();
				TypeConversion.PrimitiveConverters.RegisterInt64Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt8Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt16Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt32Converters();
				TypeConversion.PrimitiveConverters.RegisterUInt64Converters();
				TypeConversion.PrimitiveConverters.RegisterFloat32Converters();
				TypeConversion.PrimitiveConverters.RegisterFloat64Converters();
				TypeConversion.PrimitiveConverters.RegisterBooleanConverters();
				TypeConversion.PrimitiveConverters.RegisterCharConverters();
				TypeConversion.PrimitiveConverters.RegisterStringConverters();
				TypeConversion.PrimitiveConverters.RegisterObjectConverters();
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(Guid), new TypeConverter<string, Guid>(delegate(ref string g)
				{
					return new Guid(g);
				}));
			}

			private static void RegisterInt8Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(char), new TypeConverter<sbyte, char>(delegate(ref sbyte v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(bool), new TypeConverter<sbyte, bool>(delegate(ref sbyte v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(short), new TypeConverter<sbyte, short>(delegate(ref sbyte v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(int), new TypeConverter<sbyte, int>(delegate(ref sbyte v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(long), new TypeConverter<sbyte, long>(delegate(ref sbyte v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(byte), new TypeConverter<sbyte, byte>(delegate(ref sbyte v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(ushort), new TypeConverter<sbyte, ushort>(delegate(ref sbyte v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(uint), new TypeConverter<sbyte, uint>(delegate(ref sbyte v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(ulong), new TypeConverter<sbyte, ulong>(delegate(ref sbyte v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(float), new TypeConverter<sbyte, float>(delegate(ref sbyte v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(double), new TypeConverter<sbyte, double>(delegate(ref sbyte v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(object), new TypeConverter<sbyte, object>(delegate(ref sbyte v)
				{
					return v;
				}));
			}

			private static void RegisterInt16Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(sbyte), new TypeConverter<short, sbyte>(delegate(ref short v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(char), new TypeConverter<short, char>(delegate(ref short v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(bool), new TypeConverter<short, bool>(delegate(ref short v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(int), new TypeConverter<short, int>(delegate(ref short v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(long), new TypeConverter<short, long>(delegate(ref short v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(byte), new TypeConverter<short, byte>(delegate(ref short v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(ushort), new TypeConverter<short, ushort>(delegate(ref short v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(uint), new TypeConverter<short, uint>(delegate(ref short v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(ulong), new TypeConverter<short, ulong>(delegate(ref short v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(float), new TypeConverter<short, float>(delegate(ref short v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(double), new TypeConverter<short, double>(delegate(ref short v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(object), new TypeConverter<short, object>(delegate(ref short v)
				{
					return v;
				}));
			}

			private static void RegisterInt32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(sbyte), new TypeConverter<int, sbyte>(delegate(ref int v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(char), new TypeConverter<int, char>(delegate(ref int v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(bool), new TypeConverter<int, bool>(delegate(ref int v)
				{
					return v != 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(short), new TypeConverter<int, short>(delegate(ref int v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(long), new TypeConverter<int, long>(delegate(ref int v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(byte), new TypeConverter<int, byte>(delegate(ref int v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(ushort), new TypeConverter<int, ushort>(delegate(ref int v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(uint), new TypeConverter<int, uint>(delegate(ref int v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(ulong), new TypeConverter<int, ulong>(delegate(ref int v)
				{
					return (ulong)((long)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(float), new TypeConverter<int, float>(delegate(ref int v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(double), new TypeConverter<int, double>(delegate(ref int v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(object), new TypeConverter<int, object>(delegate(ref int v)
				{
					return v;
				}));
			}

			private static void RegisterInt64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(sbyte), new TypeConverter<long, sbyte>(delegate(ref long v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(char), new TypeConverter<long, char>(delegate(ref long v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(bool), new TypeConverter<long, bool>(delegate(ref long v)
				{
					return v != 0L;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(short), new TypeConverter<long, short>(delegate(ref long v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(int), new TypeConverter<long, int>(delegate(ref long v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(byte), new TypeConverter<long, byte>(delegate(ref long v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(ushort), new TypeConverter<long, ushort>(delegate(ref long v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(uint), new TypeConverter<long, uint>(delegate(ref long v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(ulong), new TypeConverter<long, ulong>(delegate(ref long v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(float), new TypeConverter<long, float>(delegate(ref long v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(double), new TypeConverter<long, double>(delegate(ref long v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(object), new TypeConverter<long, object>(delegate(ref long v)
				{
					return v;
				}));
			}

			private static void RegisterUInt8Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(sbyte), new TypeConverter<byte, sbyte>(delegate(ref byte v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(char), new TypeConverter<byte, char>(delegate(ref byte v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(bool), new TypeConverter<byte, bool>(delegate(ref byte v)
				{
					return v > 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(short), new TypeConverter<byte, short>(delegate(ref byte v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(int), new TypeConverter<byte, int>(delegate(ref byte v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(long), new TypeConverter<byte, long>(delegate(ref byte v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(ushort), new TypeConverter<byte, ushort>(delegate(ref byte v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(uint), new TypeConverter<byte, uint>(delegate(ref byte v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(ulong), new TypeConverter<byte, ulong>(delegate(ref byte v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(float), new TypeConverter<byte, float>(delegate(ref byte v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(double), new TypeConverter<byte, double>(delegate(ref byte v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(object), new TypeConverter<byte, object>(delegate(ref byte v)
				{
					return v;
				}));
			}

			private static void RegisterUInt16Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(sbyte), new TypeConverter<ushort, sbyte>(delegate(ref ushort v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(char), new TypeConverter<ushort, char>(delegate(ref ushort v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(bool), new TypeConverter<ushort, bool>(delegate(ref ushort v)
				{
					return v > 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(short), new TypeConverter<ushort, short>(delegate(ref ushort v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(int), new TypeConverter<ushort, int>(delegate(ref ushort v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(long), new TypeConverter<ushort, long>(delegate(ref ushort v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(byte), new TypeConverter<ushort, byte>(delegate(ref ushort v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(uint), new TypeConverter<ushort, uint>(delegate(ref ushort v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(ulong), new TypeConverter<ushort, ulong>(delegate(ref ushort v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(float), new TypeConverter<ushort, float>(delegate(ref ushort v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(double), new TypeConverter<ushort, double>(delegate(ref ushort v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(object), new TypeConverter<ushort, object>(delegate(ref ushort v)
				{
					return v;
				}));
			}

			private static void RegisterUInt32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(sbyte), new TypeConverter<uint, sbyte>(delegate(ref uint v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(char), new TypeConverter<uint, char>(delegate(ref uint v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(bool), new TypeConverter<uint, bool>(delegate(ref uint v)
				{
					return v > 0U;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(short), new TypeConverter<uint, short>(delegate(ref uint v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(int), new TypeConverter<uint, int>(delegate(ref uint v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(long), new TypeConverter<uint, long>(delegate(ref uint v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(byte), new TypeConverter<uint, byte>(delegate(ref uint v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(ushort), new TypeConverter<uint, ushort>(delegate(ref uint v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(ulong), new TypeConverter<uint, ulong>(delegate(ref uint v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(float), new TypeConverter<uint, float>(delegate(ref uint v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(double), new TypeConverter<uint, double>(delegate(ref uint v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(object), new TypeConverter<uint, object>(delegate(ref uint v)
				{
					return v;
				}));
			}

			private static void RegisterUInt64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(sbyte), new TypeConverter<ulong, sbyte>(delegate(ref ulong v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(char), new TypeConverter<ulong, char>(delegate(ref ulong v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(bool), new TypeConverter<ulong, bool>(delegate(ref ulong v)
				{
					return v > 0UL;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(short), new TypeConverter<ulong, short>(delegate(ref ulong v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(int), new TypeConverter<ulong, int>(delegate(ref ulong v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(long), new TypeConverter<ulong, long>(delegate(ref ulong v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(byte), new TypeConverter<ulong, byte>(delegate(ref ulong v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(ushort), new TypeConverter<ulong, ushort>(delegate(ref ulong v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(uint), new TypeConverter<ulong, uint>(delegate(ref ulong v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(float), new TypeConverter<ulong, float>(delegate(ref ulong v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(double), new TypeConverter<ulong, double>(delegate(ref ulong v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(object), new TypeConverter<ulong, object>(delegate(ref ulong v)
				{
					return v;
				}));
			}

			private static void RegisterFloat32Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(sbyte), new TypeConverter<float, sbyte>(delegate(ref float v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(char), new TypeConverter<float, char>(delegate(ref float v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(bool), new TypeConverter<float, bool>(delegate(ref float v)
				{
					return Math.Abs(v) > float.Epsilon;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(short), new TypeConverter<float, short>(delegate(ref float v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(int), new TypeConverter<float, int>(delegate(ref float v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(long), new TypeConverter<float, long>(delegate(ref float v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(byte), new TypeConverter<float, byte>(delegate(ref float v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(ushort), new TypeConverter<float, ushort>(delegate(ref float v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(uint), new TypeConverter<float, uint>(delegate(ref float v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(ulong), new TypeConverter<float, ulong>(delegate(ref float v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(double), new TypeConverter<float, double>(delegate(ref float v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(object), new TypeConverter<float, object>(delegate(ref float v)
				{
					return v;
				}));
			}

			private static void RegisterFloat64Converters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(sbyte), new TypeConverter<double, sbyte>(delegate(ref double v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(char), new TypeConverter<double, char>(delegate(ref double v)
				{
					return (char)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(bool), new TypeConverter<double, bool>(delegate(ref double v)
				{
					return Math.Abs(v) > double.Epsilon;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(short), new TypeConverter<double, short>(delegate(ref double v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(int), new TypeConverter<double, int>(delegate(ref double v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(long), new TypeConverter<double, long>(delegate(ref double v)
				{
					return (long)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(byte), new TypeConverter<double, byte>(delegate(ref double v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(ushort), new TypeConverter<double, ushort>(delegate(ref double v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(uint), new TypeConverter<double, uint>(delegate(ref double v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(ulong), new TypeConverter<double, ulong>(delegate(ref double v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(float), new TypeConverter<double, float>(delegate(ref double v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(object), new TypeConverter<double, object>(delegate(ref double v)
				{
					return v;
				}));
			}

			private static void RegisterBooleanConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(char), new TypeConverter<bool, char>(delegate(ref bool v)
				{
					return v ? '\u0001' : '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(sbyte), new TypeConverter<bool, sbyte>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(short), new TypeConverter<bool, short>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(int), new TypeConverter<bool, int>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(long), new TypeConverter<bool, long>(delegate(ref bool v)
				{
					return v ? 1L : 0L;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(byte), new TypeConverter<bool, byte>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(ushort), new TypeConverter<bool, ushort>(delegate(ref bool v)
				{
					return v ? 1 : 0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(uint), new TypeConverter<bool, uint>(delegate(ref bool v)
				{
					return v ? 1U : 0U;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(ulong), new TypeConverter<bool, ulong>(delegate(ref bool v)
				{
					return v ? 1UL : 0UL;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(float), new TypeConverter<bool, float>(delegate(ref bool v)
				{
					return v ? 1f : 0f;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(double), new TypeConverter<bool, double>(delegate(ref bool v)
				{
					return v ? 1.0 : 0.0;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(object), new TypeConverter<bool, object>(delegate(ref bool v)
				{
					return v;
				}));
			}

			private static void RegisterCharConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(char), new TypeConverter<string, char>(delegate(ref string v)
				{
					bool flag = v.Length != 1;
					if (flag)
					{
						throw new Exception("Not a valid char");
					}
					return v[0];
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(bool), new TypeConverter<char, bool>(delegate(ref char v)
				{
					return v > '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(sbyte), new TypeConverter<char, sbyte>(delegate(ref char v)
				{
					return (sbyte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(short), new TypeConverter<char, short>(delegate(ref char v)
				{
					return (short)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(int), new TypeConverter<char, int>(delegate(ref char v)
				{
					return (int)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(long), new TypeConverter<char, long>(delegate(ref char v)
				{
					return (long)((ulong)v);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(byte), new TypeConverter<char, byte>(delegate(ref char v)
				{
					return (byte)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(ushort), new TypeConverter<char, ushort>(delegate(ref char v)
				{
					return (ushort)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(uint), new TypeConverter<char, uint>(delegate(ref char v)
				{
					return (uint)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(ulong), new TypeConverter<char, ulong>(delegate(ref char v)
				{
					return (ulong)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(float), new TypeConverter<char, float>(delegate(ref char v)
				{
					return (float)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(double), new TypeConverter<char, double>(delegate(ref char v)
				{
					return (double)v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(object), new TypeConverter<char, object>(delegate(ref char v)
				{
					return v;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(string), new TypeConverter<char, string>(delegate(ref char v)
				{
					return v.ToString();
				}));
			}

			private static void RegisterStringConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(char), new TypeConverter<string, char>(delegate(ref string v)
				{
					return (!string.IsNullOrEmpty(v)) ? v[0] : '\0';
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(char), typeof(string), new TypeConverter<char, string>(delegate(ref char v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(bool), new TypeConverter<string, bool>(delegate(ref string v)
				{
					bool flag2;
					bool flag = bool.TryParse(v, out flag2);
					bool flag3;
					if (flag)
					{
						flag3 = flag2;
					}
					else
					{
						double num;
						flag3 = double.TryParse(v, out num) && TypeConversion.Convert<double, bool>(ref num);
					}
					return flag3;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(bool), typeof(string), new TypeConverter<bool, string>(delegate(ref bool v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(sbyte), new TypeConverter<string, sbyte>(delegate(ref string v)
				{
					sbyte b;
					bool flag4 = sbyte.TryParse(v, out b);
					sbyte b2;
					if (flag4)
					{
						b2 = b;
					}
					else
					{
						double num2;
						b2 = (double.TryParse(v, out num2) ? TypeConversion.Convert<double, sbyte>(ref num2) : 0);
					}
					return b2;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(sbyte), typeof(string), new TypeConverter<sbyte, string>(delegate(ref sbyte v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(short), new TypeConverter<string, short>(delegate(ref string v)
				{
					short num3;
					bool flag5 = short.TryParse(v, out num3);
					short num4;
					if (flag5)
					{
						num4 = num3;
					}
					else
					{
						double num5;
						num4 = (double.TryParse(v, out num5) ? TypeConversion.Convert<double, short>(ref num5) : 0);
					}
					return num4;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(short), typeof(string), new TypeConverter<short, string>(delegate(ref short v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(int), new TypeConverter<string, int>(delegate(ref string v)
				{
					int num6;
					bool flag6 = int.TryParse(v, out num6);
					int num7;
					if (flag6)
					{
						num7 = num6;
					}
					else
					{
						double num8;
						num7 = (double.TryParse(v, out num8) ? TypeConversion.Convert<double, int>(ref num8) : 0);
					}
					return num7;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(int), typeof(string), new TypeConverter<int, string>(delegate(ref int v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(long), new TypeConverter<string, long>(delegate(ref string v)
				{
					long num9;
					bool flag7 = long.TryParse(v, out num9);
					long num10;
					if (flag7)
					{
						num10 = num9;
					}
					else
					{
						double num11;
						num10 = (double.TryParse(v, out num11) ? TypeConversion.Convert<double, long>(ref num11) : 0L);
					}
					return num10;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(long), typeof(string), new TypeConverter<long, string>(delegate(ref long v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(byte), new TypeConverter<string, byte>(delegate(ref string v)
				{
					byte b3;
					bool flag8 = byte.TryParse(v, out b3);
					byte b4;
					if (flag8)
					{
						b4 = b3;
					}
					else
					{
						double num12;
						b4 = (double.TryParse(v, out num12) ? TypeConversion.Convert<double, byte>(ref num12) : 0);
					}
					return b4;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(byte), typeof(string), new TypeConverter<byte, string>(delegate(ref byte v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(ushort), new TypeConverter<string, ushort>(delegate(ref string v)
				{
					ushort num13;
					bool flag9 = ushort.TryParse(v, out num13);
					ushort num14;
					if (flag9)
					{
						num14 = num13;
					}
					else
					{
						double num15;
						num14 = (double.TryParse(v, out num15) ? TypeConversion.Convert<double, ushort>(ref num15) : 0);
					}
					return num14;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ushort), typeof(string), new TypeConverter<ushort, string>(delegate(ref ushort v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(uint), new TypeConverter<string, uint>(delegate(ref string v)
				{
					uint num16;
					bool flag10 = uint.TryParse(v, out num16);
					uint num17;
					if (flag10)
					{
						num17 = num16;
					}
					else
					{
						double num18;
						num17 = (double.TryParse(v, out num18) ? TypeConversion.Convert<double, uint>(ref num18) : 0U);
					}
					return num17;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(uint), typeof(string), new TypeConverter<uint, string>(delegate(ref uint v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(ulong), new TypeConverter<string, ulong>(delegate(ref string v)
				{
					ulong num19;
					bool flag11 = ulong.TryParse(v, out num19);
					ulong num20;
					if (flag11)
					{
						num20 = num19;
					}
					else
					{
						double num21;
						num20 = (double.TryParse(v, out num21) ? TypeConversion.Convert<double, ulong>(ref num21) : 0UL);
					}
					return num20;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(ulong), typeof(string), new TypeConverter<ulong, string>(delegate(ref ulong v)
				{
					return v.ToString();
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(float), new TypeConverter<string, float>(delegate(ref string v)
				{
					float num22;
					bool flag12 = float.TryParse(v, out num22);
					float num23;
					if (flag12)
					{
						num23 = num22;
					}
					else
					{
						double num24;
						num23 = (double.TryParse(v, out num24) ? TypeConversion.Convert<double, float>(ref num24) : 0f);
					}
					return num23;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(float), typeof(string), new TypeConverter<float, string>(delegate(ref float v)
				{
					return v.ToString(CultureInfo.InvariantCulture);
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(string), typeof(double), new TypeConverter<string, double>(delegate(ref string v)
				{
					double num25;
					double.TryParse(v, out num25);
					return num25;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(double), typeof(string), new TypeConverter<double, string>(delegate(ref double v)
				{
					return v.ToString(CultureInfo.InvariantCulture);
				}));
			}

			private static void RegisterObjectConverters()
			{
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(char), new TypeConverter<object, char>(delegate(ref object v)
				{
					object obj = v;
					char c2;
					if (obj is char)
					{
						char c = (char)obj;
						c2 = c;
					}
					else
					{
						c2 = '\0';
					}
					return c2;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(bool), new TypeConverter<object, bool>(delegate(ref object v)
				{
					object obj2 = v;
					bool flag2;
					if (obj2 is bool)
					{
						bool flag = (bool)obj2;
						flag2 = flag;
					}
					else
					{
						flag2 = false;
					}
					return flag2;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(sbyte), new TypeConverter<object, sbyte>(delegate(ref object v)
				{
					object obj3 = v;
					sbyte b2;
					if (obj3 is sbyte)
					{
						sbyte b = (sbyte)obj3;
						b2 = b;
					}
					else
					{
						b2 = 0;
					}
					return b2;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(short), new TypeConverter<object, short>(delegate(ref object v)
				{
					object obj4 = v;
					short num2;
					if (obj4 is short)
					{
						short num = (short)obj4;
						num2 = num;
					}
					else
					{
						num2 = 0;
					}
					return num2;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(int), new TypeConverter<object, int>(delegate(ref object v)
				{
					object obj5 = v;
					int num4;
					if (obj5 is int)
					{
						int num3 = (int)obj5;
						num4 = num3;
					}
					else
					{
						num4 = 0;
					}
					return num4;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(long), new TypeConverter<object, long>(delegate(ref object v)
				{
					object obj6 = v;
					long num6;
					if (obj6 is long)
					{
						long num5 = (long)obj6;
						num6 = num5;
					}
					else
					{
						num6 = 0L;
					}
					return num6;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(byte), new TypeConverter<object, byte>(delegate(ref object v)
				{
					object obj7 = v;
					byte b4;
					if (obj7 is byte)
					{
						byte b3 = (byte)obj7;
						b4 = b3;
					}
					else
					{
						b4 = 0;
					}
					return b4;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(ushort), new TypeConverter<object, ushort>(delegate(ref object v)
				{
					object obj8 = v;
					ushort num8;
					if (obj8 is ushort)
					{
						ushort num7 = (ushort)obj8;
						num8 = num7;
					}
					else
					{
						num8 = 0;
					}
					return num8;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(uint), new TypeConverter<object, uint>(delegate(ref object v)
				{
					object obj9 = v;
					uint num10;
					if (obj9 is uint)
					{
						uint num9 = (uint)obj9;
						num10 = num9;
					}
					else
					{
						num10 = 0U;
					}
					return num10;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(ulong), new TypeConverter<object, ulong>(delegate(ref object v)
				{
					object obj10 = v;
					ulong num12;
					if (obj10 is ulong)
					{
						ulong num11 = (ulong)obj10;
						num12 = num11;
					}
					else
					{
						num12 = 0UL;
					}
					return num12;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(float), new TypeConverter<object, float>(delegate(ref object v)
				{
					object obj11 = v;
					float num14;
					if (obj11 is float)
					{
						float num13 = (float)obj11;
						num14 = num13;
					}
					else
					{
						num14 = 0f;
					}
					return num14;
				}));
				TypeConversion.s_GlobalConverters.Register(typeof(object), typeof(double), new TypeConverter<object, double>(delegate(ref object v)
				{
					object obj12 = v;
					double num16;
					if (obj12 is double)
					{
						double num15 = (double)obj12;
						num16 = num15;
					}
					else
					{
						num16 = 0.0;
					}
					return num16;
				}));
			}
		}
	}
}
