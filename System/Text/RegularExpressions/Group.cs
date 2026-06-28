using System;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Group : Capture
	{
		internal Group(string text, int index, int length, int n_caps)
			: base(text, index, length)
		{
			this.success = true;
			this.captures = new CaptureCollection(n_caps);
			this.captures.SetValue(this, n_caps - 1);
		}

		internal Group(string text, int index, int length)
			: base(text, index, length)
		{
			this.success = true;
		}

		internal Group()
			: base(string.Empty)
		{
			this.success = false;
			this.captures = new CaptureCollection(0);
		}

		[global::System.MonoTODO("not thread-safe")]
		public static Group Synchronized(Group inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			return inner;
		}

		public CaptureCollection Captures
		{
			get
			{
				return this.captures;
			}
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
		}

		internal static Group Fail = new Group();

		private bool success;

		private CaptureCollection captures;
	}
}
