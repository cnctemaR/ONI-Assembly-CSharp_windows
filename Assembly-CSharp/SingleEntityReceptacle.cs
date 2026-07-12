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

	public IReadOnlyList<Tag> possibleDepositObjectTags
	{
		get
		{
			return this.possibleDepositTagsList;
		}
	}

	public bool HasDepositTag(Tag tag)
	{
		return this.possibleDepositTagsList.Contains(tag);
	}

	public bool IsValidEntity(GameObject candidate)
	{
		IReceptacleDirection component = candidate.GetComponent<IReceptacleDirection>();
		bool flag = this.rotatable != null || component == null || component.Direction == this.Direction;
		int num = 0;
		while (flag && num < this.additionalCriteria.Count)
		{
			flag = this.additionalCriteria[num](candidate);
			num++;
		}
		return flag;
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
		if (this.occupyingObject == null && !this.requestedEntityTag.IsValid)
		{
			this.requestedEntityAdditionalFilterTag = null;
		}
		if (this.occupyingObject == null && this.requestedEntityTag.IsValid)
		{
			this.CreateOrder(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
		}
		base.Subscribe<SingleEntityReceptacle>(-592767678, SingleEntityReceptacle.OnOperationalChangedDelegate);
	}

	public void AddDepositTag(Tag t)
	{
		this.possibleDepositTagsList.Add(t);
	}

	public void AddAdditionalCriteria(Func<GameObject, bool> criteria)
	{
		this.additionalCriteria.Add(criteria);
	}

	public void SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection d)
	{
		this.direction = d;
	}

	public virtual void SetPreview(Tag entityTag, bool solid = false)
	{
	}

	public virtual void CreateOrder(Tag entityTag, Tag additionalFilterTag)
	{
		this.requestedEntityTag = entityTag;
		this.requestedEntityAdditionalFilterTag = additionalFilterTag;
		this.CreateFetchChore(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
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
		WorldContainer myWorld = this.GetMyWorld();
		if (!flag && myWorld != null)
		{
			foreach (Tag tag in this.fetchChore.tags)
			{
				if (myWorld.worldInventory.GetTotalAmount(tag, true) > 0f)
				{
					if (myWorld.worldInventory.GetTotalAmount(this.requestedEntityAdditionalFilterTag, true) > 0f || this.requestedEntityAdditionalFilterTag == Tag.Invalid)
					{
						flag = true;
						break;
					}
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

	protected void CreateFetchChore(Tag entityTag, Tag additionalRequiredTag)
	{
		if (this.fetchChore == null && entityTag.IsValid && entityTag != GameTags.Empty)
		{
			this.fetchChore = new FetchChore(this.choreType, this.storage, 1f, new HashSet<Tag> { entityTag }, FetchChore.MatchCriteria.MatchID, (additionalRequiredTag.IsValid && additionalRequiredTag != GameTags.Empty) ? additionalRequiredTag : Tag.Invalid, null, null, true, new Action<Chore>(this.OnFetchComplete), delegate(Chore chore)
			{
				this.UpdateStatusItem();
			}, delegate(Chore chore)
			{
				this.UpdateStatusItem();
			}, Operational.State.Functional, 0);
			MaterialNeeds.UpdateNeed(this.requestedEntityTag, 1f, base.gameObject.GetMyWorldId());
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
			this.UnsubscribeFromOccupant();
			this.storage.DropAll(false, false, default(Vector3), true, null);
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
			MaterialNeeds.UpdateNeed(this.requestedEntityTag, -1f, base.gameObject.GetMyWorldId());
			this.fetchChore.Cancel("User canceled");
			this.fetchChore = null;
		}
		this.requestedEntityTag = Tag.Invalid;
		this.requestedEntityAdditionalFilterTag = Tag.Invalid;
		this.UpdateStatusItem();
		this.SetPreview(Tag.Invalid, false);
	}

	private void OnOccupantDestroyed(object data)
	{
		this.occupyingObject = null;
		this.ClearOccupant();
		if (this.autoReplaceEntity && this.requestedEntityTag.IsValid && this.requestedEntityTag != GameTags.Empty)
		{
			this.CreateOrder(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag);
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
		this.OnDepositObject(this.fetchChore.fetchTarget.gameObject);
	}

	public void ForceDeposit(GameObject depositedObject)
	{
		if (this.occupyingObject != null)
		{
			this.ClearOccupant();
		}
		this.OnDepositObject(depositedObject);
	}

	private void OnDepositObject(GameObject depositedObject)
	{
		this.SetPreview(Tag.Invalid, false);
		MaterialNeeds.UpdateNeed(this.requestedEntityTag, -1f, base.gameObject.GetMyWorldId());
		KBatchedAnimController component = depositedObject.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.GetBatchInstanceData().ClearOverrideTransformMatrix();
		}
		this.occupyingObject = this.SpawnOccupyingObject(depositedObject);
		if (this.occupyingObject != null)
		{
			this.ConfigureOccupyingObject(this.occupyingObject);
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
			this.requestedEntityAdditionalFilterTag = Tag.Invalid;
		}
		this.UpdateActive();
		this.UpdateStatusItem();
		if (this.destroyEntityOnDeposit)
		{
			Util.KDestroyGameObject(depositedObject);
		}
		base.Trigger(-731304873, this.occupyingObject);
	}

	protected virtual GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		return depositedEntity;
	}

	protected virtual void ConfigureOccupyingObject(GameObject source)
	{
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

	public ChoreType choreType = Db.Get().ChoreTypes.Fetch;

	[Serialize]
	public bool autoReplaceEntity;

	[Serialize]
	public Tag requestedEntityTag;

	[Serialize]
	public Tag requestedEntityAdditionalFilterTag;

	[Serialize]
	protected Ref<KSelectable> occupyObjectRef = new Ref<KSelectable>();

	[SerializeField]
	private List<Tag> possibleDepositTagsList = new List<Tag>();

	[SerializeField]
	private List<Func<GameObject, bool>> additionalCriteria = new List<Func<GameObject, bool>>();

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
