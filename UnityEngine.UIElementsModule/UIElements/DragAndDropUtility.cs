using System;

namespace UnityEngine.UIElements
{
	internal static class DragAndDropUtility
	{
		internal static IDragAndDrop GetDragAndDrop(IPanel panel)
		{
			bool flag = panel.contextType == ContextType.Player;
			IDragAndDrop dragAndDrop2;
			if (flag)
			{
				IDragAndDrop dragAndDrop;
				if ((dragAndDrop = DragAndDropUtility.s_DragAndDropPlayMode) == null)
				{
					dragAndDrop = (DragAndDropUtility.s_DragAndDropPlayMode = new DefaultDragAndDropClient());
				}
				dragAndDrop2 = dragAndDrop;
			}
			else
			{
				IDragAndDrop dragAndDrop3;
				if ((dragAndDrop3 = DragAndDropUtility.s_DragAndDropEditor) == null)
				{
					IDragAndDrop dragAndDrop5;
					if (DragAndDropUtility.s_MakeDragAndDropClientFunc == null)
					{
						IDragAndDrop dragAndDrop4 = new DefaultDragAndDropClient();
						dragAndDrop5 = dragAndDrop4;
					}
					else
					{
						dragAndDrop5 = DragAndDropUtility.s_MakeDragAndDropClientFunc();
					}
					dragAndDrop3 = (DragAndDropUtility.s_DragAndDropEditor = dragAndDrop5);
				}
				dragAndDrop2 = dragAndDrop3;
			}
			return dragAndDrop2;
		}

		internal static void RegisterMakeClientFunc(Func<IDragAndDrop> makeClient)
		{
			DragAndDropUtility.s_MakeDragAndDropClientFunc = makeClient;
			DragAndDropUtility.s_DragAndDropEditor = null;
		}

		private static Func<IDragAndDrop> s_MakeDragAndDropClientFunc;

		private static IDragAndDrop s_DragAndDropEditor;

		private static IDragAndDrop s_DragAndDropPlayMode;
	}
}
