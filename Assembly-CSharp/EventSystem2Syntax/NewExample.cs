using System;

namespace EventSystem2Syntax
{
	internal class NewExample : KMonoBehaviour2
	{
		protected override void OnPrefabInit()
		{
			base.Subscribe<NewExample, NewExample.ObjectDestroyedEvent>(new Action<NewExample, NewExample.ObjectDestroyedEvent>(NewExample.OnObjectDestroyed));
			base.Trigger<NewExample.ObjectDestroyedEvent>(new NewExample.ObjectDestroyedEvent
			{
				parameter = false
			});
		}

		private static void OnObjectDestroyed(NewExample example, NewExample.ObjectDestroyedEvent evt)
		{
		}

		private struct ObjectDestroyedEvent : IEventData
		{
			public bool parameter;
		}
	}
}
