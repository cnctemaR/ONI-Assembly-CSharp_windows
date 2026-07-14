using System;
using UnityEngine;

public static class BackwallManager
{
	public unsafe static void UpdateFromSim(Sim.GameDataUpdate* data)
	{
		BackwallManager.element_idx = data->backwallElement;
		BackwallManager.mass = data->backwallMass;
		BackwallManager.temperature = data->backwallTemperature;
		for (int i = 0; i < data->numBackwallShouldTransitionInfos; i++)
		{
			int gameCell = data->backwallShouldTransitionInfos[i].gameCell;
			Element element = BackwallManager.At(gameCell).Element;
			if (element != null && !element.IsVacuum)
			{
				ushort num = ushort.MaxValue;
				float num2 = 0f;
				bool flag = false;
				float num3;
				ushort num4;
				float num5;
				bool flag2;
				if (BackwallManager.At(gameCell).Temperature > element.highTemp)
				{
					num3 = BackwallManager.At(gameCell).Temperature - 1.5f;
					num4 = element.highTempTransition.idx;
					num5 = (1f - element.highTempTransitionOreMassConversion) * BackwallManager.At(gameCell).Mass;
					flag2 = element.highTempTransition.IsSolid;
					if (element.highTempTransitionOreID != (SimHashes)0)
					{
						Element element2 = ElementLoader.FindElementByHash(element.highTempTransitionOreID);
						num = element2.idx;
						flag = element2.IsSolid;
						num2 = BackwallManager.At(gameCell).Mass - num5;
					}
				}
				else
				{
					if (BackwallManager.At(gameCell).Temperature >= element.lowTemp)
					{
						goto IL_0267;
					}
					num3 = BackwallManager.At(gameCell).Temperature + 1.5f;
					num4 = element.lowTempTransition.idx;
					num5 = (1f - element.lowTempTransitionOreMassConversion) * BackwallManager.At(gameCell).Mass;
					flag2 = element.lowTempTransition.IsSolid;
					if (element.lowTempTransitionOreID != (SimHashes)0)
					{
						Element element3 = ElementLoader.FindElementByHash(element.lowTempTransitionOreID);
						num = element3.idx;
						flag = element3.IsSolid;
						num2 = BackwallManager.At(gameCell).Mass - num5;
					}
				}
				if (flag2)
				{
					SimMessages.SetBackwallData(gameCell, num4, num5, num3);
				}
				else
				{
					SimMessages.SetBackwallData(gameCell, ElementLoader.GetElementIndex(SimHashes.Vacuum), 0f, 0f);
					SimMessages.AddRemoveSubstance(gameCell, num4, CellEventLogger.Instance.OreMelted, num5, num3, byte.MaxValue, 0, true, -1);
				}
				if (num2 > 0.001f)
				{
					if (flag)
					{
						Element element4 = ElementLoader.elements[(int)num];
						GameObject gameObject = element4.substance.SpawnResource(Grid.CellToPos(gameCell), num2, num3, byte.MaxValue, 0, true, false, true);
						element4.substance.ActivateSubstanceGameObject(gameObject, byte.MaxValue, 0);
					}
					else
					{
						SimMessages.AddRemoveSubstance(gameCell, num, CellEventLogger.Instance.OreMelted, num2, num3, byte.MaxValue, 0, true, -1);
					}
				}
			}
			IL_0267:;
		}
	}

	public static bool HasBackwall(int cell)
	{
		Element element = BackwallManager.At(cell).Element;
		return element != null && element.IsSolid;
	}

	public static BackwallManager.BackwallIndexer At(int index)
	{
		return new BackwallManager.BackwallIndexer(index);
	}

	public static void Clear()
	{
		BackwallManager.element_idx = null;
		BackwallManager.mass = null;
		BackwallManager.temperature = null;
	}

	private unsafe static ushort* element_idx;

	private unsafe static float* mass;

	private unsafe static float* temperature;

	public readonly struct BackwallIndexer
	{
		public BackwallIndexer(int index)
		{
			this.index = index;
		}

		public unsafe Element Element
		{
			get
			{
				if (BackwallManager.element_idx[this.index] == 65535)
				{
					return null;
				}
				return ElementLoader.elements[(int)BackwallManager.element_idx[this.index]];
			}
		}

		public unsafe float Mass
		{
			get
			{
				return BackwallManager.mass[this.index];
			}
		}

		public unsafe float Temperature
		{
			get
			{
				return BackwallManager.temperature[this.index];
			}
		}

		public readonly int index;
	}
}
