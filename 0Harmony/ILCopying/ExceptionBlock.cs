using System;

namespace Harmony.ILCopying
{
	public class ExceptionBlock
	{
		public ExceptionBlock(ExceptionBlockType blockType, Type catchType)
		{
			this.blockType = blockType;
			this.catchType = catchType;
		}

		public ExceptionBlockType blockType;

		public Type catchType;
	}
}
