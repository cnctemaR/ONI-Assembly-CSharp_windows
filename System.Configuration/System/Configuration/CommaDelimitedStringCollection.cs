using System;
using System.Collections.Specialized;

namespace System.Configuration
{
	public sealed class CommaDelimitedStringCollection : StringCollection
	{
		public bool IsModified
		{
			get
			{
				if (this.modified)
				{
					return true;
				}
				string text = this.ToString();
				return text != null && text.GetHashCode() != this.originalStringHash;
			}
		}

		public new bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public new string this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (this.readOnly)
				{
					throw new ConfigurationErrorsException("The configuration is read only");
				}
				base[index] = value;
				this.modified = true;
			}
		}

		public new void Add(string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Add(value);
			this.modified = true;
		}

		public new void AddRange(string[] range)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.AddRange(range);
			this.modified = true;
		}

		public new void Clear()
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Clear();
			this.modified = true;
		}

		public CommaDelimitedStringCollection Clone()
		{
			CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
			string[] array = new string[base.Count];
			base.CopyTo(array, 0);
			commaDelimitedStringCollection.AddRange(array);
			commaDelimitedStringCollection.originalStringHash = this.originalStringHash;
			return commaDelimitedStringCollection;
		}

		public new void Insert(int index, string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Insert(index, value);
			this.modified = true;
		}

		public new void Remove(string value)
		{
			if (this.readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base.Remove(value);
			this.modified = true;
		}

		public void SetReadOnly()
		{
			this.readOnly = true;
		}

		public override string ToString()
		{
			if (base.Count == 0)
			{
				return null;
			}
			string[] array = new string[base.Count];
			base.CopyTo(array, 0);
			return string.Join(",", array);
		}

		internal void UpdateStringHash()
		{
			string text = this.ToString();
			if (text == null)
			{
				this.originalStringHash = 0;
				return;
			}
			this.originalStringHash = text.GetHashCode();
		}

		private bool modified;

		private bool readOnly;

		private int originalStringHash;
	}
}
