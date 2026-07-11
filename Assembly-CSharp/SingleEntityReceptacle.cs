using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SingleEntityReceptacle")]
public class SingleEntityReceptacle : Workable, IRender1000ms
{
	public FetchChore GetActiveRequest
	{
		get
		{
			return this.fetchChore;
		}
	}

	protected GameObject occupyingObject
	{
		get
		{
			if (this.occupyObjectRef.Get() != null)
			{
				return this.occupyObjectRef.Get().gameObject;
			}
			return null;
		}
		set
		{
			if (value == null)
			{
				this.occupyObjectRef.Set(null);
				return;
			}
			this.occupyObjectRef.Set(value.GetComponent<KSelectable>());
		}
	}

	public GameObject Occupant
	{
		get
		{
			return this.occupyingObject;
		}
	}

	public Tag[] possibleDepositObjectTags
	{
		get
		{
			return this.possibleDepositTagsList.ToArray();
		}
	}

	public SingleEntityReceptacle.ReceptacleDirection Direction
	{
		get
		{
			return this.direction;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.occupyingObject != null)
		{
			this.PositionOccupyingObject();
			this.SubscribeToOccupant();
		}
		this.UpdateStatusItem();
		if (this.occupyingObject == null && this.requestedEntityTag.IsValid)
		{
			this.CreateOrder(this.requestedEntityTag);
		}
		base.Subscribe<SingleEntityReceptacle>(-592767678, SingleEntityReceptacle.OnOperationalChangedDelegate);
	}

	public void AddDepositTag(Tag t)
	{
		this.possibleDepositTagsList.Add(t);
	}

	public void SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection d)
	{
		this.direction = d;
	}

	public virtual void SetPreview(Tag entityTag, bool solid = false)
	{
	}

	public virtual void CreateOrder(Tag entityTag)
	{
		this.requestedEntityTag = entityTag;
		this.CreateFetchChore(this.requestedEntityTag);
		this.SetPreview(entityTag, true);
		this.UpdateStatusItem();
	}

	public void Render1000ms(float dt)
	{
		this.UpdateStatusItem();
	}

	protected void UpdateStatusItem()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.Occupant != null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, null, null);
			return;
		}
		if (this.fetchChore == null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemNeed, null);
			return;
		}
		bool flag = this.fetchChore.fetcher != null;
		if (!flag)
		{
			foreach (Tag tag in this.fetchChore.tags)
			{
				if (WorldInventory.Instance.GetTotalAmount(tag) > 0f)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemAwaitingDelivery, null);
			return;
		}
		component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, this.statusItemNoneAvailable, null);
	}

	protected void CreateFetchChore(Tag entityTag)
	{
		if (this.fetchChore == null && entityTag.IsValid && entityTag != GameTags.Empty)
		{
			this.fetchChore = new FetchChore(Db.Get().ChoreTypes.FarmFetch, this.storage, 1f, new Tag[] { entityTag }, null, null, null, true, new Action<Chore>(this.OnFetchComplete), delegate(Chore chore)
			{
				this.UpdateStatusItem();
			}, delegate(Chore chore)
			{
				this.UpdateStatusItem();
			}, FetchOrder2.OperationalRequirement.Functional, 0);
			MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, 1f);
			this.UpdateStatusItem();
		}
	}

	public virtual void OrderRemoveOccupant()
	{
		this.ClearOccupant();
	}

	protected virtual void ClearOccupant()
	{
		if (this.occupyingObject)
		{
			this.storage.DropAll(false, false, default(Vector3), true);
		}
		this.occupyingObject = null;
		this.UpdateActive();
		this.UpdateStatusItem();
		base.Trigger(-731304873, this.occupyingObject);
	}

	public void CancelActiveRequest()
	{
		if (this.fetchChore != null)
		{
			MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, -1f);
			this.fetchChore.Cancel("User canceled");
			this.fetchChore = null;
		}
		this.requestedEntityTag = Tag.Invalid;
		this.UpdateStatusItem();
		this.SetPreview(Tag.Invalid, false);
	}

	private void OnOccupantDestroyed(object data)
	{
		this.occupyingObject = null;
		this.ClearOccupant();
		if (this.autoReplaceEntity && this.requestedEntityTag.IsValid && this.requestedEntityTag != GameTags.Empty)
		{
			this.CreateOrder(this.requestedEntityTag);
		}
	}

	protected virtual void SubscribeToOccupant()
	{
		if (this.occupyingObject != null)
		{
			base.Subscribe(this.occupyingObject, 1969584890, new Action<object>(this.OnOccupantDestroyed));
		}
	}

	protected virtual void UnsubscribeFromOccupant()
	{
		if (this.occupyingObject != null)
		{
			base.Unsubscribe(this.occupyingObject, 1969584890, new Action<object>(this.OnOccupantDestroyed));
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		if (this.fetchChore == null)
		{
			global::Debug.LogWarningFormat(base.gameObject, "{0} OnFetchComplete fetchChore null", new object[] { base.gameObject });
			return;
		}
		if (this.fetchChore.fetchTarget == null)
		{
			global::Debug.LogWarningFormat(base.gameObject, "{0} OnFetchComplete fetchChore.fetchTarget null", new object[] { base.gameObject });
			return;
		}
		this.OnDepositObject(this.fetchChore.fetchTarget.GetComponent<Pickupable>());
	}

	public void ForceDepositPickupable(Pickupable pickupable)
	{
		this.OnDepositObject(pickupable);
	}

	private void OnDepositObject(Pickupable pickupable)
	{
		this.SetPreview(Tag.Invalid, false);
		MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, -1f);
		KBatchedAnimController component = pickupable.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.GetBatchInstanceData().ClearOverrideTransformMatrix();
		}
		this.occupyingObject = this.SpawnOccupyingObject(pickupable.gameObject);
		if (this.occupyingObject != null)
		{
			this.occupyingObject.SetActive(true);
			this.PositionOccupyingObject();
			this.SubscribeToOccupant();
		}
		else
		{
			global::Debug.LogWarning(base.gameObject.name + " EntityReceptacle did not spawn occupying entity.");
		}
		if (this.fetchChore != null)
		{
			this.fetchChore.Cancel("receptacle filled");
			this.fetchChore = null;
		}
		if (!this.autoReplaceEntity)
		{
			this.requestedEntityTag = Tag.Invalid;
		}
		this.UpdateActive();
		this.UpdateStatusItem();
		if (this.destroyEntityOnDeposit)
		{
			Util.KDestroyGameObject(pickupable.gameObject);
		}
		base.Trigger(-731304873, this.occupyingObject);
	}

	public virtual GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		return depositedEntity;
	}

	protected virtual void PositionOccupyingObject()
	{
		if (this.rotatable != null)
		{
			this.occupyingObject.transform.SetPosition(base.gameObject.transform.GetPosition() + this.rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition));
		}
		else
		{
			this.occupyingObject.transform.SetPosition(base.gameObject.transform.GetPosition() + this.occupyingObjectRelativePosition);
		}
		KBatchedAnimController component = this.occupyingObject.GetComponent<KBatchedAnimController>();
		component.enabled = false;
		component.enabled = true;
	}

	private void UpdateActive()
	{
		if (this.Equals(null) || this == null || base.gameObject.Equals(null) || base.gameObject == null)
		{
			return;
		}
		if (this.operational != null)
		{
			this.operational.SetActive(this.operational.IsOperational && this.occupyingObject != null, false);
		}
	}

	protected override void OnCleanUp()
	{
		this.CancelActiveRequest();
		this.UnsubscribeFromOccupant();
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		this.UpdateActive();
		if (this.occupyingObject)
		{
			this.occupyingObject.Trigger(this.operational.IsOperational ? 1628751838 : 960378201, null);
		}
	}

	[MyCmpGet]
	protected Operational operational;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpGet]
	public Rotatable rotatable;

	protected FetchChore fetchChore;

	[Serialize]
	public bool autoReplaceEntity;

	[Serialize]
	public Tag requestedEntityTag;

	[Serialize]
	protected Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();

	[SerializeField]
	private List<Tag> possibleDepositTagsList = new List<Tag>();

	[SerializeField]
	protected bool destroyEntityOnDeposit;

	[SerializeField]
	protected SingleEntityReceptacle.ReceptacleDirection direction;

	public Vector3 occupyingObjectRelativePosition = new Vector3(0f, 1f, 3f);

	protected StatusItem statusItemAwaitingDelivery;

	protected StatusItem statusItemNeed;

	protected StatusItem statusItemNoneAvailable;

	private static readonly EventSystem.IntraObjectHandler<SingleEntityReceptacle> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SingleEntityReceptacle>(delegate(SingleEntityReceptacle component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public enum ReceptacleDirection
	{
		Top,
		Side,
		Bottom
	}
}
