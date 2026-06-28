using System;
using BaseTemplateClasses;
using UnityEngine;

public static class TemplateLoader
{
	public static void Stamp(BaseTemplate template, Vector2 rootLocation, global::System.Action callback)
	{
		TemplateLoader.Template = template;
		TemplateLoader.BuildPhase1((int)rootLocation.x, (int)rootLocation.y, delegate
		{
			TemplateLoader.BuildPhase2((int)rootLocation.x, (int)rootLocation.y, delegate
			{
				TemplateLoader.BuildPhase3((int)rootLocation.x, (int)rootLocation.y, callback);
			});
		});
	}

	private static void BuildPhase1(int baseX, int baseY, global::System.Action callback)
	{
		if (Grid.WidthInCells < 16)
		{
			return;
		}
		TemplateLoader.AddStartLocation(baseX, baseY, TemplateLoader.Template, callback);
	}

	private static void BuildPhase2(int baseX, int baseY, global::System.Action callback)
	{
		int num = Grid.OffsetCell(0, baseX, baseY);
		if (TemplateLoader.Template == null)
		{
			Debug.LogError("No stamp template");
		}
		if (TemplateLoader.Template.buildings != null)
		{
			for (int i = 0; i < TemplateLoader.Template.buildings.Count; i++)
			{
				BaseTemplatePrefabInfo baseTemplatePrefabInfo = TemplateLoader.Template.buildings[i];
				if (baseTemplatePrefabInfo != null && !(baseTemplatePrefabInfo.id == string.Empty))
				{
					int num2 = baseTemplatePrefabInfo.location_x;
					int location_y = baseTemplatePrefabInfo.location_y;
					int widthInCells = Assets.GetBuildingDef(baseTemplatePrefabInfo.id).WidthInCells;
					if (widthInCells % 2 != 0 && widthInCells >= 3)
					{
						num2--;
					}
					if (baseTemplatePrefabInfo.id == "Headquarters")
					{
						num2--;
					}
					GameObject gameObject = Scenario.PlaceBuilding(num, num2, location_y, baseTemplatePrefabInfo.id, baseTemplatePrefabInfo.element);
					BuildingComplete component = gameObject.GetComponent<BuildingComplete>();
					Rotatable component2 = gameObject.GetComponent<Rotatable>();
					if (component2 != null)
					{
						component2.SetOrientation(baseTemplatePrefabInfo.rotationOrientation);
					}
					component.GetComponent<PrimaryElement>().Temperature = baseTemplatePrefabInfo.temperature;
					if (baseTemplatePrefabInfo.id == "Door")
					{
						for (int j = 0; j < component.PlacementCells.Length; j++)
						{
							SimMessages.ReplaceElement(component.PlacementCells[j], SimHashes.Vacuum, CellEventLogger.Instance.TemplateLoader, 0f, 0f, -1);
						}
					}
					if (baseTemplatePrefabInfo.storage != null && baseTemplatePrefabInfo.storage.Count > 0)
					{
						Storage component3 = component.gameObject.GetComponent<Storage>();
						if (component3 == null)
						{
							Debug.LogWarning("No storage component on stampTemplate building " + baseTemplatePrefabInfo.id + ". Saved storage contents will be ignored.");
						}
						for (int k = 0; k < baseTemplatePrefabInfo.storage.Count; k++)
						{
							BaseTemplateStorageItem baseTemplateStorageItem = baseTemplatePrefabInfo.storage[k];
							string id = baseTemplateStorageItem.id;
							GameObject gameObject2;
							if (baseTemplateStorageItem.isOre)
							{
								Substance substance = ElementLoader.FindElementByHash(baseTemplateStorageItem.element).substance;
								gameObject2 = substance.SpawnResource(Vector3.zero, baseTemplateStorageItem.mass, baseTemplateStorageItem.temperature, false, false);
							}
							else
							{
								gameObject2 = Scenario.SpawnPrefab(num, 0, 0, id, Grid.SceneLayer.Use, Folder.Entities);
								gameObject2.SetActive(true);
								Edible component4 = gameObject2.GetComponent<Edible>();
								Rottable.Instance smi = gameObject2.GetSMI<Rottable.Instance>();
								if (component4)
								{
									component4.rations = (float)baseTemplateStorageItem.rations.rations;
								}
								if (smi != null)
								{
									smi.RotAmount = baseTemplateStorageItem.rottable.rotAmount;
								}
							}
							component3.Store(gameObject2, true, false);
							gameObject2.GetComponent<SavedObject>().inStorage = true;
						}
					}
					if (TemplateLoader.Template.getUtilityConnections != null && i < TemplateLoader.Template.getUtilityConnections.Length && TemplateLoader.Template.getUtilityConnections[i] != null)
					{
						BaseTemplateConduitConnection baseTemplateConduitConnection = TemplateLoader.Template.getUtilityConnections[i];
						int num3 = Grid.OffsetCell(num, num2, location_y);
						switch (baseTemplateConduitConnection.systemType)
						{
						case BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Electrical:
							Game.Instance.electricalConduitSystem.SetConnections(baseTemplateConduitConnection.connection, num3, true);
							break;
						case BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Liquid:
							Game.Instance.liquidConduitSystem.SetConnections(baseTemplateConduitConnection.connection, num3, true);
							break;
						case BaseTemplateConduitConnection.BaseTemplateConduitSystemType.Gas:
							Game.Instance.gasConduitSystem.SetConnections(baseTemplateConduitConnection.connection, num3, true);
							break;
						}
					}
				}
			}
		}
		HandleVector<global::System.Action>.Handle handle = Game.Instance.callbackManager.Add(callback, "TemplateLoader");
		Sim.Cell cell = Grid.Cell[num];
		SimMessages.ReplaceElement(num, ElementLoader.elements[(int)Grid.Cell[num].elementIdx].id, CellEventLogger.Instance.TemplateLoader, cell.mass, cell.temperature, handle.index);
		handle.index = -1;
	}

	private static void BuildPhase3(int baseX, int baseY, global::System.Action callback)
	{
		int num = Grid.OffsetCell(0, baseX, baseY);
		foreach (BuildingComplete buildingComplete in Components.BuildingCompletes)
		{
			if (buildingComplete.PrefabID().Name == "Headquarters")
			{
				break;
			}
		}
		for (int i = 0; i < TemplateLoader.Template.pickupables.Count; i++)
		{
			if (TemplateLoader.Template.pickupables[i] != null && !(TemplateLoader.Template.pickupables[i].id == string.Empty))
			{
				int location_x = TemplateLoader.Template.pickupables[i].location_x;
				int location_y = TemplateLoader.Template.pickupables[i].location_y;
				GameObject gameObject = Scenario.SpawnPrefab(num, location_x, location_y, TemplateLoader.Template.pickupables[i].id, Grid.SceneLayer.Use, Folder.Entities);
				Edible component = gameObject.GetComponent<Edible>();
				if (component != null && TemplateLoader.Template.pickupables[i].rations.rations != 0f)
				{
					component.rations = TemplateLoader.Template.pickupables[i].rations.rations;
				}
				Rottable.Instance smi = gameObject.GetSMI<Rottable.Instance>();
				if (smi != null)
				{
					smi.RotAmount = TemplateLoader.Template.pickupables[i].rottable.rotAmount;
				}
			}
		}
		for (int j = 0; j < TemplateLoader.Template.elementalOres.Count; j++)
		{
			if (TemplateLoader.Template.elementalOres[j] != null && !(TemplateLoader.Template.elementalOres[j].id == string.Empty))
			{
				int location_x2 = TemplateLoader.Template.elementalOres[j].location_x;
				int location_y2 = TemplateLoader.Template.elementalOres[j].location_y;
				Substance substance = ElementLoader.FindElementByHash(TemplateLoader.Template.elementalOres[j].element).substance;
				int num2 = Grid.OffsetCell(num, location_x2, location_y2);
				Vector3 vector = Grid.CellToPosCCC(num2, Grid.SceneLayer.Use);
				substance.SpawnResource(vector, TemplateLoader.Template.elementalOres[j].mass, TemplateLoader.Template.elementalOres[j].temperature, false, false);
			}
		}
		TemplateLoader.Template = null;
		if (callback != null)
		{
			callback();
		}
	}

	private static void AddStartLocation(int baseX, int baseY, BaseTemplate template, global::System.Action callback)
	{
		HandleVector<global::System.Action>.Handle handle = Game.Instance.callbackManager.Add(callback, "TemplateLoaderAddStartLoc");
		if (template == null)
		{
			Debug.LogError("Template Loader does not have template.");
		}
		for (int i = 0; i < template.cells.Count; i++)
		{
			int num = Grid.XYToCell(template.cells[i].location_x + baseX, template.cells[i].location_y + baseY);
			SimHashes element = template.cells[i].element;
			float mass = template.cells[i].mass;
			float temperature = template.cells[i].temperature;
			SimMessages.ReplaceElement(num, element, CellEventLogger.Instance.TemplateLoader, mass, temperature, handle.index);
			handle.index = -1;
		}
	}

	private static BaseTemplate Template;
}
