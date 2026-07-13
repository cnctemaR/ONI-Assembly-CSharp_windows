using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct PlantElementAbsorber
{
	public void Clear()
	{
		this.storage = null;
		this.consumedElements = null;
	}

	private static bool CheckPreConditions(Storage storage, float dt)
	{
		if (storage == null)
		{
			return false;
		}
		if (dt <= 0f)
		{
			DebugUtil.DevLogError("A delta time of 0 will produce degenerate consumeCommands.");
			return false;
		}
		return true;
	}

	public static bool PlanConsume(Storage storage, PlantElementAbsorber.LocalInfo localInfo, float dt, List<PlantElementAbsorber.Planner.ConsumeCommand> consumeCommands)
	{
		if (!PlantElementAbsorber.CheckPreConditions(storage, dt))
		{
			return false;
		}
		PlantElementAbsorber.Planner planner = new PlantElementAbsorber.Planner(storage);
		bool flag = planner.PlanConsume(localInfo.massConsumptionRate * dt, localInfo.tag, consumeCommands);
		planner.Recycle();
		return flag;
	}

	public static bool PlanConsume(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumedElements, float dt, List<PlantElementAbsorber.Planner.ConsumeCommand> consumeCommands)
	{
		if (!PlantElementAbsorber.CheckPreConditions(storage, dt))
		{
			return false;
		}
		PlantElementAbsorber.<>c__DisplayClass9_0 CS$<>8__locals1;
		CS$<>8__locals1.planner = new PlantElementAbsorber.Planner(storage);
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in consumedElements)
		{
			if (consumeCommands != null)
			{
				consumeCommands.Clear();
			}
			if (CS$<>8__locals1.planner.PlanConsume(consumeInfo.massConsumptionRate * dt, consumeInfo.tag, consumeCommands))
			{
				return PlantElementAbsorber.<PlanConsume>g__OnExit|9_0(true, ref CS$<>8__locals1);
			}
		}
		return PlantElementAbsorber.<PlanConsume>g__OnExit|9_0(false, ref CS$<>8__locals1);
	}

	public readonly bool PlanConsume(float dt, List<PlantElementAbsorber.Planner.ConsumeCommand> consumeCommands)
	{
		if (this.consumedElements != null)
		{
			return PlantElementAbsorber.PlanConsume(this.storage, this.consumedElements, dt, consumeCommands);
		}
		return PlantElementAbsorber.PlanConsume(this.storage, this.localInfo, dt, consumeCommands);
	}

	public static float FindLargest(Storage storage, PlantElementAbsorber.ConsumeInfo[] consumedElements)
	{
		if (storage == null)
		{
			return 0f;
		}
		if (consumedElements.Length == 0)
		{
			return 0f;
		}
		PlantElementAbsorber.Planner planner = new PlantElementAbsorber.Planner(storage);
		float num = -1f;
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in consumedElements)
		{
			float num2 = planner.Sum(consumeInfo.tag);
			if (num == -1f || num2 > num)
			{
				num = num2;
			}
		}
		planner.Recycle();
		return num;
	}

	public readonly float FindLargest()
	{
		if (this.consumedElements != null)
		{
			return PlantElementAbsorber.FindLargest(this.storage, this.consumedElements);
		}
		if (this.storage == null)
		{
			return 0f;
		}
		PlantElementAbsorber.Planner planner = new PlantElementAbsorber.Planner(this.storage);
		float num = planner.Sum(this.localInfo.tag);
		planner.Recycle();
		return num;
	}

	[CompilerGenerated]
	internal static bool <PlanConsume>g__OnExit|9_0(bool result, ref PlantElementAbsorber.<>c__DisplayClass9_0 A_1)
	{
		A_1.planner.Recycle();
		return result;
	}

	public Storage storage;

	public PlantElementAbsorber.LocalInfo localInfo;

	public PlantElementAbsorber.ConsumeInfo[] consumedElements;

	public struct ConsumeInfo
	{
		public ConsumeInfo(Tag tag, float mass_consumption_rate)
		{
			this.tag = tag;
			this.massConsumptionRate = mass_consumption_rate;
		}

		public Tag tag;

		public float massConsumptionRate;
	}

	public struct LocalInfo
	{
		public Tag tag;

		public float massConsumptionRate;
	}

	public readonly struct Planner
	{
		public Planner(Storage storage)
		{
			this.prefabIds = ListPool<KPrefabID, PlantElementAbsorber.Planner>.Allocate();
			this.primaryElements = ListPool<PrimaryElement, PlantElementAbsorber.Planner>.Allocate();
			DebugUtil.DevAssert(storage != null, "Initializing Planner with a null Storage.", null);
			if (storage == null)
			{
				return;
			}
			foreach (GameObject gameObject in storage.items)
			{
				KPrefabID kprefabID;
				PrimaryElement primaryElement;
				if (gameObject.TryGetComponent<KPrefabID>(out kprefabID) && gameObject.TryGetComponent<PrimaryElement>(out primaryElement))
				{
					this.prefabIds.Add(kprefabID);
					this.primaryElements.Add(primaryElement);
				}
			}
		}

		public bool PlanConsume(float requiredMass, Tag tag, List<PlantElementAbsorber.Planner.ConsumeCommand> consumeCommands)
		{
			for (int num = 0; num != this.prefabIds.Count; num++)
			{
				if (this.prefabIds[num].HasTag(tag))
				{
					PrimaryElement primaryElement = this.primaryElements[num];
					float num2 = Mathf.Min(requiredMass, primaryElement.Mass);
					requiredMass -= num2;
					if (consumeCommands != null)
					{
						consumeCommands.Add(new PlantElementAbsorber.Planner.ConsumeCommand
						{
							primaryElement = primaryElement,
							deltaMass = num2
						});
					}
					if (requiredMass <= 0f)
					{
						return true;
					}
				}
			}
			return false;
		}

		public float Sum(Tag tag)
		{
			float num = 0f;
			for (int num2 = 0; num2 != this.prefabIds.Count; num2++)
			{
				if (this.prefabIds[num2].HasTag(tag))
				{
					num += this.primaryElements[num2].Mass;
				}
			}
			return num;
		}

		public void Recycle()
		{
			this.prefabIds.Recycle();
			this.primaryElements.Recycle();
		}

		private readonly ListPool<KPrefabID, PlantElementAbsorber.Planner>.PooledList prefabIds;

		private readonly ListPool<PrimaryElement, PlantElementAbsorber.Planner>.PooledList primaryElements;

		public struct ConsumeCommand
		{
			public PrimaryElement primaryElement;

			public float deltaMass;
		}
	}
}
