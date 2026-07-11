using System;

namespace EventSystem2Syntax
{
	internal class OldExample : KMonoBehaviour2
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			base.Subscribe(0, new Action<object>(this.OnObjectDestroyed));
			bool flag = false;
			base.Trigger(0, flag);
		}

		private void OnObjectDestroyed(object data)
		{
			bool flag = (bool)data;
			Debug.Log(flag, null);
		}
	}
}
