using System;
using Klei.Actions;
using UnityEngine;

namespace Klei.Input
{
	[ActionType("InterfaceTool", "Dig", true)]
	public abstract class DigAction
	{
		public void Uproot(int cell)
		{
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			int num;
			int num2;
			Grid.CellToXY(cell, out num, out num2);
			GameScenePartitioner.Instance.GatherEntries(num, num2, 1, 1, GameScenePartitioner.Instance.plants, pooledList);
			if (pooledList.Count > 0)
			{
				this.EntityDig((pooledList[0].obj as Component).GetComponent<IDigActionEntity>());
			}
			pooledList.Recycle();
		}

		public abstract void Dig(int cell, int distFromOrigin);

		protected abstract void EntityDig(IDigActionEntity digAction);
	}
}
