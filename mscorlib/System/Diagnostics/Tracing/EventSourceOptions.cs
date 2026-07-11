using System;

namespace System.Diagnostics.Tracing
{
	public struct EventSourceOptions
	{
		public EventLevel Level
		{
			get
			{
				return (EventLevel)this.level;
			}
			set
			{
				this.level = checked((byte)value);
				this.valuesSet |= 4;
			}
		}

		public EventOpcode Opcode
		{
			get
			{
				return (EventOpcode)this.opcode;
			}
			set
			{
				this.opcode = checked((byte)value);
				this.valuesSet |= 8;
			}
		}

		internal bool IsOpcodeSet
		{
			get
			{
				return (this.valuesSet & 8) > 0;
			}
		}

		public EventKeywords Keywords
		{
			get
			{
				return this.keywords;
			}
			set
			{
				this.keywords = value;
				this.valuesSet |= 1;
			}
		}

		public EventTags Tags
		{
			get
			{
				return this.tags;
			}
			set
			{
				this.tags = value;
				this.valuesSet |= 2;
			}
		}

		public EventActivityOptions ActivityOptions
		{
			get
			{
				return this.activityOptions;
			}
			set
			{
				this.activityOptions = value;
				this.valuesSet |= 16;
			}
		}

		internal EventKeywords keywords;

		internal EventTags tags;

		internal EventActivityOptions activityOptions;

		internal byte level;

		internal byte opcode;

		internal byte valuesSet;

		internal const byte keywordsSet = 1;

		internal const byte tagsSet = 2;

		internal const byte levelSet = 4;

		internal const byte opcodeSet = 8;

		internal const byte activityOptionsSet = 16;
	}
}
