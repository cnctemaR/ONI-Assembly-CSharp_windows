using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LiquidHeaterBubbleEmitter : KMonoBehaviour, ISim1000ms
{
	private float BubbleHeatFraction
	{
		get
		{
			return Mathf.Lerp(0.001f, 0.01f, this.spaceHeater.UserSliderSetting);
		}
	}

	public float DivertEnergy(float exhaustKW, float dt)
	{
		if (this.spaceHeater.CurrentPowerConsumption < this.BubblePowerThreshold)
		{
			this.accruedEnergyKJ = 0f;
			return 0f;
		}
		float num = exhaustKW * this.BubbleHeatFraction;
		this.accruedEnergyKJ += num * dt;
		return num;
	}

	public void Sim1000ms(float dt)
	{
		if (this.accruedEnergyKJ <= 0f)
		{
			return;
		}
		int num = this.building.PlacementCells.Length;
		float num2 = this.accruedEnergyKJ / (float)num;
		float num3 = 0f;
		foreach (int num4 in this.building.PlacementCells)
		{
			Element element = Grid.Element[num4];
			if (element.IsLiquid && element.HasTransitionUp)
			{
				float num5 = Grid.Mass[num4];
				if (num5 > 0f)
				{
					float num6 = Grid.Temperature[num4];
					float num7 = element.highTemp + 3f;
					float num8 = num7 - num6;
					if (num8 > 0f)
					{
						float num9 = element.specificHeatCapacity * num8;
						float num10 = num2 / num9;
						if (num10 >= 0.0020000022f)
						{
							float num11 = Mathf.Min(num10, num5);
							float num12 = num11;
							byte b = Grid.DiseaseIdx[num4];
							DebugUtil.DevAssert(num5 > 0f, "Cell mass is zero or negative, cannot scale disease count.", null);
							int num13 = (int)((float)Grid.DiseaseCount[num4] * (num11 / num5));
							int num14 = num13;
							Vector2 vector = Grid.CellToPosCCC(num4, Grid.SceneLayer.Front);
							if (this.TryEmitOreByproduct(element, num11, ref num12, b, num13, ref num14, num4, num6, vector) != LiquidHeaterBubbleEmitter.EmitResult.InsufficientMass)
							{
								num3 += num11 * num9;
								SimMessages.AddRemoveSubstance(num4, element.id, CellEventLogger.Instance.ElementEmitted, -num11, num6, b, -num14, true, -1);
								BubbleManager.Disease disease = new BubbleManager.Disease
								{
									Idx = b,
									Count = num14
								};
								BubbleManager.instance.SpawnBubble(element.highTempTransitionTarget, vector, num12, num7, disease, null);
							}
						}
					}
				}
			}
		}
		this.accruedEnergyKJ = Mathf.Max(this.accruedEnergyKJ - num3, 0f);
	}

	private LiquidHeaterBubbleEmitter.EmitResult TryEmitOreByproduct(Element sourceElement, float boiledMass, ref float bubbleMass, byte diseaseIdx, int diseaseCount, ref int bubbleDiseaseCount, int cell, float cellTemp, Vector2 bubblePosition)
	{
		SimHashes highTempTransitionOreID = sourceElement.highTempTransitionOreID;
		if (highTempTransitionOreID == (SimHashes)0)
		{
			return LiquidHeaterBubbleEmitter.EmitResult.None;
		}
		float highTempTransitionOreMassConversion = sourceElement.highTempTransitionOreMassConversion;
		if (highTempTransitionOreMassConversion <= 0f)
		{
			return LiquidHeaterBubbleEmitter.EmitResult.None;
		}
		float num = boiledMass * highTempTransitionOreMassConversion;
		if (num < 0.001f)
		{
			return LiquidHeaterBubbleEmitter.EmitResult.InsufficientMass;
		}
		int num2 = (int)((float)diseaseCount * highTempTransitionOreMassConversion);
		bubbleMass -= num;
		bubbleDiseaseCount -= num2;
		Element element = ElementLoader.FindElementByHash(highTempTransitionOreID);
		if (element.IsSolid)
		{
			element.substance.SpawnResource(bubblePosition, num, cellTemp, diseaseIdx, num2, false, false, false);
		}
		else
		{
			SimMessages.AddRemoveSubstance(cell, highTempTransitionOreID, CellEventLogger.Instance.ElementEmitted, num, cellTemp, diseaseIdx, num2, true, -1);
		}
		return LiquidHeaterBubbleEmitter.EmitResult.Emitted;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private SpaceHeater spaceHeater;

	private const float MIN_BUBBLE_HEAT_FRACTION = 0.001f;

	private const float MAX_BUBBLE_HEAT_FRACTION = 0.01f;

	private const float MIN_EMIT_MASS = 0.0020000022f;

	public float BubblePowerThreshold;

	[Serialize]
	private float accruedEnergyKJ;

	private enum EmitResult
	{
		None,
		InsufficientMass,
		Emitted
	}
}
