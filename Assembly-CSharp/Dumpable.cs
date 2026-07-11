using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Dumpable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Dumpable>(493375141, Dumpable.OnRefreshUserMenuDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForDumping)
		{
			this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}
		base.SetWorkTime(0.1f);
	}

	public void ToggleDumping()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
			return;
		}
		if (this.isMarkedForDumping)
		{
			this.isMarkedForDumping = false;
			this.chore.Cancel("Cancel Dumping!");
			this.chore = null;
			base.ShowProgressBar(false);
			return;
		}
		this.isMarkedForDumping = true;
		this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		this.isMarkedForDumping = false;
		this.chore = null;
		this.Dump();
	}

	public void Dump()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	public void Dump(Vector3 pos)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(Grid.PosToCell(pos), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.HasTag(GameTags.Stored))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = (this.isMarkedForDumping ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME_OFF, new global::System.Action(this.ToggleDumping), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DUMP.TOOLTIP_OFF, true) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.DUMP.NAME, new global::System.Action(this.ToggleDumping), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.DUMP.TOOLTIP, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private Chore chore;

	[Serialize]
	private bool isMarkedForDumping;

	private static readonly EventSystem.IntraObjectHandler<Dumpable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Dumpable>(delegate(Dumpable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
