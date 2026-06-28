using System;
using Klei.AI;
using TemplateClasses;
using UnityEngine;

public static class TemplateLoader
{
	public static void Stamp(TemplateContainer template, Vector2 rootLocation, global::System.Action on_complete_callback)
	{
		TemplateLoader.template = template;
		TemplateLoader.BuildPhase1((int)rootLocation.x, (int)rootLocation.y, delegate
		{
			TemplateLoader.BuildPhase2((int)rootLocation.x, (int)rootLocation.y, delegate
			{
				TemplateLoader.BuildPhase3((int)rootLocation.x, (int)rootLocation.y, delegate
				{
					TemplateLoader.BuildPhase4((int)rootLocation.x, (int)rootLocation.y, on_complete_callback);
				});
			});
		});
	}

	private static void BuildPhase1(int baseX, int baseY, global::System.Action callback)
	{
		if (Grid.WidthInCells < 16)
		{
			return;
		}
		CellOffset[] array = new CellOffset[TemplateLoader.template.cells.Count];
		for (int i = 0; i < TemplateLoader.template.cells.Count; i++)
		{
			array[i] = new CellOffset(TemplateLoader.template.cells[i].location_x, TemplateLoader.template.cells[i].location_y);
		}
		TemplateLoader.ClearPickups(baseX, baseY, array);
		if (TemplateLoader.template.cells.Count > 0)
		{
			TemplateLoader.ApplyGridProperties(baseX, baseY, TemplateLoader.template);
			TemplateLoader.PlaceCells(baseX, baseY, TemplateLoader.template, callback);
			TemplateLoader.ClearEntities<Crop>(baseX, baseY, array);
			TemplateLoader.ClearEntities<Health>(baseX, baseY, array);
			TemplateLoader.ClearEntities<Geyser>(baseX, baseY, array);
		}
		else
		{
			callback();
		}
	}

	private static void BuildPhase2(int baseX, int baseY, global::System.Action callback)
	{
		int num = Grid.OffsetCell(0, baseX, baseY);
		if (TemplateLoader.template == null)
		{
			global::Debug.LogError("No stamp template", null);
		}
		if (TemplateLoader.template.buildings != null)
		{
			for (int i = 0; i < TemplateLoader.template.buildings.Count; i++)
			{
				TemplateLoader.PlaceBuilding(TemplateLoader.template.buildings[i], num);
			}
		}
		HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback, false));
		SimMessages.ReplaceElement(num, ElementLoader.elements[(int)Grid.ElementIdx[num]].id, CellEventLogger.Instance.TemplateLoader, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num], handle.index);
		handle.index = -1;
	}

	public static GameObject PlaceBuilding(Prefab prefab, int root_cell)
	{
		if (prefab == null || prefab.id == string.Empty)
		{
			return null;
		}
		BuildingDef buildingDef = Assets.GetBuildingDef(prefab.id);
		if (buildingDef == null)
		{
			return null;
		}
		int num = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, num, location_y)))
		{
			return null;
		}
		int widthInCells = Assets.GetBuildingDef(prefab.id).WidthInCells;
		if (widthInCells >= 3)
		{
			num--;
		}
		GameObject gameObject = Scenario.PlaceBuilding(root_cell, num, location_y, prefab.id, prefab.element);
		if (gameObject == null)
		{
			global::Debug.LogWarning("Null prefab for " + prefab.id, null);
			return gameObject;
		}
		BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
		Rotatable component2 = gameObject.GetComponent<Rotatable>();
		if (component2 != null)
		{
			component2.SetOrientation(prefab.rotationOrientation);
		}
		PrimaryElement component3 = component.GetComponent<PrimaryElement>();
		if (prefab.temperature > 0f)
		{
			component3.Temperature = prefab.temperature;
		}
		component3.AddDisease(Db.Get().Diseases.GetIndex(prefab.diseaseName), prefab.diseaseCount, "TemplateLoader.PlaceBuilding");
		if (prefab.id == "Door")
		{
			for (int i = 0; i < component.PlacementCells.Length; i++)
			{
				SimMessages.ReplaceElement(component.PlacementCells[i], SimHashes.Vacuum, CellEventLogger.Instance.TemplateLoader, 0f, 0f, byte.MaxValue, 0, -1);
			}
		}
		if (prefab.amounts != null)
		{
			foreach (Prefab.template_amount_value template_amount_value in prefab.amounts)
			{
				try
				{
					if (Db.Get().Amounts.Get(template_amount_value.id) != null)
					{
						gameObject.GetAmounts().SetValue(template_amount_value.id, template_amount_value.value);
					}
				}
				catch
				{
					global::Debug.LogWarning(string.Format("Building does not have amount with ID {0}", template_amount_value.id), null);
				}
			}
		}
		if (prefab.other_values != null)
		{
			Prefab.template_amount_value[] other_values = prefab.other_values;
			for (int k = 0; k < other_values.Length; k++)
			{
				Prefab.template_amount_value template_amount_value2 = other_values[k];
				string id = template_amount_value2.id;
				if (id != null)
				{
					if (!(id == "joulesAvailable"))
					{
						if (!(id == "sealedDoorDirection"))
						{
							if (id == "switchSetting")
							{
								LogicSwitch s = gameObject.GetComponent<LogicSwitch>();
								if (s && ((s.IsSwitchedOn && template_amount_value2.value == 0f) || (!s.IsSwitchedOn && template_amount_value2.value == 1f)))
								{
									s.SetFirstFrameCallback(delegate
									{
										s.HandleToggle();
									});
								}
							}
						}
						else
						{
							Unsealable component4 = gameObject.GetComponent<Unsealable>();
							if (component4)
							{
								component4.facingRight = template_amount_value2.value != 0f;
							}
						}
					}
					else
					{
						Battery component5 = gameObject.GetComponent<Battery>();
						if (component5)
						{
							component5.AddEnergy(template_amount_value2.value);
						}
					}
				}
			}
		}
		if (prefab.storage != null && prefab.storage.Count > 0)
		{
			Storage component6 = component.gameObject.GetComponent<Storage>();
			if (component6 == null)
			{
				global::Debug.LogWarning("No storage component on stampTemplate building " + prefab.id + ". Saved storage contents will be ignored.", null);
			}
			int l = 0;
			while (l < prefab.storage.Count)
			{
				StorageItem storageItem = prefab.storage[l];
				string id2 = storageItem.id;
				GameObject gameObject2;
				if (storageItem.isOre)
				{
					Substance substance = ElementLoader.FindElementByHash(storageItem.element).substance;
					gameObject2 = substance.SpawnResource(Vector3.zero, storageItem.units, storageItem.temperature, Db.Get().Diseases.GetIndex(storageItem.diseaseName), storageItem.diseaseCount, false, false);
					goto IL_0506;
				}
				gameObject2 = Scenario.SpawnPrefab(root_cell, 0, 0, id2, Grid.SceneLayer.Ore, Folder.Entities);
				if (gameObject2 == null)
				{
					global::Debug.LogWarning("Null prefab for " + id2, null);
				}
				else
				{
					gameObject2.SetActive(true);
					PrimaryElement component7 = gameObject2.GetComponent<PrimaryElement>();
					component7.Units = storageItem.units;
					component7.Temperature = storageItem.temperature;
					component7.AddDisease(Db.Get().Diseases.GetIndex(storageItem.diseaseName), storageItem.diseaseCount, "TemplateLoader.PlaceBuilding");
					global::Rottable.Instance smi = gameObject2.GetSMI<global::Rottable.Instance>();
					if (smi != null)
					{
						smi.RotValue = storageItem.rottable.rotAmount;
						goto IL_0506;
					}
					goto IL_0506;
				}
				IL_053D:
				l++;
				continue;
				IL_0506:
				GameObject gameObject3 = component6.Store(gameObject2, true, true, true, false);
				if (gameObject3 != null)
				{
					gameObject3.GetComponent<Pickupable>().OnStore(component6);
				}
				gameObject2.GetComponent<SavedObject>().inStorage = true;
				goto IL_053D;
			}
		}
		if (prefab.connections != 0)
		{
			TemplateLoader.PlaceUtilityConnection(gameObject, prefab, root_cell);
		}
		return gameObject;
	}

	public static void PlaceUtilityConnection(GameObject spawned, Prefab bc, int root_cell)
	{
		int cell = Grid.OffsetCell(root_cell, bc.location_x, bc.location_y);
		UtilityConnections connection = (UtilityConnections)bc.connections;
		string id = bc.id;
		switch (id)
		{
		case "Wire":
		case "InsulatedWire":
		case "HighWattageWire":
			spawned.GetComponent<Wire>().SetFirstFrameCallback(delegate
			{
				Game.Instance.electricalConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component != null)
				{
					component.Refresh();
				}
			});
			break;
		case "GasConduit":
		case "InsulatedGasConduit":
			spawned.GetComponent<Conduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.gasConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component2 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component2 != null)
				{
					component2.Refresh();
				}
			});
			break;
		case "LiquidConduit":
		case "InsulatedLiquidConduit":
			spawned.GetComponent<Conduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.liquidConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component3 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component3 != null)
				{
					component3.Refresh();
				}
			});
			break;
		case "SolidConduit":
			spawned.GetComponent<SolidConduit>().SetFirstFrameCallback(delegate
			{
				Game.Instance.solidConduitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component4 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component4 != null)
				{
					component4.Refresh();
				}
			});
			break;
		case "LogicWire":
			spawned.GetComponent<LogicWire>().SetFirstFrameCallback(delegate
			{
				Game.Instance.logicCircuitSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component5 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component5 != null)
				{
					component5.Refresh();
				}
			});
			break;
		case "TravelTube":
			spawned.GetComponent<TravelTube>().SetFirstFrameCallback(delegate
			{
				Game.Instance.travelTubeSystem.SetConnections(connection, cell, true);
				KAnimGraphTileVisualizer component6 = spawned.GetComponent<KAnimGraphTileVisualizer>();
				if (component6 != null)
				{
					component6.Refresh();
				}
			});
			break;
		}
	}

	public static GameObject PlacePickupables(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		GameObject gameObject = Scenario.SpawnPrefab(root_cell, location_x, location_y, prefab.id, Grid.SceneLayer.Ore, Folder.Entities);
		if (gameObject == null)
		{
			global::Debug.LogWarning("Null prefab for " + prefab.id, null);
			return null;
		}
		gameObject.SetActive(true);
		if (prefab.units != 0f)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.Units = prefab.units;
			component.Temperature = ((prefab.temperature <= 0f) ? component.Element.defaultValues.temperature : prefab.temperature);
			component.AddDisease(Db.Get().Diseases.GetIndex(prefab.diseaseName), prefab.diseaseCount, "TemplateLoader.PlacePickupables");
		}
		global::Rottable.Instance smi = gameObject.GetSMI<global::Rottable.Instance>();
		if (smi != null)
		{
			smi.RotValue = prefab.rottable.rotAmount;
		}
		return gameObject;
	}

	public static GameObject PlaceOtherEntities(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		GameObject prefab2 = Assets.GetPrefab(new Tag(prefab.id));
		if (prefab2 == null)
		{
			return null;
		}
		KBatchedAnimController kbatchedAnimController = prefab2.AddOrGet<KBatchedAnimController>();
		GameObject gameObject = Scenario.SpawnPrefab(root_cell, location_x, location_y, prefab.id, kbatchedAnimController.sceneLayer, Folder.Entities);
		if (gameObject == null)
		{
			global::Debug.LogWarning("Null prefab for " + prefab.id, null);
			return null;
		}
		gameObject.SetActive(true);
		if (prefab.amounts != null)
		{
			foreach (Prefab.template_amount_value template_amount_value in prefab.amounts)
			{
				try
				{
					gameObject.GetAmounts().SetValue(template_amount_value.id, template_amount_value.value);
				}
				catch
				{
					global::Debug.LogWarning(string.Format("Entity {0} does not have amount with ID {1}", gameObject.GetProperName(), template_amount_value.id), null);
				}
			}
		}
		return gameObject;
	}

	public static GameObject PlaceElementalOres(Prefab prefab, int root_cell)
	{
		int location_x = prefab.location_x;
		int location_y = prefab.location_y;
		if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, location_x, location_y)))
		{
			return null;
		}
		Substance substance = ElementLoader.FindElementByHash(prefab.element).substance;
		int num = Grid.OffsetCell(root_cell, location_x, location_y);
		Vector3 vector = Grid.CellToPosCCC(num, Grid.SceneLayer.Ore);
		byte index = Db.Get().Diseases.GetIndex(prefab.diseaseName);
		if (prefab.temperature <= 0f)
		{
			global::Debug.LogWarning("Template trying to spawn zero temperature substance!", null);
			prefab.temperature = 300f;
		}
		return substance.SpawnResource(vector, prefab.units, prefab.temperature, index, prefab.diseaseCount, false, false);
	}

	private static void BuildPhase3(int baseX, int baseY, global::System.Action callback)
	{
		if (TemplateLoader.template != null)
		{
			int num = Grid.OffsetCell(0, baseX, baseY);
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes)
			{
				KAnimGraphTileVisualizer component = buildingComplete.GetComponent<KAnimGraphTileVisualizer>();
				if (component != null)
				{
					component.Refresh();
				}
			}
			for (int i = 0; i < TemplateLoader.template.pickupables.Count; i++)
			{
				if (TemplateLoader.template.pickupables[i] != null && !(TemplateLoader.template.pickupables[i].id == string.Empty))
				{
					TemplateLoader.PlacePickupables(TemplateLoader.template.pickupables[i], num);
				}
			}
			for (int j = 0; j < TemplateLoader.template.elementalOres.Count; j++)
			{
				if (TemplateLoader.template.elementalOres[j] != null && !(TemplateLoader.template.elementalOres[j].id == string.Empty))
				{
					TemplateLoader.PlaceElementalOres(TemplateLoader.template.elementalOres[j], num);
				}
			}
		}
		if (callback != null)
		{
			callback();
		}
	}

	private static void BuildPhase4(int baseX, int baseY, global::System.Action callback)
	{
		if (TemplateLoader.template != null)
		{
			int num = Grid.OffsetCell(0, baseX, baseY);
			for (int i = 0; i < TemplateLoader.template.otherEntities.Count; i++)
			{
				if (TemplateLoader.template.otherEntities[i] != null && !(TemplateLoader.template.otherEntities[i].id == string.Empty))
				{
					TemplateLoader.PlaceOtherEntities(TemplateLoader.template.otherEntities[i], num);
				}
			}
			TemplateLoader.template = null;
		}
		if (callback != null)
		{
			callback();
		}
	}

	private static void ClearPickups(int baseX, int baseY, CellOffset[] template_as_offsets)
	{
		if (WorldGenSpawner.Instance != null)
		{
			WorldGenSpawner.Instance.ClearSpawnersInArea(new Vector2((float)baseX, (float)baseY), template_as_offsets);
		}
		foreach (Pickupable pickupable in Components.Pickupables)
		{
			if (Grid.IsCellOffsetOf(Grid.XYToCell(baseX, baseY), pickupable.gameObject, template_as_offsets))
			{
				Util.KDestroyGameObject(pickupable.gameObject);
			}
		}
	}

	private static void ClearEntities<T>(int rootX, int rootY, CellOffset[] TemplateOffsets) where T : KMonoBehaviour
	{
		T[] array = (T[])global::UnityEngine.Object.FindObjectsOfType(typeof(T));
		foreach (T t in array)
		{
			if (Grid.IsCellOffsetOf(Grid.PosToCell(t.gameObject), Grid.XYToCell(rootX, rootY), TemplateOffsets))
			{
				Util.KDestroyGameObject(t.gameObject);
			}
		}
	}

	private static void PlaceCells(int baseX, int baseY, TemplateContainer template, global::System.Action callback)
	{
		HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback, false));
		if (template == null)
		{
			global::Debug.LogError("Template Loader does not have template.", null);
		}
		for (int i = 0; i < template.cells.Count; i++)
		{
			int num = Grid.XYToCell(template.cells[i].location_x + baseX, template.cells[i].location_y + baseY);
			if (!Grid.IsValidCell(num))
			{
				global::Debug.LogError(string.Format("Trying to replace invalid cells cell{0} root{1}:{2} offset{3}:{4}", new object[]
				{
					num,
					baseX,
					baseY,
					template.cells[i].location_x,
					template.cells[i].location_y
				}), null);
			}
			SimHashes element = template.cells[i].element;
			float mass = template.cells[i].mass;
			float temperature = template.cells[i].temperature;
			byte index = Db.Get().Diseases.GetIndex(template.cells[i].diseaseName);
			int diseaseCount = template.cells[i].diseaseCount;
			SimMessages.ReplaceElement(num, element, CellEventLogger.Instance.TemplateLoader, mass, temperature, index, diseaseCount, handle.index);
			handle.index = -1;
		}
	}

	public static void ApplyGridProperties(int baseX, int baseY, TemplateContainer template)
	{
		for (int i = 0; i < template.cells.Count; i++)
		{
			int num = Grid.XYToCell(template.cells[i].location_x + baseX, template.cells[i].location_y + baseY);
			if (Grid.IsValidCell(num))
			{
				if (template.cells[i].preventFoWReveal)
				{
					Grid.PreventFogOfWarReveal[num] = true;
					Grid.Visible[num] = 0;
				}
			}
		}
	}

	private static TemplateContainer template;
}
