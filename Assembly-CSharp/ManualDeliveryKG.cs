using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ManualDeliveryKG")]
public class ManualDeliveryKG : KMonoBehaviour, ISim1000ms
{
	public Tag RequestedItemTag
	{
		get
		{
			if (this.deliveryRequests.Count == 0)
			{
				return Tag.Invalid;
			}
			return this.deliveryRequests[0].Id;
		}
		set
		{
			this.AbortDelivery("Requested Item Tag Changed");
			this.ClearRequests();
			this.RequestItemInternal(value, this.minimumMass, null);
		}
	}

	public Tag[] ForbiddenTags
	{
		get
		{
			return this.forbiddenTags;
		}
		set
		{
			this.forbiddenTags = value;
			this.AbortDelivery("Forbidden Tags Changed");
		}
	}

	public bool IsPaused
	{
		get
		{
			return this.paused;
		}
	}

	public float Capacity
	{
		get
		{
			return this.capacity;
		}
	}

	public float MinimumMass
	{
		get
		{
			return this.minimumMass;
		}
		set
		{
			this.minimumMass = value;
			if (this.deliveryRequests != null && this.deliveryRequests.Count == 1)
			{
				this.deliveryRequests[0].MinimumAmountKG = this.minimumMass;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DebugUtil.Assert(this.choreTypeIDHash.IsValid, "ManualDeliveryKG Must have a valid chore type specified!", base.name);
		if (this.allowPause)
		{
			base.Subscribe<ManualDeliveryKG>(493375141, ManualDeliveryKG.OnRefreshUserMenuDelegate);
			base.Subscribe<ManualDeliveryKG>(-111137758, ManualDeliveryKG.OnRefreshUserMenuDelegate);
		}
		base.Subscribe<ManualDeliveryKG>(-592767678, ManualDeliveryKG.OnOperationalChangedDelegate);
		if (this.storage != null)
		{
			this.SetStorage(this.storage);
		}
		Prioritizable.AddRef(base.gameObject);
		if (this.userPaused && this.allowPause)
		{
			this.OnPause();
		}
	}

	protected override void OnCleanUp()
	{
		this.AbortDelivery("ManualDeliverKG destroyed");
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void SetStorage(Storage storage)
	{
		if (this.storage != null)
		{
			this.storage.Unsubscribe(this.onStorageChangeSubscription);
			this.onStorageChangeSubscription = -1;
		}
		this.AbortDelivery("storage pointer changed");
		this.storage = storage;
		if (this.storage != null && base.isSpawned)
		{
			global::Debug.Assert(this.onStorageChangeSubscription == -1);
			this.onStorageChangeSubscription = this.storage.Subscribe<ManualDeliveryKG>(-1697596308, ManualDeliveryKG.OnStorageChangedDelegate);
		}
	}

	public void Pause(bool pause, string reason)
	{
		if (this.paused != pause)
		{
			this.paused = pause;
			if (pause)
			{
				this.AbortDelivery(reason);
			}
		}
	}

	public void ClearRequests()
	{
		for (int i = this.deliveryRequests.Count - 1; i >= 0; i--)
		{
			this.deliveryRequests[i].Reset();
			ManualDeliveryKG.requestPool.ReleaseInstance(this.deliveryRequests[i]);
			this.deliveryRequests.RemoveAt(i);
		}
	}

	public void RequestItem(Tag id, float minimumAmountKg)
	{
		this.RequestItemInternal(id, minimumAmountKg, null);
	}

	public void RequestItem(Tag[] idSet, float minimumAmountKg)
	{
		this.RequestItemInternal(Tag.Invalid, minimumAmountKg, idSet);
	}

	private void RequestItemInternal(Tag id, float minimumAmountKg, Tag[] idSet = null)
	{
		ManualDeliveryKG.Request instance = ManualDeliveryKG.requestPool.GetInstance();
		instance.Id = id;
		instance.MinimumAmountKG = minimumAmountKg;
		int num = 0;
		while (idSet != null && num < idSet.Length)
		{
			instance.IdSet.Add(idSet[num]);
			num++;
		}
		this.deliveryRequests.Add(instance);
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateDeliveryState();
	}

	[ContextMenu("UpdateDeliveryState")]
	public void UpdateDeliveryState()
	{
		if (this.deliveryRequests == null || this.storage == null)
		{
			return;
		}
		this.UpdateFetchList();
	}

	private void CalculateDeliveryStats(out float storedMass, out float requestKG, out bool requiresRefill)
	{
		requestKG = 0f;
		storedMass = 0f;
		float num = 0f;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < this.deliveryRequests.Count; i++)
		{
			ManualDeliveryKG.Request request = this.deliveryRequests[i];
			if (request.IdSet.Count == 0)
			{
				request.LastStoredAmount = this.storage.GetMassAvailable(request.Id);
			}
			else
			{
				request.LastStoredAmount = 0f;
				foreach (Tag tag in request.IdSet)
				{
					request.LastStoredAmount += this.storage.GetMassAvailable(tag);
				}
			}
			if (request.LastStoredAmount < num2)
			{
				num = request.MinimumAmountKG;
				num2 = request.LastStoredAmount;
			}
			storedMass += request.LastStoredAmount;
			requestKG += request.MinimumAmountKG;
		}
		requiresRefill = storedMass <= this.refillMass;
		if (requestKG <= 0f)
		{
			return;
		}
		requiresRefill |= num2 <= num / requestKG * this.refillMass;
	}

	private void RequestDeliveryInternal(float storedMass, float requestKG)
	{
		if (storedMass >= this.capacity)
		{
			return;
		}
		ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.choreTypeIDHash);
		this.fetchList = new FetchList2(this.storage, byHash);
		for (int i = 0; i < this.deliveryRequests.Count; i++)
		{
			ManualDeliveryKG.Request request = this.deliveryRequests[i];
			float num = this.capacity * (request.MinimumAmountKG / requestKG) - request.LastStoredAmount;
			if (num > Mathf.Epsilon)
			{
				if (this.RoundFetchAmountToInt)
				{
					num = (float)((int)num);
				}
				num = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, num);
				this.fetchList.MinimumAmount[request.Id] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, request.MinimumAmountKG);
				if (request.IdSet.Count == 0)
				{
					FetchList2 fetchList = this.fetchList;
					Tag id = request.Id;
					float num2 = num;
					fetchList.Add(id, this.forbiddenTags, num2, Operational.State.None);
				}
				else
				{
					FetchList2 fetchList2 = this.fetchList;
					HashSet<Tag> idSet = request.IdSet;
					float num2 = num;
					fetchList2.Add(idSet, this.forbiddenTags, num2, Operational.State.None);
				}
			}
		}
		this.fetchList.ShowStatusItem = this.ShowStatusItem;
		this.fetchList.Submit(null, false);
	}

	public void RequestDelivery()
	{
		if (this.fetchList != null)
		{
			return;
		}
		float num;
		float num2;
		bool flag;
		this.CalculateDeliveryStats(out num, out num2, out flag);
		this.RequestDeliveryInternal(num, num2);
	}

	private void UpdateFetchList()
	{
		if (this.paused)
		{
			return;
		}
		if (this.fetchList != null && this.fetchList.IsComplete)
		{
			this.fetchList = null;
		}
		bool flag = this.fetchList != null;
		bool flag2 = this.operational != null && !this.operational.MeetsRequirements(this.operationalRequirement);
		if (flag2 && flag)
		{
			this.fetchList.Cancel("Operational requirements");
			this.fetchList = null;
		}
		if (flag || flag2)
		{
			return;
		}
		float num;
		float num2;
		bool flag3;
		this.CalculateDeliveryStats(out num, out num2, out flag3);
		if (flag3)
		{
			this.RequestDeliveryInternal(num, num2);
		}
	}

	public void AbortDelivery(string reason)
	{
		if (this.fetchList != null)
		{
			FetchList2 fetchList = this.fetchList;
			this.fetchList = null;
			fetchList.Cancel(reason);
		}
	}

	protected void OnStorageChanged(object data)
	{
		this.UpdateDeliveryState();
	}

	private void OnPause()
	{
		this.userPaused = true;
		this.Pause(true, "Forbid manual delivery");
	}

	private void OnResume()
	{
		this.userPaused = false;
		this.Pause(false, "Allow manual delivery");
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.allowPause)
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((!this.paused) ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME, new global::System.Action(this.OnPause), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF, new global::System.Action(this.OnResume), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private void OnOperationalChanged(object data)
	{
		this.UpdateDeliveryState();
	}

	private static ObjectPool<ManualDeliveryKG.Request> requestPool = new ObjectPool<ManualDeliveryKG.Request>(() => new ManualDeliveryKG.Request(), 64);

	public float capacity = 100f;

	public float refillMass = 10f;

	public bool allowPause;

	public bool RoundFetchAmountToInt;

	public HashedString choreTypeIDHash;

	public Operational.State operationalRequirement;

	[SerializeField]
	private float minimumMass = 10f;

	[SerializeField]
	private Storage storage;

	[SerializeField]
	private bool paused;

	[Serialize]
	private bool userPaused;

	[MyCmpGet]
	private Operational operational;

	public bool ShowStatusItem = true;

	[SerializeField]
	private List<ManualDeliveryKG.Request> deliveryRequests = new List<ManualDeliveryKG.Request>();

	private FetchList2 fetchList;

	private Tag[] forbiddenTags;

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnStorageChanged(data);
	});

	[Serializable]
	public class Request
	{
		public void Reset()
		{
			this.Id = Tag.Invalid;
			this.MinimumAmountKG = 0f;
			this.LastStoredAmount = 0f;
			this.IdSet.Clear();
		}

		public Tag Id;

		public HashSet<Tag> IdSet = new HashSet<Tag>();

		public float MinimumAmountKG;

		public float LastStoredAmount;
	}
}
