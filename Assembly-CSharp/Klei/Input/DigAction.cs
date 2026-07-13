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
			if (!Grid.ObjectLayers[1].ContainsKey(cell))
			{
				if (Grid.ObjectLayers[5].ContainsKey(cell))
				{
					GameObject gameObject = Grid.ObjectLayers[5][cell];
					if (gameObject == null)
					{
						return;
					}
					IDigActionEntity component = gameObject.GetComponent<IDigActionEntity>();
					this.EntityDig(component);
				}
				return;
			}
			GameObject gameObject2 = Grid.ObjectLayers[1][cell];
			if (gameObject2 == null)
			{
				return;
			}
			IDigActionEntity component2 = gameObject2.GetComponent<IDigActionEntity>();
			this.EntityDig(component2);
		}

		public abstract void Dig(int cell, int distFromOrigin);

		protected abstract void EntityDig(IDigActionEntity digAction);
	}
}
