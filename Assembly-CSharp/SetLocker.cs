using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>, ISidescreenButtonControl
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void ChooseContents()
	{
		this.contents = this.possible_contents_ids[global::UnityEngine.Random.Range(0, this.possible_contents_ids.GetLength(0))];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void DropContents()
	{
		if (this.contents == null)
		{
			return;
		}
		for (int i = 0; i < this.contents.Length; i++)
		{
			Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), this.dropOffset.x, this.dropOffset.y, this.contents[i], Grid.SceneLayer.Front).SetActive(true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(this.contents[i].ToTag()).GetProperName(), base.smi.master.transform, 1.5f, false);
		}
		if (DlcManager.IsExpansion1Active() && this.numDataBanks.Length >= 2)
		{
			int num = global::UnityEngine.Random.Range(this.numDataBanks[0], this.numDataBanks[1]);
			for (int j = 0; j <= num; j++)
			{
				Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), this.dropOffset.x, this.dropOffset.y, "OrbitalResearchDatabank", Grid.SceneLayer.Front).SetActive(true);
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab("OrbitalResearchDatabank".ToTag()).GetProperName(), base.smi.master.transform, 1.5f, false);
			}
		}
		base.gameObject.Trigger(-372600542, this);
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
		this.chore = new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, delegate(Chore o)
		{
			this.CompleteChore();
		}, null, null, true, null, false, true, Assets.GetAnim(this.overrideAnim), false, true, true, PriorityScreen.PriorityClass.high, 5, false, true);
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
		this.chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public string SidescreenButtonText
	{
		get
		{
			return (this.chore == null) ? UI.USERMENUACTIONS.OPENPOI.NAME : UI.USERMENUACTIONS.OPENPOI.NAME_OFF;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return (this.chore == null) ? UI.USERMENUACTIONS.OPENPOI.TOOLTIP : UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF;
		}
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		if (this.chore == null)
		{
			this.OnClickOpen();
			return;
		}
		this.OnClickCancel();
	}

	public bool SidescreenButtonInteractable()
	{
		return !this.used;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride text)
	{
		throw new NotImplementedException();
	}

	public string[][] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public Vector2I dropOffset = Vector2I.zero;

	public int[] numDataBanks;

	[Serialize]
	private string[] contents;

	[Serialize]
	private bool used;

	private Chore chore;

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
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.closed.PlayAnim("on").Enter(delegate(SetLocker.StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component = smi.master.GetComponent<LoopingSounds>();
					if (component != null)
					{
						component.StartSound(GlobalAssets.GetSound(smi.master.machineSound, false));
					}
				}
			});
			this.open.PlayAnim("working").OnAnimQueueComplete(this.off).Exit(delegate(SetLocker.StatesInstance smi)
			{
				smi.master.DropContents();
			});
			this.off.PlayAnim("off").Enter(delegate(SetLocker.StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component2 = smi.master.GetComponent<LoopingSounds>();
					if (component2 != null)
					{
						component2.StopSound(GlobalAssets.GetSound(smi.master.machineSound, false));
					}
				}
			});
		}

		public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State closed;

		public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State open;

		public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State off;
	}
}
