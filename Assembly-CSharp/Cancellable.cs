using System;

public class Cancellable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(2127324410, new EventSystem.EventHandler(this.OnCancel));
	}

	protected virtual void OnCancel(object data)
	{
		this.DeleteObject();
	}
}
