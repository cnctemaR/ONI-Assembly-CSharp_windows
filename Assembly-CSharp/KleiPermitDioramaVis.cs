using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.allVisList = ReflectionUtil.For<KleiPermitDioramaVis>(this).CollectValuesForFieldsThatInheritOrImplement<IKleiPermitDioramaVisTarget>(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.ConfigureSetup();
		}
	}

	public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
	{
		foreach (IKleiPermitDioramaVisTarget kleiPermitDioramaVisTarget in this.allVisList)
		{
			kleiPermitDioramaVisTarget.GetGameObject().SetActive(false);
		}
		IKleiPermitDioramaVisTarget permitVisTarget = this.GetPermitVisTarget(permit, permitPresInfo);
		permitVisTarget.GetGameObject().SetActive(true);
		permitVisTarget.ConfigureWith(permit, permitPresInfo);
	}

	public IKleiPermitDioramaVisTarget GetPermitVisTarget(PermitResource permit, PermitPresentationInfo permitPresInfo)
	{
		KleiPermitDioramaVis.lastRenderedPermit = permit;
		if (permit == null)
		{
			return this.fallbackVis.WithError(string.Format("Given invalid permit: {0}", permit));
		}
		if (permit.PermitCategory == PermitCategory.Equipment || permit.PermitCategory == PermitCategory.DupeTops || permit.PermitCategory == PermitCategory.DupeBottoms || permit.PermitCategory == PermitCategory.DupeGloves || permit.PermitCategory == PermitCategory.DupeShoes || permit.PermitCategory == PermitCategory.DupeHats || permit.PermitCategory == PermitCategory.DupeAccessories)
		{
			return this.equipmentVis;
		}
		if (permit.PermitCategory == PermitCategory.Building)
		{
			bool flag;
			BuildLocationRule buildLocationRule;
			KleiPermitVisUtil.GetBuildLocationRule(permit).Deconstruct(out flag, out buildLocationRule);
			bool flag2 = flag;
			BuildLocationRule buildLocationRule2 = buildLocationRule;
			if (!flag2)
			{
				return this.fallbackVis.WithError("Couldn't get BuildLocationRule on permit with id \"" + permit.Id + "\"");
			}
			switch (buildLocationRule2)
			{
			case BuildLocationRule.OnFloor:
				return this.buildingOnFloorVis;
			case BuildLocationRule.OnCeiling:
			{
				string prefabID = KleiPermitVisUtil.GetBuildingDef(permit).Value.PrefabID;
				if (prefabID == "FlowerVaseHanging" || prefabID == "FlowerVaseHangingFancy")
				{
					return this.hangingPlanterVis;
				}
				return this.pedestalAndItemVis;
			}
			case BuildLocationRule.OnWall:
			case BuildLocationRule.InCorner:
			case BuildLocationRule.NotInTiles:
				return this.pedestalAndItemVis;
			}
			return this.fallbackVis.WithError(string.Format("No visualization available for building with BuildLocationRule of {0}", buildLocationRule2));
		}
		else
		{
			if (permit.PermitCategory != PermitCategory.Artwork)
			{
				return this.fallbackVis.WithError("No visualization has been defined for permit with id \"" + permit.Id + "\"");
			}
			bool flag;
			BuildingDef buildingDef;
			KleiPermitVisUtil.GetBuildingDef(permit).Deconstruct(out flag, out buildingDef);
			bool flag3 = flag;
			BuildingDef buildingDef2 = buildingDef;
			if (!flag3)
			{
				return this.fallbackVis.WithError("Couldn't find building def for Artable " + permit.Id);
			}
			ArtableStage artableStage = (ArtableStage)permit;
			if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|11_0<Sculpture>(buildingDef2))
			{
				return this.artableSculptureVis;
			}
			if (KleiPermitDioramaVis.<GetPermitVisTarget>g__Has|11_0<Painting>(buildingDef2))
			{
				return this.artablePaintingVis;
			}
			return this.fallbackVis.WithError("No visualization available for Artable " + permit.Id);
		}
	}

	[CompilerGenerated]
	internal static bool <GetPermitVisTarget>g__Has|11_0<T>(BuildingDef buildingDef) where T : Component
	{
		return !buildingDef.BuildingComplete.GetComponent<T>().IsNullOrDestroyed();
	}

	[SerializeField]
	private KleiPermitDioramaVis_Fallback fallbackVis;

	[SerializeField]
	private KleiPermitDioramaVis_DupeEquipment equipmentVis;

	[SerializeField]
	private KleiPermitDioramaVis_BuildingOnFloor buildingOnFloorVis;

	[SerializeField]
	private KleiPermitDioramaVis_PedestalAndItem pedestalAndItemVis;

	[SerializeField]
	private KleiPermitDioramaVis_HangingPlanter hangingPlanterVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtablePainting artablePaintingVis;

	[SerializeField]
	private KleiPermitDioramaVis_ArtableSculpture artableSculptureVis;

	private IReadOnlyList<IKleiPermitDioramaVisTarget> allVisList;

	public static PermitResource lastRenderedPermit;
}
