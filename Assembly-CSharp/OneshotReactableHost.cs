using System;
using UnityEngine;

public class OneshotReactableHost : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("CleanupOneshotReactable", this.lifetime, new Action<object>(this.OnExpire), null, null);
	}

	public void SetReactable(Reactable reactable)
	{
		this.reactable = reactable;
	}

	private void OnExpire(object obj)
	{
		if (!this.reactable.IsReacting)
		{
			this.reactable.Cleanup();
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		GameScheduler.Instance.Schedule("CleanupOneshotReactable", 0.5f, new Action<object>(this.OnExpire), null, null);
	}

	private Reactable reactable;

	public float lifetime = 1f;
}
