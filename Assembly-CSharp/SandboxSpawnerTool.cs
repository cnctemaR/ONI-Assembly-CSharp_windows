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
		if (SandboxToolParameterMenu.instance.settings.Entity.PrefabID() == MinionConfig.ID)
		{
			this.SpawnMinion();
		}
		else
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(SandboxToolParameterMenu.instance.settings.Entity.PrefabTag), Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures, Folder.Entities, null, 0);
			gameObject.SetActive(true);
		}
		UISounds.PlaySound(UISounds.Sound.ClickObject);
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
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), SceneOrganizer.Instance.GetFolder(Folder.Entities), null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		MinionStartingStats minionStartingStats = new MinionStartingStats(false);
		minionStartingStats.Apply(gameObject);
	}

	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;
}
