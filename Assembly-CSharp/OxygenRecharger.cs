using System;
using System.Collections.Generic;
using STRINGS;

public class OxygenRecharger : BuildingWorkable, IEffectDescriptor
{
	private OxygenRecharger()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsTankLow";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			SuitTank tank = OxygenRecharger.GetTank(context.consumer.GetComponent<Worker>());
			return tank != null && tank.IsLow();
		};
		this.IsTankLow = precondition;
		Chore.Precondition precondition2 = default(Chore.Precondition);
		precondition2.id = "OwnsMaterials";
		precondition2.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Storage storage = ((OxygenRecharger)data).storage;
			SuitTank tank2 = OxygenRecharger.GetTank(context.consumer.GetComponent<Worker>());
			if (storage == null || tank2 == null)
			{
				return false;
			}
			float massAvailable = storage.GetMassAvailable(new Tag(tank2.element));
			return massAvailable >= OxygenRecharger.requiredMass;
		};
		this.OwnsMaterials = precondition2;
		base..ctor();
		base.SetWorkTime(this.fullTankRechargeTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_suitrecharger_kanim") };
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.animController = base.GetComponent<KAnimControllerBase>();
		base.Subscribe(this.storage.gameObject, -1697596308, new Action<object>(this.OnStorageChange));
		this.CreateChore();
		foreach (Tag tag in this.requiredMaterials)
		{
			Element element = ElementLoader.GetElement(tag);
			if (element.IsSolid || element.IsGas)
			{
				this.CreateFetchList(tag);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(this.storage.gameObject, -1697596308, new Action<object>(this.OnStorageChange));
		base.OnCleanUp();
	}

	private void OnStorageChange(object data)
	{
		for (int i = 0; i < this.requiredMaterials.Length; i++)
		{
			Element element = ElementLoader.GetElement(this.requiredMaterials[i]);
			if (this.storage.GetMassAvailable(this.requiredMaterials[i]) >= OxygenRecharger.requiredMass + 1f)
			{
				if (element.IsGas)
				{
					base.GetComponent<ConduitConsumer>().enabled = false;
				}
			}
			else if (element.IsGas)
			{
				base.GetComponent<ConduitConsumer>().enabled = true;
			}
			else if (element.IsSolid || element.IsLiquid)
			{
				this.CreateFetchList(this.requiredMaterials[i]);
			}
		}
	}

	private void CreateFetchList(Tag mat)
	{
		if (this.pendingMaterials.Contains(mat))
		{
			return;
		}
		FetchList2 fetchList = new FetchList2(this.storage);
		fetchList.Add(mat, OxygenRecharger.requiredMass + 1f, FetchOrder2.OperationalRequirement.None);
		fetchList.Submit(new global::System.Action(this.OnFetchComplete), true);
		if (!this.pendingMaterials.Contains(mat))
		{
			this.pendingMaterials.Add(mat);
		}
	}

	private void OnFetchComplete()
	{
		for (int i = this.pendingMaterials.Count - 1; i >= 0; i--)
		{
			if (this.storage.GetMassAvailable(this.pendingMaterials[i]) >= OxygenRecharger.requiredMass + 1f)
			{
				this.pendingMaterials.Remove(this.pendingMaterials[i]);
			}
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.animController.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.CreateChore();
	}

	protected override void OnStartWork(Worker worker)
	{
		SuitTank tank = OxygenRecharger.GetTank(worker);
		if (tank == null)
		{
			Debug.LogError("Something went wront, the worker is supposed to carry an oxygen tank.", null);
			return;
		}
		this.animController.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Queue("loop", KAnim.PlayMode.Loop, 1f, 0f);
		base.SetWorkTime(this.fullTankRechargeTime * (1f - tank.PercentFull()));
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		SuitTank tank = OxygenRecharger.GetTank(worker);
		if (tank != null)
		{
			this.storage.Consume(new Tag(tank.element), OxygenRecharger.requiredMass / this.fullTankRechargeTime * dt);
			tank.amount += tank.GetInitialAmount() / this.fullTankRechargeTime * dt;
		}
		else
		{
			Debug.LogError("Something went wrong, the worker is supposed to carry an oxygen tank.", null);
		}
		return base.OnWorkTick(worker, dt);
	}

	private void CreateChore()
	{
		this.chore = new WorkChore<OxygenRecharger>(Db.Get().ChoreTypes.Recharge, this, null, true, null, null, null, false, null, true, default(Tag), null, false, true);
		this.chore.AddPrecondition(this.IsTankLow, null);
		this.chore.AddPrecondition(this.OwnsMaterials, this);
	}

	private static SuitTank GetTank(Worker worker)
	{
		AssignableSlotInstance slot = worker.GetComponent<Equipment>().GetSlot(EquipmentSet.Get().slotSet.Get("Suit"));
		if (slot.IsAssigned())
		{
			return slot.assignable.GetComponent<SuitTank>();
		}
		return null;
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Tag tag in this.requiredMaterials)
		{
			string text = tag.ProperName();
			string keywordStyle = GameUtil.GetKeywordStyle(tag);
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(OxygenRecharger.requiredMass, GameUtil.TimeSlice.None, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, keywordStyle, text, GameUtil.GetFormattedMass(OxygenRecharger.requiredMass, GameUtil.TimeSlice.None, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
			list.Add(descriptor);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.REFILLOXYGENTANK, UI.BUILDINGEFFECTS.TOOLTIPS.REFILLOXYGENTANK, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(def))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.EffectDescriptors(def))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	private Chore chore;

	private float fullTankRechargeTime = 40f;

	private KAnimControllerBase animController;

	private Tag[] requiredMaterials = new Tag[]
	{
		new Tag("Oxygen")
	};

	private static float requiredMass = 50f;

	[MyCmpGet]
	private Storage storage;

	private List<Tag> pendingMaterials = new List<Tag>();

	public Chore.Precondition IsTankLow;

	public Chore.Precondition OwnsMaterials;
}
