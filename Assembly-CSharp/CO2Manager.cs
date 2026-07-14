using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CO2Manager")]
public class CO2Manager : KMonoBehaviour, ISim33ms
{
	public static void DestroyInstance()
	{
		CO2Manager.instance = null;
	}

	protected override void OnPrefabInit()
	{
		CO2Manager.instance = this;
		this.prefab.gameObject.SetActive(false);
		this.breathPrefab.SetActive(false);
		this.exhaustPrefab.SetActive(false);
		this.co2Pool = new GameObjectPool(new Func<GameObject>(this.InstantiateCO2), new Action<GameObject>(CO2Manager.Deactivate), 16);
		this.breathPool = new GameObjectPool(new Func<GameObject>(this.InstantiateBreath), new Action<GameObject>(CO2Manager.Deactivate), 16);
		this.exhaustPool = new GameObjectPool(new Func<GameObject>(this.InstantiateExhaust), new Action<GameObject>(CO2Manager.Deactivate), 16);
	}

	private GameObject InstantiateCO2()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.prefab, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private static void Deactivate(GameObject _)
	{
	}

	private GameObject InstantiateBreath()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.breathPrefab, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private GameObject InstantiateExhaust()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.exhaustPrefab, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	public void Sim33ms(float dt)
	{
		Vector2I vector2I = default(Vector2I);
		Vector2I vector2I2 = default(Vector2I);
		Vector3 vector = this.acceleration * dt;
		int num = this.co2Items.Count;
		for (int i = 0; i < num; i++)
		{
			CO2 co = this.co2Items[i];
			co.velocity += vector;
			co.lifetimeRemaining -= dt;
			Grid.PosToXY(co.transform.GetPosition(), out vector2I);
			co.transform.SetPosition(co.transform.GetPosition() + co.velocity * dt);
			Grid.PosToXY(co.transform.GetPosition(), out vector2I2);
			int num2 = Grid.XYToCell(vector2I.x, vector2I.y);
			for (int j = vector2I.y; j >= vector2I2.y; j--)
			{
				int num3 = Grid.XYToCell(vector2I.x, j);
				bool flag = !Grid.IsValidCell(num3) || co.lifetimeRemaining <= 0f;
				if (!flag)
				{
					Element element = Grid.Element[num3];
					flag = element.IsLiquid || element.IsSolid || (Grid.Properties[num3] & 1) > 0;
				}
				if (flag)
				{
					int num4 = num3;
					bool flag2 = false;
					int num5;
					if (num2 != num3)
					{
						num4 = num2;
						flag2 = true;
					}
					else if (CO2Manager.TryFindBreathableSpawnCell(num3, out num5))
					{
						flag2 = true;
						num4 = num5;
					}
					if (flag2)
					{
						co.TriggerDestroy();
						SimMessages.ModifyMass(num4, co.mass, byte.MaxValue, 0, CellEventLogger.Instance.CO2ManagerFixedUpdate, co.temperature, SimHashes.CarbonDioxide);
						num--;
						this.co2Items[i] = this.co2Items[num];
						this.co2Items.RemoveAt(num);
						break;
					}
				}
				num2 = num3;
			}
		}
	}

	private static bool TryFindBreathableSpawnCell(int cell, out int spawnCell)
	{
		bool flag = false;
		int num = -1;
		int num2 = -1;
		bool flag2 = false;
		foreach (CellOffset cellOffset in GasBreatherFromWorldProvider.DEFAULT_BREATHABLE_OFFSETS)
		{
			int num3 = Grid.OffsetCell(cell, cellOffset);
			if (Grid.IsValidCell(num3))
			{
				Element element = Grid.Element[num3];
				if (element.id == SimHashes.CarbonDioxide || element.HasTag(GameTags.Breathable))
				{
					num = num3;
					flag = true;
					flag2 = true;
					break;
				}
				if (element.IsGas)
				{
					num2 = num3;
					flag2 = true;
				}
			}
		}
		spawnCell = (flag ? num : num2);
		return flag2;
	}

	public CO2 SpawnCO2(Vector3 position, float mass, float temperature, bool flip)
	{
		return this.SpawnCO2(position, mass, temperature, flip, 0f);
	}

	public CO2 SpawnCO2(Vector3 position, float mass, float temperature, bool flip, float rotation)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		GameObject gameObject = this.co2Pool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		CO2 component = gameObject.GetComponent<CO2>();
		component.mass = mass;
		component.temperature = temperature;
		component.velocity = Vector3.zero;
		component.lifetimeRemaining = 3f;
		KBatchedAnimController component2 = component.GetComponent<KBatchedAnimController>();
		component2.TintColour = this.tintColour;
		component2.onDestroySelf = new Action<GameObject>(this.OnDestroyCO2);
		component2.FlipX = flip;
		component.StartLoop();
		this.co2Items.Add(component);
		return component;
	}

	public void SpawnBreath(Vector3 position, float mass, float temperature, bool flip)
	{
		int num;
		if (Grid.IsVisiblyInLiquid(position) && !CO2Manager.TryFindBreathableSpawnCell(Grid.PosToCell(position), out num))
		{
			BubbleManager.instance.SpawnBubble(SimHashes.CarbonDioxide, position, mass, temperature, BubbleManager.Disease.None, null);
			return;
		}
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		this.SpawnCO2(position, mass, temperature, flip);
		GameObject gameObject = this.breathPool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.TintColour = this.tintColour;
		component.onDestroySelf = new Action<GameObject>(this.OnDestroyBreath);
		component.FlipX = flip;
		component.Play("breath", KAnim.PlayMode.Once, 1f, 0f);
	}

	public void SpawnExhaust(Vector3 position, Vector3 velocity, int co2Cell, float mass, float temperature)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		float num = Mathf.Repeat(Vector3.Angle(Vector3.down, velocity) * Mathf.Sign(velocity.x), 360f);
		SimMessages.ModifyMass(co2Cell, mass, byte.MaxValue, 0, CellEventLogger.Instance.CO2ManagerFixedUpdate, temperature, SimHashes.CarbonDioxide);
		GameObject gameObject = this.exhaustPool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		CO2 co = gameObject.AddOrGet<CO2>();
		co.mass = mass;
		co.temperature = temperature;
		co.lifetimeRemaining = 3f;
		co.affectedByGravity = false;
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.onDestroySelf = new Action<GameObject>(this.OnDestroyExhaust);
		component.Rotation = num;
		component.Play("smoke_particle", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void OnDestroyCO2(GameObject co2_go)
	{
		co2_go.SetActive(false);
		this.co2Pool.ReleaseInstance(co2_go);
	}

	private void OnDestroyBreath(GameObject breath_go)
	{
		breath_go.SetActive(false);
		this.breathPool.ReleaseInstance(breath_go);
	}

	private void OnDestroyExhaust(GameObject go)
	{
		go.SetActive(false);
		this.exhaustPool.ReleaseInstance(go);
	}

	private const float CO2Lifetime = 3f;

	[SerializeField]
	private Vector3 acceleration;

	[SerializeField]
	private CO2 prefab;

	[SerializeField]
	private GameObject breathPrefab;

	[SerializeField]
	private GameObject exhaustPrefab;

	[SerializeField]
	private Color tintColour;

	private List<CO2> co2Items = new List<CO2>();

	private GameObjectPool breathPool;

	private GameObjectPool exhaustPool;

	private GameObjectPool co2Pool;

	public static CO2Manager instance;
}
