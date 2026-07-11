using System;
using Klei.AI;
using UnityEngine;

public class AtmoSuit : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<AtmoSuit>(-1697596308, AtmoSuit.OnStorageChangedDelegate);
	}

	private void RefreshStatusEffects(object data)
	{
		Equippable component = base.GetComponent<Equippable>();
		bool flag = base.GetComponent<Storage>().Has(GameTags.AnyWater);
		if (component.assignee != null && flag)
		{
			Ownables soleOwner = component.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if (targetGameObject)
				{
					Effects component2 = targetGameObject.GetComponent<Effects>();
					if (!component2.HasEffect("SoiledSuit"))
					{
						component2.Add("SoiledSuit", true);
					}
				}
			}
		}
	}

	private static readonly EventSystem.IntraObjectHandler<AtmoSuit> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<AtmoSuit>(delegate(AtmoSuit component, object data)
	{
		component.RefreshStatusEffects(data);
	});
}
