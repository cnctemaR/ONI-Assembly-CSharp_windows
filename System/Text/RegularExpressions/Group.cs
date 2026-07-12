using System;
using Unity;

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
			this.Name = name;
		}

		public bool Success
		{
			get
			{
				return this._capcount != 0;
			}
		}

		public string Name { get; }

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
			if (inner.Success)
			{
				captures.ForceInitialized();
			}
			return inner;
		}

		internal Group()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal static readonly Group s_emptyGroup = new Group(string.Empty, Array.Empty<int>(), 0, string.Empty);

		internal readonly int[] _caps;

		internal int _capcount;

		internal CaptureCollection _capcoll;
	}
}
