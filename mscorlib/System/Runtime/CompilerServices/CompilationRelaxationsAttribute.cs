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
			this.m_relaxations = relaxations;
		}

		public CompilationRelaxationsAttribute(CompilationRelaxations relaxations)
		{
			this.m_relaxations = (int)relaxations;
		}

		public int CompilationRelaxations
		{
			get
			{
				return this.m_relaxations;
			}
		}

		private int m_relaxations;
	}
}
