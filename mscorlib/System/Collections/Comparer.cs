using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Comparer : ISerializable, IComparer
	{
		private Comparer()
		{
		}

		public Comparer(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			this.m_compareInfo = culture.CompareInfo;
		}

		public int Compare(object a, object b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			if (this.m_compareInfo != null)
			{
				string text = a as string;
				string text2 = b as string;
				if (text != null && text2 != null)
				{
					return this.m_compareInfo.Compare(text, text2);
				}
			}
			if (a is IComparable)
			{
				return (a as IComparable).CompareTo(b);
			}
			if (b is IComparable)
			{
				return -(b as IComparable).CompareTo(a);
			}
			throw new ArgumentException(Locale.GetText("Neither 'a' nor 'b' implements IComparable."));
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("CompareInfo", this.m_compareInfo, typeof(CompareInfo));
		}

		public static readonly Comparer Default = new Comparer();

		public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);

		private CompareInfo m_compareInfo;
	}
}
