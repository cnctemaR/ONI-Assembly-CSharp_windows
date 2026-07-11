using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Diagnostics
{
	public class TraceSource
	{
		public TraceSource(string name)
			: this(name, SourceLevels.Off)
		{
		}

		public TraceSource(string name, SourceLevels sourceLevels)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Hashtable hashtable = DiagnosticsConfiguration.Settings["sources"] as Hashtable;
			TraceSourceInfo traceSourceInfo = ((hashtable == null) ? null : (hashtable[name] as TraceSourceInfo));
			this.source_switch = new SourceSwitch(name);
			if (traceSourceInfo == null)
			{
				this.listeners = new TraceListenerCollection();
			}
			else
			{
				this.source_switch.Level = traceSourceInfo.Levels;
				this.listeners = traceSourceInfo.Listeners;
			}
		}

		public global::System.Collections.Specialized.StringDictionary Attributes
		{
			get
			{
				return this.source_switch.Attributes;
			}
		}

		public TraceListenerCollection Listeners
		{
			get
			{
				return this.listeners;
			}
		}

		public string Name
		{
			get
			{
				return this.source_switch.DisplayName;
			}
		}

		public SourceSwitch Switch
		{
			get
			{
				return this.source_switch;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.source_switch = value;
			}
		}

		public void Close()
		{
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.Close();
				}
			}
		}

		public void Flush()
		{
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.Flush();
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, object data)
		{
			if (!this.source_switch.ShouldTrace(eventType))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceData(null, this.Name, eventType, id, data);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			if (!this.source_switch.ShouldTrace(eventType))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceData(null, this.Name, eventType, id, data);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id)
		{
			if (!this.source_switch.ShouldTrace(eventType))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceEvent(null, this.Name, eventType, id);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string message)
		{
			if (!this.source_switch.ShouldTrace(eventType))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceEvent(null, this.Name, eventType, id, message);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			if (!this.source_switch.ShouldTrace(eventType))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceEvent(null, this.Name, eventType, id, format, args);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceInformation(string format)
		{
		}

		[Conditional("TRACE")]
		public void TraceInformation(string format, params object[] args)
		{
		}

		[Conditional("TRACE")]
		public void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			if (!this.source_switch.ShouldTrace(TraceEventType.Transfer))
			{
				return;
			}
			object syncRoot = ((ICollection)this.listeners).SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj;
					traceListener.TraceTransfer(null, this.Name, id, message, relatedActivityId);
				}
			}
		}

		protected virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		private SourceSwitch source_switch;

		private TraceListenerCollection listeners;
	}
}
