using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Electrobank : KMonoBehaviour, ISim1000ms, ISim200ms, IConsumableUIItem, IGameObjectEffectDescriptor
{
	public string ID { get; private set; }

	public bool IsFullyCharged
	{
		get
		{
			return this.charge == Electrobank.capacity;
		}
	}

	public float Charge
	{
		get
		{
			return this.charge;
		}
	}

	protected override void OnPrefabInit()
	{
		this.ID = base.gameObject.PrefabID().ToString();
		base.Subscribe(748399584, new Action<object>(this.OnCraft));
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(856640610, new Action<object>(this.ClearHealthBar));
		this.radiationEmitter = base.GetComponent<RadiationEmitter>();
		this.UpdateRadiationEmitter();
	}

	private void OnCraft(object data)
	{
		WorldResourceAmountTracker<ElectrobankTracker>.Get().RegisterAmountProduced(this.Charge);
	}

	private void UpdateRadiationEmitter()
	{
		if (this.radiationEmitter == null)
		{
			return;
		}
		bool flag = this.timeSincePowerDrawn < 0.5f;
		this.radiationEmitter.emitRads = (flag ? this.radioactivityTuning : 0f);
		this.radiationEmitter.Refresh();
	}

	public static GameObject ReplaceEmptyWithCharged(GameObject EmptyElectrobank, bool dropFromStorage = false)
	{
		Vector3 position = EmptyElectrobank.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Electrobank"), position);
		gameObject.GetComponent<PrimaryElement>().SetElement(EmptyElectrobank.GetComponent<PrimaryElement>().Element.id, true);
		gameObject.SetActive(true);
		Storage storage = EmptyElectrobank.GetComponent<Pickupable>().storage;
		EmptyElectrobank.DeleteObject();
		if (storage != null && !dropFromStorage)
		{
			storage.Store(gameObject, false, false, true, false);
		}
		return gameObject;
	}

	public static GameObject ReplaceChargedWithEmpty(GameObject ChargedElectrobank, bool dropFromStorage = false)
	{
		Vector3 position = ChargedElectrobank.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EmptyElectrobank"), position);
		gameObject.GetComponent<PrimaryElement>().SetElement(ChargedElectrobank.GetComponent<PrimaryElement>().Element.id, true);
		gameObject.SetActive(true);
		Storage storage = ChargedElectrobank.GetComponent<Pickupable>().storage;
		ChargedElectrobank.DeleteObject();
		if (storage != null && !dropFromStorage)
		{
			storage.Store(gameObject, false, false, true, false);
		}
		return gameObject;
	}

	public static GameObject ReplaceEmptyWithGarbage(GameObject ChargedElectrobank, bool dropFromStorage = false)
	{
		Vector3 position = ChargedElectrobank.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("GarbageElectrobank"), position);
		gameObject.GetComponent<PrimaryElement>().SetElement(ChargedElectrobank.GetComponent<PrimaryElement>().Element.id, true);
		gameObject.SetActive(true);
		Storage storage = ChargedElectrobank.GetComponent<Pickupable>().storage;
		ChargedElectrobank.DeleteObject();
		if (storage != null && !dropFromStorage)
		{
			storage.Store(gameObject, false, false, true, false);
		}
		return gameObject;
	}

	public float AddPower(float joules)
	{
		if (joules < 0f)
		{
			joules = 0f;
		}
		float num = Mathf.Min(joules, Electrobank.capacity - this.charge);
		this.charge += num;
		return num;
	}

	public float RemovePower(float joules, bool dropWhenEmpty)
	{
		float num = Mathf.Min(this.charge, joules);
		this.charge -= num;
		if (this.charge <= 0f)
		{
			this.OnEmpty(dropWhenEmpty);
		}
		if (num > 0f)
		{
			this.timeSincePowerDrawn = 0f;
		}
		return num;
	}

	protected virtual void OnEmpty(bool dropWhenEmpty)
	{
		if (this.rechargeable)
		{
			Electrobank.ReplaceChargedWithEmpty(base.gameObject, dropWhenEmpty);
			return;
		}
		if (!this.keepEmpty)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public void FullyCharge()
	{
		this.charge = Electrobank.capacity;
	}

	public virtual void Explode()
	{
		int num = Grid.PosToCell(base.gameObject.transform.position);
		float num2 = Grid.Temperature[num];
		num2 += this.charge / (Grid.Mass[num] * Grid.Element[num].specificHeatCapacity);
		num2 = Mathf.Clamp(num2, 1f, 9999f);
		SimMessages.ReplaceElement(num, Grid.Element[num].id, CellEventLogger.Instance.SandBoxTool, Grid.Mass[num], num2, Grid.DiseaseIdx[num], Grid.DiseaseCount[num], -1);
		Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactMetal, base.gameObject.transform.position, 0f);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Battery_explode", false), base.gameObject.transform.position, 1f);
		if (this.rechargeable)
		{
			Electrobank.ReplaceEmptyWithGarbage(base.gameObject, false);
			return;
		}
		base.gameObject.DeleteObject();
	}

	protected void LaunchNearbyStuff()
	{
		ListPool<ScenePartitionerEntry, Comet>.PooledList pooledList = ListPool<ScenePartitionerEntry, Comet>.Allocate();
		Vector3 position = base.transform.position;
		GameScenePartitioner.Instance.GatherEntries((int)position.x - 3, (int)position.y - 3, 6, 6, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
		{
			GameObject gameObject = (scenePartitionerEntry.obj as Pickupable).gameObject;
			if (!(gameObject.GetComponent<MinionIdentity>() != null) && !(gameObject.GetComponent<CreatureBrain>() != null) && gameObject.GetDef<RobotAi.Def>() == null)
			{
				Vector2 vector = gameObject.transform.GetPosition() - position;
				vector = vector.normalized;
				vector *= (float)global::UnityEngine.Random.Range(4, 6);
				vector.y += (float)global::UnityEngine.Random.Range(2, 4);
				if (GameComps.Fallers.Has(gameObject))
				{
					GameComps.Fallers.Remove(gameObject);
				}
				if (GameComps.Gravities.Has(gameObject))
				{
					GameComps.Gravities.Remove(gameObject);
				}
				GameComps.Fallers.Add(gameObject, vector);
			}
		}
		pooledList.Recycle();
	}

	public void Sim1000ms(float dt)
	{
		if (this.pickupable.KPrefabID.HasTag(GameTags.Stored))
		{
			return;
		}
		this.EvaluateWaterDamage(dt);
		this.UpdateHealthBar();
	}

	public virtual void Sim200ms(float dt)
	{
		this.UpdateRadiationEmitter();
		this.timeSincePowerDrawn = Mathf.Min(this.timeSincePowerDrawn + dt, 10f);
	}

	private void EvaluateWaterDamage(float dt)
	{
		if (Grid.IsValidCell(this.pickupable.cachedCell) && Grid.Element[this.pickupable.cachedCell].HasTag(GameTags.AnyWater) && global::UnityEngine.Random.Range(1, 101) > 75)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.POWER_BANK_WATER_DAMAGE, base.transform, 1.5f, false);
			this.Damage(global::UnityEngine.Random.Range(0f, dt));
		}
	}

	public void Damage(float amount)
	{
		Game.Instance.SpawnFX(SpawnFXHashes.ElectrobankDamage, Grid.PosToCell(base.gameObject), 0f);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Battery_sparks_short", false), base.gameObject.transform.position, 1f);
		this.currentHealth -= amount;
		if (this.healthBar == null)
		{
			this.CreateHealthBar();
		}
		this.healthBar.Update();
		this.lastDamageTime = Time.time;
		if (this.currentHealth <= 0f)
		{
			this.Explode();
		}
	}

	protected override void OnCleanUp()
	{
		this.ClearHealthBar(null);
		base.OnCleanUp();
	}

	public void CreateHealthBar()
	{
		this.healthBar = ProgressBar.CreateProgressBar(base.gameObject, () => this.currentHealth / 10f);
		this.healthBar.SetVisibility(true);
		this.healthBar.barColor = Util.ColorFromHex("CC3333");
	}

	public void UpdateHealthBar()
	{
		if (this.healthBar != null && Time.time - this.lastDamageTime > 5f)
		{
			this.ClearHealthBar(null);
		}
	}

	public void ClearHealthBar(object data = null)
	{
		if (this.healthBar != null)
		{
			Util.KDestroyGameObject(this.healthBar);
			this.healthBar = null;
		}
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELECTROBANKS, GameUtil.GetFormattedJoules(this.Charge, "F1", GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELECTROBANKS, GameUtil.GetFormattedJoules(this.Charge, "F1", GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	public string ConsumableId
	{
		get
		{
			return this.PrefabID().Name;
		}
	}

	public string ConsumableName
	{
		get
		{
			return this.GetProperName();
		}
	}

	public int MajorOrder
	{
		get
		{
			return 500;
		}
	}

	public int MinorOrder
	{
		get
		{
			return 0;
		}
	}

	public bool Display
	{
		get
		{
			return true;
		}
	}

	private static float capacity = 120000f;

	[Serialize]
	private float charge = Electrobank.capacity;

	private const float MAX_HEALTH = 10f;

	[Serialize]
	private float currentHealth = 10f;

	[Serialize]
	private float timeSincePowerDrawn = 0.5f;

	private const float RADIATION_EMITTER_TIMEOUT = 0.5f;

	public float radioactivityTuning;

	private RadiationEmitter radiationEmitter;

	private float lastDamageTime;

	public ProgressBar healthBar;

	public bool rechargeable;

	public bool keepEmpty;

	[MyCmpGet]
	private Pickupable pickupable;
}
