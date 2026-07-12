using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using STRINGS;
using UnityEngine;

public class ClusterGrid
{
	public static void DestroyInstance()
	{
		ClusterGrid.Instance = null;
	}

	private ClusterFogOfWarManager.Instance GetFOWManager()
	{
		if (this.m_fowManager == null)
		{
			this.m_fowManager = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
		}
		return this.m_fowManager;
	}

	public bool IsValidCell(AxialI cell)
	{
		return this.cellContents.ContainsKey(cell);
	}

	public ClusterGrid(int numRings)
	{
		ClusterGrid.Instance = this;
		this.GenerateGrid(numRings);
		this.m_onClusterLocationChangedDelegate = new Action<object>(this.OnClusterLocationChanged);
	}

	public ClusterRevealLevel GetCellRevealLevel(AxialI cell)
	{
		return this.GetFOWManager().GetCellRevealLevel(cell);
	}

	public bool IsCellVisible(AxialI cell)
	{
		return this.GetFOWManager().IsLocationRevealed(cell);
	}

	public float GetRevealCompleteFraction(AxialI cell)
	{
		return this.GetFOWManager().GetRevealCompleteFraction(cell);
	}

	public bool IsVisible(ClusterGridEntity entity)
	{
		return entity.IsVisible && this.IsCellVisible(entity.Location);
	}

	public List<ClusterGridEntity> GetVisibleEntitiesAtCell(AxialI cell)
	{
		if (this.IsValidCell(cell) && this.GetFOWManager().IsLocationRevealed(cell))
		{
			return this.cellContents[cell].Where<ClusterGridEntity>((ClusterGridEntity entity) => entity.IsVisible).ToList<ClusterGridEntity>();
		}
		return new List<ClusterGridEntity>();
	}

	public ClusterGridEntity GetVisibleEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		return AxialUtil.GetRing(cell, 0).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetVisibleEntitiesAtCell)).FirstOrDefault<ClusterGridEntity>((ClusterGridEntity entity) => entity.Layer == entityLayer);
	}

	public ClusterGridEntity GetVisibleEntityOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return AxialUtil.GetRing(cell, 1).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetVisibleEntitiesAtCell)).FirstOrDefault<ClusterGridEntity>((ClusterGridEntity entity) => entity.Layer == entityLayer);
	}

	public List<ClusterGridEntity> GetHiddenEntitiesOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 0).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell))
			where entity.Layer == entityLayer
			select entity).ToList<ClusterGridEntity>();
	}

	public ClusterGridEntity GetEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
	{
		return AxialUtil.GetRing(cell, 0).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetEntitiesOnCell)).FirstOrDefault<ClusterGridEntity>((ClusterGridEntity entity) => entity.Layer == entityLayer);
	}

	public List<ClusterGridEntity> GetHiddenEntitiesAtCell(AxialI cell)
	{
		if (this.cellContents.ContainsKey(cell) && !this.GetFOWManager().IsLocationRevealed(cell))
		{
			return this.cellContents[cell].Where<ClusterGridEntity>((ClusterGridEntity entity) => entity.IsVisible).ToList<ClusterGridEntity>();
		}
		return new List<ClusterGridEntity>();
	}

	public List<ClusterGridEntity> GetNotVisibleEntitiesAtAdjacentCell(AxialI cell)
	{
		return AxialUtil.GetRing(cell, 1).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell)).ToList<ClusterGridEntity>();
	}

	public List<ClusterGridEntity> GetNotVisibleEntitiesOfLayerAtAdjacentCell(AxialI cell, EntityLayer entityLayer)
	{
		return (from entity in AxialUtil.GetRing(cell, 1).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell))
			where entity.Layer == entityLayer
			select entity).ToList<ClusterGridEntity>();
	}

	public ClusterGridEntity GetAsteroidAtCell(AxialI cell)
	{
		if (!this.cellContents.ContainsKey(cell))
		{
			return null;
		}
		return this.cellContents[cell].Where<ClusterGridEntity>((ClusterGridEntity e) => e.Layer == EntityLayer.Asteroid).FirstOrDefault<ClusterGridEntity>();
	}

	public bool HasVisibleAsteroidAtCell(AxialI cell)
	{
		return this.GetVisibleEntityOfLayerAtCell(cell, EntityLayer.Asteroid) != null;
	}

	public void RegisterEntity(ClusterGridEntity entity)
	{
		this.cellContents[entity.Location].Add(entity);
		entity.Subscribe(-1298331547, this.m_onClusterLocationChangedDelegate);
	}

	public void UnregisterEntity(ClusterGridEntity entity)
	{
		this.cellContents[entity.Location].Remove(entity);
		entity.Unsubscribe(-1298331547, this.m_onClusterLocationChangedDelegate);
	}

	public void OnClusterLocationChanged(object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		global::Debug.Assert(this.IsValidCell(clusterLocationChangedEvent.oldLocation), string.Format("ChangeEntityCell move order FROM invalid location: {0} {1}", clusterLocationChangedEvent.oldLocation, clusterLocationChangedEvent.entity));
		global::Debug.Assert(this.IsValidCell(clusterLocationChangedEvent.newLocation), string.Format("ChangeEntityCell move order TO invalid location: {0} {1}", clusterLocationChangedEvent.newLocation, clusterLocationChangedEvent.entity));
		this.cellContents[clusterLocationChangedEvent.oldLocation].Remove(clusterLocationChangedEvent.entity);
		this.cellContents[clusterLocationChangedEvent.newLocation].Add(clusterLocationChangedEvent.entity);
	}

	private AxialI GetNeighbor(AxialI cell, AxialI direction)
	{
		return cell + direction;
	}

	public int GetCellRing(AxialI cell)
	{
		Vector3I vector3I = cell.ToCube();
		Vector3I vector3I2 = new Vector3I(vector3I.x, vector3I.y, vector3I.z);
		Vector3I vector3I3 = new Vector3I(0, 0, 0);
		return (int)((float)((Mathf.Abs(vector3I2.x - vector3I3.x) + Mathf.Abs(vector3I2.y - vector3I3.y) + Mathf.Abs(vector3I2.z - vector3I3.z)) / 2));
	}

	private void CleanUpGrid()
	{
		this.cellContents.Clear();
	}

	private int GetHexDistance(AxialI a, AxialI b)
	{
		Vector3I vector3I = a.ToCube();
		Vector3I vector3I2 = b.ToCube();
		return Mathf.Max(new int[]
		{
			Mathf.Abs(vector3I.x - vector3I2.x),
			Mathf.Abs(vector3I.y - vector3I2.y),
			Mathf.Abs(vector3I.z - vector3I2.z)
		});
	}

	public List<ClusterGridEntity> GetEntitiesInRange(AxialI center, int range = 1)
	{
		List<ClusterGridEntity> list = new List<ClusterGridEntity>();
		foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> keyValuePair in this.cellContents)
		{
			if (this.GetHexDistance(keyValuePair.Key, center) <= range)
			{
				list.AddRange(keyValuePair.Value);
			}
		}
		return list;
	}

	public List<ClusterGridEntity> GetEntitiesOnCell(AxialI cell)
	{
		return this.cellContents[cell];
	}

	public bool IsInRange(AxialI a, AxialI b, int range = 1)
	{
		return this.GetHexDistance(a, b) <= range;
	}

	private void GenerateGrid(int rings)
	{
		this.CleanUpGrid();
		this.numRings = rings;
		for (int i = -rings + 1; i < rings; i++)
		{
			for (int j = -rings + 1; j < rings; j++)
			{
				for (int k = -rings + 1; k < rings; k++)
				{
					if (i + j + k == 0)
					{
						AxialI axialI = new AxialI(i, j);
						this.cellContents.Add(axialI, new List<ClusterGridEntity>());
					}
				}
			}
		}
	}

	public Vector3 GetPosition(ClusterGridEntity entity)
	{
		float num = (float)entity.Location.R;
		float num2 = (float)entity.Location.Q;
		List<ClusterGridEntity> list = this.cellContents[entity.Location];
		if (list.Count <= 1 || !entity.SpaceOutInSameHex())
		{
			return AxialUtil.AxialToWorld(num, num2);
		}
		int num3 = 0;
		int num4 = 0;
		foreach (ClusterGridEntity clusterGridEntity in list)
		{
			if (entity == clusterGridEntity)
			{
				num3 = num4;
			}
			if (clusterGridEntity.SpaceOutInSameHex())
			{
				num4++;
			}
		}
		if (list.Count > num4)
		{
			num4 += 5;
			num3 += 5;
		}
		else if (num4 > 0)
		{
			num4++;
			num3++;
		}
		if (num4 == 0 || num4 == 1)
		{
			return AxialUtil.AxialToWorld(num, num2);
		}
		float num5 = Mathf.Min(Mathf.Pow((float)num4, 0.5f), 1f) * 0.5f;
		float num6 = Mathf.Pow((float)num3 / (float)num4, 0.5f);
		float num7 = 0.81f;
		float num8 = Mathf.Pow((float)num4, 0.5f) * num7;
		float num9 = 6.2831855f * num8 * num6;
		float num10 = Mathf.Cos(num9) * num5 * num6;
		float num11 = Mathf.Sin(num9) * num5 * num6;
		return AxialUtil.AxialToWorld(num, num2) + new Vector3(num10, num11, 0f);
	}

	public List<AxialI> GetPath(AxialI start, AxialI end, ClusterDestinationSelector destination_selector)
	{
		string text;
		return this.GetPath(start, end, destination_selector, out text);
	}

	public List<AxialI> GetPath(AxialI start, AxialI end, ClusterDestinationSelector destination_selector, out string fail_reason)
	{
		ClusterGrid.<>c__DisplayClass38_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.destination_selector = destination_selector;
		CS$<>8__locals1.start = start;
		CS$<>8__locals1.end = end;
		fail_reason = null;
		if (!CS$<>8__locals1.destination_selector.canNavigateFogOfWar && !this.IsCellVisible(CS$<>8__locals1.end))
		{
			fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_FOG_OF_WAR;
			return null;
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = this.GetVisibleEntityOfLayerAtCell(CS$<>8__locals1.end, EntityLayer.Asteroid);
		if (visibleEntityOfLayerAtCell != null && CS$<>8__locals1.destination_selector.requireLaunchPadOnAsteroidDestination)
		{
			bool flag = false;
			using (IEnumerator enumerator = Components.LaunchPads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((LaunchPad)enumerator.Current).GetMyWorldLocation() == visibleEntityOfLayerAtCell.Location)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_LAUNCH_PAD;
				return null;
			}
		}
		if (visibleEntityOfLayerAtCell == null && CS$<>8__locals1.destination_selector.requireAsteroidDestination)
		{
			fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_REQUIRE_ASTEROID;
			return null;
		}
		CS$<>8__locals1.frontier = new HashSet<AxialI>();
		CS$<>8__locals1.visited = new HashSet<AxialI>();
		CS$<>8__locals1.buffer = new HashSet<AxialI>();
		CS$<>8__locals1.cameFrom = new Dictionary<AxialI, AxialI>();
		CS$<>8__locals1.frontier.Add(CS$<>8__locals1.start);
		while (!CS$<>8__locals1.frontier.Contains(CS$<>8__locals1.end) && CS$<>8__locals1.frontier.Count > 0)
		{
			this.<GetPath>g__ExpandFrontier|38_0(ref CS$<>8__locals1);
		}
		if (CS$<>8__locals1.frontier.Contains(CS$<>8__locals1.end))
		{
			List<AxialI> list = new List<AxialI>();
			AxialI axialI = CS$<>8__locals1.end;
			while (axialI != CS$<>8__locals1.start)
			{
				list.Add(axialI);
				axialI = CS$<>8__locals1.cameFrom[axialI];
			}
			list.Reverse();
			return list;
		}
		fail_reason = UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_PATH;
		return null;
	}

	public void GetLocationDescription(AxialI location, out Sprite sprite, out string label, out string sublabel)
	{
		ClusterGridEntity clusterGridEntity = this.GetVisibleEntitiesAtCell(location).Find((ClusterGridEntity x) => x.Layer == EntityLayer.Asteroid);
		ClusterGridEntity visibleEntityOfLayerAtAdjacentCell = this.GetVisibleEntityOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
		if (clusterGridEntity != null)
		{
			sprite = clusterGridEntity.GetUISprite();
			label = clusterGridEntity.Name;
			WorldContainer component = clusterGridEntity.GetComponent<WorldContainer>();
			sublabel = Strings.Get(component.worldType);
			return;
		}
		if (visibleEntityOfLayerAtAdjacentCell != null)
		{
			sprite = visibleEntityOfLayerAtAdjacentCell.GetUISprite();
			label = UI.SPACEDESTINATIONS.ORBIT.NAME_FMT.Replace("{Name}", visibleEntityOfLayerAtAdjacentCell.Name);
			WorldContainer component2 = visibleEntityOfLayerAtAdjacentCell.GetComponent<WorldContainer>();
			sublabel = Strings.Get(component2.worldType);
			return;
		}
		if (this.IsCellVisible(location))
		{
			sprite = Assets.GetSprite("hex_unknown");
			label = UI.SPACEDESTINATIONS.EMPTY_SPACE.NAME;
			sublabel = "";
			return;
		}
		sprite = Assets.GetSprite("unknown_far");
		label = UI.SPACEDESTINATIONS.FOG_OF_WAR_SPACE.NAME;
		sublabel = "";
	}

	[CompilerGenerated]
	private void <GetPath>g__ExpandFrontier|38_0(ref ClusterGrid.<>c__DisplayClass38_0 A_1)
	{
		A_1.buffer.Clear();
		foreach (AxialI axialI in A_1.frontier)
		{
			foreach (AxialI axialI2 in AxialI.DIRECTIONS)
			{
				AxialI neighbor = this.GetNeighbor(axialI, axialI2);
				if (!A_1.visited.Contains(neighbor) && this.IsValidCell(neighbor) && (this.IsCellVisible(neighbor) || A_1.destination_selector.canNavigateFogOfWar) && (!this.HasVisibleAsteroidAtCell(neighbor) || !(neighbor != A_1.start) || !(neighbor != A_1.end)))
				{
					A_1.buffer.Add(neighbor);
					if (!A_1.cameFrom.ContainsKey(neighbor))
					{
						A_1.cameFrom.Add(neighbor, axialI);
					}
				}
			}
			A_1.visited.Add(axialI);
		}
		HashSet<AxialI> frontier = A_1.frontier;
		A_1.frontier = A_1.buffer;
		A_1.buffer = frontier;
	}

	public static ClusterGrid Instance;

	public const float NodeDistanceScale = 600f;

	private const float MAX_OFFSET_RADIUS = 0.5f;

	public int numRings;

	private ClusterFogOfWarManager.Instance m_fowManager;

	private Action<object> m_onClusterLocationChangedDelegate;

	public Dictionary<AxialI, List<ClusterGridEntity>> cellContents = new Dictionary<AxialI, List<ClusterGridEntity>>();
}
