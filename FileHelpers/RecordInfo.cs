using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FileHelpers.Events;

namespace FileHelpers
{
	internal sealed class RecordInfo : IRecordInfo, ICloneable
	{
		public void RemoveField(string fieldname)
		{
			int fieldIndex = this.GetFieldIndex(fieldname);
			this.Fields[fieldIndex] = null;
			this.Fields = Array.FindAll<FieldBase>(this.Fields, (FieldBase x) => x != null);
		}

		public int SizeHint { get; private set; }

		public Type RecordType { get; private set; }

		public bool IgnoreEmptyLines { get; set; }

		public bool IgnoreEmptySpaces { get; private set; }

		public string CommentMarker { get; set; }

		public int FieldCount
		{
			get
			{
				return this.Fields.Length;
			}
		}

		public FieldBase[] Fields { get; private set; }

		public int IgnoreFirst { get; set; }

		public int IgnoreLast { get; set; }

		public bool NotifyRead { get; private set; }

		public bool NotifyWrite { get; private set; }

		public bool CommentAnyPlace { get; set; }

		public RecordCondition RecordCondition { get; set; }

		public Regex RecordConditionRegEx { get; private set; }

		public string RecordConditionSelector { get; set; }

		public RecordOperations Operations { get; private set; }

		public bool IsDelimited
		{
			get
			{
				return this.Fields[0] is DelimitedField;
			}
		}

		private RecordInfo()
		{
		}

		private RecordInfo(Type recordType)
		{
			this.SizeHint = 32;
			this.RecordConditionSelector = string.Empty;
			this.RecordCondition = RecordCondition.None;
			this.CommentAnyPlace = true;
			this.RecordType = recordType;
			this.InitRecordFields();
			this.Operations = new RecordOperations(this);
		}

		private void InitRecordFields()
		{
			TypedRecordAttribute firstInherited = Attributes.GetFirstInherited<TypedRecordAttribute>(this.RecordType);
			if (firstInherited == null)
			{
				throw new BadUsageException(Messages.Errors.ClassWithOutRecordAttribute.ClassName(this.RecordType.Name).Text);
			}
			if (ReflectionHelper.GetDefaultConstructor(this.RecordType) == null)
			{
				throw new BadUsageException(Messages.Errors.ClassWithOutDefaultConstructor.ClassName(this.RecordType.Name).Text);
			}
			Attributes.WorkWithFirst<IgnoreFirstAttribute>(this.RecordType, delegate(IgnoreFirstAttribute a)
			{
				this.IgnoreFirst = a.NumberOfLines;
			});
			Attributes.WorkWithFirst<IgnoreLastAttribute>(this.RecordType, delegate(IgnoreLastAttribute a)
			{
				this.IgnoreLast = a.NumberOfLines;
			});
			Attributes.WorkWithFirst<IgnoreEmptyLinesAttribute>(this.RecordType, delegate(IgnoreEmptyLinesAttribute a)
			{
				this.IgnoreEmptyLines = true;
				this.IgnoreEmptySpaces = a.IgnoreSpaces;
			});
			Attributes.WorkWithFirst<IgnoreCommentedLinesAttribute>(this.RecordType, delegate(IgnoreCommentedLinesAttribute a)
			{
				this.IgnoreEmptyLines = true;
				this.CommentMarker = a.CommentMarker;
				this.CommentAnyPlace = a.AnyPlace;
			});
			Attributes.WorkWithFirst<ConditionalRecordAttribute>(this.RecordType, delegate(ConditionalRecordAttribute a)
			{
				this.RecordCondition = a.Condition;
				this.RecordConditionSelector = a.ConditionSelector;
				if (this.RecordCondition == RecordCondition.ExcludeIfMatchRegex || this.RecordCondition == RecordCondition.IncludeIfMatchRegex)
				{
					this.RecordConditionRegEx = new Regex(this.RecordConditionSelector, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
				}
			});
			if (RecordInfo.CheckGenericInterface(this.RecordType, typeof(INotifyRead<>), new Type[] { this.RecordType }))
			{
				this.NotifyRead = true;
			}
			if (RecordInfo.CheckGenericInterface(this.RecordType, typeof(INotifyWrite<>), new Type[] { this.RecordType }))
			{
				this.NotifyWrite = true;
			}
			List<FieldInfo> list = new List<FieldInfo>(ReflectionHelper.RecursiveGetFields(this.RecordType));
			this.Fields = RecordInfo.CreateCoreFields(list, firstInherited);
			if (this.FieldCount == 0)
			{
				throw new BadUsageException(Messages.Errors.ClassWithOutFields.ClassName(this.RecordType.Name).Text);
			}
			if (firstInherited is FixedLengthRecordAttribute)
			{
				this.SizeHint = 0;
				for (int i = 0; i < this.FieldCount; i++)
				{
					this.SizeHint += ((FixedLengthField)this.Fields[i]).FieldLength;
				}
			}
		}

		private static FieldBase[] CreateCoreFields(IList<FieldInfo> fields, TypedRecordAttribute recordAttribute)
		{
			List<FieldBase> list = new List<FieldBase>();
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldBase fieldBase = FieldBase.CreateField(fields[i], recordAttribute);
				if (fieldBase != null)
				{
					if (fieldBase.FieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), false))
					{
						num++;
					}
					else
					{
						num2++;
					}
					list.Add(fieldBase);
					if (list.Count > 1)
					{
						RecordInfo.CheckForOrderProblems(fieldBase, list);
					}
				}
			}
			if (num > 0 && num2 > 0 && RecordInfo.SumOrder(list) == 0)
			{
				throw new BadUsageException(Messages.Errors.MixOfStandardAndAutoPropertiesFields.ClassName(list[0].FieldInfo.DeclaringType.Name).Text);
			}
			RecordInfo.SortFieldsByOrder(list);
			if (list.Count > 0)
			{
				list[0].IsFirst = true;
				list[list.Count - 1].IsLast = true;
			}
			RecordInfo.CheckForOptionalAndArrayProblems(list);
			return list.ToArray();
		}

		private static int SumOrder(List<FieldBase> fields)
		{
			int num = 0;
			foreach (FieldBase fieldBase in fields)
			{
				num += fieldBase.FieldOrder ?? 0;
			}
			return num;
		}

		private static void CheckForOptionalAndArrayProblems(List<FieldBase> resFields)
		{
			for (int i = 0; i < resFields.Count; i++)
			{
				FieldBase fieldBase = resFields[i];
				if (i >= 1)
				{
					FieldBase fieldBase2 = resFields[i - 1];
					fieldBase2.NextIsOptional = fieldBase.IsOptional;
					if (fieldBase2.IsOptional && !fieldBase.IsOptional && !fieldBase.InNewLine)
					{
						throw new BadUsageException(Messages.Errors.ExpectingFieldOptional.FieldName(fieldBase2.FieldInfo.Name).Text);
					}
					if (fieldBase2.IsArray)
					{
						if (fieldBase2.ArrayMinLength == -2147483648)
						{
							throw new BadUsageException(Messages.Errors.MissingFieldArrayLenghtInNotLastField.FieldName(fieldBase2.FieldInfo.Name).Text);
						}
						if (fieldBase2.ArrayMinLength != fieldBase2.ArrayMaxLength)
						{
							throw new BadUsageException(Messages.Errors.SameMinMaxLengthForArrayNotLastField.FieldName(fieldBase2.FieldInfo.Name).Text);
						}
					}
				}
			}
		}

		private static void SortFieldsByOrder(List<FieldBase> resFields)
		{
			if (resFields.FindAll((FieldBase x) => x.FieldOrder != null).Count > 0)
			{
				resFields.Sort((FieldBase x, FieldBase y) => x.FieldOrder.Value.CompareTo(y.FieldOrder.Value));
			}
		}

		private static void CheckForOrderProblems(FieldBase currentField, List<FieldBase> resFields)
		{
			if (currentField.FieldOrder != null)
			{
				FieldBase fieldBase = resFields.Find((FieldBase x) => x.FieldOrder == null);
				if (fieldBase != null)
				{
					throw new BadUsageException(Messages.Errors.PartialFieldOrder.FieldName(fieldBase.FieldInfo.Name).Text);
				}
				FieldBase fieldBase2 = resFields.Find((FieldBase x) => x != currentField && x.FieldOrder == currentField.FieldOrder);
				if (fieldBase2 != null)
				{
					throw new BadUsageException(Messages.Errors.SameFieldOrder.FieldName1(currentField.FieldInfo.Name).FieldName2(fieldBase2.FieldInfo.Name).Text);
				}
			}
			else
			{
				FieldBase fieldBase3 = resFields.Find((FieldBase x) => x.FieldOrder != null);
				if (fieldBase3 != null)
				{
					string text = FieldBase.AutoPropertyName(currentField.FieldInfo);
					if (string.IsNullOrEmpty(text))
					{
						throw new BadUsageException(Messages.Errors.PartialFieldOrder.FieldName(currentField.FieldInfo.Name).Text);
					}
					throw new BadUsageException(Messages.Errors.PartialFieldOrderInAutoProperty.PropertyName(text).Text);
				}
			}
		}

		public int GetFieldIndex(string fieldName)
		{
			if (this.mMapFieldIndex == null)
			{
				this.mMapFieldIndex = new Dictionary<string, int>(this.FieldCount, StringComparer.Ordinal);
				for (int i = 0; i < this.FieldCount; i++)
				{
					this.mMapFieldIndex.Add(this.Fields[i].FieldInfo.Name, i);
					if (this.Fields[i].FieldInfo.Name != this.Fields[i].FieldFriendlyName)
					{
						this.mMapFieldIndex.Add(this.Fields[i].FieldFriendlyName, i);
					}
				}
			}
			int num;
			if (!this.mMapFieldIndex.TryGetValue(fieldName, out num))
			{
				throw new BadUsageException(Messages.Errors.FieldNotFound.FieldName(fieldName).ClassName(this.RecordType.Name).Text);
			}
			return num;
		}

		public FieldInfo GetFieldInfo(string name)
		{
			foreach (FieldBase fieldBase in this.Fields)
			{
				if (fieldBase.FieldInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return fieldBase.FieldInfo;
				}
			}
			return null;
		}

		public static IRecordInfo Resolve(Type type)
		{
			return RecordInfo.RecordInfoFactory.Resolve(type);
		}

		public object Clone()
		{
			RecordInfo recordInfo = new RecordInfo();
			recordInfo.CommentAnyPlace = this.CommentAnyPlace;
			recordInfo.CommentMarker = this.CommentMarker;
			recordInfo.IgnoreEmptyLines = this.IgnoreEmptyLines;
			recordInfo.IgnoreEmptySpaces = this.IgnoreEmptySpaces;
			recordInfo.IgnoreFirst = this.IgnoreFirst;
			recordInfo.IgnoreLast = this.IgnoreLast;
			recordInfo.NotifyRead = this.NotifyRead;
			recordInfo.NotifyWrite = this.NotifyWrite;
			recordInfo.Operations = this.Operations.Clone(recordInfo);
			recordInfo.RecordCondition = this.RecordCondition;
			recordInfo.RecordConditionRegEx = this.RecordConditionRegEx;
			recordInfo.RecordConditionSelector = this.RecordConditionSelector;
			recordInfo.RecordType = this.RecordType;
			recordInfo.SizeHint = this.SizeHint;
			recordInfo.Fields = new FieldBase[this.Fields.Length];
			for (int i = 0; i < this.Fields.Length; i++)
			{
				recordInfo.Fields[i] = (FieldBase)this.Fields[i].Clone();
			}
			return recordInfo;
		}

		public static bool CheckGenericInterface(Type type, Type interfaceType, params Type[] genericsArgs)
		{
			foreach (Type type2 in type.GetInterfaces())
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition() == interfaceType)
				{
					Type[] genericArguments = type2.GetGenericArguments();
					if (genericArguments.Length == genericsArgs.Length)
					{
						bool flag = false;
						for (int j = 0; j < genericArguments.Length; j++)
						{
							if (genericArguments[j] != genericsArgs[j])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return true;
						}
					}
					throw new BadUsageException(string.Concat(new object[]
					{
						"The class: ",
						type.Name,
						" must implement the interface ",
						interfaceType.MakeGenericType(genericsArgs),
						" and not ",
						type2
					}));
				}
			}
			return false;
		}

		private Dictionary<string, int> mMapFieldIndex;

		private static class RecordInfoFactory
		{
			public static IRecordInfo Resolve(Type type)
			{
				IRecordInfo recordInfo2;
				lock (type)
				{
					RecordInfo recordInfo;
					lock (RecordInfo.RecordInfoFactory.mRecordInfoCache)
					{
						if (RecordInfo.RecordInfoFactory.mRecordInfoCache.TryGetValue(type, out recordInfo))
						{
							return (IRecordInfo)recordInfo.Clone();
						}
					}
					recordInfo = new RecordInfo(type);
					lock (RecordInfo.RecordInfoFactory.mRecordInfoCache)
					{
						if (!RecordInfo.RecordInfoFactory.mRecordInfoCache.ContainsKey(type))
						{
							RecordInfo.RecordInfoFactory.mRecordInfoCache.Add(type, recordInfo);
						}
					}
					recordInfo2 = (IRecordInfo)recordInfo.Clone();
				}
				return recordInfo2;
			}

			private static readonly Dictionary<Type, RecordInfo> mRecordInfoCache = new Dictionary<Type, RecordInfo>();
		}
	}
}
