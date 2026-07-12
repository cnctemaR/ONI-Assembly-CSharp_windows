using System;

namespace UnityEngine.UIElements
{
	internal static class ListViewDraggerExtension
	{
		public static ListView.RecycledItem GetRecycledItemFromIndex(this ListView listView, int index)
		{
			foreach (ListView.RecycledItem recycledItem in listView.Pool)
			{
				bool flag = recycledItem.index.Equals(index);
				if (flag)
				{
					return recycledItem;
				}
			}
			return null;
		}
	}
}
