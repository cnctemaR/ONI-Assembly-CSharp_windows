using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RadiationGermSpawner")]
public class RadiationGermSpawner : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.cellRatio = (float)(Grid.CellCount / 1024);
		this.disease_idx = byte.MaxValue;
	}

	private void Update()
	{
	}

	private void EvaluateRadiation()
	{
		for (int i = 0; i < 1024; i++)
		{
			int num = (this.nextEvaluatedCell + i) % Grid.CellCount;
			if (Grid.RadiationCount[num] >= 0)
			{
				int num2 = Mathf.RoundToInt((float)Grid.RadiationCount[num] * 100f * (Time.deltaTime * this.cellRatio));
				if (Grid.DiseaseIdx[num] == this.disease_idx)
				{
					SimMessages.ModifyDiseaseOnCell(num, this.disease_idx, num2);
				}
				else
				{
					int num3 = Grid.DiseaseCount[num] - num2;
					if (num3 < 0)
					{
						SimMessages.ModifyDiseaseOnCell(num, this.disease_idx, num3);
					}
					else
					{
						SimMessages.ModifyDiseaseOnCell(num, Grid.DiseaseIdx[num], -num2);
					}
				}
			}
		}
		this.nextEvaluatedCell = (this.nextEvaluatedCell + 1024) % Grid.CellCount;
	}

	private const float GERM_SCALE = 100f;

	private const int CELLS_PER_UPDATE = 1024;

	private int nextEvaluatedCell;

	private float cellRatio;

	private byte disease_idx;
}
