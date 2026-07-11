using System;
using KSerialization;
using UnityEngine;

public class Grave : StateMachineComponent<Grave.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Grave>(-1697596308, Grave.OnStorageChangedDelegate);
		this.epitaphIdx = global::UnityEngine.Random.Range(0, int.MaxValue);
	}

	protected override void OnSpawn()
	{
		base.GetComponent<Storage>().SetOffsets(Grave.DELIVERY_OFFSETS);
		Storage component = base.GetComponent<Storage>();
		Storage storage = component;
		storage.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(storage.OnWorkableEventCB, new Action<Workable.WorkableEvent>(this.OnWorkEvent));
		KAnimFile anim = Assets.GetAnim("anim_bury_dupe_kanim");
		int num = 0;
		KAnim.Anim anim2;
		for (;;)
		{
			anim2 = anim.GetData().GetAnim(num);
			if (anim2 == null)
			{
				goto IL_009C;
			}
			if (anim2.name == "working_pre")
			{
				break;
			}
			num++;
		}
		float num2 = (float)(anim2.numFrames - 3) / anim2.frameRate;
		component.SetWorkTime(num2);
		IL_009C:
		base.OnSpawn();
		base.smi.StartSM();
		Components.Graves.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.Graves.Remove(this);
		base.OnCleanUp();
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject != null)
		{
			this.graveName = gameObject.name;
			Util.KDestroyGameObject(gameObject);
		}
	}

	private void OnWorkEvent(Workable.WorkableEvent evt)
	{
		if (evt != Workable.WorkableEvent.WorkStarted)
		{
		}
	}

	[Serialize]
	public string graveName;

	[Serialize]
	public int epitaphIdx;

	[Serialize]
	public float burialTime = -1f;

	private static readonly CellOffset[] DELIVERY_OFFSETS = new CellOffset[] { default(CellOffset) };

	private static readonly EventSystem.IntraObjectHandler<Grave> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<Grave>(delegate(Grave component, object data)
	{
		component.OnStorageChanged(data);
	});

	public class StatesInstance : GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.GameInstance
	{
		public StatesInstance(Grave master)
			: base(master)
		{
		}

		public void CreateFetchTask()
		{
			this.chore = new FetchChore(Db.Get().ChoreTypes.OperateFetch, base.GetComponent<Storage>(), 1f, new Tag[] { GameTags.Corpse }, null, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0, null);
			this.chore.allowMultifetch = false;
		}

		public void CancelFetchTask()
		{
			this.chore.Cancel("Exit State");
			this.chore = null;
		}

		private FetchChore chore;
	}

	public class States : GameStateMachine<Grave.States, Grave.StatesInstance, Grave>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.empty;
			base.serializable = true;
			this.empty.PlayAnim("open").Enter("CreateFetchTask", delegate(Grave.StatesInstance smi)
			{
				smi.CreateFetchTask();
			}).Exit("CancelFetchTask", delegate(Grave.StatesInstance smi)
			{
				smi.CancelFetchTask();
			})
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GraveEmpty)
				.EventTransition(GameHashes.OnStorageChange, this.full, null);
			this.full.PlayAnim("closed").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Grave).Enter(delegate(Grave.StatesInstance smi)
			{
				if (smi.master.burialTime < 0f)
				{
					smi.master.burialTime = GameClock.Instance.GetTime();
				}
			});
		}

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State empty;

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State full;
	}
}
