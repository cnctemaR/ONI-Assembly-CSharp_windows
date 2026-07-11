using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security;
using Unity;

namespace System.Diagnostics.Tracing
{
	public class EventWrittenEventArgs : EventArgs
	{
		public string EventName
		{
			get
			{
				if (this.m_eventName != null || this.EventId < 0)
				{
					return this.m_eventName;
				}
				return this.m_eventSource.m_eventData[this.EventId].Name;
			}
			internal set
			{
				this.m_eventName = value;
			}
		}

		public int EventId { get; internal set; }

		public Guid ActivityId
		{
			[SecurityCritical]
			get
			{
				return EventSource.CurrentThreadActivityId;
			}
		}

		public Guid RelatedActivityId
		{
			[SecurityCritical]
			get;
			internal set; }

		public ReadOnlyCollection<object> Payload { get; internal set; }

		public ReadOnlyCollection<string> PayloadNames
		{
			get
			{
				if (this.m_payloadNames == null)
				{
					List<string> list = new List<string>();
					foreach (ParameterInfo parameterInfo in this.m_eventSource.m_eventData[this.EventId].Parameters)
					{
						list.Add(parameterInfo.Name);
					}
					this.m_payloadNames = new ReadOnlyCollection<string>(list);
				}
				return this.m_payloadNames;
			}
			internal set
			{
				this.m_payloadNames = value;
			}
		}

		public EventSource EventSource
		{
			get
			{
				return this.m_eventSource;
			}
		}

		public EventKeywords Keywords
		{
			get
			{
				if (this.EventId < 0)
				{
					return this.m_keywords;
				}
				return (EventKeywords)this.m_eventSource.m_eventData[this.EventId].Descriptor.Keywords;
			}
		}

		public EventOpcode Opcode
		{
			get
			{
				if (this.EventId < 0)
				{
					return this.m_opcode;
				}
				return (EventOpcode)this.m_eventSource.m_eventData[this.EventId].Descriptor.Opcode;
			}
		}

		public EventTask Task
		{
			get
			{
				if (this.EventId < 0)
				{
					return EventTask.None;
				}
				return (EventTask)this.m_eventSource.m_eventData[this.EventId].Descriptor.Task;
			}
		}

		public EventTags Tags
		{
			get
			{
				if (this.EventId < 0)
				{
					return this.m_tags;
				}
				return this.m_eventSource.m_eventData[this.EventId].Tags;
			}
		}

		public string Message
		{
			get
			{
				if (this.EventId < 0)
				{
					return this.m_message;
				}
				return this.m_eventSource.m_eventData[this.EventId].Message;
			}
			internal set
			{
				this.m_message = value;
			}
		}

		public EventChannel Channel
		{
			get
			{
				if (this.EventId < 0)
				{
					return EventChannel.None;
				}
				return (EventChannel)this.m_eventSource.m_eventData[this.EventId].Descriptor.Channel;
			}
		}

		public byte Version
		{
			get
			{
				if (this.EventId < 0)
				{
					return 0;
				}
				return this.m_eventSource.m_eventData[this.EventId].Descriptor.Version;
			}
		}

		public EventLevel Level
		{
			get
			{
				if (this.EventId < 0)
				{
					return EventLevel.LogAlways;
				}
				return (EventLevel)this.m_eventSource.m_eventData[this.EventId].Descriptor.Level;
			}
		}

		internal EventWrittenEventArgs(EventSource eventSource)
		{
			this.m_eventSource = eventSource;
		}

		internal EventWrittenEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string m_message;

		private string m_eventName;

		private EventSource m_eventSource;

		private ReadOnlyCollection<string> m_payloadNames;

		internal EventTags m_tags;

		internal EventOpcode m_opcode;

		internal EventKeywords m_keywords;
	}
}
