using System;
using System.Runtime.Serialization;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Group : Capture
	{
		internal Group(string text, int[] caps, int capcount, string name)
			: base(text, (capcount == 0) ? 0 : caps[(capcount - 1) * 2], (capcount == 0) ? 0 : caps[capcount * 2 - 1])
		{
			this._caps = caps;
			this._capcount = capcount;
			this._name = name;
		}

		public bool Success
		{
			get
			{
				return this._capcount != 0;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public CaptureCollection Captures
		{
			get
			{
				if (this._capcoll == null)
				{
					this._capcoll = new CaptureCollection(this);
				}
				return this._capcoll;
			}
		}

		public static Group Synchronized(Group inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			CaptureCollection captures = inner.Captures;
			if (inner._capcount > 0)
			{
				Capture capture = captures[0];
			}
			return inner;
		}

		internal static Group _emptygroup = new Group(string.Empty, new int[0], 0, string.Empty);

		internal int[] _caps;

		internal int _capcount;

		internal CaptureCollection _capcoll;

		[OptionalField]
		internal string _name;
	}
}
