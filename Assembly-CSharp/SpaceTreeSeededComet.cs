using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTreeSeededComet : Comet
{
	protected override void DepositTiles(int cell, Element element, int world, int prev_cell, float temperature)
	{
		SpaceTreeSeededComet.<>c__DisplayClass0_0 CS$<>8__locals1 = new SpaceTreeSeededComet.<>c__DisplayClass0_0();
		CS$<>8__locals1.world = world;
		CS$<>8__locals1.<>4__this = this;
		float depthOfElement = (float)base.GetDepthOfElement(cell, element, CS$<>8__locals1.world);
		float num = 1f;
		float num2 = (depthOfElement - (float)this.addTilesMinHeight) / (float)(this.addTilesMaxHeight - this.addTilesMinHeight);
		if (!float.IsNaN(num2))
		{
			num -= num2;
		}
		int num3 = Mathf.Min(this.addTiles, Mathf.Clamp(Mathf.RoundToInt((float)this.addTiles * num), 1, this.addTiles));
		ListPool<int, Comet>.PooledList pooledList = ListPool<int, Comet>.Allocate();
		FloodFill.BreadthCollect(prev_cell, new Func<int, FloodFill.BoundaryCheckResult>(CS$<>8__locals1.<DepositTiles>g__condition|0), pooledList, 11);
		float num4 = ((num3 > 0) ? (this.addTileMass / (float)this.addTiles) : 1f);
		int num5 = this.addDiseaseCount / num3;
		float value = global::UnityEngine.Random.value;
		float num6 = ((num3 == 0) ? (-1f) : (1f / (float)num3));
		float num7 = 0f;
		bool flag = false;
		using (List<int>.Enumerator enumerator = pooledList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int viable_cell = enumerator.Current;
				if (num3 <= 0)
				{
					break;
				}
				num7 += num6;
				bool flag2 = !flag && num6 >= 0f && value <= num7;
				int num8 = (flag2 ? Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
				{
					SpaceTreeSeededComet.PlantTreeOnSolidTileCreated(viable_cell, CS$<>8__locals1.<>4__this.addTilesMaxHeight);
				}, false)).index : (-1));
				SimMessages.AddRemoveSubstance(viable_cell, element.id, CellEventLogger.Instance.ElementEmitted, num4, temperature, this.diseaseIdx, num5, true, num8);
				num3--;
				flag = flag || flag2;
			}
		}
		pooledList.Recycle();
	}

	private static void PlantTreeOnSolidTileCreated(int cell, int tileMaxHeight)
	{
		byte b = Grid.WorldIdx[cell];
		int num = 2;
		int num2 = Grid.OffsetCell(cell, new CellOffset(0, tileMaxHeight));
		int num3 = num2;
		bool flag = false;
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		for (;;)
		{
			num2 = num3;
			num3 = Grid.OffsetCell(num2, 0, -1);
			if (!Grid.IsValidCell(num3))
			{
				break;
			}
			if (Grid.Solid[num3] && SpaceTreeSeededComet.CanGrowOnCell(num2, b))
			{
				flag = true;
			}
			num--;
			if (flag || num <= 0)
			{
				goto IL_005F;
			}
		}
		return;
		IL_005F:
		if (flag)
		{
			GameObject prefab = Assets.GetPrefab("SpaceTree");
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			Vector3 vector = Grid.CellToPosCBC(num2, component.sceneLayer);
			Util.KInstantiate(prefab, vector).SetActive(true);
		}
	}

	public static bool CanGrowOnCell(int spawnCell, byte worldIdx)
	{
		CellOffset[] occupiedCellsOffsets = Assets.GetPrefab("SpaceTree").GetComponent<OccupyArea>().OccupiedCellsOffsets;
		bool flag = true;
		int num = 0;
		while (flag && num < occupiedCellsOffsets.Length)
		{
			int num2 = Grid.OffsetCell(spawnCell, occupiedCellsOffsets[num]);
			flag = flag && Grid.IsValidCellInWorld(num2, (int)worldIdx);
			flag = flag && (!Grid.IsSolidCell(num2) || Grid.Element[num2].HasTag(GameTags.Unstable));
			flag = flag && Grid.Objects[num2, 1] == null;
			flag = flag && Grid.Objects[num2, 5] == null;
			flag = flag && !Grid.Foundation[num2];
			num++;
		}
		return flag;
	}
}
