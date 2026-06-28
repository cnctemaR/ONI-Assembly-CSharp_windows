using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class SingleEntityReceptacle : KMonoBehaviour
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
			}
			else
			{
				this.occupyObjectRef.Set(value.GetComponent<KSelectable>());
			}
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SubscribeToOccupant();
		this.UpdateStatusItem(this);
		if (this.occupyingObject == null && this.requestedEntityTag.IsValid)
		{
			this.CreateOrder(this.requestedEntityTag);
		}
	}

	public void AddDespoitTag(Tag t)
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

	public void CreateOrder(Tag entityTag)
	{
		this.requestedEntityTag = entityTag;
		this.CreateFetchChore(this.requestedEntityTag);
		this.SetPreview(entityTag, true);
		this.UpdateStatusItem(this);
	}

	protected void UpdateStatusItem(object data)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.Occupant != null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, null, null);
			return;
		}
		if (this.fetchChore != null)
		{
			bool flag = false;
			foreach (Tag tag in this.fetchChore.tags)
			{
				if (WorldInventory.Instance.GetTotalAmount(tag) > 0f)
				{
					component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, Db.Get().BuildingStatusItems.AwaitingSeedDelivery, null);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, Db.Get().BuildingStatusItems.NoAvailableSeed, null);
			}
		}
		else
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.EntityReceptacle, Db.Get().BuildingStatusItems.NeedSeed, null);
		}
	}

	protected void CreateFetchChore(Tag entityTag)
	{
		if (this.fetchChore == null && entityTag.IsValid)
		{
			Action<Chore> action = new Action<Chore>(this.OnFetchComplete);
			Action<Chore> action2 = delegate(Chore chore)
			{
				this.UpdateStatusItem(this);
			};
			this.fetchChore = new FetchChore(this.storage, 1f, new Tag[] { entityTag }, null, true, action, action2, delegate(Chore chore)
			{
				this.UpdateStatusItem(this);
			}, FetchOrder2.OperationalRequirement.Functional, 0);
			MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, 1f);
			this.updateStatusItemsHandle = UIScheduler.Instance.SchedulePeriodic("SingleEntityReceptacle.StatusUpdate", 1f, new Action<object>(this.UpdateStatusItem), this, null);
			this.UpdateStatusItem(this);
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
			Util.KDestroyGameObject(this.occupyingObject);
		}
		this.occupyingObject = null;
		this.SetOperation();
		this.UpdateStatusItem(this);
		this.Trigger(-731304873, this.occupyingObject);
	}

	public void CancelActiveRequest()
	{
		if (this.fetchChore != null)
		{
			this.updateStatusItemsHandle.Clear();
			MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, -1f);
			this.fetchChore.Cancel("User canceled");
			this.fetchChore = null;
		}
		this.requestedEntityTag = Tag.Invalid;
		this.UpdateStatusItem(this);
		this.SetPreview(Tag.Invalid, false);
	}

	private void ClearOccupantEventHandler(object data)
	{
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
			base.Subscribe(this.occupyingObject, 1969584890, new Action<object>(this.ClearOccupantEventHandler));
		}
	}

	protected virtual void UnsubscribeFromOccupant()
	{
		if (this.occupyingObject != null)
		{
			base.Unsubscribe(this.occupyingObject, 1969584890, new Action<object>(this.ClearOccupantEventHandler));
		}
	}

	private void OnFetchComplete(Chore chore)
	{
		this.SetPreview(Tag.Invalid, false);
		Pickupable fetchTarget = this.fetchChore.fetchTarget;
		MaterialNeeds.Instance.UpdateNeed(this.requestedEntityTag, -1f);
		this.occupyingObject = this.SpawnOccupyingObject(fetchTarget.gameObject);
		if (this.occupyingObject != null)
		{
			this.occupyingObject.SetActive(true);
			this.PositionOccupyingObject();
			this.SubscribeToOccupant();
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + " EntityReceptacle did not spawn occupying entity.");
		}
		this.fetchChore = null;
		if (!this.autoReplaceEntity)
		{
			this.requestedEntityTag = Tag.Invalid;
		}
		this.updateStatusItemsHandle.Clear();
		this.SetOperation();
		this.UpdateStatusItem(this);
		this.Subscribe(-592767678, delegate
		{
			this.SetOperation();
		});
		if (this.destroyEntityOnDeposit)
		{
			Util.KDestroyGameObject(fetchTarget.gameObject);
		}
		this.Trigger(-731304873, this.occupyingObject);
	}

	public virtual GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		return depositedEntity;
	}

	protected void PositionOccupyingObject()
	{
		this.occupyingObject.transform.position = Vector3.zero;
		this.occupyingObject.transform.SetParent(base.gameObject.transform, false);
		this.occupyingObject.transform.localPosition = Vector3.zero;
		if (this.rotatable != null)
		{
			this.occupyingObject.transform.localPosition = this.rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition);
		}
		else
		{
			this.occupyingObject.transform.localPosition = this.occupyingObjectRelativePosition;
		}
	}

	private void SetOperation()
	{
		if (this.Equals(null) || this == null || base.gameObject.Equals(null) || base.gameObject == null)
		{
			return;
		}
		Operational component = base.GetComponent<Operational>();
		if (component.IsOperational && this.occupyingObject != null)
		{
			component.SetActive(true, false);
		}
		else
		{
			component.SetActive(false, false);
		}
	}

	protected override void OnCleanUp()
	{
		this.CancelActiveRequest();
		this.UnsubscribeFromOccupant();
		this.updateStatusItemsHandle.Clear();
		base.OnCleanUp();
	}

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	protected Storage storage;

	[MyCmpGet]
	public Rotatable rotatable;

	protected FetchChore fetchChore;

	protected bool autoReplaceEntity;

	[Serialize]
	public Tag requestedEntityTag;

	[Serialize]
	private Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();

	private SchedulerHandle updateStatusItemsHandle;

	[SerializeField]
	private List<Tag> possibleDepositTagsList = new List<Tag>();

	[SerializeField]
	protected bool destroyEntityOnDeposit;

	[SerializeField]
	protected SingleEntityReceptacle.ReceptacleDirection direction;

	public Vector3 occupyingObjectRelativePosition = new Vector3(0f, 1f, 3f);

	public enum ReceptacleDirection
	{
		Top,
		Side,
		Bottom
	}
}
