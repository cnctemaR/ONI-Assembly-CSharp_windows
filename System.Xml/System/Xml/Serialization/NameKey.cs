using System;

namespace System.Xml.Serialization
{
	internal class NameKey
	{
		internal NameKey(string name, string ns)
		{
			this.name = name;
			this.ns = ns;
		}

		public override bool Equals(object other)
		{
			if (!(other is NameKey))
			{
				return false;
			}
			NameKey nameKey = (NameKey)other;
			return this.name == nameKey.name && this.ns == nameKey.ns;
		}

		public override int GetHashCode()
		{
			return ((this.ns == null) ? "<null>".GetHashCode() : this.ns.GetHashCode()) ^ ((this.name == null) ? 0 : this.name.GetHashCode());
		}

		private string ns;

		private string name;
	}
}
