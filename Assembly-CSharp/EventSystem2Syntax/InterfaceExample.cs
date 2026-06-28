using System;

namespace EventSystem2Syntax
{
	public class InterfaceExample : KMonoBehaviour, IObjectDestroyedHandler
	{
		protected override void OnPrefabInit()
		{
			this.Subscribe(GameHashes.ObjectDestroyed);
		}

		public void OnObjectDestroyed(KPrefabID obj)
		{
		}
	}
}
