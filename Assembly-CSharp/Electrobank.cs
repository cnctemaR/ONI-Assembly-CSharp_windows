using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Electrobank : KMonoBehaviour, ISim1000ms, IConsumableUIItem, IGameObjectEffectDescriptor
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
	}

	private void OnCraft(object data)
	{
		WorldResourceAmountTracker<ElectrobankTracker>.Get().RegisterAmountProduced(this.Charge);
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

	public void AddPower(float joules)
	{
		this.charge = Mathf.Clamp(this.charge + joules, 0f, Electrobank.capacity);
	}

	public float RemovePower(float joules, bool dropWhenEmpty)
	{
		float num = Mathf.Min(this.charge, joules);
		this.charge -= num;
		if (this.charge <= 0f)
		{
			if (this.rechargeable)
			{
				Electrobank.ReplaceChargedWithEmpty(base.gameObject, dropWhenEmpty);
			}
			else
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}
		return num;
	}

	public void FullyCharge()
	{
		this.charge = Electrobank.capacity;
	}

	public void Explode()
	{
		int num = Grid.PosToCell(base.gameObject.transform.position);
		float num2 = Grid.Temperature[num];
		num2 += this.charge / (Grid.Mass[num] * Grid.Element[num].specificHeatCapacity);
		num2 = Mathf.Clamp(num2, 1f, 9999f);
		SimMessages.ReplaceElement(num, Grid.Element[num].id, CellEventLogger.Instance.SandBoxTool, Grid.Mass[num], num2, Grid.DiseaseIdx[num], Grid.DiseaseCount[num], -1);
		Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactMetal, base.gameObject.transform.position, 0f);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Battery_explode", false), base.gameObject.transform.position, 1f);
		Electrobank.ReplaceEmptyWithGarbage(base.gameObject, false);
	}

	public void Sim1000ms(float dt)
	{
		if (this.pickupable.KPrefabID.HasTag(GameTags.Stored))
		{
			return;
		}
		if (Grid.IsValidCell(this.pickupable.cachedCell) && Grid.Element[this.pickupable.cachedCell].HasTag(GameTags.AnyWater))
		{
			this.Damage(dt);
		}
	}

	private void Damage(float amount)
	{
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, DUPLICANTS.MODIFIERS.WATERDAMAGE.NAME, base.transform, 1.5f, false);
		Game.Instance.SpawnFX(SpawnFXHashes.BuildingSpark, Grid.PosToCell(base.gameObject), 0f);
		this.health -= amount;
		if (this.health <= 0f)
		{
			this.Explode();
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

	[Serialize]
	private float health = 10f;

	public bool rechargeable;

	[MyCmpGet]
	private Pickupable pickupable;
}
