using System;

namespace System
{
	[Serializable]
	internal class ReflectionOnlyType : RuntimeType
	{
		private ReflectionOnlyType()
		{
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				throw new InvalidOperationException(Environment.GetResourceString("The requested operation is invalid in the ReflectionOnly context."));
			}
		}
	}
}
