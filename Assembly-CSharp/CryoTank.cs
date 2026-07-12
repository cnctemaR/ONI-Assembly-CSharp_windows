using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CryoTank : StateMachineComponent<CryoTank.StatesInstance>, ISidescreenButtonControl
{
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
		return this.HasDefrostedFriend();
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride text)
	{
		throw new NotImplementedException();
	}

	public int HorizontalGroupID()
	{
		return -1;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Demolishable component = base.GetComponent<Demolishable>();
		if (component != null)
		{
			component.allowDemolition = !this.HasDefrostedFriend();
		}
	}

	public bool HasDefrostedFriend()
	{
		return base.smi.IsInsideState(base.smi.sm.closed) && this.chore == null;
	}

	public void DropContents()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(base.transform.position), this.dropOffset), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		new MinionStartingStats(false, null, "AncientKnowledge", false).Apply(gameObject);
		gameObject.GetComponent<MinionIdentity>().arrivalTime = (float)global::UnityEngine.Random.Range(-2000, -1000);
		MinionResume component = gameObject.GetComponent<MinionResume>();
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			component.ForceAddSkillPoint();
		}
		base.smi.sm.defrostedDuplicant.Set(gameObject, base.smi, false);
		gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
		ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
		if (component2 != null)
		{
			base.smi.defrostAnimChore = new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_cryo_chamber_kanim", new HashedString[] { "defrost", "defrost_exit" }, KAnim.PlayMode.Once, false);
			Vector3 position = gameObject.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Gas);
			gameObject.transform.SetPosition(position);
			gameObject.GetMyWorld().SetDupeVisited();
		}
		SaveGame.Instance.GetComponent<ColonyAchievementTracker>().defrostedDuplicant = true;
	}

	public void ShowEventPopup()
	{
		GameObject gameObject = base.smi.sm.defrostedDuplicant.Get(base.smi);
		if (this.opener != null && gameObject != null)
		{
			SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.CryoFriend, -1, null).smi as SimpleEvent.StatesInstance;
			statesInstance.minions = new GameObject[] { gameObject, this.opener };
			statesInstance.SetTextParameter("dupe", this.opener.GetProperName());
			statesInstance.SetTextParameter("friend", gameObject.GetProperName());
			statesInstance.ShowEventPopup();
		}
	}

	public void Cheer()
	{
		GameObject gameObject = base.smi.sm.defrostedDuplicant.Get(base.smi);
		if (this.opener != null && gameObject != null)
		{
			Db db = Db.Get();
			this.opener.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
			new EmoteChore(this.opener.GetComponent<Effects>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 1, null);
			gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
			new EmoteChore(gameObject.GetComponent<Effects>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 1, null);
		}
	}

	private void OnClickOpen()
	{
		this.ActivateChore(null);
	}

	private void OnClickCancel()
	{
		this.CancelActivateChore(null);
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
			this.CompleteActivateChore();
		}, null, null, true, null, false, true, Assets.GetAnim(this.overrideAnim), false, true, true, PriorityScreen.PriorityClass.high, 5, false, true);
	}

	public void CancelActivateChore(object param = null)
	{
		if (this.chore == null)
		{
			return;
		}
		this.chore.Cancel("User cancelled");
		this.chore = null;
	}

	private void CompleteActivateChore()
	{
		this.opener = this.chore.driver.gameObject;
		base.smi.GoTo(base.smi.sm.open);
		this.chore = null;
		Demolishable component = base.smi.GetComponent<Demolishable>();
		if (component != null)
		{
			component.allowDemolition = true;
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public string[][] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public CellOffset dropOffset = CellOffset.none;

	private GameObject opener;

	private Chore chore;

	public class StatesInstance : GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.GameInstance
	{
		public StatesInstance(CryoTank master)
			: base(master)
		{
		}

		public Chore defrostAnimChore;
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
			this.open.GoTo(this.defrost).Exit(delegate(CryoTank.StatesInstance smi)
			{
				smi.master.DropContents();
			});
			this.defrost.PlayAnim("defrost").OnAnimQueueComplete(this.defrostExit).Update(delegate(CryoTank.StatesInstance smi, float dt)
			{
				smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
			}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(CryoTank.StatesInstance smi)
				{
					smi.master.ShowEventPopup();
				});
			this.defrostExit.PlayAnim("defrost_exit").Update(delegate(CryoTank.StatesInstance smi, float dt)
			{
				if (smi.defrostAnimChore == null || smi.defrostAnimChore.isComplete)
				{
					smi.GoTo(this.off);
				}
			}, UpdateRate.SIM_200ms, false).Exit(delegate(CryoTank.StatesInstance smi)
			{
				smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Move);
				smi.master.Cheer();
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

		public StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.TargetParameter defrostedDuplicant;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State closed;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State open;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State defrost;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State defrostExit;

		public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State off;
	}
}
