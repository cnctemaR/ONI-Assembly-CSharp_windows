using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RunInstallerAttribute : Attribute
	{
		public RunInstallerAttribute(bool runInstaller)
		{
			this.RunInstaller = runInstaller;
		}

		public bool RunInstaller { get; }

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			RunInstallerAttribute runInstallerAttribute = obj as RunInstallerAttribute;
			return runInstallerAttribute != null && runInstallerAttribute.RunInstaller == this.RunInstaller;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this.Equals(RunInstallerAttribute.Default);
		}

		public static readonly RunInstallerAttribute Yes = new RunInstallerAttribute(true);

		public static readonly RunInstallerAttribute No = new RunInstallerAttribute(false);

		public static readonly RunInstallerAttribute Default = RunInstallerAttribute.No;
	}
}
