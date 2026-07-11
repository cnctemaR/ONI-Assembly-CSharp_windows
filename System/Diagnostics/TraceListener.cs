using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	public abstract class TraceListener : MarshalByRefObject, IDisposable
	{
		protected TraceListener()
			: this(string.Empty)
		{
		}

		protected TraceListener(string name)
		{
			this.Name = name;
		}

		public int IndentLevel
		{
			get
			{
				return this.indentLevel;
			}
			set
			{
				this.indentLevel = value;
			}
		}

		public int IndentSize
		{
			get
			{
				return this.indentSize;
			}
			set
			{
				this.indentSize = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		protected bool NeedIndent
		{
			get
			{
				return this.needIndent;
			}
			set
			{
				this.needIndent = value;
			}
		}

		[global::System.MonoLimitation("This property exists but is never considered.")]
		public virtual bool IsThreadSafe
		{
			get
			{
				return false;
			}
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual void Fail(string message)
		{
			this.Fail(message, string.Empty);
		}

		public virtual void Fail(string message, string detailMessage)
		{
			this.WriteLine("---- DEBUG ASSERTION FAILED ----");
			this.WriteLine("---- Assert Short Message ----");
			this.WriteLine(message);
			this.WriteLine("---- Assert Long Message ----");
			this.WriteLine(detailMessage);
			this.WriteLine(string.Empty);
		}

		public virtual void Flush()
		{
		}

		public virtual void Write(object o)
		{
			this.Write(o.ToString());
		}

		public abstract void Write(string message);

		public virtual void Write(object o, string category)
		{
			this.Write(o.ToString(), category);
		}

		public virtual void Write(string message, string category)
		{
			this.Write(category + ": " + message);
		}

		protected virtual void WriteIndent()
		{
			this.NeedIndent = false;
			string text = new string(' ', this.IndentLevel * this.IndentSize);
			this.Write(text);
		}

		public virtual void WriteLine(object o)
		{
			this.WriteLine(o.ToString());
		}

		public abstract void WriteLine(string message);

		public virtual void WriteLine(object o, string category)
		{
			this.WriteLine(o.ToString(), category);
		}

		public virtual void WriteLine(string message, string category)
		{
			this.WriteLine(category + ": " + message);
		}

		internal static string FormatArray(ICollection list, string joiner)
		{
			string[] array = new string[list.Count];
			int num = 0;
			foreach (object obj in list)
			{
				array[num++] = ((obj == null) ? string.Empty : obj.ToString());
			}
			return string.Join(joiner, array);
		}

		[ComVisible(false)]
		public virtual void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
			{
				return;
			}
			this.WriteLine(string.Format("{0} {1}: {2} : {3}", new object[] { source, eventType, id, data }));
			if (eventCache == null)
			{
				return;
			}
			if ((this.TraceOutputOptions & TraceOptions.ProcessId) != TraceOptions.None)
			{
				this.WriteLine("    ProcessId=" + eventCache.ProcessId);
			}
			if ((this.TraceOutputOptions & TraceOptions.LogicalOperationStack) != TraceOptions.None)
			{
				this.WriteLine("    LogicalOperationStack=" + TraceListener.FormatArray(eventCache.LogicalOperationStack, ", "));
			}
			if ((this.TraceOutputOptions & TraceOptions.ThreadId) != TraceOptions.None)
			{
				this.WriteLine("    ThreadId=" + eventCache.ThreadId);
			}
			if ((this.TraceOutputOptions & TraceOptions.DateTime) != TraceOptions.None)
			{
				this.WriteLine("    DateTime=" + eventCache.DateTime.ToString("o"));
			}
			if ((this.TraceOutputOptions & TraceOptions.Timestamp) != TraceOptions.None)
			{
				this.WriteLine("    Timestamp=" + eventCache.Timestamp);
			}
			if ((this.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None)
			{
				this.WriteLine("    Callstack=" + eventCache.Callstack);
			}
		}

		[ComVisible(false)]
		public virtual void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				return;
			}
			this.TraceData(eventCache, source, eventType, id, TraceListener.FormatArray(data, " "));
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
		{
			this.TraceEvent(eventCache, source, eventType, id, null);
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			this.TraceData(eventCache, source, eventType, id, message);
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			this.TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
		}

		[ComVisible(false)]
		public virtual void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			this.TraceEvent(eventCache, source, TraceEventType.Transfer, id, string.Format("{0}, relatedActivityId={1}", message, relatedActivityId));
		}

		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		public global::System.Collections.Specialized.StringDictionary Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		[ComVisible(false)]
		public TraceFilter Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				this.filter = value;
			}
		}

		[ComVisible(false)]
		public TraceOptions TraceOutputOptions
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		[ThreadStatic]
		private int indentLevel;

		[ThreadStatic]
		private int indentSize = 4;

		[ThreadStatic]
		private global::System.Collections.Specialized.StringDictionary attributes = new global::System.Collections.Specialized.StringDictionary();

		[ThreadStatic]
		private TraceFilter filter;

		[ThreadStatic]
		private TraceOptions options;

		private string name;

		private bool needIndent = true;
	}
}
