using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.contents = this.possible_contents_ids[global::UnityEngine.Random.Range(0, this.possible_contents_ids.Length)];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	public void DropContents()
	{
		Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 1, this.contents, Grid.SceneLayer.Front, Folder.Entities).SetActive(true);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(this.contents.ToTag()).GetProperName(), base.smi.master.transform, 1.5f, false);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.IsInsideState(base.smi.sm.closed) && !this.used)
		{
			if (this.chore != null)
			{
				UserMenu userMenu = this.userMenu;
				string text = "action_harvest";
				string text2 = UI.USERMENUACTIONS.OPENPOI.NAME_OFF;
				global::System.Action action = new global::System.Action(this.OnClickCancel);
				string text3 = UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF;
				userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = this.userMenu;
				string text3 = "action_harvest";
				string text2 = UI.USERMENUACTIONS.OPENPOI.NAME;
				global::System.Action action = new global::System.Action(this.OnClickOpen);
				string text = UI.USERMENUACTIONS.OPENPOI.TOOLTIP;
				userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
			}
		}
	}

	private void OnClickOpen()
	{
		this.ActivateChore(null);
	}

	private void OnClickCancel()
	{
		this.CancelChore(null);
	}

	public void ActivateChore(object param = null)
	{
		if (this.chore != null)
		{
			return;
		}
		base.GetComponent<Workable>().SetWorkTime(1.5f);
		ChoreType emptyStorage = Db.Get().ChoreTypes.EmptyStorage;
		KAnimFile anim = Assets.GetAnim("anim_interacts_clothingfactory_kanim");
		this.chore = new WorkChore<Workable>(emptyStorage, this, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, true, default(Tag), anim, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
		this.OnRefreshUserMenu(null);
	}

	public void CancelChore(object param = null)
	{
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel("User cancelled");
		this.chore = null;
	}

	private void CompleteChore()
	{
		this.used = true;
		base.smi.GoTo(base.smi.sm.open);
		this.DropContents();
		this.chore.Cleanup();
		this.chore = null;
		this.userMenu.Refresh();
	}

	[Serialize]
	private string contents = string.Empty;

	private string[] possible_contents_ids = new string[] { "Warm_Vest", "Cool_Vest", "Funky_Vest" };

	[Serialize]
	private bool used;

	private Chore chore;

	[MyCmpAdd]
	private UserMenu userMenu;

	public class StatesInstance : GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.GameInstance
	{
		public StatesInstance(SetLocker master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.closed;
			this.closed.PlayAnim("on");
			this.open.PlayAnim("working");
		}

		public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State closed;

		public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State open;
	}
}
