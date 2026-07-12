using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureLure : StateMachineComponent<CreatureLure.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.operational = base.GetComponent<Operational>();
		base.Subscribe<CreatureLure>(-905833192, CreatureLure.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		CreatureLure component = ((GameObject)data).GetComponent<CreatureLure>();
		if (component != null)
		{
			this.ChangeBaitSetting(component.activeBaitSetting);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (this.activeBaitSetting == Tag.Invalid)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, null);
		}
		else
		{
			this.ChangeBaitSetting(this.activeBaitSetting);
			this.OnStorageChange(null);
		}
		base.Subscribe<CreatureLure>(-1697596308, CreatureLure.OnStorageChangeDelegate);
	}

	private void OnStorageChange(object data = null)
	{
		bool flag = this.baitStorage.GetAmountAvailable(this.activeBaitSetting) > 0f;
		this.operational.SetFlag(this.baited, flag);
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
			this.baitStorage.DropAll(false, false, default(Vector3), true, null);
		}
		base.smi.GoTo(base.smi.sm.idle);
		this.baitStorage.storageFilters = new List<Tag> { this.activeBaitSetting };
		if (baitSetting != Tag.Invalid)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoLureElementSelected, false);
			if (base.smi.master.baitStorage.IsEmpty())
			{
				this.CreateFetchChore();
				return;
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
		this.fetchChore = new FetchChore(Db.Get().ChoreTypes.RanchingFetch, this.baitStorage, 100f, new HashSet<Tag> { this.activeBaitSetting }, FetchChore.MatchCriteria.MatchID, Tag.Invalid, null, null, true, null, null, null, Operational.State.None, 0);
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.AwaitingBaitDelivery, null);
	}

	public static float CONSUMPTION_RATE = 1f;

	[Serialize]
	public Tag activeBaitSetting;

	public List<Tag> baitTypes;

	public Storage baitStorage;

	protected FetchChore fetchChore;

	private Operational operational;

	private Operational.Flag baited = new Operational.Flag("Baited", Operational.Flag.Type.Requirement);

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<CreatureLure> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureLure>(delegate(CreatureLure component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CreatureLure> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CreatureLure>(delegate(CreatureLure component, object data)
	{
		component.OnStorageChange(data);
	});

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
						return;
					}
					if (smi.master.operational.IsOperational)
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
				HashedString hashedString = "slime_mold";
				SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
				component.TryRemoveSymbolOverride(hashedString, 0);
				component.AddSymbolOverride(hashedString, symbol, 0);
				smi.GetSMI<Lure.Instance>().SetActiveLures(new Tag[] { smi.master.activeBaitSetting });
			}).Exit(new StateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State.Callback(CreatureLure.States.ClearBait)).QueueAnim("working_pre", false, null)
				.QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.OnStorageChange, this.empty, (CreatureLure.StatesInstance smi) => smi.master.baitStorage.IsEmpty() && smi.master.activeBaitSetting != Tag.Invalid)
				.EventTransition(GameHashes.OperationalChanged, this.idle, (CreatureLure.StatesInstance smi) => !smi.master.operational.IsOperational && !smi.master.baitStorage.IsEmpty());
			this.empty.QueueAnim("working_pst", false, null).QueueAnim("off", false, null).Enter(delegate(CreatureLure.StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			})
				.EventTransition(GameHashes.OnStorageChange, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational)
				.EventTransition(GameHashes.OperationalChanged, this.working, (CreatureLure.StatesInstance smi) => !smi.master.baitStorage.IsEmpty() && smi.master.operational.IsOperational);
		}

		private static void ClearBait(StateMachine.Instance smi)
		{
			if (smi.GetSMI<Lure.Instance>() != null)
			{
				smi.GetSMI<Lure.Instance>().SetActiveLures(null);
			}
		}

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State idle;

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State working;

		public GameStateMachine<CreatureLure.States, CreatureLure.StatesInstance, CreatureLure, object>.State empty;
	}
}
