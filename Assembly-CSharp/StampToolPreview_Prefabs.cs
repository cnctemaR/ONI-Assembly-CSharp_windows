using System;
using Database;
using TemplateClasses;
using UnityEngine;

public class StampToolPreview_Prefabs : IStampToolPreviewPlugin
{
	public void Setup(StampToolPreviewContext context)
	{
		if (!context.stampTemplate.elementalOres.IsNullOrDestroyed())
		{
			foreach (Prefab prefab in context.stampTemplate.elementalOres)
			{
				StampToolPreview_Prefabs.SpawnPrefab(context, prefab);
			}
		}
		if (!context.stampTemplate.otherEntities.IsNullOrDestroyed())
		{
			foreach (Prefab prefab2 in context.stampTemplate.otherEntities)
			{
				StampToolPreview_Prefabs.SpawnPrefab(context, prefab2);
			}
		}
		if (!context.stampTemplate.backwallEntities.IsNullOrDestroyed())
		{
			foreach (Prefab prefab3 in context.stampTemplate.backwallEntities)
			{
				StampToolPreview_Prefabs.SpawnPrefab(context, prefab3);
			}
		}
		if (!context.stampTemplate.buildings.IsNullOrDestroyed())
		{
			foreach (Prefab prefab4 in context.stampTemplate.buildings)
			{
				StampToolPreview_Prefabs.SpawnPrefab(context, prefab4);
			}
		}
		if (!context.stampTemplate.elementalOres.IsNullOrDestroyed())
		{
			foreach (Prefab prefab5 in context.stampTemplate.elementalOres)
			{
				StampToolPreview_Prefabs.SpawnPrefab(context, prefab5);
			}
		}
	}

	public static void SpawnPrefab(StampToolPreviewContext context, Prefab prefabInfo)
	{
		GameObject gameObject = Assets.TryGetPrefab(prefabInfo.id);
		if (gameObject.IsNullOrDestroyed())
		{
			return;
		}
		if (gameObject.GetComponent<Building>().IsNullOrDestroyed())
		{
			StampToolPreview_Prefabs.SpawnPrefab_Default(context, prefabInfo, gameObject);
			return;
		}
		Building component = gameObject.GetComponent<Building>();
		if (component.Def.IsTilePiece)
		{
			StampToolPreview_Prefabs.SpawnPrefab_Tile(context, prefabInfo, component);
			return;
		}
		StampToolPreview_Prefabs.SpawnPrefab_Building(context, prefabInfo, component);
	}

	public static void SpawnPrefab_Tile(StampToolPreviewContext context, Prefab prefabInfo, Building buildingPrefab)
	{
		TextureAtlas textureAtlas = buildingPrefab.Def.BlockTilePlaceAtlas;
		if (textureAtlas == null)
		{
			textureAtlas = buildingPrefab.Def.BlockTileAtlas;
		}
		if (textureAtlas == null || textureAtlas.items == null || textureAtlas.items.Length < 0)
		{
			return;
		}
		GameObject gameObject;
		MeshRenderer meshRenderer;
		StampToolPreviewUtil.MakeQuad(out gameObject, out meshRenderer, 1.5f, new Vector4?(textureAtlas.items[0].uvBox));
		gameObject.name = string.Format("TilePlacer {0}", buildingPrefab.PrefabID());
		gameObject.transform.SetParent(context.previewParent.transform, false);
		gameObject.transform.SetLocalPosition(new Vector2((float)prefabInfo.location_x, (float)prefabInfo.location_y + Grid.HalfCellSizeInMeters));
		Material material = StampToolPreviewUtil.MakeMaterial(textureAtlas.texture);
		material.name = string.Format("Tile ({0}) ({1})", buildingPrefab.PrefabID(), material.name);
		meshRenderer.material = material;
		context.cleanupFn = (global::System.Action)Delegate.Combine(context.cleanupFn, new global::System.Action(delegate
		{
			if (!gameObject.IsNullOrDestroyed())
			{
				global::UnityEngine.Object.Destroy(gameObject);
			}
			if (!material.IsNullOrDestroyed())
			{
				global::UnityEngine.Object.Destroy(material);
			}
		}));
		context.onErrorChangeFn = (Action<string>)Delegate.Combine(context.onErrorChangeFn, new Action<string>(delegate(string error)
		{
			if (meshRenderer.IsNullOrDestroyed())
			{
				return;
			}
			meshRenderer.material.color = ((error != null) ? StampToolPreviewUtil.COLOR_ERROR : StampToolPreviewUtil.COLOR_OK);
		}));
	}

	public static void SpawnPrefab_Building(StampToolPreviewContext context, Prefab prefabInfo, Building buildingPrefab)
	{
		int num = LayerMask.NameToLayer("Place");
		GameObject gameObject;
		if (buildingPrefab.Def.BuildingPreview.IsNullOrDestroyed())
		{
			gameObject = BuildingLoader.Instance.CreateBuildingPreview(buildingPrefab.Def);
		}
		else
		{
			gameObject = buildingPrefab.Def.BuildingPreview;
		}
		Building spawn = GameUtil.KInstantiate(gameObject, Vector3.zero, Grid.SceneLayer.Building, null, num).GetComponent<Building>();
		context.cleanupFn = (global::System.Action)Delegate.Combine(context.cleanupFn, new global::System.Action(delegate
		{
			if (spawn.IsNullOrDestroyed())
			{
				return;
			}
			global::UnityEngine.Object.Destroy(spawn.gameObject);
		}));
		Rotatable component = spawn.GetComponent<Rotatable>();
		if (component != null)
		{
			component.SetOrientation(prefabInfo.rotationOrientation);
		}
		KBatchedAnimController kanim = spawn.GetComponent<KBatchedAnimController>();
		if (kanim != null)
		{
			kanim.visibilityType = KAnimControllerBase.VisibilityType.Always;
			kanim.isMovable = true;
			kanim.Offset = buildingPrefab.Def.GetVisualizerOffset();
			kanim.name = kanim.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
			kanim.TintColour = StampToolPreviewUtil.COLOR_OK;
			kanim.SetLayer(num);
		}
		spawn.transform.SetParent(context.previewParent.transform, false);
		spawn.transform.SetLocalPosition(new Vector2((float)prefabInfo.location_x, (float)prefabInfo.location_y));
		context.frameAfterSetupFn = (global::System.Action)Delegate.Combine(context.frameAfterSetupFn, new global::System.Action(delegate
		{
			if (spawn.IsNullOrDestroyed())
			{
				return;
			}
			spawn.gameObject.SetActive(false);
			spawn.gameObject.SetActive(true);
			if (kanim.IsNullOrDestroyed())
			{
				return;
			}
			string text = "";
			if ((prefabInfo.connections & 1) != 0)
			{
				text += "L";
			}
			if ((prefabInfo.connections & 2) != 0)
			{
				text += "R";
			}
			if ((prefabInfo.connections & 4) != 0)
			{
				text += "U";
			}
			if ((prefabInfo.connections & 8) != 0)
			{
				text += "D";
			}
			if (text == "")
			{
				text = "None";
			}
			if (kanim != null && kanim.HasAnimation(text))
			{
				string text2 = text + "_place";
				bool flag = kanim.HasAnimation(text2);
				kanim.Play(flag ? text2 : text, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}));
		context.onErrorChangeFn = (Action<string>)Delegate.Combine(context.onErrorChangeFn, new Action<string>(delegate(string error)
		{
			if (kanim.IsNullOrDestroyed())
			{
				return;
			}
			Color color = ((error != null) ? StampToolPreviewUtil.COLOR_ERROR : StampToolPreviewUtil.COLOR_OK);
			if (buildingPrefab.Def.SceneLayer == Grid.SceneLayer.Backwall)
			{
				color.a = 0.2f;
			}
			kanim.TintColour = color;
		}));
		BuildingFacade component2 = spawn.GetComponent<BuildingFacade>();
		if (component2 != null && !prefabInfo.facadeId.IsNullOrWhiteSpace())
		{
			BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().TryGet(prefabInfo.facadeId);
			if (buildingFacadeResource != null && buildingFacadeResource.IsUnlocked())
			{
				component2.ApplyBuildingFacade(buildingFacadeResource, false);
			}
		}
	}

	public static void SpawnPrefab_Default(StampToolPreviewContext context, Prefab prefabInfo, GameObject prefab)
	{
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if (component == null)
		{
			return;
		}
		string text = prefab.GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
		int num = LayerMask.NameToLayer("Place");
		GameObject spawn = new GameObject(text);
		spawn.SetActive(false);
		KBatchedAnimController kanim = spawn.AddComponent<KBatchedAnimController>();
		if (!component.IsNullOrDestroyed())
		{
			kanim.AnimFiles = component.AnimFiles;
			kanim.visibilityType = KAnimControllerBase.VisibilityType.Always;
			kanim.isMovable = true;
			kanim.name = text;
			kanim.TintColour = StampToolPreviewUtil.COLOR_OK;
			kanim.SetLayer(num);
		}
		spawn.transform.SetParent(context.previewParent.transform, false);
		OccupyArea component2 = prefab.GetComponent<OccupyArea>();
		int num2;
		if (component2.IsNullOrDestroyed() || component2._UnrotatedOccupiedCellsOffsets.Length == 0)
		{
			num2 = 0;
		}
		else
		{
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			foreach (CellOffset cellOffset in component2._UnrotatedOccupiedCellsOffsets)
			{
				if (cellOffset.x < num3)
				{
					num3 = cellOffset.x;
				}
				if (cellOffset.x > num4)
				{
					num4 = cellOffset.x;
				}
			}
			num2 = num4 - num3 + 1;
		}
		if (num2 != 0 && num2 % 2 == 0)
		{
			spawn.transform.SetLocalPosition(new Vector2((float)prefabInfo.location_x + Grid.HalfCellSizeInMeters, (float)prefabInfo.location_y));
		}
		else
		{
			spawn.transform.SetLocalPosition(new Vector2((float)prefabInfo.location_x, (float)prefabInfo.location_y));
		}
		context.frameAfterSetupFn = (global::System.Action)Delegate.Combine(context.frameAfterSetupFn, new global::System.Action(delegate
		{
			if (spawn.IsNullOrDestroyed())
			{
				return;
			}
			spawn.gameObject.SetActive(false);
			spawn.gameObject.SetActive(true);
			if (kanim.IsNullOrDestroyed())
			{
				return;
			}
			kanim.Play("place", KAnim.PlayMode.Loop, 1f, 0f);
		}));
		context.cleanupFn = (global::System.Action)Delegate.Combine(context.cleanupFn, new global::System.Action(delegate
		{
			if (spawn.IsNullOrDestroyed())
			{
				return;
			}
			global::UnityEngine.Object.Destroy(spawn.gameObject);
		}));
		context.onErrorChangeFn = (Action<string>)Delegate.Combine(context.onErrorChangeFn, new Action<string>(delegate(string error)
		{
			if (kanim.IsNullOrDestroyed())
			{
				return;
			}
			kanim.TintColour = ((error != null) ? StampToolPreviewUtil.COLOR_ERROR : StampToolPreviewUtil.COLOR_OK);
		}));
	}
}
