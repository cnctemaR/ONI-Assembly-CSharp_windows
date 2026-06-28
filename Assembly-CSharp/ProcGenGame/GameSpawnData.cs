using System;
using System.Collections.Generic;
using KSerialization;
using TemplateClasses;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public class GameSpawnData
	{
		public void AddRange(IEnumerable<KeyValuePair<int, string>> newItems)
		{
			foreach (KeyValuePair<int, string> keyValuePair in newItems)
			{
				this.ClearTemplatesInCell(keyValuePair.Key);
				Vector2I vector2I = Grid.CellToXY(keyValuePair.Key);
				Prefab prefab = new Prefab(keyValuePair.Value, Prefab.Type.Other, vector2I.x, vector2I.y, (SimHashes)0, -1f, 1f, null, 0, Orientation.Neutral, null, null, 0);
				this.otherEntities.Add(prefab);
			}
		}

		public void ClearTemplatesInArea(int root_cell, CellOffset[] area)
		{
			foreach (CellOffset cellOffset in area)
			{
				this.ClearTemplatesInCell(Grid.OffsetCell(root_cell, cellOffset));
			}
		}

		public void ClearTemplatesInCell(int cell)
		{
			this.ClearCellFromCollection(cell, this.buildings);
			this.ClearCellFromCollection(cell, this.pickupables);
			this.ClearCellFromCollection(cell, this.elementalOres);
			this.ClearCellFromCollection(cell, this.otherEntities);
			for (int i = 0; i < this.preventFoWReveal.Count; i++)
			{
				if (this.preventFoWReveal[i].Key == Grid.CellToXY(cell))
				{
					this.preventFoWReveal.RemoveAt(i);
					i--;
				}
			}
		}

		private void ClearCellFromCollection(int checkCell, List<Prefab> collection)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (checkCell == Grid.XYToCell(collection[i].location_x, collection[i].location_y))
				{
					collection.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddTemplate(TemplateContainer template, Vector2I position)
		{
			CellOffset[] array = new CellOffset[template.cells.Count];
			for (int i = 0; i < template.cells.Count; i++)
			{
				array[i] = new CellOffset(template.cells[i].location_x, template.cells[i].location_y);
			}
			this.ClearTemplatesInArea(Grid.XYToCell(position.x, position.y), array);
			for (int j = 0; j < template.buildings.Count; j++)
			{
				this.buildings.Add((Prefab)template.buildings[j].Clone(position));
			}
			for (int k = 0; k < template.pickupables.Count; k++)
			{
				this.pickupables.Add((Prefab)template.pickupables[k].Clone(position));
			}
			for (int l = 0; l < template.elementalOres.Count; l++)
			{
				this.elementalOres.Add((Prefab)template.elementalOres[l].Clone(position));
			}
			for (int m = 0; m < template.otherEntities.Count; m++)
			{
				this.otherEntities.Add((Prefab)template.otherEntities[m].Clone(position));
			}
			for (int n = 0; n < template.cells.Count; n++)
			{
				this.preventFoWReveal.Add(new KeyValuePair<Vector2I, bool>(new Vector2I(position.x + template.cells[n].location_x, position.y + template.cells[n].location_y), template.cells[n].preventFoWReveal));
			}
		}

		public Vector2I baseStartPos;

		public List<Prefab> buildings = new List<Prefab>();

		public List<Prefab> pickupables = new List<Prefab>();

		public List<Prefab> elementalOres = new List<Prefab>();

		public List<Prefab> otherEntities = new List<Prefab>();

		public List<KeyValuePair<Vector2I, bool>> preventFoWReveal = new List<KeyValuePair<Vector2I, bool>>();
	}
}
