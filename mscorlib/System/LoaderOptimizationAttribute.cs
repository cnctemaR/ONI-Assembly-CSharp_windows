using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class LoaderOptimizationAttribute : Attribute
	{
		public LoaderOptimizationAttribute(byte value)
		{
			this._val = value;
		}

		public LoaderOptimizationAttribute(LoaderOptimization value)
		{
			this._val = (byte)value;
		}

		public LoaderOptimization Value
		{
			get
			{
				return (LoaderOptimization)this._val;
			}
		}

		internal byte _val;
	}
}
