using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxSpawnerTool : InterfaceTool
{
	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		colors.Add(new ToolMenu.CellColorData(this.currentCell, this.radiusIndicatorColor));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		this.currentCell = Grid.PosToCell(cursorPos);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		this.Place(Grid.PosToCell(cursor_pos));
	}

	private void Place(int cell)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			return;
		}
		string stringSetting = SandboxToolParameterMenu.instance.settings.GetStringSetting("SandboxTools.SelectedEntity");
		GameObject prefab = Assets.GetPrefab(stringSetting);
		if (stringSetting == MinionConfig.ID)
		{
			this.SpawnMinion();
		}
		else if (prefab.GetComponent<Building>() != null)
		{
			BuildingDef def = prefab.GetComponent<Building>().Def;
			def.Build(cell, Orientation.Neutral, null, def.DefaultElements(), 298.15f, true, -1f);
		}
		else
		{
			GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures, null, 0);
			if (gameObject.GetComponent<Pickupable>() != null && !gameObject.HasTag(GameTags.Creature))
			{
				gameObject.transform.position += Vector3.up * (Grid.CellSizeInMeters / 3f);
			}
			gameObject.SetActive(true);
		}
		GameUtil.KInstantiate(this.fxPrefab, Grid.CellToPosCCC(this.currentCell, Grid.SceneLayer.FXFront), Grid.SceneLayer.FXFront, null, 0).GetComponent<KAnimControllerBase>().Play("placer", KAnim.PlayMode.Once, 1f, 0f);
		KFMOD.PlayUISound(this.soundPath);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.entitySelector.row.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		new MinionStartingStats(false, null, null, false).Apply(gameObject);
		gameObject.GetMyWorld().SetDupeVisited();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.SandboxCopyElement))
		{
			int num = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			List<ObjectLayer> list = new List<ObjectLayer>();
			list.Add(ObjectLayer.Pickupables);
			list.Add(ObjectLayer.Plants);
			list.Add(ObjectLayer.Minion);
			list.Add(ObjectLayer.Building);
			if (Grid.IsValidCell(num))
			{
				foreach (ObjectLayer objectLayer in list)
				{
					GameObject gameObject = Grid.Objects[num, (int)objectLayer];
					if (gameObject)
					{
						SandboxToolParameterMenu.instance.settings.SetStringSetting("SandboxTools.SelectedEntity", gameObject.PrefabID().ToString());
						break;
					}
				}
			}
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;

	private string soundPath = GlobalAssets.GetSound("SandboxTool_Spawner", false);

	[SerializeField]
	private GameObject fxPrefab;
}
