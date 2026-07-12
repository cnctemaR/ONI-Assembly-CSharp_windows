using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CryoTank : StateMachineComponent<CryoTank.StatesInstance>, ISidescreenButtonControl
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public string SidescreenButtonText
	{
		get
		{
			return BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTON;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTONTOOLTIP;
		}
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		this.OnClickOpen();
	}

	public bool SidescreenButtonInteractable()
	{
		return base.smi.IsInsideState(base.smi.sm.closed) && this.chore == null;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void DropContents()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(base.gameObject), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		new MinionStartingStats(false, null, "AncientKnowledge").Apply(gameObject);
		gameObject.GetComponent<MinionIdentity>().arrivalTime = (float)global::UnityEngine.Random.Range(-2000, -1000);
		MinionResume component = gameObject.GetComponent<MinionResume>();
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			component.ForceAddSkillPoint();
		}
		if (this.opener != null)
		{
			this.opener.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
			gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
			SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.CryoFriend, -1).smi as SimpleEvent.StatesInstance;
			statesInstance.minions = new GameObject[] { gameObject, this.opener };
			statesInstance.SetTextParameter("dupe", this.opener.GetProperName());
			statesInstance.SetTextParameter("friend", gameObject.GetProperName());
			statesInstance.ShowEventPopup(null);
		}
		SaveGame.Instance.GetComponent<ColonyAchievementTracker>().defrostedDuplicant = true;
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
		this.opener = this.chore.driver.gameObject;
		base.smi.GoTo(base.smi.sm.open);
		this.chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public string[][] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public Vector2I dropOffset = Vector2I.zero;

	[Serialize]
	private string[] contents;

	private GameObject opener;

	private Chore chore;

	public class StatesInstance : GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.GameInstance
	{
		public StatesInstance(CryoTank master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.closed;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.closed.PlayAnim("on").Enter(delegate(CryoTank.StatesInstance smi)
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
			this.open.PlayAnim("working").OnAnimQueueComplete(this.off).Exit(delegate(CryoTank.StatesInstance smi)
			{
				smi.master.DropContents();
			});
			this.off.PlayAnim("off").Enter(delegate(CryoTank.StatesInstance smi)
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

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State closed;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State open;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State off;
	}
}
