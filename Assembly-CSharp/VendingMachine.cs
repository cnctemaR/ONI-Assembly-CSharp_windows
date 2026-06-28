using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class VendingMachine : StateMachineComponent<VendingMachine.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	public void DropContents()
	{
		string text = this.contents_ids[global::UnityEngine.Random.Range(0, this.contents_ids.Length)];
		Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 1, 1, text, Grid.SceneLayer.Front, Folder.Entities).SetActive(true);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(text.ToTag()).GetProperName(), base.smi.master.transform, 1.5f, false);
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
		base.GetComponent<Workable>().SetWorkTime(2f);
		ChoreType emptyStorage = Db.Get().ChoreTypes.EmptyStorage;
		KAnimFile anim = Assets.GetAnim("anim_break_kanim");
		this.chore = new WorkChore<Workable>(emptyStorage, this, null, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, true, anim, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
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
		this.chore.Cleanup();
		this.chore = null;
		this.userMenu.Refresh();
	}

	[NonSerialized]
	public string machineSound = "VendingMachine_LP";

	private string[] contents_ids = new string[] { "FieldRation" };

	[Serialize]
	private bool used;

	private Chore chore;

	[MyCmpAdd]
	private UserMenu userMenu;

	public class StatesInstance : GameStateMachine<VendingMachine.States, VendingMachine.StatesInstance, VendingMachine, object>.GameInstance
	{
		public StatesInstance(VendingMachine master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<VendingMachine.States, VendingMachine.StatesInstance, VendingMachine>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.closed;
			this.closed.PlayAnim("on").Enter(delegate(VendingMachine.StatesInstance smi)
			{
				LoopingSounds component = smi.master.GetComponent<LoopingSounds>();
				if (component != null)
				{
					component.StartSound(GlobalAssets.GetSound(smi.master.machineSound, false), smi.master.transform.GetPosition());
				}
			});
			this.open.PlayAnim("working").OnAnimQueueComplete(this.off).Exit(delegate(VendingMachine.StatesInstance smi)
			{
				smi.master.DropContents();
			});
			this.off.PlayAnim("off").Enter(delegate(VendingMachine.StatesInstance smi)
			{
				LoopingSounds component2 = smi.master.GetComponent<LoopingSounds>();
				if (component2 != null)
				{
					component2.StopSound(GlobalAssets.GetSound(smi.master.machineSound, false));
				}
			});
		}

		public GameStateMachine<VendingMachine.States, VendingMachine.StatesInstance, VendingMachine, object>.State closed;

		public GameStateMachine<VendingMachine.States, VendingMachine.StatesInstance, VendingMachine, object>.State open;

		public GameStateMachine<VendingMachine.States, VendingMachine.StatesInstance, VendingMachine, object>.State off;
	}
}
