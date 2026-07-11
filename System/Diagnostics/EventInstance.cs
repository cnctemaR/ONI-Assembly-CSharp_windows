using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	public class EventInstance
	{
		public EventInstance(long instanceId, int categoryId)
			: this(instanceId, categoryId, EventLogEntryType.Information)
		{
		}

		public EventInstance(long instanceId, int categoryId, EventLogEntryType entryType)
		{
			this.InstanceId = instanceId;
			this.CategoryId = categoryId;
			this.EntryType = entryType;
		}

		public int CategoryId
		{
			get
			{
				return this._categoryId;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._categoryId = value;
			}
		}

		public EventLogEntryType EntryType
		{
			get
			{
				return this._entryType;
			}
			set
			{
				if (!Enum.IsDefined(typeof(EventLogEntryType), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(EventLogEntryType));
				}
				this._entryType = value;
			}
		}

		public long InstanceId
		{
			get
			{
				return this._instanceId;
			}
			set
			{
				if (value < 0L || value > (long)((ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._instanceId = value;
			}
		}

		private int _categoryId;

		private EventLogEntryType _entryType;

		private long _instanceId;
	}
}
