using System;

namespace HarmonyLib
{
	public class ExceptionBlock
	{
		public ExceptionBlock(ExceptionBlockType blockType, Type catchType = null)
		{
			this.blockType = blockType;
			this.catchType = catchType ?? typeof(object);
		}

		public ExceptionBlockType blockType;

		public Type catchType;
	}
}
