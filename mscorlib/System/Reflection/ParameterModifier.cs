using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public struct ParameterModifier
	{
		public ParameterModifier(int parameterCount)
		{
			if (parameterCount <= 0)
			{
				throw new ArgumentException("Must specify one or more parameters.");
			}
			this._byref = new bool[parameterCount];
		}

		public bool this[int index]
		{
			get
			{
				return this._byref[index];
			}
			set
			{
				this._byref[index] = value;
			}
		}

		private bool[] _byref;
	}
}
