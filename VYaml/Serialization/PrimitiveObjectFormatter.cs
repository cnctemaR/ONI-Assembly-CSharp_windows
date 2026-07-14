using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class PrimitiveObjectFormatter : IYamlFormatter<object>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(2)] object value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			Type type = value.GetType();
			int num;
			if (PrimitiveObjectFormatter.TypeToJumpCode.TryGetValue(type, out num))
			{
				switch (num)
				{
				case 0:
					emitter.WriteBool((bool)value);
					return;
				case 1:
					emitter.WriteInt32((int)((char)value));
					return;
				case 2:
					emitter.WriteInt32((int)((sbyte)value));
					return;
				case 3:
					emitter.WriteUInt32((uint)((byte)value));
					return;
				case 4:
					emitter.WriteInt32((int)((short)value));
					return;
				case 5:
					emitter.WriteUInt32((uint)((ushort)value));
					return;
				case 6:
					emitter.WriteInt32((int)value);
					return;
				case 7:
					emitter.WriteUInt32((uint)value);
					return;
				case 8:
					emitter.WriteInt64((long)value);
					return;
				case 9:
					emitter.WriteUInt64((ulong)value);
					return;
				case 10:
					emitter.WriteFloat((float)value);
					return;
				case 11:
					emitter.WriteDouble((double)value);
					return;
				case 12:
					DateTimeFormatter.Instance.Serialize(ref emitter, (DateTime)value, context);
					return;
				case 13:
					emitter.WriteString((string)value, ScalarStyle.Any);
					return;
				case 14:
					ByteArrayFormatter.Instance.Serialize(ref emitter, (byte[])value, context);
					return;
				}
			}
			if (type.IsEnum)
			{
				EnumAsStringNonGenericHelper.Serialize(ref emitter, type, value, context);
				return;
			}
			IDictionary dictionary = value as IDictionary;
			if (dictionary != null)
			{
				emitter.BeginMapping(MappingStyle.Block);
				foreach (object obj in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					this.Serialize(ref emitter, dictionaryEntry.Key, context);
					this.Serialize(ref emitter, dictionaryEntry.Value, context);
				}
				emitter.EndMapping();
				return;
			}
			ICollection collection = value as ICollection;
			if (collection != null)
			{
				emitter.BeginSequence(SequenceStyle.Block);
				foreach (object obj2 in collection)
				{
					this.Serialize(ref emitter, obj2, context);
				}
				emitter.EndSequence();
				return;
			}
			throw new YamlSerializerException(string.Format("Not supported primitive object resolver. type: {0}", type));
		}

		[return: Nullable(2)]
		public object Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			switch (parser.CurrentEventType)
			{
			case ParseEventType.Scalar:
			{
				if (parser.IsNullScalar())
				{
					parser.Read();
					return null;
				}
				bool flag;
				if (parser.TryGetScalarAsBool(out flag))
				{
					parser.Read();
					return flag;
				}
				int num;
				if (parser.TryGetScalarAsInt32(out num))
				{
					parser.Read();
					return num;
				}
				long num2;
				if (parser.TryGetScalarAsInt64(out num2))
				{
					parser.Read();
					return num2;
				}
				double num3;
				if (parser.TryGetScalarAsDouble(out num3))
				{
					parser.Read();
					return num3;
				}
				object scalarAsString = parser.GetScalarAsString();
				parser.Read();
				return scalarAsString;
			}
			case ParseEventType.SequenceStart:
			{
				List<object> list = new List<object>();
				parser.Read();
				while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
				{
					object obj = context.DeserializeWithAlias<object>(this, ref parser);
					list.Add(obj);
				}
				parser.ReadWithVerify(ParseEventType.SequenceEnd);
				return list;
			}
			case ParseEventType.MappingStart:
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				parser.Read();
				while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
				{
					object obj2 = context.DeserializeWithAlias<object>(this, ref parser);
					object obj3 = context.DeserializeWithAlias<object>(this, ref parser);
					dictionary.Add(obj2, obj3);
				}
				parser.ReadWithVerify(ParseEventType.MappingEnd);
				return dictionary;
			}
			}
			throw new InvalidOperationException();
		}

		public static readonly PrimitiveObjectFormatter Instance = new PrimitiveObjectFormatter();

		private static readonly Dictionary<Type, int> TypeToJumpCode = new Dictionary<Type, int>
		{
			{
				typeof(bool),
				0
			},
			{
				typeof(char),
				1
			},
			{
				typeof(sbyte),
				2
			},
			{
				typeof(byte),
				3
			},
			{
				typeof(short),
				4
			},
			{
				typeof(ushort),
				5
			},
			{
				typeof(int),
				6
			},
			{
				typeof(uint),
				7
			},
			{
				typeof(long),
				8
			},
			{
				typeof(ulong),
				9
			},
			{
				typeof(float),
				10
			},
			{
				typeof(double),
				11
			},
			{
				typeof(DateTime),
				12
			},
			{
				typeof(string),
				13
			},
			{
				typeof(byte[]),
				14
			}
		};
	}
}
