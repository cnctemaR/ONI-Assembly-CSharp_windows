using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Configuration
{
	internal class ConfigNameValueCollection : global::System.Collections.Specialized.NameValueCollection
	{
		public ConfigNameValueCollection()
		{
		}

		public ConfigNameValueCollection(ConfigNameValueCollection col)
			: base(col.Count, col)
		{
		}

		public ConfigNameValueCollection(IHashCodeProvider hashProvider, IComparer comparer)
			: base(hashProvider, comparer)
		{
		}

		public void ResetModified()
		{
			this.modified = false;
		}

		public bool IsModified
		{
			get
			{
				return this.modified;
			}
		}

		public override void Set(string name, string value)
		{
			base.Set(name, value);
			this.modified = true;
		}

		private bool modified;
	}
}
