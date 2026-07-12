using System;

namespace System
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public sealed class CLSCompliantAttribute : Attribute
	{
		public CLSCompliantAttribute(bool isCompliant)
		{
			this._compliant = isCompliant;
		}

		public bool IsCompliant
		{
			get
			{
				return this._compliant;
			}
		}

		private bool _compliant;
	}
}
