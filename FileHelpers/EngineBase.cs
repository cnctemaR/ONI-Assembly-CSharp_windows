using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using FileHelpers.Events;
using FileHelpers.Options;

namespace FileHelpers
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class EngineBase
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal IRecordInfo RecordInfo { get; private set; }

		internal EngineBase(Type recordType)
			: this(recordType, Encoding.Default)
		{
		}

		internal EngineBase(Type recordType, Encoding encoding)
		{
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			this.mEncoding = Encoding.Default;
			this.mErrorManager = new ErrorManager();
			base..ctor();
			if (recordType == null)
			{
				throw new BadUsageException(Messages.Errors.NullRecordClass.Text);
			}
			if (recordType.IsValueType)
			{
				throw new BadUsageException(Messages.Errors.StructRecordClass.RecordType(recordType.Name).Text);
			}
			this.mRecordType = recordType;
			this.RecordInfo = FileHelpers.RecordInfo.Resolve(recordType);
			this.mEncoding = encoding;
			this.CreateRecordOptions();
		}

		internal EngineBase(RecordInfo ri)
		{
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			this.mEncoding = Encoding.Default;
			this.mErrorManager = new ErrorManager();
			base..ctor();
			this.mRecordType = ri.RecordType;
			this.RecordInfo = ri;
			this.CreateRecordOptions();
		}

		public int LineNumber
		{
			get
			{
				return this.mLineNumber;
			}
		}

		public int TotalRecords
		{
			get
			{
				return this.mTotalRecords;
			}
		}

		public string GetFileHeader()
		{
			string text = "\t";
			if (this.RecordInfo.IsDelimited)
			{
				text = ((DelimitedRecordOptions)this.Options).Delimiter;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.RecordInfo.Fields.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(text);
				}
				FieldBase fieldBase = this.RecordInfo.Fields[i];
				stringBuilder.Append(fieldBase.FieldFriendlyName);
			}
			return stringBuilder.ToString();
		}

		public Type RecordType
		{
			get
			{
				return this.mRecordType;
			}
		}

		public string HeaderText
		{
			get
			{
				return this.mHeaderText;
			}
			set
			{
				this.mHeaderText = value;
			}
		}

		public string FooterText
		{
			get
			{
				return this.mFooterText;
			}
			set
			{
				this.mFooterText = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.mEncoding;
			}
			set
			{
				this.mEncoding = value;
			}
		}

		public ErrorManager ErrorManager
		{
			get
			{
				return this.mErrorManager;
			}
		}

		public ErrorMode ErrorMode
		{
			get
			{
				return this.mErrorManager.ErrorMode;
			}
			set
			{
				this.mErrorManager.ErrorMode = value;
			}
		}

		internal void ResetFields()
		{
			this.mLineNumber = 0;
			this.mErrorManager.ClearErrors();
			this.mTotalRecords = 0;
		}

		public event EventHandler<ProgressEventArgs> Progress;

		protected bool MustNotifyProgress
		{
			get
			{
				return this.Progress != null;
			}
		}

		protected void OnProgress(ProgressEventArgs e)
		{
			if (this.Progress == null)
			{
				return;
			}
			this.Progress(this, e);
		}

		private void CreateRecordOptions()
		{
			if (this.RecordInfo.IsDelimited)
			{
				this.Options = new DelimitedRecordOptions(this.RecordInfo);
				return;
			}
			this.Options = new FixedRecordOptions(this.RecordInfo);
		}

		public RecordOptions Options { get; private set; }

		internal const int DefaultReadBufferSize = 102400;

		internal const int DefaultWriteBufferSize = 102400;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal int mLineNumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal int mTotalRecords;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type mRecordType;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal string mHeaderText;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected string mFooterText;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected Encoding mEncoding;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected ErrorManager mErrorManager;
	}
}
