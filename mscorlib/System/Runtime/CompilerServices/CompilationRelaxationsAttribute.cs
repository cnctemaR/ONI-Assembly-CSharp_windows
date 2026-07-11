using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Method)]
	[ComVisible(true)]
	[Serializable]
	public class CompilationRelaxationsAttribute : Attribute
	{
		public CompilationRelaxationsAttribute(int relaxations)
		{
			this.relax = relaxations;
		}

		public CompilationRelaxationsAttribute(CompilationRelaxations relaxations)
		{
			this.relax = (int)relaxations;
		}

		public int CompilationRelaxations
		{
			get
			{
				return this.relax;
			}
		}

		private int relax;
	}
}
