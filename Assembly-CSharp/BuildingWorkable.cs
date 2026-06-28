using System;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingWorkable : Workable
{
	protected BuildingDef Def
	{
		get
		{
			return this.Building.Def;
		}
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return this.WorkAnims;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.progressbar_y_offset = 0.408f;
		if (this.Def.MinionEffect != null && this.Def.MinionEffect != string.Empty)
		{
			this.effect = Db.Get().effects.Get(this.Def.MinionEffect);
		}
	}

	private void AddEffect()
	{
		if (this.effect != null && base.worker != null)
		{
			DebugUtil.Assert(this.effectInstance == null, "Assert!");
			this.effectInstance = base.worker.GetComponent<Effects>().Add(this.effect, false);
		}
	}

	private void ClearEffect()
	{
		if (this.effectInstance != null)
		{
			this.effectInstance.Remove();
			this.effectInstance = null;
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		Debug.Assert(worker != null, "How did we get a null worker?");
		this.AddEffect();
		this.Trigger(-1358696400, worker);
		worker.Trigger(-1358696400, base.gameObject);
	}

	protected override void OnStopWork(Worker worker)
	{
		Debug.Assert(worker != null, "How did we get a null worker?");
		this.ClearEffect();
		this.Trigger(116081340, worker);
		worker.Trigger(116081340, base.gameObject);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Debug.Assert(worker != null, "How did we get a null worker?");
		this.Trigger(155714618, worker);
		worker.Trigger(155714618, base.gameObject);
		this.ClearEffect();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.ClearEffect();
	}

	[MyCmpReq]
	private Building Building;

	private Effect effect;

	private EffectInstance effectInstance;

	protected static readonly string[] DefaultWorkAnims = new string[] { "working_pre", "working_loop" };

	protected string[] WorkAnims = BuildingWorkable.DefaultWorkAnims;
}
