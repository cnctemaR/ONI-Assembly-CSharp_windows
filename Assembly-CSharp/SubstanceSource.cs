using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class SubstanceSource : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.pickupable.SetWorkTime(SubstanceSource.MaxPickupTime);
		this.pickupable.preferPrimaryCell = false;
	}

	protected override void OnSpawn()
	{
		this.pickupable.SetWorkTime(10f);
	}

	protected abstract CellOffset[] GetOffsetGroup();

	protected abstract IChunkManager GetChunkManager();

	public SimHashes GetElementID()
	{
		return this.primaryElement.ElementID;
	}

	public Tag GetElementTag()
	{
		Tag tag = Tag.Invalid;
		if (base.gameObject != null)
		{
			if (this.primaryElement != null && this.primaryElement.Element != null)
			{
				tag = this.primaryElement.Element.tag;
			}
		}
		return tag;
	}

	public Tag GetMaterialCategoryTag()
	{
		Tag tag = Tag.Invalid;
		if (base.gameObject != null)
		{
			if (this.primaryElement != null && this.primaryElement.Element != null)
			{
				tag = this.primaryElement.Element.GetMaterialCategoryTag();
			}
		}
		return tag;
	}

	private bool enableRefresh;

	private static readonly float MaxPickupTime = 8f;

	[MyCmpReq]
	public Pickupable pickupable;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public int consumeCell;
}
