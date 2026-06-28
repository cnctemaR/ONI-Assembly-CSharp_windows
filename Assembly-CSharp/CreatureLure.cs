using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureLure : StateMachineComponent<CreatureLure.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.operational = base.GetComponent<Operational>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.Lures.Add(this);
		if (this.activeBaitSetting == Tag.Invalid)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, null);
		}
		else
		{
			this.ChangeBaitSetting(this.activeBaitSetting);
		}
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
	}

	protected override void OnCleanUp()
	{
		Components.Lures.Remove(this);
		base.OnCleanUp();
	}

	private void OnStorageChange(object data = null)
	{
		this.operational.SetFlag(this.baited, this.baitStorage.GetAmountAvailable(this.activeBaitSetting) > 0f);
	}

	public void ChangeBaitSetting(Tag baitSetting)
	{
		if (this.fetchChore != null)
		{
			this.fetchChore.Cancel("SwitchedResource");
		}
		if (baitSetting != this.activeBaitSetting)
		{
			this.activeBaitSetting = baitSetting;
			this.baitStorage.DropAll(false);
		}
		base.smi.GoTo(base.smi.sm.idle);
		this.baitStorage.storageFilters = new List<Tag> { this.activeBaitSetting };
		if (baitSetting != Tag.Invalid)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, false);
			if (base.smi.master.baitStorage.IsEmpty())
			{
				this.CreateFetchChore();
			}
		}
		else
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, null);
		}
	}

	protected void CreateFetchChore()
	{
		if (this.fetchChore != null)
		{
			this.fetchChore.Cancel("Overwrite");
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, false);
		if (this.activeBaitSetting == Tag.Invalid)
		{
			return;
		}
		this.fetchChore = new FetchChore(Db.Get().ChoreTypes.Transport, this.baitStorage, 100f, new Tag[] { this.activeBaitSetting }, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0, null);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, null);
	}

	public static float CONSUMPTION_RATE = 1f;

	public CellOffset[] lurePoints = new CellOffset[]
	{
		new CellOffset(0, 0)
	};

	[Serialize]
	public Tag activeBaitSetting;

	public List<Tag> baitTypes;

	public Storage baitStorage;

	protected FetchChore fetchChore;

	private Operational operational;

	private Operational.Flag baited = new Operational.Flag("Baited", Operational.Flag.Type.Requirement);

	public class StatesInstance : GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.GameInstance
	{
		public StatesInstance(CreatureLure master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.PlayAnim("off", KAnim.PlayMode.Loop).Enter(delegate(CreatureLure.StatesInstance smi)
			{
				if (smi.master.activeBaitSetting != Tag.Invalid)
				{
					if (smi.master.baitStorage.IsEmpty())
					{
						smi.master.CreateFetchChore();
					}
					else if (smi.master.operational.IsOperational)
					{
						smi.GoTo(this.working);
					}
				}
			}).EventTransition(GameHashes.OnStorageChange, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid && smi.master.operational.IsOperational);
			this.working.Enter(delegate(CreatureLure.StatesInstance smi)
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, false);
				HashedString batchTag = ElementLoader.FindElementByName(smi.master.activeBaitSetting.ToString()).substance.anim.batchTag;
				KAnim.Build build = ElementLoader.FindElementByName(smi.master.activeBaitSetting.ToString()).substance.anim.GetData().build;
				KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
				KAnimHashedString kanimHashedString = new KAnimHashedString("slime_mold");
				smi.master.GetComponent<KBatchedAnimController>().RemoveSymbolOverride(kanimHashedString);
				smi.master.GetComponent<KBatchedAnimController>().AddSymbolOverride(kanimHashedString, batchTag, symbol, false);
			}).QueueAnim("working_pre", false, null).QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.OnStorageChange, this.empty, (CreatureLure.StatesInstance smi) => smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid)
				.EventTransition(GameHashes.OperationalChanged, this.idle, (CreatureLure.StatesInstance smi) => !smi.master.operational.IsOperational && !smi.master.baitStorage.IsEmpty());
			this.empty.QueueAnim("working_pst", false, null).QueueAnim("off", false, null).Enter(delegate(CreatureLure.StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			})
				.EventTransition(GameHashes.OnStorageChange, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational);
		}

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State idle;

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State working;

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State empty;
	}
}
