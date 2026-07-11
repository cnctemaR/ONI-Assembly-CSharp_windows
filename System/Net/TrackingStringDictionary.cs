using System;
using System.Collections.Specialized;

namespace System.Net
{
	internal class TrackingStringDictionary : StringDictionary
	{
		internal TrackingStringDictionary()
			: this(false)
		{
		}

		internal TrackingStringDictionary(bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;
		}

		internal bool IsChanged
		{
			get
			{
				return this.isChanged;
			}
			set
			{
				this.isChanged = value;
			}
		}

		public override void Add(string key, string value)
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException(global::SR.GetString("The collection is read-only."));
			}
			base.Add(key, value);
			this.isChanged = true;
		}

		public override void Clear()
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException(global::SR.GetString("The collection is read-only."));
			}
			base.Clear();
			this.isChanged = true;
		}

		public override void Remove(string key)
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException(global::SR.GetString("The collection is read-only."));
			}
			base.Remove(key);
			this.isChanged = true;
		}

		public override string this[string key]
		{
			get
			{
				return base[key];
			}
			set
			{
				if (this.isReadOnly)
				{
					throw new InvalidOperationException(global::SR.GetString("The collection is read-only."));
				}
				base[key] = value;
				this.isChanged = true;
			}
		}

		private bool isChanged;

		private bool isReadOnly;
	}
}
