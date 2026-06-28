using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class UnstableGroundManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		UnstableGroundManager.EffectInfo[] array = this.effects;
		for (int i = 0; i < array.Length; i++)
		{
			UnstableGroundManager.EffectInfo effectInfo = array[i];
			GameObject prefab = effectInfo.prefab;
			prefab.SetActive(false);
			UnstableGroundManager.EffectRuntimeInfo effectRuntimeInfo = default(UnstableGroundManager.EffectRuntimeInfo);
			ObjectPool pool = new ObjectPool(() => this.InstantiateObj(prefab), 16);
			effectRuntimeInfo.pool = pool;
			effectRuntimeInfo.releaseFunc = delegate(GameObject go)
			{
				this.ReleaseGO(go);
				pool.ReleaseInstance(go);
			};
			this.runtimeInfo[effectInfo.element] = effectRuntimeInfo;
		}
	}

	private void ReleaseGO(GameObject go)
	{
		if (GameComps.Gravities.Has(go))
		{
			GameComps.Gravities.Remove(go);
		}
		go.SetActive(false);
	}

	private GameObject InstantiateObj(GameObject prefab)
	{
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.BuildingBack, Folder.FX, null, 0);
		gameObject.SetActive(false);
		gameObject.name = "UnstablePool";
		return gameObject;
	}

	public void Spawn(int cell, Element element, float mass, float temperature)
	{
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			global::Debug.LogError("Tried to spawn unstable ground with NaN temperature", null);
			temperature = 293f;
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(vector, element, mass, temperature);
		kbatchedAnimController.Play("start", KAnim.PlayMode.Once, 1f, 0f);
		kbatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		kbatchedAnimController.gameObject.name = "Falling " + element.name;
		GameComps.Gravities.Add(kbatchedAnimController.gameObject, Vector2.zero, null);
		this.fallingObjects.Add(kbatchedAnimController.gameObject);
		this.SpawnPuff(vector, element, mass, temperature);
		Substance substance = element.substance;
		if (substance != null && substance.fallingStartSound != null && CameraController.Instance.IsAudibleSound(vector, substance.fallingStartSound))
		{
			SoundEvent.PlayOneShot(substance.fallingStartSound, vector);
		}
	}

	private void SpawnOld(Vector3 pos, Element element, float mass, float temperature)
	{
		if (!element.IsUnstable)
		{
			Output.LogError(new object[] { "Spawning falling ground with a stable element" });
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(pos, element, mass, temperature);
		GameComps.Gravities.Add(kbatchedAnimController.gameObject, Vector2.zero, null);
		kbatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		this.fallingObjects.Add(kbatchedAnimController.gameObject);
		kbatchedAnimController.gameObject.name = "SpawnOld " + element.name;
	}

	private void SpawnPuff(Vector3 pos, Element element, float mass, float temperature)
	{
		if (!element.IsUnstable)
		{
			Output.LogError(new object[] { "Spawning sand puff with a stable element" });
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(pos, element, mass, temperature);
		kbatchedAnimController.Play("sandPuff", KAnim.PlayMode.Once, 1f, 0f);
		kbatchedAnimController.gameObject.name = "Puff " + element.name;
		kbatchedAnimController.transform.position += this.spawnPuffOffset;
	}

	private KBatchedAnimController Spawn(Vector3 pos, Element element, float mass, float temperature)
	{
		UnstableGroundManager.EffectRuntimeInfo effectRuntimeInfo = this.runtimeInfo[element.id];
		GameObject instance = effectRuntimeInfo.pool.GetInstance();
		instance.transform.SetPosition(pos);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			global::Debug.LogError("Tried to spawn unstable ground with NaN temperature", null);
			temperature = 293f;
		}
		PrimaryElement component = instance.GetComponent<PrimaryElement>();
		component.ElementID = element.id;
		component.Mass = mass;
		component.Temperature = temperature;
		instance.SetActive(true);
		KBatchedAnimController component2 = instance.GetComponent<KBatchedAnimController>();
		component2.onDestroySelf = effectRuntimeInfo.releaseFunc;
		component2.Stop();
		if (element.substance != null)
		{
			component2.TintColour = element.substance.colour;
		}
		return component2;
	}

	public List<int> GetCellsContainingFallingAbove(Vector2I cellXY)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.fallingObjects.Count; i++)
		{
			GameObject gameObject = this.fallingObjects[i];
			Vector2I vector2I;
			Grid.PosToXY(gameObject.transform.position, out vector2I);
			if (vector2I.x == cellXY.x || vector2I.y >= cellXY.y)
			{
				int num = Grid.PosToCell(vector2I);
				list.Add(num);
			}
		}
		for (int j = 0; j < this.pendingCells.Count; j++)
		{
			Vector2I vector2I2 = Grid.CellToXY(this.pendingCells[j]);
			if (vector2I2.x == cellXY.x || vector2I2.y >= cellXY.y)
			{
				list.Add(this.pendingCells[j]);
			}
		}
		return list;
	}

	private void RemoveFromPending(int cell)
	{
		this.pendingCells.Remove(cell);
	}

	private void Update()
	{
		if (App.isLoading)
		{
			return;
		}
		int i = 0;
		while (i < this.fallingObjects.Count)
		{
			GameObject gameObject = this.fallingObjects[i];
			Vector3 position = gameObject.transform.position;
			int cell = Grid.PosToCell(position);
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(num) || Grid.Element[num].IsSolid || (Grid.Cell[num].properties & 4) != 0)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				this.pendingCells.Add(cell);
				HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
				{
					this.RemoveFromPending(cell);
				}, false), "UnstableGroundManager");
				SimMessages.AddRemoveSubstance(cell, component.ElementID, CellEventLogger.Instance.UnstableGround, component.Mass, component.Temperature, handle.index);
				if (component.Element.substance != null && component.Element.substance.fallingStopSound != null && CameraController.Instance.IsAudibleSound(position, component.Element.substance.fallingStopSound))
				{
					SoundEvent.PlayOneShot(component.Element.substance.fallingStopSound, position);
				}
				GameUtil.KInstantiate(EffectPrefabs.Instance.OreAbsorb, position + this.landEffectOffset, Grid.SceneLayer.Front, Folder.FX, null, 0);
				this.fallingObjects[i] = this.fallingObjects[this.fallingObjects.Count - 1];
				this.fallingObjects.RemoveAt(this.fallingObjects.Count - 1);
				this.ReleaseGO(gameObject);
			}
			else
			{
				i++;
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (this.fallingObjects.Count > 0)
		{
			this.serializedInfo = new List<UnstableGroundManager.SerializedInfo>();
		}
		foreach (GameObject gameObject in this.fallingObjects)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			this.serializedInfo.Add(new UnstableGroundManager.SerializedInfo
			{
				position = gameObject.transform.position,
				element = component.ElementID,
				mass = component.Mass,
				temperature = component.Temperature
			});
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.serializedInfo = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializedInfo == null)
		{
			return;
		}
		this.fallingObjects.Clear();
		foreach (UnstableGroundManager.SerializedInfo serializedInfo in this.serializedInfo)
		{
			Element element = ElementLoader.FindElementByHash(serializedInfo.element);
			this.SpawnOld(serializedInfo.position, element, serializedInfo.mass, serializedInfo.temperature);
		}
	}

	[SerializeField]
	private Vector3 spawnPuffOffset;

	[SerializeField]
	private Vector3 landEffectOffset;

	[SerializeField]
	private UnstableGroundManager.EffectInfo[] effects;

	private List<GameObject> fallingObjects = new List<GameObject>();

	private List<int> pendingCells = new List<int>();

	private Dictionary<SimHashes, UnstableGroundManager.EffectRuntimeInfo> runtimeInfo = new Dictionary<SimHashes, UnstableGroundManager.EffectRuntimeInfo>();

	[Serialize]
	private List<UnstableGroundManager.SerializedInfo> serializedInfo;

	[Serializable]
	private struct EffectInfo
	{
		[HashedEnum]
		public SimHashes element;

		public GameObject prefab;
	}

	private struct EffectRuntimeInfo
	{
		public ObjectPool pool;

		public Action<GameObject> releaseFunc;
	}

	private struct SerializedInfo
	{
		public Vector3 position;

		public SimHashes element;

		public float mass;

		public float temperature;
	}
}
