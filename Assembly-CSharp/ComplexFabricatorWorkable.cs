using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class ComplexFabricatorWorkable : Workable
{
	public StatusItem WorkerStatusItem
	{
		get
		{
			return this.workerStatusItem;
		}
		set
		{
			this.workerStatusItem = value;
		}
	}

	public AttributeConverter AttributeConvertor
	{
		get
		{
			return this.attributeConverter;
		}
		set
		{
			this.attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		get
		{
			return this.attributeExperienceMultiplier;
		}
		set
		{
			this.attributeExperienceMultiplier = value;
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	public override string GetConversationTopic()
	{
		string conversationTopic = this.fabricator.GetConversationTopic();
		return (conversationTopic == null) ? base.GetConversationTopic() : conversationTopic;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.operational.SetActive(true, false);
		if (this.fabricator.CurrentMachineOrder != null)
		{
			this.InstantiateVisualizer(this.fabricator.CurrentMachineOrder);
		}
		else
		{
			DebugUtil.DevAssert(false, new object[] { "ComplexFabricatorWorkable.OnStartWork called but CurrentMachineOrder is null", base.gameObject });
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.operational.SetActive(false, false);
	}

	public void ResetWorkTime()
	{
		this.workTimeRemaining = this.GetWorkTime();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.OnWorkTickActions != null)
		{
			this.OnWorkTickActions(worker, dt);
		}
		if (this.meter != null)
		{
			this.UpdateMeter(worker, dt);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		ComplexFabricator.MachineOrder currentMachineOrder = this.fabricator.CurrentMachineOrder;
		if (currentMachineOrder != null)
		{
			this.workTime = currentMachineOrder.parentOrder.recipe.time;
			return this.workTime;
		}
		return -1f;
	}

	public void CreateOrder(ComplexFabricator.MachineOrder buildable_order, ChoreType choreType, Tag[] choreTags)
	{
		buildable_order.chore = new WorkChore<ComplexFabricatorWorkable>(choreType, this, null, choreTags, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		if (this.workTimeRemaining <= 0f)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.fabricator.OnCompleteMachineOrder();
		this.DestroyVisualizer();
	}

	private void InstantiateVisualizer(ComplexFabricator.MachineOrder order)
	{
		if (this.visualizer != null)
		{
			this.DestroyVisualizer();
		}
		if (this.visualizerLink != null)
		{
			this.visualizerLink.Unregister();
			this.visualizerLink = null;
		}
		if (order.parentOrder.recipe.FabricationVisualizer == null)
		{
			return;
		}
		this.visualizer = Util.KInstantiate(order.parentOrder.recipe.FabricationVisualizer, null, null);
		this.visualizer.transform.parent = this.meter.meterController.transform;
		this.visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
		this.visualizer.SetActive(true);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = this.visualizer.GetComponent<KBatchedAnimController>();
		this.visualizerLink = new KAnimLink(component, component2);
	}

	private void UpdateMeter(Worker worker, float dt)
	{
		float workTime = this.GetWorkTime();
		float num = (workTime - base.WorkTimeRemaining) / workTime;
		this.meter.SetPositionPercent(num);
	}

	private void DestroyVisualizer()
	{
		if (this.visualizer != null)
		{
			if (this.visualizerLink != null)
			{
				this.visualizerLink.Unregister();
				this.visualizerLink = null;
			}
			Util.KDestroyGameObject(this.visualizer);
			this.visualizer = null;
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private ComplexFabricator fabricator;

	public Action<Worker, float> OnWorkTickActions;

	public MeterController meter;

	protected GameObject visualizer;

	protected KAnimLink visualizerLink;
}
