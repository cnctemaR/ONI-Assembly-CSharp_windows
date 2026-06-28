using System;

namespace EventSystem2Syntax
{
	public class StaticExample : KMonoBehaviour
	{
		protected override void OnPrefabInit()
		{
			this.Subscribe(GameHashes.ObjectDestroyed, new Action<StaticExample, KPrefabID>(StaticExample.OnObjectDestroyed));
		}

		private static void OnObjectDestroyed(StaticExample example, KPrefabID obj)
		{
		}
	}
}
