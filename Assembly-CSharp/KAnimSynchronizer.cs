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
		controller.Play(controller.defaultAnim, KAnim.PlayMode.Loop, 1f, 0f);
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

	private void Clear(KAnimSynchronizedController controller)
	{
		controller.Play(controller.synchronizedController.defaultAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void Add(KAnimSynchronizedController controller)
	{
		this.SyncedControllers.Add(controller);
	}

	public void Remove(KAnimSynchronizedController controller)
	{
		this.Clear(controller);
		this.SyncedControllers.Remove(controller);
	}

	public void Clear()
	{
		foreach (KAnimControllerBase kanimControllerBase in this.Targets)
		{
			if (!(kanimControllerBase == null) && kanimControllerBase.AnimFiles != null)
			{
				this.Clear(kanimControllerBase);
			}
		}
		this.Targets.Clear();
		foreach (KAnimSynchronizedController kanimSynchronizedController in this.SyncedControllers)
		{
			if (!(kanimSynchronizedController.synchronizedController == null) && kanimSynchronizedController.synchronizedController.AnimFiles != null)
			{
				this.Clear(kanimSynchronizedController);
			}
		}
		this.SyncedControllers.Clear();
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
		if (currentAnim != null && !string.IsNullOrEmpty(controller.defaultAnim) && !controller.HasAnimation(currentAnim.name))
		{
			controller.Play(controller.defaultAnim, KAnim.PlayMode.Loop, 1f, 0f);
			return;
		}
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
			float num = component.transform.GetPosition().x;
			num += (this.masterController.FlipX ? (-0.5f) : 0.5f);
			component.Face(num);
			return;
		}
		controller.FlipX = this.masterController.FlipX;
		controller.FlipY = this.masterController.FlipY;
	}

	public void SyncController(KAnimSynchronizedController controller)
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
		string text = ((currentAnim != null) ? (currentAnim.name + controller.Postfix) : string.Empty);
		if (!string.IsNullOrEmpty(controller.synchronizedController.defaultAnim) && !controller.synchronizedController.HasAnimation(text))
		{
			controller.Play(controller.synchronizedController.defaultAnim, KAnim.PlayMode.Loop, 1f, 0f);
			return;
		}
		if (currentAnim == null)
		{
			return;
		}
		KAnim.PlayMode mode = this.masterController.GetMode();
		float playSpeed = this.masterController.GetPlaySpeed();
		float elapsedTime = this.masterController.GetElapsedTime();
		controller.Play(text, mode, playSpeed, elapsedTime);
		Facing component = controller.synchronizedController.GetComponent<Facing>();
		if (component != null)
		{
			float num = component.transform.GetPosition().x;
			num += (this.masterController.FlipX ? (-0.5f) : 0.5f);
			component.Face(num);
			return;
		}
		controller.synchronizedController.FlipX = this.masterController.FlipX;
		controller.synchronizedController.FlipY = this.masterController.FlipY;
	}

	public void Sync()
	{
		for (int i = 0; i < this.Targets.Count; i++)
		{
			KAnimControllerBase kanimControllerBase = this.Targets[i];
			this.Sync(kanimControllerBase);
		}
		for (int j = 0; j < this.SyncedControllers.Count; j++)
		{
			KAnimSynchronizedController kanimSynchronizedController = this.SyncedControllers[j];
			this.SyncController(kanimSynchronizedController);
		}
	}

	public void SyncTime()
	{
		float elapsedTime = this.masterController.GetElapsedTime();
		for (int i = 0; i < this.Targets.Count; i++)
		{
			this.Targets[i].SetElapsedTime(elapsedTime);
		}
		for (int j = 0; j < this.SyncedControllers.Count; j++)
		{
			this.SyncedControllers[j].synchronizedController.SetElapsedTime(elapsedTime);
		}
	}

	private KAnimControllerBase masterController;

	private List<KAnimControllerBase> Targets = new List<KAnimControllerBase>();

	private List<KAnimSynchronizedController> SyncedControllers = new List<KAnimSynchronizedController>();
}
