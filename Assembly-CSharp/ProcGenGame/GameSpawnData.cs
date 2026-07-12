using System;
using System.Collections.Generic;
using Klei.CustomSettings;
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
				Vector2I vector2I = Grid.CellToXY(keyValuePair.Key);
				Prefab prefab = new Prefab(keyValuePair.Value, Prefab.Type.Other, vector2I.x, vector2I.y, (SimHashes)0, -1f, 1f, null, 0, Orientation.Neutral, null, null, 0, null);
				this.otherEntities.Add(prefab);
			}
		}

		public void AddTemplate(TemplateContainer template, Vector2I position, ref Dictionary<int, int> claimedCells)
		{
			int num = Grid.XYToCell(position.x, position.y);
			bool flag = true;
			if (DlcManager.IsExpansion1Active() && CustomGameSettings.Instance != null)
			{
				flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Teleporters).id == "Enabled";
			}
			if (template.buildings != null)
			{
				foreach (Prefab prefab in template.buildings)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(num, prefab.location_x, prefab.location_y)) && (flag || !this.IsWarpTeleporter(prefab)))
					{
						this.buildings.Add(prefab.Clone(position));
					}
				}
			}
			if (template.pickupables != null)
			{
				foreach (Prefab prefab2 in template.pickupables)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(num, prefab2.location_x, prefab2.location_y)))
					{
						this.pickupables.Add(prefab2.Clone(position));
					}
				}
			}
			if (template.elementalOres != null)
			{
				foreach (Prefab prefab3 in template.elementalOres)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(num, prefab3.location_x, prefab3.location_y)))
					{
						this.elementalOres.Add(prefab3.Clone(position));
					}
				}
			}
			if (template.otherEntities != null)
			{
				foreach (Prefab prefab4 in template.otherEntities)
				{
					if (!claimedCells.ContainsKey(Grid.OffsetCell(num, prefab4.location_x, prefab4.location_y)) && (flag || !this.IsWarpTeleporter(prefab4)))
					{
						this.otherEntities.Add(prefab4.Clone(position));
					}
				}
			}
			if (template.cells != null)
			{
				for (int i = 0; i < template.cells.Count; i++)
				{
					int num2 = Grid.XYToCell(position.x + template.cells[i].location_x, position.y + template.cells[i].location_y);
					if (!claimedCells.ContainsKey(num2))
					{
						claimedCells[num2] = 1;
						this.preventFoWReveal.Add(new KeyValuePair<Vector2I, bool>(new Vector2I(position.x + template.cells[i].location_x, position.y + template.cells[i].location_y), template.cells[i].preventFoWReveal));
					}
					else
					{
						Dictionary<int, int> dictionary = claimedCells;
						int j = num2;
						dictionary[j]++;
					}
				}
			}
			if (template.info != null && template.info.discover_tags != null)
			{
				foreach (Tag tag in template.info.discover_tags)
				{
					this.discoveredResources.Add(tag);
				}
			}
		}

		private bool IsWarpTeleporter(Prefab prefab)
		{
			return prefab.id == "WarpPortal" || prefab.id == WarpReceiverConfig.ID || prefab.id == "WarpConduitSender" || prefab.id == "WarpConduitReceiver";
		}

		public Vector2I baseStartPos;

		public List<Prefab> buildings = new List<Prefab>();

		public List<Prefab> pickupables = new List<Prefab>();

		public List<Prefab> elementalOres = new List<Prefab>();

		public List<Prefab> otherEntities = new List<Prefab>();

		public List<Tag> discoveredResources = new List<Tag>();

		public List<KeyValuePair<Vector2I, bool>> preventFoWReveal = new List<KeyValuePair<Vector2I, bool>>();
	}
}
