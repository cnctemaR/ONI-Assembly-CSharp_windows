using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDeliveryPoint : StateMachineComponent<CreatureDeliveryPoint.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.filteredStorage = new FilteredStorage(this, null, this.filterTint, this.noFilterTint, null);
		base.GetComponent<Storage>().SetOffsets(Grid.DefaultOffset);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.filteredStorage.FilterChanged();
		WorldInventory.Instance.Discover(EntityTemplates.GetBaggedCreatureTag("Glom".ToTag()), GameTags.BagableCreature);
		WorldInventory.Instance.Discover(EntityTemplates.GetBaggedCreatureTag("Hatch".ToTag()), GameTags.BagableCreature);
		WorldInventory.Instance.Discover(EntityTemplates.GetBaggedCreatureTag("Oilfloater".ToTag()), GameTags.BagableCreature);
	}

	protected override void OnCleanUp()
	{
		this.filteredStorage.CleanUp();
		base.smi.StopSM("OnCleanUp");
		base.OnCleanUp();
	}

	[SerializeField]
	public Color32 noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);

	[SerializeField]
	public Color32 filterTint = new Color(1f, 1f, 1f, 1f);

	private FilteredStorage filteredStorage;

	public class SMInstance : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.GameInstance
	{
		public SMInstance(CreatureDeliveryPoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.waiting;
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.creatureDelivered, (CreatureDeliveryPoint.SMInstance smi) => !smi.GetComponent<Storage>().IsEmpty());
			this.creatureDelivered.Enter(delegate(CreatureDeliveryPoint.SMInstance smi)
			{
				Storage component = smi.master.GetComponent<Storage>();
				List<GameObject> items = component.items;
				int count = items.Count;
				for (int i = count - 1; i >= 0; i--)
				{
					GameObject gameObject = items[i];
					component.Remove(gameObject);
					KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
					Tag unbaggedCreatureTag = EntityTemplates.GetUnbaggedCreatureTag(component2.PrefabTag);
					GameObject prefab = Assets.GetPrefab(unbaggedCreatureTag);
					GameObject gameObject2 = Util.KInstantiate(prefab, Folder.Entities);
					gameObject2.transform.position = smi.master.transform.position;
					gameObject2.SetActive(true);
					Util.KDestroyGameObject(gameObject);
				}
			}).GoTo(this.waiting);
		}

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State creatureDelivered;
	}
}
