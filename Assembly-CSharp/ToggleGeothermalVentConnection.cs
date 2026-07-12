using System;

public class ToggleGeothermalVentConnection : Toggleable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetWorkTime(10f);
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim(GeothermalVentConfig.TOGGLE_ANIM_OVERRIDE) };
		this.workAnims = new HashedString[] { GeothermalVentConfig.TOGGLE_ANIMATION };
		this.workingPstComplete = null;
		this.workingPstFailed = null;
		this.workLayer = Grid.SceneLayer.Front;
		this.synchronizeAnims = false;
		this.workAnimPlayMode = KAnim.PlayMode.Once;
		base.SetOffsets(new CellOffset[] { CellOffset.none });
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.buildingAnimController.Play(GeothermalVentConfig.TOGGLE_ANIMATION, KAnim.PlayMode.Once, 1f, 0f);
		if (this.workerFacing == null || this.workerFacing.gameObject != worker.gameObject)
		{
			this.workerFacing = worker.GetComponent<Facing>();
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.workerFacing != null)
		{
			this.workerFacing.Face(this.workerFacing.transform.GetLocalPosition().x + 0.5f);
		}
		return base.OnWorkTick(worker, dt);
	}

	[MyCmpGet]
	private KBatchedAnimController buildingAnimController;

	private Facing workerFacing;
}
