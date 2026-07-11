using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	[Serializable]
	public sealed class ReferenceAssemblyAttribute : Attribute
	{
		public ReferenceAssemblyAttribute()
		{
		}

		public ReferenceAssemblyAttribute(string description)
		{
			this._description = description;
		}

		public string Description
		{
			get
			{
				return this._description;
			}
		}

		private string _description;
	}
}
