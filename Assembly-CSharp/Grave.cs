using System;
using KSerialization;
using UnityEngine;

public class Grave : StateMachineComponent<Grave.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.Subscribe(1502190696, new Action<object>(this.OnDestroyObject));
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
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
		if (this.graveName != null)
		{
			GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.Bones, Folder.Misc);
			gameObject.transform.position = this.transform.position;
			gameObject.SetActive(true);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Temperature = gameObject.GetComponent<PrimaryElement>().Temperature;
		}
	}

	[Serialize]
	public string graveName;

	public class StatesInstance : GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.GameInstance
	{
		public StatesInstance(Grave master)
			: base(master)
		{
		}

		public void CreateFetchTask()
		{
			this.chore = new FetchChore(base.GetComponent<Storage>(), 1f, new Tag[] { GameTags.Corpse }, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0);
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
			this.empty.PlayAnim("open", KAnim.PlayMode.Once, null).Enter("CreateFetchTask", delegate(Grave.StatesInstance smi)
			{
				smi.CreateFetchTask();
			}).Exit("CancelFetchTask", delegate(Grave.StatesInstance smi)
			{
				smi.CancelFetchTask();
			})
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GraveEmpty)
				.EventTransition(GameHashes.OnStorageChange, this.full, null);
			this.full.PlayAnim("closed", KAnim.PlayMode.Once, null).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Grave);
		}

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State empty;

		public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State full;
	}
}
