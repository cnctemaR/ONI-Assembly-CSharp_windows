using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class EventAttribute : Attribute
	{
		public EventAttribute(int eventId)
		{
			this.EventId = eventId;
			this.Level = EventLevel.Informational;
			this.m_opcodeSet = false;
		}

		public int EventId { get; private set; }

		public EventLevel Level { get; set; }

		public EventKeywords Keywords { get; set; }

		public EventOpcode Opcode
		{
			get
			{
				return this.m_opcode;
			}
			set
			{
				this.m_opcode = value;
				this.m_opcodeSet = true;
			}
		}

		internal bool IsOpcodeSet
		{
			get
			{
				return this.m_opcodeSet;
			}
		}

		public EventTask Task { get; set; }

		public EventChannel Channel { get; set; }

		public byte Version { get; set; }

		public string Message { get; set; }

		public EventTags Tags { get; set; }

		public EventActivityOptions ActivityOptions { get; set; }

		private EventOpcode m_opcode;

		private bool m_opcodeSet;
	}
}
