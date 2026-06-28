using System;

public class KAnimGridTileVisualizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Refresh();
		this.Subscribe(-1503271301, new EventSystem.EventHandler(this.OnSelectionChanged));
		this.Subscribe(-1201923725, new EventSystem.EventHandler(this.OnHighlightChanged));
	}

	protected override void OnCleanUp()
	{
		if (this.Building != null)
		{
			int num = Grid.PosToCell(this.transform.position);
			ObjectLayer tileLayer = this.Building.Def.TileLayer;
			if (Grid.Objects[num, (int)tileLayer] == base.gameObject)
			{
				Grid.Objects[num, (int)tileLayer] = null;
			}
			TileVisualizer.RefreshCell(num, tileLayer);
		}
		base.OnCleanUp();
	}

	private void OnSelectionChanged(object data)
	{
		bool flag = (bool)data;
		World.Instance.blockTileRenderer.SelectCell(Grid.PosToCell(this.transform.position), flag);
	}

	private void OnHighlightChanged(object data)
	{
		bool flag = (bool)data;
		World.Instance.blockTileRenderer.HighlightCell(Grid.PosToCell(this.transform.position), flag);
	}

	public void Refresh()
	{
		if (this.Building == null)
		{
			this.Building = base.GetComponentInParent<Building>();
		}
		if (this.Building == null)
		{
			return;
		}
		if (this.controller != null)
		{
			int num = Grid.PosToCell(this.transform.position);
			string tile = this.GetTile(num, this.Building.Def.TileLayer);
			if (tile != string.Empty)
			{
				this.controller.Play(tile, KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	private unsafe string GetTile(int cell, ObjectLayer layer)
	{
		string text = string.Empty;
		int* ptr = stackalloc int[checked(4 * 4)];
		*ptr = Grid.CellLeft(cell);
		ptr[1] = Grid.CellRight(cell);
		ptr[2] = Grid.CellAbove(cell);
		ptr[3] = Grid.CellBelow(cell);
		for (int i = 0; i < 4; i++)
		{
			if (Grid.IsValidCell(ptr[i]) && Grid.Objects[ptr[i], (int)layer] != null)
			{
				text += "LRTB"[i];
			}
		}
		if (text == string.Empty)
		{
			text = "LR";
		}
		return text;
	}

	private Building Building;

	[MyCmpGet]
	private KAnimControllerBase controller;

	private KAnimControllerBase sub_controller;
}
