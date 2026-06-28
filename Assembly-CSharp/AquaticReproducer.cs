using System;
using UnityEngine;

public class AquaticReproducer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.timeUntilNextSpawn = this.cycleLength;
		this.handle = GameScheduler.Instance.SchedulePeriodic("AquaticReproducer", this.UpdateFrequency, new Action<object>(this.CheckReproduce), null, null, 0f, null);
	}

	protected override void OnCleanUp()
	{
		this.handle.ClearScheduler();
		base.OnCleanUp();
	}

	private void CheckReproduce(object data)
	{
		this.timeUntilNextSpawn -= this.UpdateFrequency;
		if (this.timeUntilNextSpawn <= 0f)
		{
			this.Reproduce();
			this.timeUntilNextSpawn += this.cycleLength + global::UnityEngine.Random.Range(-this.randomCycleOffset, this.randomCycleOffset);
		}
	}

	private void Reproduce()
	{
		BodyOfWater bodyContainedIn = WaterBodyProbe.Instance.GetBodyContainedIn(base.gameObject);
		if (bodyContainedIn == null)
		{
			return;
		}
		if ((float)bodyContainedIn.containedObjects.Count >= (float)bodyContainedIn.waterCells.Count * this.maxmimumDensity)
		{
			return;
		}
		if (!this.SpawnEgg)
		{
			Tag prefabTag = base.GetComponent<KPrefabID>().PrefabTag;
			int num = -1;
			int num2 = 0;
			while (num == -1)
			{
				int num3 = bodyContainedIn.waterCells[global::UnityEngine.Random.Range(0, bodyContainedIn.waterCells.Count)];
				foreach (GameObject gameObject in bodyContainedIn.containedObjects)
				{
					if (Grid.PosToCell(gameObject) != num3 && Grid.IsSubstantialLiquid(num3, 0.35f) && Grid.IsSubstantialLiquid(Grid.CellDownRight(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellDownLeft(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellUpRight(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellUpLeft(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellAbove(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellBelow(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellLeft(num3), 0.35f) && Grid.IsSubstantialLiquid(Grid.CellRight(num3), 0.35f))
					{
						num = num3;
						break;
					}
				}
				num2++;
				if (num2 >= 10)
				{
					return;
				}
			}
			GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab(prefabTag), Grid.CellToPos(num), Quaternion.identity, null, null, true, 0);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, gameObject2.GetComponent<KPrefabID>().GetProperName(), gameObject2.transform, 1.5f, false);
			gameObject2.SetActive(true);
		}
		else
		{
			base.Trigger(230069070, null);
		}
	}

	public float cycleLength = 600f;

	public float randomCycleOffset = 100f;

	public float maxmimumDensity = 0.12f;

	private float timeUntilNextSpawn;

	public bool SpawnEgg;

	public Tag EggPrefabTag;

	private float UpdateFrequency = 1f;

	private SchedulerHandle handle;
}
