using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ItemPedestal")]
public class ItemPedestal : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<ItemPedestal>(-731304873, ItemPedestal.OnOccupantChangedDelegate);
		if (this.receptacle.Occupant)
		{
			KBatchedAnimController component = this.receptacle.Occupant.GetComponent<KBatchedAnimController>();
			if (component)
			{
				component.enabled = true;
				component.sceneLayer = Grid.SceneLayer.Move;
			}
			this.OnOccupantChanged(this.receptacle.Occupant);
		}
	}

	private void OnOccupantChanged(object data)
	{
		Attributes attributes = this.GetAttributes();
		if (this.decorModifier != null)
		{
			attributes.Remove(this.decorModifier);
			attributes.Remove(this.decorRadiusModifier);
			this.decorModifier = null;
			this.decorRadiusModifier = null;
		}
		if (data != null)
		{
			GameObject gameObject = (GameObject)data;
			global::UnityEngine.Object component = gameObject.GetComponent<DecorProvider>();
			float num = 5f;
			float num2 = 3f;
			if (component != null)
			{
				num = Mathf.Max(Db.Get().BuildingAttributes.Decor.Lookup(gameObject).GetTotalValue() * 2f, 5f);
				num2 = Db.Get().BuildingAttributes.DecorRadius.Lookup(gameObject).GetTotalValue() + 2f;
			}
			string text = string.Format(BUILDINGS.PREFABS.ITEMPEDESTAL.DISPLAYED_ITEM_FMT, gameObject.GetComponent<KPrefabID>().PrefabTag.ProperName());
			this.decorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, num, text, false, false, true);
			this.decorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, num2, text, false, false, true);
			attributes.Add(this.decorModifier);
			attributes.Add(this.decorRadiusModifier);
		}
	}

	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[MyCmpReq]
	private DecorProvider decorProvider;

	private const float MINIMUM_DECOR = 5f;

	private const float STORED_DECOR_MODIFIER = 2f;

	private const int RADIUS_BONUS = 2;

	private AttributeModifier decorModifier;

	private AttributeModifier decorRadiusModifier;

	private static readonly EventSystem.IntraObjectHandler<ItemPedestal> OnOccupantChangedDelegate = new EventSystem.IntraObjectHandler<ItemPedestal>(delegate(ItemPedestal component, object data)
	{
		component.OnOccupantChanged(data);
	});
}
