using System;
using System.Collections.Generic;
using Database;
using KSerialization;

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
			this.ApplyBuildingFacade(Db.GetBuildingFacades().TryGet(this.currentFacade));
		}
	}

	public void ApplyBuildingFacade(BuildingFacadeResource facade)
	{
		if (facade == null)
		{
			this.ClearFacade();
			return;
		}
		this.currentFacade = facade.Id;
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim(facade.AnimFile) };
		this.ChangeBuilding(array, facade.Name, facade.Description, facade.InteractFile);
	}

	private void ClearFacade()
	{
		Building component = base.GetComponent<Building>();
		this.ChangeBuilding(component.Def.AnimFiles, component.Def.Name, component.Def.Desc, null);
	}

	private void ChangeBuilding(KAnimFile[] animFiles, string displayName, string desc, Dictionary<string, string> interactAnimsNames = null)
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
		Building[] components = base.GetComponents<Building>();
		foreach (Building building in components)
		{
			building.SetDescription(desc);
			building.GetComponent<KBatchedAnimController>().SwapAnims(animFiles);
		}
		base.GetComponent<KSelectable>().SetName(displayName);
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
}
