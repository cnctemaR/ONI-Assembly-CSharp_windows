using System;
using Klei.AI;
using TUNING;

public class RefineryWorkable : Workable
{
	private Refinery GetRefinery
	{
		get
		{
			if (this.refinery == null)
			{
				this.refinery = base.GetComponent<Refinery>();
			}
			return this.refinery;
		}
	}

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

	protected override void OnSpawn()
	{
		this.refinery = base.GetComponent<Refinery>();
		base.OnSpawn();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	public override string GetConversationTopic()
	{
		string conversationTopic = this.refinery.GetConversationTopic();
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
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.operational.SetActive(false, false);
	}

	public void OnCancelOrder()
	{
		this.workTimeRemaining = this.GetWorkTime();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.OnWorkTickActions != null)
		{
			this.OnWorkTickActions(worker, dt);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		if (this.GetRefinery.GetMachineOrders.Count > 0)
		{
			Refinery.MachineOrder machineOrder = this.refinery.GetMachineOrders[0];
			this.workTime = machineOrder.parentOrder.recipe.time;
			return this.workTime;
		}
		return -1f;
	}

	public void CreateOrder(Refinery.MachineOrder buildable_order, ChoreType choreType, Tag[] choreTags)
	{
		buildable_order.chore = new WorkChore<RefineryWorkable>(choreType, this, null, choreTags, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		if (this.workTimeRemaining <= 0f)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.refinery.OnCompleteWork();
	}

	[MyCmpReq]
	protected Operational operational;

	private Refinery refinery;

	public Action<Worker, float> OnWorkTickActions;
}
