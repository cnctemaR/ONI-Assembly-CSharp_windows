using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace System.Security.Cryptography.Asn1
{
	internal static class AsnSerializer
	{
		private static AsnSerializer.Deserializer TryOrFail<T>(AsnSerializer.TryDeserializer<T> tryDeserializer)
		{
			return delegate(AsnReader reader)
			{
				T t;
				if (tryDeserializer(reader, out t))
				{
					return t;
				}
				throw new CryptographicException("ASN1 corrupted data.");
			};
		}

		private static FieldInfo[] GetOrderedFields(Type typeT)
		{
			return AsnSerializer.s_orderedFields.GetOrAdd(typeT, delegate(Type t)
			{
				FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fields.Length == 0)
				{
					return Array.Empty<FieldInfo>();
				}
				try
				{
					int metadataToken = fields[0].MetadataToken;
				}
				catch (InvalidOperationException)
				{
					return fields;
				}
				Array.Sort<FieldInfo>(fields, (FieldInfo x, FieldInfo y) => x.MetadataToken.CompareTo(y.MetadataToken));
				return fields;
			});
		}

		private static ChoiceAttribute GetChoiceAttribute(Type typeT)
		{
			ChoiceAttribute customAttribute = typeT.GetCustomAttribute<ChoiceAttribute>(false);
			if (customAttribute == null)
			{
				return null;
			}
			if (customAttribute.AllowNull && !AsnSerializer.CanBeNull(typeT))
			{
				throw new AsnSerializationConstraintException(SR.Format("[Choice].AllowNull=true is not valid because type '{0}' cannot have a null value.", typeT.FullName));
			}
			return customAttribute;
		}

		private static bool CanBeNull(Type t)
		{
			return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		private static void PopulateChoiceLookup(Dictionary<ValueTuple<TagClass, int>, LinkedList<FieldInfo>> lookup, Type typeT, LinkedList<FieldInfo> currentSet)
		{
			foreach (FieldInfo fieldInfo in AsnSerializer.GetOrderedFields(typeT))
			{
				Type type = fieldInfo.FieldType;
				if (!AsnSerializer.CanBeNull(type))
				{
					throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on [Choice] type '{1}' can not be assigned a null value.", fieldInfo.Name, fieldInfo.DeclaringType.FullName));
				}
				type = AsnSerializer.UnpackIfNullable(type);
				if (currentSet.Contains(fieldInfo))
				{
					throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on [Choice] type '{1}' has introduced a type chain cycle.", fieldInfo.Name, fieldInfo.DeclaringType.FullName));
				}
				LinkedListNode<FieldInfo> linkedListNode = new LinkedListNode<FieldInfo>(fieldInfo);
				currentSet.AddLast(linkedListNode);
				if (AsnSerializer.GetChoiceAttribute(type) != null)
				{
					AsnSerializer.PopulateChoiceLookup(lookup, type, currentSet);
				}
				else
				{
					AsnSerializer.SerializerFieldData serializerFieldData;
					AsnSerializer.GetFieldInfo(type, fieldInfo, out serializerFieldData);
					if (serializerFieldData.DefaultContents != null)
					{
						throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on [Choice] type '{1}' has a default value, which is not permitted.", fieldInfo.Name, fieldInfo.DeclaringType.FullName));
					}
					ValueTuple<TagClass, int> valueTuple = new ValueTuple<TagClass, int>(serializerFieldData.ExpectedTag.TagClass, serializerFieldData.ExpectedTag.TagValue);
					LinkedList<FieldInfo> linkedList;
					if (lookup.TryGetValue(valueTuple, out linkedList))
					{
						FieldInfo value = linkedList.Last.Value;
						throw new AsnSerializationConstraintException(SR.Format("The tag ({0} {1}) for field '{2}' on type '{3}' already is associated in this context with field '{4}' on type '{5}'.", new object[]
						{
							serializerFieldData.ExpectedTag.TagClass,
							serializerFieldData.ExpectedTag.TagValue,
							fieldInfo.Name,
							fieldInfo.DeclaringType.FullName,
							value.Name,
							value.DeclaringType.FullName
						}));
					}
					lookup.Add(valueTuple, new LinkedList<FieldInfo>(currentSet));
				}
				currentSet.RemoveLast();
			}
		}

		private static void SerializeChoice(Type typeT, object value, AsnWriter writer)
		{
			Dictionary<ValueTuple<TagClass, int>, LinkedList<FieldInfo>> dictionary = new Dictionary<ValueTuple<TagClass, int>, LinkedList<FieldInfo>>();
			LinkedList<FieldInfo> linkedList = new LinkedList<FieldInfo>();
			AsnSerializer.PopulateChoiceLookup(dictionary, typeT, linkedList);
			FieldInfo fieldInfo = null;
			object obj = null;
			if (value == null)
			{
				if (AsnSerializer.GetChoiceAttribute(typeT).AllowNull)
				{
					writer.WriteNull();
					return;
				}
			}
			else
			{
				foreach (FieldInfo fieldInfo2 in AsnSerializer.GetOrderedFields(typeT))
				{
					object value2 = fieldInfo2.GetValue(value);
					if (value2 != null)
					{
						if (fieldInfo != null)
						{
							throw new AsnSerializationConstraintException(SR.Format("Fields '{0}' and '{1}' on type '{2}' are both non-null when only one value is permitted.", fieldInfo2.Name, fieldInfo.Name, typeT.FullName));
						}
						fieldInfo = fieldInfo2;
						obj = value2;
					}
				}
			}
			if (fieldInfo == null)
			{
				throw new AsnSerializationConstraintException(SR.Format("An instance of [Choice] type '{0}' has no non-null fields.", typeT.FullName));
			}
			AsnSerializer.GetSerializer(fieldInfo.FieldType, fieldInfo)(obj, writer);
		}

		private static object DeserializeChoice(AsnReader reader, Type typeT)
		{
			Dictionary<ValueTuple<TagClass, int>, LinkedList<FieldInfo>> dictionary = new Dictionary<ValueTuple<TagClass, int>, LinkedList<FieldInfo>>();
			LinkedList<FieldInfo> linkedList = new LinkedList<FieldInfo>();
			AsnSerializer.PopulateChoiceLookup(dictionary, typeT, linkedList);
			Asn1Tag asn1Tag = reader.PeekTag();
			if (asn1Tag == Asn1Tag.Null)
			{
				if (AsnSerializer.GetChoiceAttribute(typeT).AllowNull)
				{
					reader.ReadNull();
					return null;
				}
				throw new CryptographicException("ASN1 corrupted data.");
			}
			else
			{
				ValueTuple<TagClass, int> valueTuple = new ValueTuple<TagClass, int>(asn1Tag.TagClass, asn1Tag.TagValue);
				LinkedList<FieldInfo> linkedList2;
				if (dictionary.TryGetValue(valueTuple, out linkedList2))
				{
					LinkedListNode<FieldInfo> linkedListNode = linkedList2.Last;
					FieldInfo fieldInfo = linkedListNode.Value;
					object obj = Activator.CreateInstance(fieldInfo.DeclaringType);
					object obj2 = AsnSerializer.GetDeserializer(fieldInfo.FieldType, fieldInfo)(reader);
					fieldInfo.SetValue(obj, obj2);
					while (linkedListNode.Previous != null)
					{
						linkedListNode = linkedListNode.Previous;
						fieldInfo = linkedListNode.Value;
						object obj3 = Activator.CreateInstance(fieldInfo.DeclaringType);
						fieldInfo.SetValue(obj3, obj);
						obj = obj3;
					}
					return obj;
				}
				throw new CryptographicException("ASN1 corrupted data.");
			}
		}

		private static void SerializeCustomType(Type typeT, object value, AsnWriter writer, Asn1Tag tag)
		{
			writer.PushSequence(tag);
			foreach (FieldInfo fieldInfo in typeT.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				AsnSerializer.GetSerializer(fieldInfo.FieldType, fieldInfo)(fieldInfo.GetValue(value), writer);
			}
			writer.PopSequence(tag);
		}

		private static object DeserializeCustomType(AsnReader reader, Type typeT, Asn1Tag expectedTag)
		{
			object obj = Activator.CreateInstance(typeT);
			AsnReader asnReader = reader.ReadSequence(expectedTag);
			foreach (FieldInfo fieldInfo in typeT.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				AsnSerializer.Deserializer deserializer = AsnSerializer.GetDeserializer(fieldInfo.FieldType, fieldInfo);
				try
				{
					fieldInfo.SetValue(obj, deserializer(asnReader));
				}
				catch (Exception ex)
				{
					throw new CryptographicException(SR.Format("Unable to set field {0} on type {1}.", fieldInfo.Name, fieldInfo.DeclaringType.FullName), ex);
				}
			}
			asnReader.ThrowIfNotEmpty();
			return obj;
		}

		private static AsnSerializer.Deserializer ExplicitValueDeserializer(AsnSerializer.Deserializer valueDeserializer, Asn1Tag expectedTag)
		{
			return (AsnReader reader) => AsnSerializer.ExplicitValueDeserializer(reader, valueDeserializer, expectedTag);
		}

		private static object ExplicitValueDeserializer(AsnReader reader, AsnSerializer.Deserializer valueDeserializer, Asn1Tag expectedTag)
		{
			AsnReader asnReader = reader.ReadSequence(expectedTag);
			object obj = valueDeserializer(asnReader);
			asnReader.ThrowIfNotEmpty();
			return obj;
		}

		private static AsnSerializer.Deserializer DefaultValueDeserializer(AsnSerializer.Deserializer valueDeserializer, bool isOptional, byte[] defaultContents, Asn1Tag? expectedTag)
		{
			return (AsnReader reader) => AsnSerializer.DefaultValueDeserializer(reader, expectedTag, valueDeserializer, defaultContents, isOptional);
		}

		private static object DefaultValueDeserializer(AsnReader reader, Asn1Tag? expectedTag, AsnSerializer.Deserializer valueDeserializer, byte[] defaultContents, bool isOptional)
		{
			if (reader.HasData)
			{
				Asn1Tag asn1Tag = reader.PeekTag();
				if (expectedTag == null || asn1Tag.AsPrimitive() == expectedTag.Value.AsPrimitive())
				{
					return valueDeserializer(reader);
				}
			}
			if (isOptional)
			{
				return null;
			}
			if (defaultContents != null)
			{
				return AsnSerializer.DefaultValue(defaultContents, valueDeserializer);
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		private unsafe static AsnSerializer.Serializer GetSerializer(Type typeT, FieldInfo fieldInfo)
		{
			byte[] defaultContents;
			Asn1Tag? explicitTag;
			bool flag;
			AsnSerializer.Serializer literalValueSerializer = AsnSerializer.GetSimpleSerializer(typeT, fieldInfo, out defaultContents, out flag, out explicitTag);
			AsnSerializer.Serializer serializer = literalValueSerializer;
			if (flag)
			{
				serializer = delegate(object obj, AsnWriter writer)
				{
					if (obj != null)
					{
						literalValueSerializer(obj, writer);
					}
				};
			}
			else if (defaultContents != null)
			{
				serializer = delegate(object obj, AsnWriter writer)
				{
					AsnReader asnReader;
					using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
					{
						literalValueSerializer(obj, asnWriter);
						asnReader = new AsnReader(asnWriter.Encode(), AsnEncodingRules.DER);
					}
					ReadOnlySpan<byte> span = asnReader.GetEncodedValue().Span;
					bool flag2 = false;
					if (span.Length == defaultContents.Length)
					{
						flag2 = true;
						for (int i = 0; i < span.Length; i++)
						{
							if (*span[i] != defaultContents[i])
							{
								flag2 = false;
								break;
							}
						}
					}
					if (!flag2)
					{
						literalValueSerializer(obj, writer);
					}
				};
			}
			if (explicitTag != null)
			{
				return delegate(object obj, AsnWriter writer)
				{
					using (AsnWriter asnWriter2 = new AsnWriter(writer.RuleSet))
					{
						serializer(obj, asnWriter2);
						if (asnWriter2.Encode().Length != 0)
						{
							writer.PushSequence(explicitTag.Value);
							serializer(obj, writer);
							writer.PopSequence(explicitTag.Value);
						}
					}
				};
			}
			return serializer;
		}

		private static AsnSerializer.Serializer GetSimpleSerializer(Type typeT, FieldInfo fieldInfo, out byte[] defaultContents, out bool isOptional, out Asn1Tag? explicitTag)
		{
			if (!typeT.IsSealed || typeT.ContainsGenericParameters)
			{
				throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is not sealed or has unbound generic parameters.", typeT.FullName));
			}
			AsnSerializer.SerializerFieldData fieldData;
			AsnSerializer.GetFieldInfo(typeT, fieldInfo, out fieldData);
			defaultContents = fieldData.DefaultContents;
			isOptional = fieldData.IsOptional;
			typeT = AsnSerializer.UnpackIfNullable(typeT);
			bool flag = AsnSerializer.GetChoiceAttribute(typeT) != null;
			Asn1Tag tag;
			if (fieldData.HasExplicitTag)
			{
				explicitTag = new Asn1Tag?(fieldData.ExpectedTag);
				tag = new Asn1Tag(fieldData.TagType.GetValueOrDefault(), false);
			}
			else
			{
				explicitTag = null;
				tag = fieldData.ExpectedTag;
			}
			if (typeT.IsPrimitive)
			{
				return AsnSerializer.GetPrimitiveSerializer(typeT, tag);
			}
			if (typeT.IsEnum)
			{
				if (typeT.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0)
				{
					return delegate(object value, AsnWriter writer)
					{
						writer.WriteNamedBitList(tag, value);
					};
				}
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteEnumeratedValue(tag, value);
				};
			}
			else if (typeT == typeof(string))
			{
				UniversalTagNumber? universalTagNumber = fieldData.TagType;
				UniversalTagNumber universalTagNumber2 = UniversalTagNumber.ObjectIdentifier;
				if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
				{
					return delegate(object value, AsnWriter writer)
					{
						writer.WriteObjectIdentifier(tag, (string)value);
					};
				}
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteCharacterString(tag, fieldData.TagType.Value, (string)value);
				};
			}
			else if (typeT == typeof(ReadOnlyMemory<byte>) && !fieldData.IsCollection)
			{
				if (fieldData.IsAny)
				{
					if (fieldData.SpecifiedTag && !fieldData.HasExplicitTag)
					{
						return delegate(object value, AsnWriter writer)
						{
							ReadOnlyMemory<byte> readOnlyMemory = (ReadOnlyMemory<byte>)value;
							Asn1Tag asn1Tag;
							int num;
							if (!Asn1Tag.TryParse(readOnlyMemory.Span, out asn1Tag, out num) || asn1Tag.AsPrimitive() != fieldData.ExpectedTag.AsPrimitive())
							{
								throw new CryptographicException("ASN1 corrupted data.");
							}
							writer.WriteEncodedValue(readOnlyMemory);
						};
					}
					return delegate(object value, AsnWriter writer)
					{
						writer.WriteEncodedValue((ReadOnlyMemory<byte>)value);
					};
				}
				else
				{
					UniversalTagNumber? universalTagNumber = fieldData.TagType;
					UniversalTagNumber universalTagNumber2 = UniversalTagNumber.BitString;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteBitString(tag, ((ReadOnlyMemory<byte>)value).Span, 0);
						};
					}
					universalTagNumber = fieldData.TagType;
					universalTagNumber2 = UniversalTagNumber.OctetString;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteOctetString(tag, ((ReadOnlyMemory<byte>)value).Span);
						};
					}
					universalTagNumber = fieldData.TagType;
					universalTagNumber2 = UniversalTagNumber.Integer;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteInteger(tag, ((ReadOnlyMemory<byte>)value).Span);
						};
					}
					throw new CryptographicException();
				}
			}
			else
			{
				if (typeT == typeof(Oid))
				{
					return delegate(object value, AsnWriter writer)
					{
						writer.WriteObjectIdentifier(fieldData.ExpectedTag, (Oid)value);
					};
				}
				if (typeT.IsArray)
				{
					if (typeT.GetArrayRank() != 1)
					{
						throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is a multi-dimensional array.", typeT.FullName));
					}
					Type elementType = typeT.GetElementType();
					if (elementType.IsArray)
					{
						throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is an array of arrays.", typeT.FullName));
					}
					AsnSerializer.Serializer serializer = AsnSerializer.GetSerializer(elementType, null);
					UniversalTagNumber? universalTagNumber = fieldData.TagType;
					UniversalTagNumber universalTagNumber2 = UniversalTagNumber.Set;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.PushSetOf(tag);
							foreach (object obj in ((Array)value))
							{
								serializer(obj, writer);
							}
							writer.PopSetOf(tag);
						};
					}
					return delegate(object value, AsnWriter writer)
					{
						writer.PushSequence(tag);
						foreach (object obj2 in ((Array)value))
						{
							serializer(obj2, writer);
						}
						writer.PopSequence(tag);
					};
				}
				else if (typeT == typeof(DateTimeOffset))
				{
					UniversalTagNumber? universalTagNumber = fieldData.TagType;
					UniversalTagNumber universalTagNumber2 = UniversalTagNumber.UtcTime;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteUtcTime(tag, (DateTimeOffset)value);
						};
					}
					universalTagNumber = fieldData.TagType;
					universalTagNumber2 = UniversalTagNumber.GeneralizedTime;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteGeneralizedTime(tag, (DateTimeOffset)value, fieldData.DisallowGeneralizedTimeFractions.Value);
						};
					}
					throw new CryptographicException();
				}
				else
				{
					if (typeT == typeof(BigInteger))
					{
						return delegate(object value, AsnWriter writer)
						{
							writer.WriteInteger(tag, (BigInteger)value);
						};
					}
					if (typeT.IsLayoutSequential)
					{
						if (flag)
						{
							return delegate(object value, AsnWriter writer)
							{
								AsnSerializer.SerializeChoice(typeT, value, writer);
							};
						}
						UniversalTagNumber? universalTagNumber = fieldData.TagType;
						UniversalTagNumber universalTagNumber2 = UniversalTagNumber.Sequence;
						if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
						{
							return delegate(object value, AsnWriter writer)
							{
								AsnSerializer.SerializeCustomType(typeT, value, writer, tag);
							};
						}
					}
					throw new AsnSerializationConstraintException(SR.Format("Could not determine how to serialize or deserialize type '{0}'.", typeT.FullName));
				}
			}
		}

		private static AsnSerializer.Deserializer GetDeserializer(Type typeT, FieldInfo fieldInfo)
		{
			AsnSerializer.SerializerFieldData serializerFieldData;
			AsnSerializer.Deserializer deserializer = AsnSerializer.GetSimpleDeserializer(typeT, fieldInfo, out serializerFieldData);
			if (serializerFieldData.HasExplicitTag)
			{
				deserializer = AsnSerializer.ExplicitValueDeserializer(deserializer, serializerFieldData.ExpectedTag);
			}
			if (serializerFieldData.IsOptional || serializerFieldData.DefaultContents != null)
			{
				Asn1Tag? asn1Tag = null;
				if (serializerFieldData.SpecifiedTag || serializerFieldData.TagType != null)
				{
					asn1Tag = new Asn1Tag?(serializerFieldData.ExpectedTag);
				}
				deserializer = AsnSerializer.DefaultValueDeserializer(deserializer, serializerFieldData.IsOptional, serializerFieldData.DefaultContents, asn1Tag);
			}
			return deserializer;
		}

		private static AsnSerializer.Deserializer GetSimpleDeserializer(Type typeT, FieldInfo fieldInfo, out AsnSerializer.SerializerFieldData fieldData)
		{
			if (!typeT.IsSealed || typeT.ContainsGenericParameters)
			{
				throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is not sealed or has unbound generic parameters.", typeT.FullName));
			}
			AsnSerializer.GetFieldInfo(typeT, fieldInfo, out fieldData);
			AsnSerializer.SerializerFieldData localFieldData = fieldData;
			typeT = AsnSerializer.UnpackIfNullable(typeT);
			if (fieldData.IsAny)
			{
				if (!(typeT == typeof(ReadOnlyMemory<byte>)))
				{
					throw new AsnSerializationConstraintException(SR.Format("Could not determine how to serialize or deserialize type '{0}'.", typeT.FullName));
				}
				Asn1Tag matchTag = fieldData.ExpectedTag;
				if (fieldData.HasExplicitTag || !fieldData.SpecifiedTag)
				{
					return (AsnReader reader) => reader.GetEncodedValue();
				}
				return delegate(AsnReader reader)
				{
					Asn1Tag asn1Tag = reader.PeekTag();
					if (matchTag.TagClass != asn1Tag.TagClass || matchTag.TagValue != asn1Tag.TagValue)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					return reader.GetEncodedValue();
				};
			}
			else
			{
				if (AsnSerializer.GetChoiceAttribute(typeT) != null)
				{
					return (AsnReader reader) => AsnSerializer.DeserializeChoice(reader, typeT);
				}
				Asn1Tag expectedTag = (fieldData.HasExplicitTag ? new Asn1Tag(fieldData.TagType.Value, false) : fieldData.ExpectedTag);
				if (typeT.IsPrimitive)
				{
					return AsnSerializer.GetPrimitiveDeserializer(typeT, expectedTag);
				}
				if (typeT.IsEnum)
				{
					if (typeT.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0)
					{
						return (AsnReader reader) => reader.GetNamedBitListValue(expectedTag, typeT);
					}
					return (AsnReader reader) => reader.GetEnumeratedValue(expectedTag, typeT);
				}
				else if (typeT == typeof(string))
				{
					UniversalTagNumber? universalTagNumber = fieldData.TagType;
					UniversalTagNumber universalTagNumber2 = UniversalTagNumber.ObjectIdentifier;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return (AsnReader reader) => reader.ReadObjectIdentifierAsString(expectedTag);
					}
					return (AsnReader reader) => reader.GetCharacterString(expectedTag, localFieldData.TagType.Value);
				}
				else if (typeT == typeof(ReadOnlyMemory<byte>) && !fieldData.IsCollection)
				{
					UniversalTagNumber? universalTagNumber = fieldData.TagType;
					UniversalTagNumber universalTagNumber2 = UniversalTagNumber.BitString;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(AsnReader reader)
						{
							int num;
							ReadOnlyMemory<byte> readOnlyMemory;
							if (reader.TryGetPrimitiveBitStringValue(expectedTag, out num, out readOnlyMemory))
							{
								return readOnlyMemory;
							}
							int length = reader.PeekEncodedValue().Length;
							byte[] array = ArrayPool<byte>.Shared.Rent(length);
							object obj;
							try
							{
								int num2;
								if (!reader.TryCopyBitStringBytes(expectedTag, array, out num, out num2))
								{
									throw new CryptographicException();
								}
								obj = new ReadOnlyMemory<byte>(array.AsSpan<byte>(0, num2).ToArray());
							}
							finally
							{
								Array.Clear(array, 0, length);
								ArrayPool<byte>.Shared.Return(array, false);
							}
							return obj;
						};
					}
					universalTagNumber = fieldData.TagType;
					universalTagNumber2 = UniversalTagNumber.OctetString;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return delegate(AsnReader reader)
						{
							ReadOnlyMemory<byte> readOnlyMemory2;
							if (reader.TryGetPrimitiveOctetStringBytes(expectedTag, out readOnlyMemory2))
							{
								return readOnlyMemory2;
							}
							int length2 = reader.PeekEncodedValue().Length;
							byte[] array2 = ArrayPool<byte>.Shared.Rent(length2);
							object obj2;
							try
							{
								int num3;
								if (!reader.TryCopyOctetStringBytes(expectedTag, array2, out num3))
								{
									throw new CryptographicException();
								}
								obj2 = new ReadOnlyMemory<byte>(array2.AsSpan<byte>(0, num3).ToArray());
							}
							finally
							{
								Array.Clear(array2, 0, length2);
								ArrayPool<byte>.Shared.Return(array2, false);
							}
							return obj2;
						};
					}
					universalTagNumber = fieldData.TagType;
					universalTagNumber2 = UniversalTagNumber.Integer;
					if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
					{
						return (AsnReader reader) => reader.GetIntegerBytes(expectedTag);
					}
					throw new CryptographicException();
				}
				else
				{
					if (typeT == typeof(Oid))
					{
						bool skipFriendlyName = !fieldData.PopulateOidFriendlyName.GetValueOrDefault();
						return (AsnReader reader) => reader.ReadObjectIdentifier(expectedTag, skipFriendlyName);
					}
					if (typeT.IsArray)
					{
						if (typeT.GetArrayRank() != 1)
						{
							throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is a multi-dimensional array.", typeT.FullName));
						}
						Type baseType = typeT.GetElementType();
						if (baseType.IsArray)
						{
							throw new AsnSerializationConstraintException(SR.Format("Type '{0}' cannot be serialized or deserialized because it is an array of arrays.", typeT.FullName));
						}
						return delegate(AsnReader reader)
						{
							LinkedList<object> linkedList = new LinkedList<object>();
							UniversalTagNumber? tagType = localFieldData.TagType;
							UniversalTagNumber universalTagNumber3 = UniversalTagNumber.Set;
							AsnReader asnReader;
							if ((tagType.GetValueOrDefault() == universalTagNumber3) & (tagType != null))
							{
								asnReader = reader.ReadSetOf(expectedTag, false);
							}
							else
							{
								asnReader = reader.ReadSequence(expectedTag);
							}
							AsnSerializer.Deserializer deserializer = AsnSerializer.GetDeserializer(baseType, null);
							while (asnReader.HasData)
							{
								LinkedListNode<object> linkedListNode = new LinkedListNode<object>(deserializer(asnReader));
								linkedList.AddLast(linkedListNode);
							}
							object[] array3 = linkedList.ToArray<object>();
							Array array4 = Array.CreateInstance(baseType, array3.Length);
							Array.Copy(array3, array4, array3.Length);
							return array4;
						};
					}
					else if (typeT == typeof(DateTimeOffset))
					{
						UniversalTagNumber? universalTagNumber = fieldData.TagType;
						UniversalTagNumber universalTagNumber2 = UniversalTagNumber.UtcTime;
						if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
						{
							if (fieldData.TwoDigitYearMax != null)
							{
								return (AsnReader reader) => reader.GetUtcTime(expectedTag, localFieldData.TwoDigitYearMax.Value);
							}
							return (AsnReader reader) => reader.GetUtcTime(expectedTag, 2049);
						}
						else
						{
							universalTagNumber = fieldData.TagType;
							universalTagNumber2 = UniversalTagNumber.GeneralizedTime;
							if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
							{
								bool disallowFractions = fieldData.DisallowGeneralizedTimeFractions.Value;
								return (AsnReader reader) => reader.GetGeneralizedTime(expectedTag, disallowFractions);
							}
							throw new CryptographicException();
						}
					}
					else
					{
						if (typeT == typeof(BigInteger))
						{
							return (AsnReader reader) => reader.GetInteger(expectedTag);
						}
						if (typeT.IsLayoutSequential)
						{
							UniversalTagNumber? universalTagNumber = fieldData.TagType;
							UniversalTagNumber universalTagNumber2 = UniversalTagNumber.Sequence;
							if ((universalTagNumber.GetValueOrDefault() == universalTagNumber2) & (universalTagNumber != null))
							{
								return (AsnReader reader) => AsnSerializer.DeserializeCustomType(reader, typeT, expectedTag);
							}
						}
						throw new AsnSerializationConstraintException(SR.Format("Could not determine how to serialize or deserialize type '{0}'.", typeT.FullName));
					}
				}
			}
		}

		private static object DefaultValue(byte[] defaultContents, AsnSerializer.Deserializer valueDeserializer)
		{
			object obj2;
			try
			{
				AsnReader asnReader = new AsnReader(defaultContents, AsnEncodingRules.DER);
				object obj = valueDeserializer(asnReader);
				if (asnReader.HasData)
				{
					throw new AsnSerializerInvalidDefaultException();
				}
				obj2 = obj;
			}
			catch (AsnSerializerInvalidDefaultException)
			{
				throw;
			}
			catch (CryptographicException ex)
			{
				throw new AsnSerializerInvalidDefaultException(ex);
			}
			return obj2;
		}

		private static void GetFieldInfo(Type typeT, FieldInfo fieldInfo, out AsnSerializer.SerializerFieldData serializerFieldData)
		{
			serializerFieldData = default(AsnSerializer.SerializerFieldData);
			object[] array = ((fieldInfo != null) ? fieldInfo.GetCustomAttributes(typeof(AsnTypeAttribute), false) : null) ?? Array.Empty<object>();
			if (array.Length > 1)
			{
				throw new AsnSerializationConstraintException(SR.Format(fieldInfo.Name, fieldInfo.DeclaringType.FullName, typeof(AsnTypeAttribute).FullName));
			}
			Type type = AsnSerializer.UnpackIfNullable(typeT);
			if (array.Length == 1)
			{
				object obj = array[0];
				serializerFieldData.WasCustomized = true;
				Type[] array2;
				if (obj is AnyValueAttribute)
				{
					serializerFieldData.IsAny = true;
					array2 = new Type[] { typeof(ReadOnlyMemory<byte>) };
				}
				else if (obj is IntegerAttribute)
				{
					array2 = new Type[] { typeof(ReadOnlyMemory<byte>) };
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Integer);
				}
				else if (obj is BitStringAttribute)
				{
					array2 = new Type[] { typeof(ReadOnlyMemory<byte>) };
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.BitString);
				}
				else if (obj is OctetStringAttribute)
				{
					array2 = new Type[] { typeof(ReadOnlyMemory<byte>) };
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.OctetString);
				}
				else
				{
					ObjectIdentifierAttribute objectIdentifierAttribute = obj as ObjectIdentifierAttribute;
					if (objectIdentifierAttribute != null)
					{
						serializerFieldData.PopulateOidFriendlyName = new bool?(objectIdentifierAttribute.PopulateFriendlyName);
						array2 = new Type[]
						{
							typeof(Oid),
							typeof(string)
						};
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.ObjectIdentifier);
						if (objectIdentifierAttribute.PopulateFriendlyName && type == typeof(string))
						{
							throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on type '{1}' has [ObjectIdentifier].PopulateFriendlyName set to true, which is not applicable to a string.  Change the field to '{2}' or set PopulateFriendlyName to false.", fieldInfo.Name, fieldInfo.DeclaringType.FullName, typeof(Oid).FullName));
						}
					}
					else if (obj is BMPStringAttribute)
					{
						array2 = new Type[] { typeof(string) };
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.BMPString);
					}
					else if (obj is IA5StringAttribute)
					{
						array2 = new Type[] { typeof(string) };
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.IA5String);
					}
					else if (obj is UTF8StringAttribute)
					{
						array2 = new Type[] { typeof(string) };
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.UTF8String);
					}
					else if (obj is PrintableStringAttribute)
					{
						array2 = new Type[] { typeof(string) };
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.PrintableString);
					}
					else if (obj is VisibleStringAttribute)
					{
						array2 = new Type[] { typeof(string) };
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.VisibleString);
					}
					else if (obj is SequenceOfAttribute)
					{
						serializerFieldData.IsCollection = true;
						array2 = null;
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Sequence);
					}
					else if (obj is SetOfAttribute)
					{
						serializerFieldData.IsCollection = true;
						array2 = null;
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Set);
					}
					else
					{
						UtcTimeAttribute utcTimeAttribute = obj as UtcTimeAttribute;
						if (utcTimeAttribute != null)
						{
							array2 = new Type[] { typeof(DateTimeOffset) };
							serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.UtcTime);
							if (utcTimeAttribute.TwoDigitYearMax != 0)
							{
								serializerFieldData.TwoDigitYearMax = new int?(utcTimeAttribute.TwoDigitYearMax);
								int? twoDigitYearMax = serializerFieldData.TwoDigitYearMax;
								int num = 99;
								if ((twoDigitYearMax.GetValueOrDefault() < num) & (twoDigitYearMax != null))
								{
									throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on type '{1}' has a [UtcTime] TwoDigitYearMax value ({2}) smaller than the minimum (99).", fieldInfo.Name, fieldInfo.DeclaringType.FullName, serializerFieldData.TwoDigitYearMax));
								}
							}
						}
						else
						{
							GeneralizedTimeAttribute generalizedTimeAttribute = obj as GeneralizedTimeAttribute;
							if (generalizedTimeAttribute == null)
							{
								throw new CryptographicException();
							}
							array2 = new Type[] { typeof(DateTimeOffset) };
							serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.GeneralizedTime);
							serializerFieldData.DisallowGeneralizedTimeFractions = new bool?(generalizedTimeAttribute.DisallowFractions);
						}
					}
				}
				if (!serializerFieldData.IsCollection && Array.IndexOf<Type>(array2, type) < 0)
				{
					string text = "Field '{0}' of type '{1}' has an effective type of '{2}' when one of ({3}) was expected.";
					object[] array3 = new object[4];
					array3[0] = fieldInfo.Name;
					array3[1] = fieldInfo.DeclaringType.Namespace;
					array3[2] = type.FullName;
					array3[3] = string.Join(", ", array2.Select<Type, string>((Type t) => t.FullName));
					throw new AsnSerializationConstraintException(SR.Format(text, array3));
				}
			}
			DefaultValueAttribute defaultValueAttribute = ((fieldInfo != null) ? fieldInfo.GetCustomAttribute<DefaultValueAttribute>(false) : null);
			serializerFieldData.DefaultContents = ((defaultValueAttribute != null) ? defaultValueAttribute.EncodedBytes : null);
			if (serializerFieldData.TagType == null && !serializerFieldData.IsAny)
			{
				if (type == typeof(bool))
				{
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Boolean);
				}
				else if (type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(BigInteger))
				{
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Integer);
				}
				else if (type.IsLayoutSequential)
				{
					serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Sequence);
				}
				else
				{
					if (type == typeof(ReadOnlyMemory<byte>) || type == typeof(string) || type == typeof(DateTimeOffset))
					{
						throw new AsnAmbiguousFieldTypeException(fieldInfo, type);
					}
					if (type == typeof(Oid))
					{
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.ObjectIdentifier);
					}
					else if (type.IsArray)
					{
						serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Sequence);
					}
					else if (type.IsEnum)
					{
						if (typeT.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0)
						{
							serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.BitString);
						}
						else
						{
							serializerFieldData.TagType = new UniversalTagNumber?(UniversalTagNumber.Enumerated);
						}
					}
					else if (fieldInfo != null)
					{
						throw new AsnSerializationConstraintException();
					}
				}
			}
			serializerFieldData.IsOptional = ((fieldInfo != null) ? fieldInfo.GetCustomAttribute<OptionalValueAttribute>(false) : null) != null;
			if (serializerFieldData.IsOptional && !AsnSerializer.CanBeNull(typeT))
			{
				throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on type '{1}' is declared [OptionalValue], but it can not be assigned a null value.", fieldInfo.Name, fieldInfo.DeclaringType.FullName));
			}
			bool flag = AsnSerializer.GetChoiceAttribute(typeT) != null;
			ExpectedTagAttribute expectedTagAttribute = ((fieldInfo != null) ? fieldInfo.GetCustomAttribute<ExpectedTagAttribute>(false) : null);
			if (expectedTagAttribute == null)
			{
				if (flag)
				{
					serializerFieldData.TagType = null;
				}
				serializerFieldData.SpecifiedTag = false;
				serializerFieldData.HasExplicitTag = false;
				serializerFieldData.ExpectedTag = new Asn1Tag(serializerFieldData.TagType.GetValueOrDefault(), false);
				return;
			}
			if (flag && !expectedTagAttribute.ExplicitTag)
			{
				throw new AsnSerializationConstraintException(SR.Format("Field '{0}' on type '{1}' has specified an implicit tag value via [ExpectedTag] for [Choice] type '{2}'. ExplicitTag must be true, or the [ExpectedTag] attribute removed.", fieldInfo.Name, fieldInfo.DeclaringType.FullName, typeT.FullName));
			}
			serializerFieldData.ExpectedTag = new Asn1Tag(expectedTagAttribute.TagClass, expectedTagAttribute.TagValue, false);
			serializerFieldData.HasExplicitTag = expectedTagAttribute.ExplicitTag;
			serializerFieldData.SpecifiedTag = true;
		}

		private static Type UnpackIfNullable(Type typeT)
		{
			return Nullable.GetUnderlyingType(typeT) ?? typeT;
		}

		private static AsnSerializer.Deserializer GetPrimitiveDeserializer(Type typeT, Asn1Tag tag)
		{
			if (typeT == typeof(bool))
			{
				return (AsnReader reader) => reader.ReadBoolean(tag);
			}
			if (typeT == typeof(int))
			{
				return AsnSerializer.TryOrFail<int>(delegate(AsnReader reader, out int value)
				{
					return reader.TryReadInt32(tag, out value);
				});
			}
			if (typeT == typeof(uint))
			{
				return AsnSerializer.TryOrFail<uint>(delegate(AsnReader reader, out uint value)
				{
					return reader.TryReadUInt32(tag, out value);
				});
			}
			if (typeT == typeof(short))
			{
				return AsnSerializer.TryOrFail<short>(delegate(AsnReader reader, out short value)
				{
					return reader.TryReadInt16(tag, out value);
				});
			}
			if (typeT == typeof(ushort))
			{
				return AsnSerializer.TryOrFail<ushort>(delegate(AsnReader reader, out ushort value)
				{
					return reader.TryReadUInt16(tag, out value);
				});
			}
			if (typeT == typeof(byte))
			{
				return AsnSerializer.TryOrFail<byte>(delegate(AsnReader reader, out byte value)
				{
					return reader.TryReadUInt8(tag, out value);
				});
			}
			if (typeT == typeof(sbyte))
			{
				return AsnSerializer.TryOrFail<sbyte>(delegate(AsnReader reader, out sbyte value)
				{
					return reader.TryReadInt8(tag, out value);
				});
			}
			if (typeT == typeof(long))
			{
				return AsnSerializer.TryOrFail<long>(delegate(AsnReader reader, out long value)
				{
					return reader.TryReadInt64(tag, out value);
				});
			}
			if (typeT == typeof(ulong))
			{
				return AsnSerializer.TryOrFail<ulong>(delegate(AsnReader reader, out ulong value)
				{
					return reader.TryReadUInt64(tag, out value);
				});
			}
			throw new AsnSerializationConstraintException(SR.Format("Could not determine how to serialize or deserialize type '{0}'.", typeT.FullName));
		}

		private static AsnSerializer.Serializer GetPrimitiveSerializer(Type typeT, Asn1Tag primitiveTag)
		{
			if (typeT == typeof(bool))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteBoolean(primitiveTag, (bool)value);
				};
			}
			if (typeT == typeof(int))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((int)value));
				};
			}
			if (typeT == typeof(uint))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((ulong)((uint)value)));
				};
			}
			if (typeT == typeof(short))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((short)value));
				};
			}
			if (typeT == typeof(ushort))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((ulong)((ushort)value)));
				};
			}
			if (typeT == typeof(byte))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((ulong)((byte)value)));
				};
			}
			if (typeT == typeof(sbyte))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)((sbyte)value));
				};
			}
			if (typeT == typeof(long))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (long)value);
				};
			}
			if (typeT == typeof(ulong))
			{
				return delegate(object value, AsnWriter writer)
				{
					writer.WriteInteger(primitiveTag, (ulong)value);
				};
			}
			throw new AsnSerializationConstraintException(SR.Format("Could not determine how to serialize or deserialize type '{0}'.", typeT.FullName));
		}

		public static T Deserialize<T>(ReadOnlyMemory<byte> source, AsnEncodingRules ruleSet)
		{
			AsnSerializer.Deserializer deserializer = AsnSerializer.GetDeserializer(typeof(T), null);
			AsnReader asnReader = new AsnReader(source, ruleSet);
			T t = (T)((object)deserializer(asnReader));
			asnReader.ThrowIfNotEmpty();
			return t;
		}

		public static T Deserialize<T>(ReadOnlyMemory<byte> source, AsnEncodingRules ruleSet, out int bytesRead)
		{
			AsnSerializer.Deserializer deserializer = AsnSerializer.GetDeserializer(typeof(T), null);
			AsnReader asnReader = new AsnReader(source, ruleSet);
			ReadOnlyMemory<byte> readOnlyMemory = asnReader.PeekEncodedValue();
			T t = (T)((object)deserializer(asnReader));
			bytesRead = readOnlyMemory.Length;
			return t;
		}

		public static AsnWriter Serialize<T>(T value, AsnEncodingRules ruleSet)
		{
			AsnWriter asnWriter = new AsnWriter(ruleSet);
			AsnWriter asnWriter2;
			try
			{
				AsnSerializer.Serialize<T>(value, asnWriter);
				asnWriter2 = asnWriter;
			}
			catch
			{
				asnWriter.Dispose();
				throw;
			}
			return asnWriter2;
		}

		public static void Serialize<T>(T value, AsnWriter existingWriter)
		{
			if (existingWriter == null)
			{
				throw new ArgumentNullException("existingWriter");
			}
			AsnSerializer.GetSerializer(typeof(T), null)(value, existingWriter);
		}

		private const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly ConcurrentDictionary<Type, FieldInfo[]> s_orderedFields = new ConcurrentDictionary<Type, FieldInfo[]>();

		private delegate void Serializer(object value, AsnWriter writer);

		private delegate object Deserializer(AsnReader reader);

		private delegate bool TryDeserializer<T>(AsnReader reader, out T value);

		private struct SerializerFieldData
		{
			internal bool WasCustomized;

			internal UniversalTagNumber? TagType;

			internal bool? PopulateOidFriendlyName;

			internal bool IsAny;

			internal bool IsCollection;

			internal byte[] DefaultContents;

			internal bool HasExplicitTag;

			internal bool SpecifiedTag;

			internal bool IsOptional;

			internal int? TwoDigitYearMax;

			internal Asn1Tag ExpectedTag;

			internal bool? DisallowGeneralizedTimeFractions;
		}
	}
}
