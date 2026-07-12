using System;
using UnityEngine;

public class FullBodyUIMinionWidget : KMonoBehaviour
{
	public KBatchedAnimController animController { get; private set; }

	protected override void OnSpawn()
	{
		this.TrySpawnDisplayMinion();
	}

	private void TrySpawnDisplayMinion()
	{
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("FullMinionUIPortrait")), this.duplicantAnimAnchor.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.gameObject.SetActive(true);
			this.animController.animScale = 0.38f;
		}
	}

	public void SetPortraitAnimator(IAssignableIdentity assignableIdentity)
	{
		if (assignableIdentity == null || assignableIdentity.IsNull())
		{
			if (Components.MinionIdentities.Count <= 0)
			{
				return;
			}
			assignableIdentity = Components.MinionIdentities[0];
			if (assignableIdentity == null || assignableIdentity.IsNull())
			{
				return;
			}
		}
		this.TrySpawnDisplayMinion();
		string text = "";
		Accessorizer component = this.animController.GetComponent<Accessorizer>();
		for (int i = component.GetAccessories().Count - 1; i >= 0; i--)
		{
			component.RemoveAccessory(component.GetAccessories()[i].Get());
		}
		MinionIdentity minionIdentity;
		StoredMinionIdentity storedMinionIdentity;
		this.GetMinionIdentity(assignableIdentity, out minionIdentity, out storedMinionIdentity);
		Accessorizer accessorizer = null;
		if (minionIdentity != null)
		{
			accessorizer = minionIdentity.GetComponent<Accessorizer>();
			foreach (ResourceRef<Accessory> resourceRef in accessorizer.GetAccessories())
			{
				component.AddAccessory(resourceRef.Get());
			}
			text = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (storedMinionIdentity != null)
		{
			foreach (ResourceRef<Accessory> resourceRef2 in storedMinionIdentity.accessories)
			{
				component.AddAccessory(resourceRef2.Get());
			}
			text = storedMinionIdentity.currentHat;
		}
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		this.animController.SetSymbolVisiblity(hat.targetSymbolId, !string.IsNullOrEmpty(text));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(text));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, !string.IsNullOrEmpty(text));
		KAnim.Build.Symbol symbol = null;
		KAnim.Build.Symbol symbol2 = null;
		if (accessorizer)
		{
			symbol = accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		else if (storedMinionIdentity != null)
		{
			symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		SymbolOverrideController component2 = this.animController.GetComponent<SymbolOverrideController>();
		component2.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, symbol, 1);
		component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, symbol2, 1);
		this.UpdateClothingOverride(component2, minionIdentity, storedMinionIdentity);
	}

	private void UpdateClothingOverride(SymbolOverrideController symbolOverrideController, MinionIdentity identity, StoredMinionIdentity storedMinionIdentity)
	{
		Equipment equipment = null;
		if (identity != null)
		{
			equipment = identity.assignableProxy.Get().GetComponent<Equipment>();
		}
		else if (storedMinionIdentity != null)
		{
			equipment = storedMinionIdentity.assignableProxy.Get().GetComponent<Equipment>();
		}
		if (this.buildOverrideData != null)
		{
			symbolOverrideController.RemoveBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
			this.buildOverrideData = null;
		}
		AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Outfit);
		if (slot.assignable != null)
		{
			Equippable component = slot.assignable.GetComponent<Equippable>();
			if (component != null)
			{
				this.buildOverrideData = new global::Tuple<KAnimFileData, int>(component.GetBuildOverride().GetData(), component.def.BuildOverridePriority);
				symbolOverrideController.AddBuildOverride(this.buildOverrideData.first, component.def.BuildOverridePriority);
			}
		}
	}

	public void UpdateClothingOverride(KAnimFileData clothingData)
	{
		SymbolOverrideController component = this.animController.GetComponent<SymbolOverrideController>();
		if (this.buildOverrideData != null)
		{
			component.RemoveBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
			this.buildOverrideData = null;
		}
		this.buildOverrideData = new global::Tuple<KAnimFileData, int>(clothingData, 4);
		component.AddBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
	}

	private void GetMinionIdentity(IAssignableIdentity assignableIdentity, out MinionIdentity minionIdentity, out StoredMinionIdentity storedMinionIdentity)
	{
		if (assignableIdentity is MinionAssignablesProxy)
		{
			minionIdentity = ((MinionAssignablesProxy)assignableIdentity).GetTargetGameObject().GetComponent<MinionIdentity>();
			storedMinionIdentity = ((MinionAssignablesProxy)assignableIdentity).GetTargetGameObject().GetComponent<StoredMinionIdentity>();
			return;
		}
		minionIdentity = assignableIdentity as MinionIdentity;
		storedMinionIdentity = assignableIdentity as StoredMinionIdentity;
	}

	[SerializeField]
	private GameObject duplicantAnimAnchor;

	public const float UI_MINION_PORTRAIT_ANIM_SCALE = 0.38f;

	private global::Tuple<KAnimFileData, int> buildOverrideData;
}
