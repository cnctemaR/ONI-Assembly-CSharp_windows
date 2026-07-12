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

	private void InitializeAnimator()
	{
		this.TrySpawnDisplayMinion();
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		Accessorizer component = this.animController.GetComponent<Accessorizer>();
		for (int i = component.GetAccessories().Count - 1; i >= 0; i--)
		{
			component.RemoveAccessory(component.GetAccessories()[i].Get());
		}
	}

	public void SetDefaultPortraitAnimator()
	{
		MinionIdentity minionIdentity = ((Components.MinionIdentities.Count > 0) ? Components.MinionIdentities[0] : null);
		if (minionIdentity == null)
		{
			return;
		}
		this.InitializeAnimator();
		this.animController.GetComponent<Accessorizer>().ApplyMinionPersonality(Db.Get().Personalities.Get(minionIdentity.personalityResourceId));
		Accessorizer component = minionIdentity.GetComponent<Accessorizer>();
		KAnim.Build.Symbol symbol = null;
		KAnim.Build.Symbol symbol2 = null;
		if (component)
		{
			symbol = component.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		this.UpdateHatOverride(null, symbol, symbol2);
		this.UpdateClothingOverride(this.animController.GetComponent<SymbolOverrideController>(), minionIdentity, null);
	}

	public void SetPortraitAnimator(IAssignableIdentity assignableIdentity)
	{
		if (assignableIdentity == null || assignableIdentity.IsNull())
		{
			this.SetDefaultPortraitAnimator();
			return;
		}
		this.InitializeAnimator();
		string text = "";
		MinionIdentity minionIdentity;
		StoredMinionIdentity storedMinionIdentity;
		this.GetMinionIdentity(assignableIdentity, out minionIdentity, out storedMinionIdentity);
		Accessorizer accessorizer = null;
		Accessorizer component = this.animController.GetComponent<Accessorizer>();
		KAnim.Build.Symbol symbol = null;
		KAnim.Build.Symbol symbol2 = null;
		if (minionIdentity != null)
		{
			accessorizer = minionIdentity.GetComponent<Accessorizer>();
			foreach (ResourceRef<Accessory> resourceRef in accessorizer.GetAccessories())
			{
				component.AddAccessory(resourceRef.Get());
			}
			text = minionIdentity.GetComponent<MinionResume>().CurrentHat;
			symbol = accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		else if (storedMinionIdentity != null)
		{
			foreach (ResourceRef<Accessory> resourceRef2 in storedMinionIdentity.accessories)
			{
				component.AddAccessory(resourceRef2.Get());
			}
			text = storedMinionIdentity.currentHat;
			symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		this.UpdateHatOverride(text, symbol, symbol2);
		this.UpdateClothingOverride(this.animController.GetComponent<SymbolOverrideController>(), minionIdentity, storedMinionIdentity);
	}

	private void UpdateHatOverride(string current_hat, KAnim.Build.Symbol hair_symbol, KAnim.Build.Symbol hat_hair_symbol)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		this.animController.SetSymbolVisiblity(hat.targetSymbolId, !string.IsNullOrEmpty(current_hat));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(current_hat));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, !string.IsNullOrEmpty(current_hat));
		SymbolOverrideController component = this.animController.GetComponent<SymbolOverrideController>();
		component.AddSymbolOverride("snapto_hair_always", hair_symbol, 1);
		component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, hat_hair_symbol, 1);
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
		AssignableSlotInstance assignableSlotInstance = equipment.GetSlot(Db.Get().AssignableSlots.Outfit);
		if (assignableSlotInstance.assignable != null)
		{
			Equippable component = assignableSlotInstance.assignable.GetComponent<Equippable>();
			if (component != null)
			{
				this.UpdateClothingOverride(component.GetBuildOverride().GetData(), component.def.BuildOverridePriority);
				return;
			}
		}
		else
		{
			assignableSlotInstance = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
			if (assignableSlotInstance.assignable != null)
			{
				this.animController.SetSymbolVisiblity("belt", true);
			}
		}
	}

	public void UpdateClothingOverride(KAnimFileData clothingData, int priority = 4)
	{
		SymbolOverrideController component = this.animController.GetComponent<SymbolOverrideController>();
		if (this.buildOverrideData != null)
		{
			component.RemoveBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
			this.buildOverrideData = null;
		}
		this.buildOverrideData = new global::Tuple<KAnimFileData, int>(clothingData, priority);
		component.AddBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
		bool flag = this.buildOverrideData.first.build.GetSymbol("belt") != null;
		this.animController.SetSymbolVisiblity("belt", flag);
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
