using System;
using Klei.AI;
using UnityEngine;

public class Worker : KMonoBehaviour
{
	public Workable workable { get; private set; }

	public float amount
	{
		get
		{
			return this._amount;
		}
		private set
		{
			this._amount = value;
		}
	}

	protected override void OnSpawn()
	{
		Components.Workers.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Workers.Remove(this);
	}

	public bool CompleteWork()
	{
		if (this.workable != null)
		{
			Workable workable = this.workable;
			this.workable = null;
			workable.CompleteWork(this);
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			component.Offset -= this.workAnimOffset;
			this.workAnimOffset = Vector3.zero;
			return true;
		}
		return false;
	}

	public bool Work()
	{
		if (this.workable == null)
		{
			return false;
		}
		if (!this.workComplete)
		{
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
				if (this.workable.WorkTick(this, num2))
				{
					this.workComplete = true;
					this.workCompleteTime = Time.time;
					KAnimControllerBase component2 = base.GetComponent<KAnimControllerBase>();
					component2.Stop();
					if (this.workable != null)
					{
						KAnimControllerBase component3 = this.workable.GetComponent<KAnimControllerBase>();
						if (component3 != null && component3.HasAnimation("working_pst"))
						{
							component3.Stop();
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
		if (base.GetComponent<KAnimControllerBase>().IsStopped() || Time.time - this.workCompleteTime > 2f)
		{
			if (this.OnWorkCompleteCallback != null)
			{
				this.OnWorkCompleteCallback();
			}
			return true;
		}
		return false;
	}

	public void StopWork()
	{
		this.workComplete = false;
		Workable workable = this.workable;
		this.workable = null;
		this.navigator.Stop(false);
		this.DetachAnimOverrides();
		base.GetComponent<AnimEventHandler>().ClearContext();
		if (this.previousStatusItem.item != null)
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, this.previousStatusItem.item, this.previousStatusItem.data);
		}
		if (workable != null)
		{
			workable.StopWork(this);
		}
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.Offset -= this.workAnimOffset;
		this.workAnimOffset = Vector3.zero;
		if (this.smi != null)
		{
			this.smi.StopSM("stopping work");
			this.smi = null;
		}
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
		this.transform.SetPosition(position);
	}

	public Workable GetWorkTarget()
	{
		return this.workable;
	}

	public void StartWork(Workable workable, float amount)
	{
		Game.Instance.StartedWork();
		string name = workable.GetType().Name;
		try
		{
			workable.StartWork(this);
			this.lastWorkTick = Time.time;
			this.workable = workable;
			this.amount = amount;
			this.workComplete = false;
			KSelectable component = base.GetComponent<KSelectable>();
			this.previousStatusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, workable.GetWorkerStatusItem(), workable);
			this.animInfo = workable.GetAnim(this);
			this.AttachOverrideAnims();
			HashedString[] workAnims = workable.GetWorkAnims(this);
			if (workAnims != null)
			{
				KAnimControllerBase component2 = base.GetComponent<KAnimControllerBase>();
				Vector3 workOffset = workable.GetWorkOffset();
				this.workAnimOffset = workOffset;
				component2.Offset += workOffset;
				component2.Play(workAnims, KAnim.PlayMode.Loop);
			}
			if (this.OnWorkStartCallback != null)
			{
				this.OnWorkStartCallback();
			}
			if (this.animInfo.smi != null)
			{
				this.smi = this.animInfo.smi;
				this.smi.StartSM();
			}
			if (workable.GetComponent<BuildingComplete>() != null)
			{
				Vector3 position = this.transform.position;
				position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront);
				this.transform.SetPosition(position);
			}
		}
		catch (Exception ex)
		{
			string text = "Exception in: Worker.StartWork(" + name + ")";
			Output.LogErrorWithObj(this, new object[] { text + "\n" + ex.ToString() });
			throw;
		}
	}

	private void AttachOverrideAnims()
	{
		if (this.animInfo.overrideAnims != null && this.animInfo.overrideAnims.Length > 0)
		{
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				component.AddAnimOverrides(this.animInfo.overrideAnims[i], 0f);
			}
			if (this.workable.GetWorkAnims(this) == null)
			{
				KAnimControllerBase component2 = this.workable.GetComponent<KAnimControllerBase>();
				Vector3 workOffset = this.workable.GetWorkOffset();
				this.workAnimOffset = workOffset;
				component2.Offset += workOffset;
				this.kanimSynchronizer = component2.GetSynchronizer();
				if (this.kanimSynchronizer != null)
				{
					this.kanimSynchronizer.Add(component);
				}
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
			}
			for (int i = 0; i < this.animInfo.overrideAnims.Length; i++)
			{
				component.RemoveAnimOverrides(this.animInfo.overrideAnims[i]);
			}
			this.animInfo.overrideAnims = null;
		}
	}

	public bool HasWorkPath
	{
		get
		{
			return this.navigator.path.nodes != null;
		}
	}

	public int TaskCost
	{
		get
		{
			return this.navigator.path.cost;
		}
	}

	public Workable GetWorkable()
	{
		return this.workable;
	}

	[MyCmpReq]
	private SnapOn snapOn;

	[MyCmpReq]
	private Facing facing;

	[MyCmpReq]
	private Navigator navigator;

	[MyCmpReq]
	private Effects effects;

	private bool workComplete;

	private float lastWorkTick;

	public global::System.Action OnWorkCompleteCallback;

	public global::System.Action OnWorkStartCallback;

	private float _amount;

	private float workCompleteTime;

	public object workCompleteData;

	private Workable.AnimInfo animInfo;

	private KAnimSynchronizer kanimSynchronizer;

	private StatusItemGroup.Entry previousStatusItem;

	private StateMachine.Instance smi;

	private Vector3 workAnimOffset = Vector3.zero;
}
