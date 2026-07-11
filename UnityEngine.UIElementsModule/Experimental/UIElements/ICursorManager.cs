using System;

namespace UnityEngine.Experimental.UIElements
{
	internal interface ICursorManager
	{
		void SetCursor(CursorStyle cursor);

		void ResetCursor();
	}
}
