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

	public AttributeConverter AttributeConverter
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

	public string SkillExperienceSkillGroup
	{
		set
		{
			this.skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			this.skillExperienceMultiplier = value;
		}
	}

	public ComplexRecipe CurrentWorkingOrder
	{
		get
		{
			return (!(this.fabricator != null)) ? null : this.fabricator.CurrentWorkingOrder;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
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
		if (this.fabricator.CurrentWorkingOrder != null)
		{
			this.InstantiateVisualizer(this.fabricator.CurrentWorkingOrder);
		}
		else
		{
			DebugUtil.DevAssertArgs(false, new object[] { "ComplexFabricatorWorkable.OnStartWork called but CurrentMachineOrder is null", base.gameObject });
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (this.OnWorkTickActions != null)
		{
			this.OnWorkTickActions(worker, dt);
		}
		this.UpdateOrderProgress(worker, dt);
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		ComplexRecipe currentWorkingOrder = this.fabricator.CurrentWorkingOrder;
		if (currentWorkingOrder != null)
		{
			this.workTime = currentWorkingOrder.time;
			return this.workTime;
		}
		return -1f;
	}

	public Chore CreateWorkChore(ChoreType choreType, float order_progress)
	{
		WorkChore<ComplexFabricatorWorkable> workChore = new WorkChore<ComplexFabricatorWorkable>(choreType, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		this.workTimeRemaining = this.GetWorkTime() * (1f - order_progress);
		return workChore;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		this.fabricator.CompleteWorkingOrder();
		this.DestroyVisualizer();
	}

	private void InstantiateVisualizer(ComplexRecipe recipe)
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
		if (recipe.FabricationVisualizer == null)
		{
			return;
		}
		this.visualizer = Util.KInstantiate(recipe.FabricationVisualizer, null, null);
		this.visualizer.transform.parent = this.meter.meterController.transform;
		this.visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
		this.visualizer.SetActive(true);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = this.visualizer.GetComponent<KBatchedAnimController>();
		this.visualizerLink = new KAnimLink(component, component2);
	}

	private void UpdateOrderProgress(Worker worker, float dt)
	{
		float workTime = this.GetWorkTime();
		float num = Mathf.Clamp01((workTime - base.WorkTimeRemaining) / workTime);
		if (this.fabricator)
		{
			this.fabricator.OrderProgress = num;
		}
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(num);
		}
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
