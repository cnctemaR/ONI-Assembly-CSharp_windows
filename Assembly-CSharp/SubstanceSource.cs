using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class SubstanceSource : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Pickupable pickupable = this.pickupable;
		pickupable.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable.OnTake, new Func<float, Pickupable>(this.OnTake));
		this.pickupable.SetWorkTime(SubstanceSource.MaxPickupTime);
		this.pickupable.preferPrimaryCell = false;
	}

	protected override void OnSpawn()
	{
		this.pickupable.SetWorkTime(SubstanceSource.MaxPickupTime);
	}

	protected abstract CellOffset[] GetOffsetGroup();

	protected abstract IChunkManager GetChunkManager();

	public void Refresh()
	{
		int num = Grid.PosToCell(this);
		SimHashes elementID = this.primaryElement.ElementID;
		float num2 = 0f;
		CellOffset[] offsetGroup = this.GetOffsetGroup();
		foreach (CellOffset cellOffset in offsetGroup)
		{
			int num3 = Grid.OffsetCell(num, cellOffset);
			if (Grid.Element[num3].id == elementID)
			{
				num2 += Grid.Cell[num3].mass;
				float num4 = Grid.Temperature[num3];
				if (num4 > 0f)
				{
					this.primaryElement.Temperature = num4;
				}
				else
				{
					global::Debug.LogWarning("Attempting to set substance source temperature to: " + num4.ToString(), null);
				}
			}
		}
		this.pickupable.TotalAmount = Mathf.Max(num2, 1f);
		float num5 = SubstanceSource.MaxPickupTime - this.pickupable.WorkTimeRemaining;
		float num6 = Mathf.Lerp(1f, SubstanceSource.MaxPickupTime, this.pickupable.TotalAmount / 200f);
		if (num5 > num6)
		{
			this.pickupable.WorkTimeRemaining = 0f;
		}
	}

	private void Update()
	{
		if (Game.IsQuitting() || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		this.Refresh();
	}

	private Pickupable OnTake(float amount)
	{
		int num = Grid.PosToCell(this);
		Element element = this.primaryElement.Element;
		IChunkManager chunkManager = this.GetChunkManager();
		Pickupable component = chunkManager.CreateChunk(element, amount, this.primaryElement.Temperature, this.transform.position).GetComponent<Pickupable>();
		SimMessages.ConsumeMass(num, element.id, amount, 3, this.cbHandle.index);
		component.TotalAmount = amount;
		component.Trigger(1335436905, this.pickupable);
		this.Refresh();
		return component;
	}

	public SimHashes GetElementID()
	{
		return this.primaryElement.ElementID;
	}

	public Tag GetElementTag()
	{
		Tag tag = Tag.Invalid;
		if (base.gameObject != null && this.primaryElement != null && this.primaryElement.Element != null)
		{
			tag = this.primaryElement.Element.tag;
		}
		return tag;
	}

	public Tag GetMaterialCategoryTag()
	{
		Tag tag = Tag.Invalid;
		if (base.gameObject != null && this.primaryElement != null && this.primaryElement.Element != null)
		{
			tag = this.primaryElement.Element.GetMaterialCategoryTag();
		}
		return tag;
	}

	private bool enableRefresh;

	private static readonly float MaxPickupTime = 10f;

	[MyCmpReq]
	protected Pickupable pickupable;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	private HandleVector<Action<object>>.Handle cbHandle = HandleVector<Action<object>>.InvalidHandle;
}
