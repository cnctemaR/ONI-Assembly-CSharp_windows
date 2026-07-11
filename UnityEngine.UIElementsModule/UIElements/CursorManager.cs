using System;

namespace UnityEngine.UIElements
{
	internal class CursorManager : ICursorManager
	{
		public void SetCursor(Cursor cursor)
		{
			bool flag = cursor.texture != null;
			if (flag)
			{
				Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.Auto);
			}
			else
			{
				bool flag2 = cursor.defaultCursorId != 0;
				if (flag2)
				{
					Debug.LogWarning("Runtime does not support setting a cursor without a texture. Use ResetCursor() to reset the cursor to the default cursor.");
				}
				this.ResetCursor();
			}
		}

		public void ResetCursor()
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
}
