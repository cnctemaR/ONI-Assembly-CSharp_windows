using System;
using System.Collections.Specialized;

namespace System.Net
{
	internal sealed class TrackingStringDictionary : StringDictionary
	{
		internal TrackingStringDictionary()
			: this(false)
		{
		}

		internal TrackingStringDictionary(bool isReadOnly)
		{
			this._isReadOnly = isReadOnly;
		}

		internal bool IsChanged
		{
			get
			{
				return this._isChanged;
			}
			set
			{
				this._isChanged = value;
			}
		}

		public override void Add(string key, string value)
		{
			if (this._isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.Add(key, value);
			this._isChanged = true;
		}

		public override void Clear()
		{
			if (this._isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.Clear();
			this._isChanged = true;
		}

		public override void Remove(string key)
		{
			if (this._isReadOnly)
			{
				throw new InvalidOperationException("The collection is read-only.");
			}
			base.Remove(key);
			this._isChanged = true;
		}

		public override string this[string key]
		{
			get
			{
				return base[key];
			}
			set
			{
				if (this._isReadOnly)
				{
					throw new InvalidOperationException("The collection is read-only.");
				}
				base[key] = value;
				this._isChanged = true;
			}
		}

		private readonly bool _isReadOnly;

		private bool _isChanged;
	}
}
