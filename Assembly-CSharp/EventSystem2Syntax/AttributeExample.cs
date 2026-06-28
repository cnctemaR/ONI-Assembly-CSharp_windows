using System;

namespace EventSystem2Syntax
{
	public class AttributeExample : KMonoBehaviour
	{
		protected override void OnPrefabInit()
		{
			this.Subscribe(GameHashes.ObjectDestroyed);
		}

		[EventHandler(GameHashes.ObjectDestroyed)]
		private void OnObjectDestroyed(KPrefabID obj)
		{
		}
	}
}
