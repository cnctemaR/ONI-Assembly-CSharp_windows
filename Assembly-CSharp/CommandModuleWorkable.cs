using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/CommandModuleWorkable")]
public class CommandModuleWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetOffsets(CommandModuleWorkable.entryOffsets);
		this.synchronizeAnims = false;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_incubator_kanim") };
		base.SetWorkTime(float.PositiveInfinity);
		this.showProgressBar = false;
		base.Subscribe<CommandModuleWorkable>(-1056989049, CommandModuleWorkable.OnLaunchDelegate);
	}

	private void OnLaunch(object data)
	{
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (worker != null)
		{
			GameObject gameObject = worker.gameObject;
			base.CompleteWork(worker);
			base.GetComponent<MinionStorage>().SerializeMinion(gameObject);
			return true;
		}
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
	}

	private static CellOffset[] entryOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1),
		new CellOffset(0, 2),
		new CellOffset(0, 3),
		new CellOffset(0, 4)
	};

	private static readonly EventSystem.IntraObjectHandler<CommandModuleWorkable> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CommandModuleWorkable>(delegate(CommandModuleWorkable component, object data)
	{
		component.OnLaunch(data);
	});
}
