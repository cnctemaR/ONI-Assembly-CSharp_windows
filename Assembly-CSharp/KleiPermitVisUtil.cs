using System;
using Database;
using UnityEngine;

public static class KleiPermitVisUtil
{
	public static void ConfigureToRenderBuilding(KBatchedAnimController buildingKAnim, BuildingFacadeResource buildingPermit)
	{
		KAnimFile anim = Assets.GetAnim(buildingPermit.AnimFile);
		buildingKAnim.Stop();
		buildingKAnim.SwapAnims(new KAnimFile[] { anim });
		buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(anim), KAnim.PlayMode.Loop, 1f, 0f);
		buildingKAnim.rectTransform().sizeDelta = 176f * Vector2.one;
	}

	public static void ConfigureToRenderBuilding(KBatchedAnimController buildingKAnim, BuildingDef buildingDef)
	{
		buildingKAnim.Stop();
		buildingKAnim.SwapAnims(buildingDef.AnimFiles);
		buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(buildingDef.AnimFiles[0]), KAnim.PlayMode.Loop, 1f, 0f);
		buildingKAnim.rectTransform().sizeDelta = 176f * Vector2.one;
	}

	public static void ConfigureToRenderBuilding(KBatchedAnimController buildingKAnim, ArtableStage artablePermit)
	{
		buildingKAnim.Stop();
		buildingKAnim.SwapAnims(new KAnimFile[] { Assets.GetAnim(artablePermit.animFile) });
		buildingKAnim.Play(artablePermit.anim, KAnim.PlayMode.Once, 1f, 0f);
		buildingKAnim.rectTransform().sizeDelta = 176f * Vector2.one;
	}

	public static void ConfigureToRenderBuilding(KBatchedAnimController buildingKAnim, DbStickerBomb artablePermit)
	{
		buildingKAnim.Stop();
		buildingKAnim.SwapAnims(new KAnimFile[] { artablePermit.animFile });
		bool flag;
		HashedString hashedString;
		KleiPermitVisUtil.GetDefaultStickerAnimHash(artablePermit.animFile).Deconstruct(out flag, out hashedString);
		bool flag2 = flag;
		HashedString hashedString2 = hashedString;
		if (flag2)
		{
			buildingKAnim.Play(hashedString2, KAnim.PlayMode.Once, 1f, 0f);
		}
		else
		{
			global::Debug.Assert(false, "Couldn't find default sticker for sticker " + artablePermit.Id);
			buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(artablePermit.animFile), KAnim.PlayMode.Once, 1f, 0f);
		}
		buildingKAnim.rectTransform().sizeDelta = 176f * Vector2.one;
	}

	public static void ConfigureBuildingPosition(RectTransform transform, PrefabDefinedUIPosition anchorPosition, BuildingDef buildingDef, Alignment alignment)
	{
		anchorPosition.SetOn(transform);
		transform.anchoredPosition += new Vector2(176f * (float)buildingDef.WidthInCells * -(alignment.x - 0.5f), 176f * (float)buildingDef.HeightInCells * -alignment.y);
	}

	public static void ConfigureBuildingPosition(RectTransform transform, Vector2 anchorPosition, BuildingDef buildingDef, Alignment alignment)
	{
		transform.anchoredPosition = anchorPosition + new Vector2(176f * (float)buildingDef.WidthInCells * -(alignment.x - 0.5f), 176f * (float)buildingDef.HeightInCells * -alignment.y);
	}

	public static HashedString GetFirstAnimHash(KAnimFile animFile)
	{
		return animFile.GetData().GetAnim(0).hash;
	}

	public static Option<HashedString> GetDefaultStickerAnimHash(KAnimFile stickerAnimFile)
	{
		KAnimFileData data = stickerAnimFile.GetData();
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.name.StartsWith("idle_sticker"))
			{
				return anim.hash;
			}
		}
		return Option.None;
	}

	public static Option<BuildLocationRule> GetBuildLocationRule(PermitResource permit)
	{
		Option<BuildingDef> buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
		if (!buildingDef.HasValue)
		{
			return Option.None;
		}
		return buildingDef.Value.BuildLocationRule;
	}

	public static Option<BuildingDef> GetBuildingDef(PermitResource permit)
	{
		BuildingFacadeResource buildingFacadeResource = permit as BuildingFacadeResource;
		if (buildingFacadeResource != null)
		{
			GameObject gameObject = Assets.TryGetPrefab(buildingFacadeResource.PrefabID);
			if (gameObject == null)
			{
				return Option.None;
			}
			BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
			if (component == null || !component)
			{
				return Option.None;
			}
			return component.Def;
		}
		else
		{
			ArtableStage artableStage = permit as ArtableStage;
			if (artableStage == null)
			{
				return Option.None;
			}
			BuildingComplete component2 = Assets.GetPrefab(artableStage.prefabId).GetComponent<BuildingComplete>();
			if (component2 == null || !component2)
			{
				return Option.None;
			}
			return component2.Def;
		}
	}

	public const float TILE_SIZE_UI = 176f;
}
