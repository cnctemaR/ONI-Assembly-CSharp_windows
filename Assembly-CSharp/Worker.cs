using System;
using Klei.AI;
using UnityEngine;

public class Worker : KMonoBehaviour
{
	public Worker.State state { get; private set; }

	public Worker.StartWorkInfo startWorkInfo { get; private set; }

	public Workable workable
	{
		get
		{
			if (this.startWorkInfo != null)
			{
				return this.startWorkInfo.workable;
			}
			return null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.state = Worker.State.Idle;
	}

	private string GetWorkableDebugString()
	{
		if (this.workable == null)
		{
			return "Null";
		}
		return this.workable.name;
	}

	public void CompleteWork()
	{
		if (this.state == Worker.State.PendingCompletion)
		{
			this.state = Worker.State.Idle;
		}
		if (this.workable != null)
		{
			this.workable.CompleteWork(this);
		}
		this.InternalStopWork(this.workable, false);
	}

	public bool Work()
	{
		if (this.state == Worker.State.PendingCompletion)
		{
			return base.GetComponent<KAnimControllerBase>().IsStopped() || Time.time - this.workCompleteTime > 2f;
		}
		float num = Time.time - this.lastWorkTick;
		this.lastWorkTick = Time.time;
		Facing component = base.GetComponent<Facing>();
		if (this.workable.ShouldFaceTargetWhenWorking())
		{
			component.Face(this.workable.transform.position);
		}
		else
		{
			component.Face(component.transform.position + Vector3.right);
		}
		if (this.workable != null)
		{
			Klei.AI.Attribute workAttribute = this.workable.GetWorkAttribute();
			if (workAttribute != null && workAttribute.IsTrainable)
			{
				float experienceMultiplier = this.workable.GetExperienceMultiplier();
				base.GetComponent<AttributeLevels>().AddExperience(workAttribute.Id, num * experienceMultiplier);
			}
			float efficiencyMultiplier = this.workable.GetEfficiencyMultiplier(this);
			float num2 = num * efficiencyMultiplier * 1f;
			if (this.workable.WorkTick(this, num2) && this.state == Worker.State.Working)
			{
				base.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption);
				this.state = Worker.State.PendingCompletion;
				this.workCompleteTime = Time.time;
				KAnimControllerBase component2 = base.GetComponent<KAnimControllerBase>();
				component2.Stop();
				if (this.workable != null && this.workable.synchronizeAnims)
				{
					KAnimControllerBase component3 = this.workable.GetComponent<KAnimControllerBase>();
					if (component3 != null && component3.HasAnimation("working_pst"))
					{
						component3.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
						component2.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
					}
				}
				if (this.animInfo.forcePlayPst)
				{
					component2.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
				}
			}
		}
		return false;
	}

	private void InternalStopWork(Workable target_workable, bool is_aborted)
	{
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Offset -= this.workAnimOffset;
		this.workAnimOffset = Vector3.zero;
		base.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
		this.DetachAnimOverrides();
		base.GetComponent<AnimEventHandler>().ClearContext();
		if (this.previousStatusItem.item != null)
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, this.previousStatusItem.item, this.previousStatusItem.data);
		}
		if (target_workable != null)
		{
			target_workable.StopWork(this, is_aborted);
		}
		if (this.smi != null)
		{
			this.smi.StopSM("stopping work");
			this.smi = null;
		}
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		base.transform.SetPosition(position);
		this.startWorkInfo = null;
	}

	public void StopWork()
	{
		if (this.state == Worker.State.PendingCompletion)
		{
			this.state = Worker.State.Idle;
			this.CompleteWork();
		}
		else if (this.state == Worker.State.Working)
		{
			this.state = Worker.State.Idle;
			this.InternalStopWork(this.workable, true);
		}
	}

	public void StartWork(Worker.StartWorkInfo start_work_info)
	{
		this.startWorkInfo = start_work_info;
		Game.Instance.StartedWork();
		DebugUtil.Assert(this.state == Worker.State.Idle, "Assert!");
		string name = this.workable.GetType().Name;
		try
		{
			this.state = Worker.State.Working;
			this.lastWorkTick = Time.time;
			this.workable.StartWork(this);
			if (this.workable == null)
			{
				global::Debug.LogWarning("Stopped work as soon as I started. This is usuually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.", null);
			}
			else
			{
				KSelectable component = base.GetComponent<KSelectable>();
				this.previousStatusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
				component.SetStatusItem(Db.Get().StatusItemCategories.Main, this.workable.GetWorkerStatusItem(), this.workable);
				this.animInfo = this.workable.GetAnim(this);
				if (this.animInfo.smi != null)
				{
					this.smi = this.animInfo.smi;
					this.smi.StartSM();
				}
				Vector3 position = base.transform.position;
				position.z = Grid.GetLayerZ(this.workable.workLayer);
				base.transform.SetPosition(position);
				KAnimControllerBase component2 = base.GetComponent<KAnimControllerBase>();
				if (this.animInfo.smi == null)
				{
					this.AttachOverrideAnims(component2);
				}
				HashedString[] workAnims = this.workable.GetWorkAnims(this);
				Vector3 workOffset = this.workable.GetWorkOffset();
				this.workAnimOffset = workOffset;
				component2.Offset += workOffset;
				if (this.animInfo.smi == null && workAnims != null)
				{
					if (this.workable.synchronizeAnims)
					{
						KAnimControllerBase component3 = this.workable.GetComponent<KAnimControllerBase>();
						if (component3 != null)
						{
							this.kanimSynchronizer = component3.GetSynchronizer();
							if (this.kanimSynchronizer != null)
							{
								this.kanimSynchronizer.Add(component2);
							}
						}
					}
					component2.Play(workAnims, KAnim.PlayMode.Loop);
				}
			}
		}
		catch (Exception ex)
		{
			string text = "Exception in: Worker.StartWork(" + name + ")";
			Output.LogErrorWithObj(this, new object[] { text + "\n" + ex.ToString() });
			throw;
		}
	}

	private void AttachOverrideAnims(KAnimControllerBase worker_controller)
	{
		if (this.animInfo.overrideAnims != null && this.animInfo.overrideAnims.Length > 0)
		{
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				worker_controller.AddAnimOverrides(this.animInfo.overrideAnims[i], 0f);
			}
		}
	}

	private void DetachAnimOverrides()
	{
		if (this.animInfo.overrideAnims != null)
		{
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			if (this.kanimSynchronizer != null)
			{
				this.kanimSynchronizer.Remove(component);
				this.kanimSynchronizer = null;
			}
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				component.RemoveAnimOverrides(this.animInfo.overrideAnims[i]);
			}
			this.animInfo.overrideAnims = null;
		}
	}

	[MyCmpReq]
	private SnapOn snapOn;

	[MyCmpReq]
	private Facing facing;

	[MyCmpReq]
	private Navigator navigator;

	[MyCmpReq]
	private MinionResume resume;

	[MyCmpReq]
	private Effects effects;

	private float lastWorkTick;

	private float workCompleteTime;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private Vector3 workAnimOffset = Vector3.zero;

	public enum State
	{
		Idle,
		Working,
		PendingCompletion
	}

	public class StartWorkInfo
	{
		public StartWorkInfo(Workable workable)
		{
			this.workable = workable;
		}

		public Workable workable { get; set; }
	}
}
