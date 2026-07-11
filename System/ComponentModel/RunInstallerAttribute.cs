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

		public bool RunInstaller
		{
			get
			{
				return this.runInstaller;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			RunInstallerAttribute runInstallerAttribute = obj as RunInstallerAttribute;
			return runInstallerAttribute != null && runInstallerAttribute.RunInstaller == this.runInstaller;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(RunInstallerAttribute.Default);
		}

		private bool runInstaller;

		public static readonly RunInstallerAttribute Yes = new RunInstallerAttribute(true);

		public static readonly RunInstallerAttribute No = new RunInstallerAttribute(false);

		public static readonly RunInstallerAttribute Default = RunInstallerAttribute.No;
	}
}
