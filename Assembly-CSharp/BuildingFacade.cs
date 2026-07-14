using System;
using System.Collections.Generic;
using Database;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingFacade : KMonoBehaviour
{
	public string CurrentFacade
	{
		get
		{
			return this.currentFacade;
		}
	}

	public bool IsOriginal
	{
		get
		{
			return this.currentFacade.IsNullOrWhiteSpace();
		}
	}

	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
		if (!this.IsOriginal)
		{
			this.ApplyBuildingFacade(Db.GetBuildingFacades().TryGet(this.currentFacade), false);
		}
	}

	public void ApplyDefaultFacade(bool shouldTryAnimate = false)
	{
		this.currentFacade = "DEFAULT_FACADE";
		this.ClearFacade(shouldTryAnimate);
	}

	public void ApplyBuildingFacade(BuildingFacadeResource facade, bool shouldTryAnimate = false)
	{
		if (facade == null)
		{
			this.ClearFacade(false);
			return;
		}
		this.currentFacade = facade.Id;
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim(facade.AnimFile) };
		this.ChangeBuilding(array, facade.Name, facade.Description, facade.InteractFile, shouldTryAnimate, facade.Data);
	}

	private void ClearFacade(bool shouldTryAnimate = false)
	{
		Building component = base.GetComponent<Building>();
		this.ChangeBuilding(component.Def.AnimFiles, component.Def.Name, component.Def.Desc, null, shouldTryAnimate, null);
	}

	private void ChangeBuilding(KAnimFile[] animFiles, string displayName, string desc, Dictionary<string, string> interactAnimsNames = null, bool shouldTryAnimate = false, Dictionary<string, string> data = null)
	{
		this.interactAnims.Clear();
		if (interactAnimsNames != null && interactAnimsNames.Count > 0)
		{
			this.interactAnims = new Dictionary<string, KAnimFile[]>();
			foreach (KeyValuePair<string, string> keyValuePair in interactAnimsNames)
			{
				this.interactAnims.Add(keyValuePair.Key, new KAnimFile[] { Assets.GetAnim(keyValuePair.Value) });
			}
		}
		Dictionary<string, string> dictionary = ((data == null) ? null : new Dictionary<string, string>(data));
		Building[] components = base.GetComponents<Building>();
		foreach (Building building in components)
		{
			KBatchedAnimController component = building.GetComponent<KBatchedAnimController>();
			HashedString batchGroupID = component.batchGroupID;
			component.SwapAnims(animFiles);
			foreach (KBatchedAnimController kbatchedAnimController in building.GetComponentsInChildren<KBatchedAnimController>(true))
			{
				if (kbatchedAnimController.batchGroupID == batchGroupID)
				{
					kbatchedAnimController.SwapAnims(animFiles);
				}
			}
			if (!this.animateIn.IsNullOrDestroyed())
			{
				global::UnityEngine.Object.Destroy(this.animateIn);
				this.animateIn = null;
			}
			if (shouldTryAnimate)
			{
				this.animateIn = BuildingFacadeAnimateIn.MakeFor(component);
				string text = "Unlocked";
				float num = 1f;
				KFMOD.PlayUISoundWithParameter(GlobalAssets.GetSound(KleiInventoryScreen.GetFacadeItemSoundName(Db.Get().Permits.TryGet(this.currentFacade)) + "_Click", false), text, num);
			}
			BuildingFacadeCustomData.ApplyCustomData(building, dictionary);
		}
		UserNameable component2 = base.GetComponent<UserNameable>();
		if (component2 != null && !string.IsNullOrEmpty(component2.savedName) && component2.savedName != base.GetComponent<Building>().Def.Name)
		{
			component2.SetName(component2.savedName);
		}
		else
		{
			base.GetComponent<KSelectable>().SetName(displayName);
			if (DetailsScreen.Instance != null && DetailsScreen.Instance.target == base.gameObject)
			{
				DetailsScreen.Instance.RefreshTitle();
			}
		}
		if (base.GetComponent<AnimTileable>() != null && components.Length != 0)
		{
			GameScenePartitioner.Instance.TriggerEvent(components[0].GetExtents(), GameScenePartitioner.Instance.objectLayers[1], null);
		}
	}

	public string GetNextFacade()
	{
		BuildingDef def = base.GetComponent<Building>().Def;
		int num = def.AvailableFacades.FindIndex((string s) => s == this.currentFacade) + 1;
		if (num >= def.AvailableFacades.Count)
		{
			num = 0;
		}
		return def.AvailableFacades[num];
	}

	[Serialize]
	private string currentFacade;

	public KAnimFile[] animFiles;

	public Dictionary<string, KAnimFile[]> interactAnims = new Dictionary<string, KAnimFile[]>();

	private BuildingFacadeAnimateIn animateIn;
}
