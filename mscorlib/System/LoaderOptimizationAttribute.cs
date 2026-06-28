using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(true)]
	public sealed class LoaderOptimizationAttribute : Attribute
	{
		public LoaderOptimizationAttribute(byte value)
		{
			this.lo = (LoaderOptimization)value;
		}

		public LoaderOptimizationAttribute(LoaderOptimization value)
		{
			this.lo = value;
		}

		public LoaderOptimization Value
		{
			get
			{
				return this.lo;
			}
		}

		private LoaderOptimization lo;
	}
}
