using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MultiMinionDiningTable : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public static int SeatCount
	{
		get
		{
			return MultiMinionDiningTableConfig.SeatCount;
		}
	}

	public bool HasSalt
	{
		get
		{
			return this.storage != null && this.storage.GetMassAvailable(TableSaltConfig.TAG) >= TableSaltTuning.CONSUMABLE_RATE;
		}
	}

	private static GameObject SpawnSeat(MultiMinionDiningTable diningTable, int diningTableCell, int seatIndex)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ApproachableLocator.ID), diningTable.transform.gameObject, "MultiMinionDiningSeat");
		Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(diningTableCell, MultiMinionDiningTableConfig.seats[seatIndex].TableRelativeLocation), Grid.SceneLayer.Move);
		gameObject.transform.SetPosition(vector);
		gameObject.SetActive(true);
		gameObject.AddOrGet<MultiMinionDiningTable.Seat>().Initialize(seatIndex);
		gameObject.AddOrGet<Reservable>();
		gameObject.GetComponent<KPrefabID>().CopyTags(diningTable.GetComponent<KPrefabID>());
		return gameObject;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		for (int i = 0; i < MultiMinionDiningTable.SeatCount; i++)
		{
			MultiMinionDiningTable.SpawnSeat(this, num, i);
		}
		this.animController.Play(MultiMinionDiningTable.ANIM, KAnim.PlayMode.Once, 1f, 0f);
		this.UpdateSaltVisibility();
		this.storage.Subscribe(-1697596308, delegate(object _)
		{
			this.UpdateSaltVisibility();
		});
	}

	public void UpdateSaltVisibility()
	{
		if (this.HasSalt)
		{
			foreach (MultiMinionDiningTable.Seat seat in base.gameObject.GetComponentsInChildren<MultiMinionDiningTable.Seat>())
			{
				bool flag = !seat.HasDiner;
				this.animController.SetSymbolVisiblity(seat.SaltSymbol, flag);
			}
			return;
		}
		foreach (MultiMinionDiningTable.Seat seat2 in base.gameObject.GetComponentsInChildren<MultiMinionDiningTable.Seat>())
		{
			this.animController.SetSymbolVisiblity(seat2.SaltSymbol, false);
		}
	}

	private void RegisterCommunalDiner(KPrefabID diner)
	{
		Effects effects;
		if (diner.TryGetComponent<Effects>(out effects))
		{
			effects.Add(MultiMinionDiningTable.COMMUNAL_DINING_EFFECT, true);
		}
		else
		{
			global::Debug.LogWarning("Diner has no Effects component");
		}
		this.communalDiners[diner.gameObject] = new MultiMinionDiningTable.Diner(this, diner);
	}

	private void UnregisterCommunalDiner(KPrefabID dinerKpid)
	{
		MultiMinionDiningTable.Diner diner;
		if (this.communalDiners.TryGetValue(dinerKpid.gameObject, out diner))
		{
			diner.CleanUp();
			this.communalDiners.Remove(dinerKpid.gameObject);
		}
	}

	private void OnDinerStartTalking(KPrefabID diner, object untypedStartTalkingEvent)
	{
		ConversationManager.StartedTalkingEvent startedTalkingEvent = untypedStartTalkingEvent as ConversationManager.StartedTalkingEvent;
		if (startedTalkingEvent == null)
		{
			return;
		}
		KPrefabID kprefabID;
		if (!startedTalkingEvent.talker.TryGetComponent<KPrefabID>(out kprefabID))
		{
			return;
		}
		if (kprefabID != diner)
		{
			return;
		}
		diner.AddTag(GameTags.WantsToTalk, false);
		diner.AddTag(GameTags.DoNotInterruptMe, false);
	}

	private void OnDinerStopTalking(KPrefabID diner, object untypedStoppedTalker)
	{
		GameObject gameObject = untypedStoppedTalker as GameObject;
		if (gameObject == null)
		{
			return;
		}
		KPrefabID kprefabID;
		if (!gameObject.TryGetComponent<KPrefabID>(out kprefabID))
		{
			return;
		}
		if (kprefabID != diner)
		{
			return;
		}
		diner.RemoveTag(GameTags.WantsToTalk);
	}

	private void OnDinerChanged(KPrefabID prevDiner, KPrefabID newDiner, int seatIndex)
	{
		MultiMinionDiningTable.Seat[] componentsInChildren = base.gameObject.GetComponentsInChildren<MultiMinionDiningTable.Seat>();
		bool flag = newDiner == null && this.HasSalt;
		this.animController.SetSymbolVisiblity(componentsInChildren[seatIndex].SaltSymbol, flag);
		if (prevDiner != null && this.communalDiners.ContainsKey(prevDiner.gameObject))
		{
			this.UnregisterCommunalDiner(prevDiner);
		}
		if (newDiner != null)
		{
			int num = 0;
			MultiMinionDiningTable.Seat[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].HasDiner)
				{
					num++;
					if (num > 1)
					{
						break;
					}
				}
			}
			if (num > 1)
			{
				foreach (MultiMinionDiningTable.Seat seat in componentsInChildren)
				{
					if (!(seat.Diner == null) && !this.communalDiners.ContainsKey(seat.Diner.gameObject))
					{
						this.RegisterCommunalDiner(seat.Diner);
					}
				}
				return;
			}
		}
		else if (this.communalDiners.Count == 1)
		{
			foreach (MultiMinionDiningTable.Seat seat2 in componentsInChildren)
			{
				if (!(seat2.Diner == null))
				{
					this.UnregisterCommunalDiner(seat2.Diner);
				}
			}
		}
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor> { MultiMinionDiningTable.COMMUNAL_DINING_DESCRIPTOR };
		if (this.HasSalt)
		{
			list.Add(MessStation.TABLE_SALT_DESCRIPTOR);
		}
		return list;
	}

	public const string SEAT_ID = "MultiMinionDiningSeat";

	[MyCmpGet]
	private readonly Storage storage;

	private static readonly HashedString ANIM = "salt";

	[MyCmpReq]
	private readonly KAnimControllerBase animController;

	private readonly Dictionary<GameObject, MultiMinionDiningTable.Diner> communalDiners = new Dictionary<GameObject, MultiMinionDiningTable.Diner>();

	private static readonly HashedString COMMUNAL_DINING_EFFECT = "CommunalDining";

	private const int MORALE_MODIFIER = 1;

	private static readonly Descriptor COMMUNAL_DINING_DESCRIPTOR = new Descriptor(string.Format(UI.BUILDINGEFFECTS.COMMUNAL_DINING, 1), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.COMMUNAL_DINING, 1), Descriptor.DescriptorType.Effect, false);

	public class Seat : Assignable, IDiningSeat
	{
		private MultiMinionDiningTableConfig.Seat SeatConfig
		{
			get
			{
				return MultiMinionDiningTableConfig.seats[this.index];
			}
		}

		public HashedString SaltSymbol
		{
			get
			{
				return this.SeatConfig.SaltSymbol;
			}
		}

		public GameObject DiningTable
		{
			get
			{
				return base.transform.parent.gameObject;
			}
		}

		public bool HasSalt
		{
			get
			{
				return this.DiningTable.GetComponent<MultiMinionDiningTable>().HasSalt;
			}
		}

		public HashedString EatAnim
		{
			get
			{
				return this.SeatConfig.EatAnim;
			}
		}

		public HashedString ReloadElectrobankAnim
		{
			get
			{
				return this.SeatConfig.ReloadElectrobankAnim;
			}
		}

		public Storage FindStorage()
		{
			return this.DiningTable.GetComponent<Storage>();
		}

		public Operational FindOperational()
		{
			return this.DiningTable.GetComponent<Operational>();
		}

		public KPrefabID Diner
		{
			get
			{
				return this.diner;
			}
			set
			{
				KPrefabID kprefabID = this.diner;
				this.diner = value;
				this.DiningTable.GetComponent<MultiMinionDiningTable>().OnDinerChanged(kprefabID, this.diner, this.index);
			}
		}

		public bool HasDiner
		{
			get
			{
				return this.Diner != null;
			}
		}

		public Seat()
		{
			this.slotID = Db.Get().AssignableSlots.MessStation.Id;
			this.canBePublic = true;
		}

		public void Initialize(int index)
		{
			this.index = index;
		}

		private int index;

		private KPrefabID diner;
	}

	private readonly struct Diner
	{
		public Diner(MultiMinionDiningTable table, KPrefabID diner)
		{
			this.kpid = diner;
			diner.AddTag(GameTags.CommunalDining, false);
			diner.AddTag(GameTags.AlwaysConverse, false);
			this.startTalkingHandler = diner.Subscribe(-594200555, delegate(object eventData)
			{
				table.OnDinerStartTalking(diner, eventData);
			});
			this.stopTalkingHandler = diner.Subscribe(25860745, delegate(object eventData)
			{
				table.OnDinerStopTalking(diner, eventData);
			});
		}

		public void CleanUp()
		{
			this.kpid.RemoveTag(GameTags.CommunalDining);
			this.kpid.RemoveTag(GameTags.AlwaysConverse);
			this.kpid.Unsubscribe(this.startTalkingHandler);
			this.kpid.Unsubscribe(this.stopTalkingHandler);
		}

		private readonly KPrefabID kpid;

		private readonly int startTalkingHandler;

		private readonly int stopTalkingHandler;
	}
}
