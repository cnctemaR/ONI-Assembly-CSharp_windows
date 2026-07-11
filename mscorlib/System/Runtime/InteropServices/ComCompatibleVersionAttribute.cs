using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComCompatibleVersionAttribute : Attribute
	{
		public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.revision = revision;
		}

		public int MajorVersion
		{
			get
			{
				return this.major;
			}
		}

		public int MinorVersion
		{
			get
			{
				return this.minor;
			}
		}

		public int BuildNumber
		{
			get
			{
				return this.build;
			}
		}

		public int RevisionNumber
		{
			get
			{
				return this.revision;
			}
		}

		private int major;

		private int minor;

		private int build;

		private int revision;
	}
}
