using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Cancellable")]
public class Cancellable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe<Cancellable>(2127324410, Cancellable.OnCancelDelegate);
	}

	protected virtual void OnCancel(object data)
	{
		this.DeleteObject();
	}

	private static readonly EventSystem.IntraObjectHandler<Cancellable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Cancellable>(delegate(Cancellable component, object data)
	{
		component.OnCancel(data);
	});
}
