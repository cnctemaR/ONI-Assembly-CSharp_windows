using System;
using KSerialization;
using STRINGS;

public class Dumpable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForDumping)
		{
			this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
		base.SetWorkTime(0.1f);
	}

	public void ToggleDumping()
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.OnCompleteWork(null);
		}
		else if (this.isMarkedForDumping)
		{
			this.isMarkedForDumping = false;
			this.chore.Cancel("Cancel Dumping!");
			this.chore = null;
			base.ShowProgressBar(false);
		}
		else
		{
			this.isMarkedForDumping = true;
			this.chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
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
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, -1);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.HasTag(GameTags.Stored))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.isMarkedForDumping)
		{
			string text = "action_empty_contents";
			string text2 = UI.USERMENUACTIONS.DUMP.NAME_OFF;
			global::System.Action action = new global::System.Action(this.ToggleDumping);
			string text3 = UI.USERMENUACTIONS.DUMP.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_empty_contents";
			string text2 = UI.USERMENUACTIONS.DUMP.NAME;
			global::System.Action action = new global::System.Action(this.ToggleDumping);
			string text = UI.USERMENUACTIONS.DUMP.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	private Chore chore;

	[Serialize]
	private bool isMarkedForDumping;
}
