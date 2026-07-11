using System;
using System.Collections;
using System.Threading;

namespace System.Runtime.Collections
{
	internal class HopperCache
	{
		public HopperCache(int hopperSize, bool weak)
		{
			this.hopperSize = hopperSize;
			this.weak = weak;
			this.outstandingHopper = new Hashtable(hopperSize * 2);
			this.strongHopper = new Hashtable(hopperSize * 2);
			this.limitedHopper = new Hashtable(hopperSize * 2);
		}

		public void Add(object key, object value)
		{
			if (this.weak && value != DBNull.Value)
			{
				value = new WeakReference(value);
			}
			if (this.strongHopper.Count >= this.hopperSize * 2)
			{
				Hashtable hashtable = this.limitedHopper;
				hashtable.Clear();
				hashtable.Add(key, value);
				try
				{
					return;
				}
				finally
				{
					this.limitedHopper = this.strongHopper;
					this.strongHopper = hashtable;
				}
			}
			this.strongHopper[key] = value;
		}

		public object GetValue(object syncObject, object key)
		{
			HopperCache.LastHolder lastHolder = this.mruEntry;
			WeakReference weakReference;
			object obj;
			if (lastHolder != null && key.Equals(lastHolder.Key))
			{
				if (!this.weak || (weakReference = lastHolder.Value as WeakReference) == null)
				{
					return lastHolder.Value;
				}
				obj = weakReference.Target;
				if (obj != null)
				{
					return obj;
				}
				this.mruEntry = null;
			}
			object obj2 = this.outstandingHopper[key];
			obj = ((this.weak && (weakReference = obj2 as WeakReference) != null) ? weakReference.Target : obj2);
			if (obj != null)
			{
				this.mruEntry = new HopperCache.LastHolder(key, obj2);
				return obj;
			}
			obj2 = this.strongHopper[key];
			obj = ((this.weak && (weakReference = obj2 as WeakReference) != null) ? weakReference.Target : obj2);
			if (obj == null)
			{
				obj2 = this.limitedHopper[key];
				obj = ((this.weak && (weakReference = obj2 as WeakReference) != null) ? weakReference.Target : obj2);
				if (obj == null)
				{
					return null;
				}
			}
			this.mruEntry = new HopperCache.LastHolder(key, obj2);
			int num = 1;
			try
			{
				try
				{
				}
				finally
				{
					num = Interlocked.CompareExchange(ref this.promoting, 1, 0);
				}
				if (num == 0)
				{
					if (this.outstandingHopper.Count >= this.hopperSize)
					{
						lock (syncObject)
						{
							Hashtable hashtable = this.limitedHopper;
							hashtable.Clear();
							hashtable.Add(key, obj2);
							try
							{
								return obj;
							}
							finally
							{
								this.limitedHopper = this.strongHopper;
								this.strongHopper = this.outstandingHopper;
								this.outstandingHopper = hashtable;
							}
						}
					}
					this.outstandingHopper[key] = obj2;
				}
			}
			finally
			{
				if (num == 0)
				{
					this.promoting = 0;
				}
			}
			return obj;
		}

		private readonly int hopperSize;

		private readonly bool weak;

		private Hashtable outstandingHopper;

		private Hashtable strongHopper;

		private Hashtable limitedHopper;

		private int promoting;

		private HopperCache.LastHolder mruEntry;

		private class LastHolder
		{
			internal LastHolder(object key, object value)
			{
				this.key = key;
				this.value = value;
			}

			internal object Key
			{
				get
				{
					return this.key;
				}
			}

			internal object Value
			{
				get
				{
					return this.value;
				}
			}

			private readonly object key;

			private readonly object value;
		}
	}
}
