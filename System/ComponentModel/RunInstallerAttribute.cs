using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RunInstallerAttribute : Attribute
	{
		public RunInstallerAttribute(bool runInstaller)
		{
			this.runInstaller = runInstaller;
		}

		public override bool Equals(object obj)
		{
			return obj is RunInstallerAttribute && ((RunInstallerAttribute)obj).RunInstaller.Equals(this.runInstaller);
		}

		public override int GetHashCode()
		{
			return this.runInstaller.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(RunInstallerAttribute.Default);
		}

		public bool RunInstaller
		{
			get
			{
				return this.runInstaller;
			}
		}

		public static readonly RunInstallerAttribute Yes = new RunInstallerAttribute(true);

		public static readonly RunInstallerAttribute No = new RunInstallerAttribute(false);

		public static readonly RunInstallerAttribute Default = new RunInstallerAttribute(false);

		private bool runInstaller;
	}
}
