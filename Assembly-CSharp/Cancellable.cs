using System;

[SkipSaveFileSerialization]
public class Cancellable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
	}

	protected virtual void OnCancel(object data)
	{
		this.DeleteObject();
	}
}
