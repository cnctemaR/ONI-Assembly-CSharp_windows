using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public sealed class InternalsVisibleToAttribute : Attribute
	{
		public InternalsVisibleToAttribute(string assemblyName)
		{
			this.assemblyName = assemblyName;
		}

		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}
		}

		public bool AllInternalsVisible
		{
			get
			{
				return this.all_visible;
			}
			set
			{
				this.all_visible = value;
			}
		}

		private string assemblyName;

		private bool all_visible = true;
	}
}
