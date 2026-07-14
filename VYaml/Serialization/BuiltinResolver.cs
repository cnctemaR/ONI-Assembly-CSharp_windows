using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class BuiltinResolver : IYamlFormatterResolver
	{
		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 1 })]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			return BuiltinResolver.FormatterCache<T>.Formatter;
		}

		[return: Nullable(2)]
		private static object TryCreateGenericFormatter(Type type)
		{
			Type type2 = null;
			if (type.IsArray)
			{
				if (type.IsSZArray)
				{
					type2 = typeof(ArrayFormatter<>).MakeGenericType(new Type[] { type.GetElementType() });
				}
				else
				{
					switch (type.GetArrayRank())
					{
					case 1:
						type2 = typeof(ArrayFormatter<>).MakeGenericType(new Type[] { type.GetElementType() });
						break;
					case 2:
						type2 = typeof(TwoDimensionalArrayFormatter<>).MakeGenericType(new Type[] { type.GetElementType() });
						break;
					case 3:
						type2 = typeof(ThreeDimensionalArrayFormatter<>).MakeGenericType(new Type[] { type.GetElementType() });
						break;
					case 4:
						type2 = typeof(FourDimensionalArrayFormatter<>).MakeGenericType(new Type[] { type.GetElementType() });
						break;
					}
				}
			}
			else if (type.IsEnum)
			{
				type2 = typeof(EnumAsStringFormatter<>).MakeGenericType(new Type[] { type });
			}
			else
			{
				type2 = BuiltinResolver.TryCreateGenericFormatterType(type, BuiltinResolver.KnownGenericTypes);
			}
			if (type2 != null)
			{
				return Activator.CreateInstance(type2);
			}
			return null;
		}

		[return: Nullable(2)]
		private static Type TryCreateGenericFormatterType(Type type, IDictionary<Type, Type> knownTypes)
		{
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				Type type2;
				if (knownTypes.TryGetValue(genericTypeDefinition, out type2))
				{
					return type2.MakeGenericType(type.GetGenericArguments());
				}
			}
			return null;
		}

		public static readonly BuiltinResolver Instance = new BuiltinResolver();

		private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>
		{
			{
				typeof(short),
				Int16Formatter.Instance
			},
			{
				typeof(int),
				Int32Formatter.Instance
			},
			{
				typeof(long),
				Int64Formatter.Instance
			},
			{
				typeof(ushort),
				UInt16Formatter.Instance
			},
			{
				typeof(uint),
				UInt32Formatter.Instance
			},
			{
				typeof(ulong),
				UInt64Formatter.Instance
			},
			{
				typeof(float),
				Float32Formatter.Instance
			},
			{
				typeof(double),
				Float64Formatter.Instance
			},
			{
				typeof(bool),
				BooleanFormatter.Instance
			},
			{
				typeof(byte),
				ByteFormatter.Instance
			},
			{
				typeof(sbyte),
				SByteFormatter.Instance
			},
			{
				typeof(DateTime),
				DateTimeFormatter.Instance
			},
			{
				typeof(char),
				CharFormatter.Instance
			},
			{
				typeof(byte[]),
				ByteArrayFormatter.Instance
			},
			{
				typeof(short?),
				NullableInt16Formatter.Instance
			},
			{
				typeof(int?),
				NullableInt32Formatter.Instance
			},
			{
				typeof(long?),
				NullableInt64Formatter.Instance
			},
			{
				typeof(ushort?),
				NullableUInt16Formatter.Instance
			},
			{
				typeof(uint?),
				NullableUInt32Formatter.Instance
			},
			{
				typeof(ulong?),
				NullableUInt64Formatter.Instance
			},
			{
				typeof(float?),
				NullableFloat32Formatter.Instance
			},
			{
				typeof(double?),
				NullableFloat64Formatter.Instance
			},
			{
				typeof(bool?),
				NullableBooleanFormatter.Instance
			},
			{
				typeof(byte?),
				NullableByteFormatter.Instance
			},
			{
				typeof(sbyte?),
				NullableSByteFormatter.Instance
			},
			{
				typeof(DateTime?),
				NullableDateTimeFormatter.Instance
			},
			{
				typeof(char?),
				NullableCharFormatter.Instance
			},
			{
				typeof(string),
				NullableStringFormatter.Instance
			},
			{
				typeof(decimal),
				DecimalFormatter.Instance
			},
			{
				typeof(decimal?),
				new StaticNullableFormatter<decimal>(DecimalFormatter.Instance)
			},
			{
				typeof(TimeSpan),
				TimeSpanFormatter.Instance
			},
			{
				typeof(TimeSpan?),
				new StaticNullableFormatter<TimeSpan>(TimeSpanFormatter.Instance)
			},
			{
				typeof(DateTimeOffset),
				DateTimeOffsetFormatter.Instance
			},
			{
				typeof(DateTimeOffset?),
				new StaticNullableFormatter<DateTimeOffset>(DateTimeOffsetFormatter.Instance)
			},
			{
				typeof(Guid),
				GuidFormatter.Instance
			},
			{
				typeof(Guid?),
				new StaticNullableFormatter<Guid>(GuidFormatter.Instance)
			},
			{
				typeof(Uri),
				UriFormatter.Instance
			},
			{
				typeof(Version),
				VersionFormatter.Instance
			},
			{
				typeof(BitArray),
				BitArrayFormatter.Instance
			},
			{
				typeof(Type),
				TypeFormatter.Instance
			},
			{
				typeof(List<short>),
				new ListFormatter<short>()
			},
			{
				typeof(List<int>),
				new ListFormatter<int>()
			},
			{
				typeof(List<long>),
				new ListFormatter<long>()
			},
			{
				typeof(List<ushort>),
				new ListFormatter<ushort>()
			},
			{
				typeof(List<uint>),
				new ListFormatter<uint>()
			},
			{
				typeof(List<ulong>),
				new ListFormatter<ulong>()
			},
			{
				typeof(List<float>),
				new ListFormatter<float>()
			},
			{
				typeof(List<double>),
				new ListFormatter<double>()
			},
			{
				typeof(List<bool>),
				new ListFormatter<bool>()
			},
			{
				typeof(List<byte>),
				new ListFormatter<byte>()
			},
			{
				typeof(List<sbyte>),
				new ListFormatter<sbyte>()
			},
			{
				typeof(List<DateTime>),
				new ListFormatter<DateTime>()
			},
			{
				typeof(List<char>),
				new ListFormatter<char>()
			},
			{
				typeof(List<string>),
				new ListFormatter<string>()
			},
			{
				typeof(object[]),
				new ArrayFormatter<object>()
			},
			{
				typeof(List<object>),
				new ListFormatter<object>()
			},
			{
				typeof(Memory<byte>),
				ByteMemoryFormatter.Instance
			},
			{
				typeof(Memory<byte>?),
				new StaticNullableFormatter<Memory<byte>>(ByteMemoryFormatter.Instance)
			},
			{
				typeof(ReadOnlyMemory<byte>),
				ByteReadOnlyMemoryFormatter.Instance
			},
			{
				typeof(ReadOnlyMemory<byte>?),
				new StaticNullableFormatter<ReadOnlyMemory<byte>>(ByteReadOnlyMemoryFormatter.Instance)
			},
			{
				typeof(ReadOnlySequence<byte>),
				ByteReadOnlySequenceFormatter.Instance
			},
			{
				typeof(ReadOnlySequence<byte>?),
				new StaticNullableFormatter<ReadOnlySequence<byte>>(ByteReadOnlySequenceFormatter.Instance)
			},
			{
				typeof(ArraySegment<byte>),
				ByteArraySegmentFormatter.Instance
			},
			{
				typeof(ArraySegment<byte>?),
				new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Instance)
			},
			{
				typeof(BigInteger),
				BigIntegerFormatter.Instance
			},
			{
				typeof(BigInteger?),
				new StaticNullableFormatter<BigInteger>(BigIntegerFormatter.Instance)
			},
			{
				typeof(Complex),
				ComplexFormatter.Instance
			},
			{
				typeof(Complex?),
				new StaticNullableFormatter<Complex>(ComplexFormatter.Instance)
			}
		};

		public static readonly Dictionary<Type, Type> KnownGenericTypes = new Dictionary<Type, Type>
		{
			{
				typeof(Tuple<>),
				typeof(TupleFormatter<>)
			},
			{
				typeof(ValueTuple<>),
				typeof(ValueTupleFormatter<>)
			},
			{
				typeof(Tuple<, >),
				typeof(TupleFormatter<, >)
			},
			{
				typeof(ValueTuple<, >),
				typeof(ValueTupleFormatter<, >)
			},
			{
				typeof(Tuple<, , >),
				typeof(TupleFormatter<, , >)
			},
			{
				typeof(ValueTuple<, , >),
				typeof(ValueTupleFormatter<, , >)
			},
			{
				typeof(Tuple<, , , >),
				typeof(TupleFormatter<, , , >)
			},
			{
				typeof(ValueTuple<, , , >),
				typeof(ValueTupleFormatter<, , , >)
			},
			{
				typeof(Tuple<, , , , >),
				typeof(TupleFormatter<, , , , >)
			},
			{
				typeof(ValueTuple<, , , , >),
				typeof(ValueTupleFormatter<, , , , >)
			},
			{
				typeof(Tuple<, , , , , >),
				typeof(TupleFormatter<, , , , , >)
			},
			{
				typeof(ValueTuple<, , , , , >),
				typeof(ValueTupleFormatter<, , , , , >)
			},
			{
				typeof(Tuple<, , , , , , >),
				typeof(TupleFormatter<, , , , , , >)
			},
			{
				typeof(ValueTuple<, , , , , , >),
				typeof(ValueTupleFormatter<, , , , , , >)
			},
			{
				typeof(Tuple<, , , , , , , >),
				typeof(TupleFormatter<, , , , , , , >)
			},
			{
				typeof(ValueTuple<, , , , , , , >),
				typeof(ValueTupleFormatter<, , , , , , , >)
			},
			{
				typeof(KeyValuePair<, >),
				typeof(KeyValuePairFormatter<, >)
			},
			{
				typeof(Nullable<>),
				typeof(NullableFormatter<>)
			},
			{
				typeof(List<>),
				typeof(ListFormatter<>)
			},
			{
				typeof(Stack<>),
				typeof(StackFormatter<>)
			},
			{
				typeof(Queue<>),
				typeof(QueueFormatter<>)
			},
			{
				typeof(LinkedList<>),
				typeof(LinkedListFormatter<>)
			},
			{
				typeof(HashSet<>),
				typeof(HashSetFormatter<>)
			},
			{
				typeof(SortedSet<>),
				typeof(SortedSetFormatter<>)
			},
			{
				typeof(BlockingCollection<>),
				typeof(BlockingCollectionFormatter<>)
			},
			{
				typeof(ConcurrentQueue<>),
				typeof(ConcurrentQueueFormatter<>)
			},
			{
				typeof(ConcurrentStack<>),
				typeof(ConcurrentStackFormatter<>)
			},
			{
				typeof(ConcurrentBag<>),
				typeof(ConcurrentBagFormatter<>)
			},
			{
				typeof(Dictionary<, >),
				typeof(DictionaryFormatter<, >)
			},
			{
				typeof(IEnumerable<>),
				typeof(InterfaceEnumerableFormatter<>)
			},
			{
				typeof(ICollection<>),
				typeof(InterfaceCollectionFormatter<>)
			},
			{
				typeof(IReadOnlyCollection<>),
				typeof(InterfaceReadOnlyCollectionFormatter<>)
			},
			{
				typeof(IList<>),
				typeof(InterfaceListFormatter<>)
			},
			{
				typeof(IReadOnlyList<>),
				typeof(InterfaceReadOnlyListFormatter<>)
			},
			{
				typeof(IDictionary<, >),
				typeof(InterfaceDictionaryFormatter<, >)
			},
			{
				typeof(IReadOnlyDictionary<, >),
				typeof(InterfaceReadOnlyDictionaryFormatter<, >)
			},
			{
				typeof(ISet<>),
				typeof(InterfaceSetFormatter<>)
			}
		};

		[NullableContext(0)]
		private static class FormatterCache<[Nullable(2)] T>
		{
			static FormatterCache()
			{
				object obj;
				if (BuiltinResolver.FormatterMap.TryGetValue(typeof(T), out obj))
				{
					BuiltinResolver.FormatterCache<T>.Formatter = (IYamlFormatter<T>)obj;
					return;
				}
				IYamlFormatter<T> yamlFormatter = BuiltinResolver.TryCreateGenericFormatter(typeof(T)) as IYamlFormatter<T>;
				if (yamlFormatter != null)
				{
					BuiltinResolver.FormatterCache<T>.Formatter = yamlFormatter;
					return;
				}
				BuiltinResolver.FormatterCache<T>.Formatter = null;
			}

			[Nullable(new byte[] { 2, 1 })]
			public static readonly IYamlFormatter<T> Formatter;
		}
	}
}
