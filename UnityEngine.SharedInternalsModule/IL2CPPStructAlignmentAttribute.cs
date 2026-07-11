using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Struct)]
	internal class IL2CPPStructAlignmentAttribute : Attribute
	{
		public IL2CPPStructAlignmentAttribute()
		{
			this.Align = 1;
		}

		public int Align;
	}
}
