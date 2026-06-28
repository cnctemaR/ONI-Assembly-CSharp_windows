using System;
using System.ComponentModel;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class EventEngineBase<T> : EngineBase where T : class
	{
		protected EventEngineBase(Type recordType)
			: base(recordType)
		{
		}

		protected EventEngineBase(Type recordType, Encoding encoding)
			: base(recordType, encoding)
		{
		}

		internal EventEngineBase(RecordInfo ri)
			: base(ri)
		{
		}

		public event BeforeReadHandler<T> BeforeReadRecord;

		public event AfterReadHandler<T> AfterReadRecord;

		public event BeforeWriteHandler<T> BeforeWriteRecord;

		public event AfterWriteHandler<T> AfterWriteRecord;

		protected bool MustNotifyRead
		{
			get
			{
				return this.BeforeReadRecord != null || this.AfterReadRecord != null || base.RecordInfo.NotifyRead;
			}
		}

		protected bool MustNotifyWrite
		{
			get
			{
				return this.BeforeWriteRecord != null || this.AfterWriteRecord != null || base.RecordInfo.NotifyWrite;
			}
		}

		protected bool OnBeforeReadRecord(BeforeReadEventArgs<T> e)
		{
			if (base.RecordInfo.NotifyRead)
			{
				((INotifyRead<T>)((object)e.Record)).BeforeRead(e);
			}
			if (this.BeforeReadRecord != null)
			{
				this.BeforeReadRecord(this, e);
			}
			return e.SkipThisRecord;
		}

		protected bool OnAfterReadRecord(string line, T record, bool lineChanged, int lineNumber)
		{
			AfterReadEventArgs<T> afterReadEventArgs = new AfterReadEventArgs<T>(this, line, lineChanged, record, lineNumber);
			if (base.RecordInfo.NotifyRead)
			{
				((INotifyRead<T>)((object)record)).AfterRead(afterReadEventArgs);
			}
			if (this.AfterReadRecord != null)
			{
				this.AfterReadRecord(this, afterReadEventArgs);
			}
			return afterReadEventArgs.SkipThisRecord;
		}

		protected bool OnBeforeWriteRecord(T record, int lineNumber)
		{
			BeforeWriteEventArgs<T> beforeWriteEventArgs = new BeforeWriteEventArgs<T>(this, record, lineNumber);
			if (base.RecordInfo.NotifyWrite)
			{
				((INotifyWrite<T>)((object)record)).BeforeWrite(beforeWriteEventArgs);
			}
			if (this.BeforeWriteRecord != null)
			{
				this.BeforeWriteRecord(this, beforeWriteEventArgs);
			}
			return beforeWriteEventArgs.SkipThisRecord;
		}

		protected string OnAfterWriteRecord(string line, T record)
		{
			AfterWriteEventArgs<T> afterWriteEventArgs = new AfterWriteEventArgs<T>(this, record, base.LineNumber, line);
			if (base.RecordInfo.NotifyWrite)
			{
				((INotifyWrite<T>)((object)record)).AfterWrite(afterWriteEventArgs);
			}
			if (this.AfterWriteRecord != null)
			{
				this.AfterWriteRecord(this, afterWriteEventArgs);
			}
			return afterWriteEventArgs.RecordLine;
		}
	}
}
