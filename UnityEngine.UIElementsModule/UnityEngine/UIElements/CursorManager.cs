using System;

namespace UnityEngine.UIElements
{
	internal class CursorManager : ICursorManager
	{
		public bool isCursorOverriden { get; private set; }

		public void SetCursor(Cursor cursor)
		{
			bool flag = cursor.texture != null;
			if (flag)
			{
				Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.Auto);
				this.isCursorOverriden = true;
			}
			else
			{
				bool flag2 = cursor.defaultCursorId != 0;
				if (flag2)
				{
					Debug.LogWarning("Runtime cursors other than the default cursor need to be defined using a texture.");
				}
				this.ResetCursor();
			}
		}

		public void ResetCursor()
		{
			bool isCursorOverriden = this.isCursorOverriden;
			if (isCursorOverriden)
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
			this.isCursorOverriden = false;
		}
	}
}
