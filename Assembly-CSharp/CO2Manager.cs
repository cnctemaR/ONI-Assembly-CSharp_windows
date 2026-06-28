using System;
using System.Collections.Generic;
using UnityEngine;

public class CO2Manager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		CO2Manager.instance = this;
		this.prefab.gameObject.SetActive(false);
		this.breathPrefab.SetActive(false);
		this.co2Pool = new ObjectPool(new Func<GameObject>(this.InstantiateCO2), 16);
		this.breathPool = new ObjectPool(new Func<GameObject>(this.InstantiateBreath), 16);
	}

	private GameObject InstantiateCO2()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.prefab, Grid.SceneLayer.Front, Folder.FX, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private GameObject InstantiateBreath()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.breathPrefab, Grid.SceneLayer.Front, Folder.FX, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector2I vector2I = default(Vector2I);
		Vector2I vector2I2 = default(Vector2I);
		Vector3 vector = this.acceleration * fixedDeltaTime;
		int num = this.co2Items.Count;
		for (int i = 0; i < num; i++)
		{
			CO2 co = this.co2Items[i];
			co.velocity += vector;
			co.lifetimeRemaining -= fixedDeltaTime;
			Grid.PosToXY(co.transform.position, out vector2I);
			co.transform.position += co.velocity * fixedDeltaTime;
			Grid.PosToXY(co.transform.position, out vector2I2);
			int num2 = Grid.XYToCell(vector2I.x, vector2I.y);
			int num3 = num2;
			for (int j = vector2I.y; j >= vector2I2.y; j--)
			{
				int num4 = Grid.XYToCell(vector2I.x, j);
				bool flag = !Grid.IsValidCell(num4) || co.lifetimeRemaining <= 0f;
				if (!flag)
				{
					Element element = Grid.Element[num4];
					flag = element.IsLiquid || element.IsSolid;
				}
				if (flag)
				{
					bool flag2 = false;
					if (num3 != num4)
					{
						flag2 = true;
					}
					else
					{
						int num5 = num4;
						while (Grid.IsValidCell(num5))
						{
							Element element2 = Grid.Element[num5];
							if (!element2.IsLiquid && !element2.IsSolid)
							{
								flag2 = true;
								break;
							}
							num5 = Grid.CellAbove(num5);
						}
					}
					co.TriggerDestroy();
					if (flag2)
					{
						SimMessages.ModifyMass(num3, co.mass, CellEventLogger.Instance.CO2ManagerFixedUpdate, co.temperature, SimHashes.CarbonDioxide);
						num--;
						this.co2Items[i] = this.co2Items[num];
						this.co2Items.RemoveAt(num);
					}
					else
					{
						Output.LogWarning(new object[] { "Couldn't emit CO2" });
					}
					break;
				}
				num3 = num4;
			}
		}
	}

	public void SpawnCO2(Vector3 position, float mass, float temperature)
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
		component.StartLoop();
		this.co2Items.Add(component);
	}

	public void SpawnBreath(Vector3 position, float mass, float temperature)
	{
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
		this.SpawnCO2(position, mass, temperature);
		GameObject gameObject = this.breathPool.GetInstance();
		gameObject.transform.SetPosition(position);
		gameObject.SetActive(true);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.TintColour = this.tintColour;
		component.onDestroySelf = new Action<GameObject>(this.OnDestroyBreath);
		component.Play("breath", KAnim.PlayMode.Once, 1f, 0f);
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

	private const float CO2Lifetime = 3f;

	[SerializeField]
	private Vector3 acceleration;

	[SerializeField]
	private CO2 prefab;

	[SerializeField]
	private GameObject breathPrefab;

	[SerializeField]
	private Color tintColour;

	private List<CO2> co2Items = new List<CO2>();

	private ObjectPool breathPool;

	private ObjectPool co2Pool;

	public static CO2Manager instance;
}
