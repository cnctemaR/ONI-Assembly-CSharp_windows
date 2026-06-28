using System;
using KSerialization;
using UnityEngine;

public class Grave : StateMachineComponent<Grave.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		base.Subscribe(1502190696, new Action<object>(this.OnDestroyObject));
		this.epitaphIdx = global::UnityEngine.Random.Range(0, int.MaxValue);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
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

	private void OnDestroyObject(object data)
	{
		if (this.graveName != null && base.smi.IsInsideState(base.smi.sm.full))
		{
			GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.Bones, Folder.Misc, base.transform.GetPosition());
			gameObject.transform.SetPosition(base.transform.GetPosition());
			gameObject.SetActive(true);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Temperature = gameObject.GetComponent<PrimaryElement>().Temperature;
		}
	}

	[Serialize]
	public string graveName;

	[Serialize]
	public int epitaphIdx;

	public class StatesInstance : GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.GameInstance
	{
		public StatesInstance(Grave master)
			: base(master)
		{
		}

		public void CreateFetchTask()
		{
			this.chore = new FetchChore(Db.Get().ChoreTypes.OperateFetch, base.GetComponent<Storage>(), 1f, new Tag[] { GameTags.Corpse }, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0, null);
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
			this.full.PlayAnim("closed").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Grave);
		}

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State empty;

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State full;
	}
}
