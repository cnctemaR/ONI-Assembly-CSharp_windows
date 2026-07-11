using System;
using System.Collections.Generic;

public class KAnimSynchronizer
{
	public KAnimSynchronizer(KAnimControllerBase master_controller)
	{
		this.masterController = master_controller;
	}

	private void Clear(KAnimControllerBase controller)
	{
		controller.Play("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void Add(KAnimControllerBase controller)
	{
		this.Targets.Add(controller);
	}

	public void Remove(KAnimControllerBase controller)
	{
		this.Clear(controller);
		this.Targets.Remove(controller);
	}

	public void Clear()
	{
		foreach (KAnimControllerBase kanimControllerBase in this.Targets)
		{
			this.Clear(kanimControllerBase);
		}
		this.Targets.Clear();
	}

	public void Sync(KAnimControllerBase controller)
	{
		if (this.masterController == null)
		{
			return;
		}
		if (controller == null)
		{
			return;
		}
		KAnim.Anim currentAnim = this.masterController.GetCurrentAnim();
		if (currentAnim == null)
		{
			return;
		}
		KAnim.PlayMode mode = this.masterController.GetMode();
		float playSpeed = this.masterController.GetPlaySpeed();
		float elapsedTime = this.masterController.GetElapsedTime();
		controller.Play(currentAnim.name, mode, playSpeed, elapsedTime);
		Facing component = controller.GetComponent<Facing>();
		if (component != null)
		{
			component.Face(component.transform.GetPosition().x + 1f);
		}
		else
		{
			controller.FlipX = this.masterController.FlipX;
			controller.FlipY = this.masterController.FlipY;
		}
	}

	public void Sync()
	{
		for (int i = 0; i < this.Targets.Count; i++)
		{
			KAnimControllerBase kanimControllerBase = this.Targets[i];
			this.Sync(kanimControllerBase);
		}
	}

	public void SyncTime()
	{
		float elapsedTime = this.masterController.GetElapsedTime();
		for (int i = 0; i < this.Targets.Count; i++)
		{
			KAnimControllerBase kanimControllerBase = this.Targets[i];
			kanimControllerBase.SetElapsedTime(elapsedTime);
		}
	}

	private KAnimControllerBase masterController;

	private List<KAnimControllerBase> Targets = new List<KAnimControllerBase>();
}
