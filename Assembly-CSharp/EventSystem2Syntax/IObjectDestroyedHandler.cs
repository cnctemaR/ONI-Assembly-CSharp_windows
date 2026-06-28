using System;

namespace EventSystem2Syntax
{
	internal interface IObjectDestroyedHandler
	{
		void OnObjectDestroyed(KPrefabID obj);
	}
}
