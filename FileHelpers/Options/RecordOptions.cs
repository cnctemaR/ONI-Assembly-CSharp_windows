using System;
using System.ComponentModel;
using System.Diagnostics;

namespace FileHelpers.Options
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class RecordOptions
	{
		internal RecordOptions(IRecordInfo info)
		{
			this.mRecordInfo = info;
			this.mRecordConditionInfo = new RecordOptions.RecordConditionInfo(info);
			this.mIgnoreCommentInfo = new RecordOptions.IgnoreCommentInfo(info);
		}

		public FieldBaseCollection Fields
		{
			get
			{
				return new FieldBaseCollection(this.mRecordInfo.Fields);
			}
		}

		public void RemoveField(string fieldname)
		{
			this.mRecordInfo.RemoveField(fieldname);
		}

		public int FieldCount
		{
			get
			{
				return this.mRecordInfo.FieldCount;
			}
		}

		public string[] FieldsNames
		{
			get
			{
				if (this.mFieldNames == null)
				{
					this.mFieldNames = new string[this.mRecordInfo.FieldCount];
					for (int i = 0; i < this.mFieldNames.Length; i++)
					{
						this.mFieldNames[i] = this.mRecordInfo.Fields[i].FieldFriendlyName;
					}
				}
				return this.mFieldNames;
			}
		}

		public Type[] FieldsTypes
		{
			get
			{
				if (this.mFieldTypes == null)
				{
					this.mFieldTypes = new Type[this.mRecordInfo.FieldCount];
					for (int i = 0; i < this.mFieldTypes.Length; i++)
					{
						this.mFieldTypes[i] = this.mRecordInfo.Fields[i].FieldInfo.FieldType;
					}
				}
				return this.mFieldTypes;
			}
		}

		public int IgnoreFirstLines
		{
			get
			{
				return this.mRecordInfo.IgnoreFirst;
			}
			set
			{
				ExHelper.PositiveValue(value);
				this.mRecordInfo.IgnoreFirst = value;
			}
		}

		public int IgnoreLastLines
		{
			get
			{
				return this.mRecordInfo.IgnoreLast;
			}
			set
			{
				ExHelper.PositiveValue(value);
				this.mRecordInfo.IgnoreLast = value;
			}
		}

		public bool IgnoreEmptyLines
		{
			get
			{
				return this.mRecordInfo.IgnoreEmptyLines;
			}
			set
			{
				this.mRecordInfo.IgnoreEmptyLines = value;
			}
		}

		public RecordOptions.RecordConditionInfo RecordCondition
		{
			get
			{
				return this.mRecordConditionInfo;
			}
		}

		public RecordOptions.IgnoreCommentInfo IgnoreCommentedLines
		{
			get
			{
				return this.mIgnoreCommentInfo;
			}
		}

		public string RecordToString(object record)
		{
			return this.mRecordInfo.Operations.RecordToString(record);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal IRecordInfo mRecordInfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string[] mFieldNames;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Type[] mFieldTypes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly RecordOptions.RecordConditionInfo mRecordConditionInfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly RecordOptions.IgnoreCommentInfo mIgnoreCommentInfo;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public sealed class RecordConditionInfo
		{
			internal RecordConditionInfo(IRecordInfo ri)
			{
				this.mRecordInfo = ri;
			}

			public RecordCondition Condition
			{
				get
				{
					return this.mRecordInfo.RecordCondition;
				}
				set
				{
					this.mRecordInfo.RecordCondition = value;
				}
			}

			public string Selector
			{
				get
				{
					return this.mRecordInfo.RecordConditionSelector;
				}
				set
				{
					this.mRecordInfo.RecordConditionSelector = value;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly IRecordInfo mRecordInfo;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public sealed class IgnoreCommentInfo
		{
			internal IgnoreCommentInfo(IRecordInfo ri)
			{
				this.mRecordInfo = ri;
			}

			public string CommentMarker
			{
				get
				{
					return this.mRecordInfo.CommentMarker;
				}
				set
				{
					if (value != null)
					{
						value = value.Trim();
					}
					this.mRecordInfo.CommentMarker = value;
				}
			}

			public bool InAnyPlace
			{
				get
				{
					return this.mRecordInfo.CommentAnyPlace;
				}
				set
				{
					this.mRecordInfo.CommentAnyPlace = value;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly IRecordInfo mRecordInfo;
		}
	}
}
