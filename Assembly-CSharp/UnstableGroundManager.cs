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
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.BuildingBack, null, 0);
		gameObject.SetActive(false);
		gameObject.name = "UnstablePool";
		return gameObject;
	}

	public void Spawn(int cell, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			global::Debug.LogError("Tried to spawn unstable ground with NaN temperature");
			temperature = 293f;
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(vector, element, mass, temperature, disease_idx, disease_count);
		kbatchedAnimController.Play("start", KAnim.PlayMode.Once, 1f, 0f);
		kbatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		kbatchedAnimController.gameObject.name = "Falling " + element.name;
		GameComps.Gravities.Add(kbatchedAnimController.gameObject, Vector2.zero, null);
		this.fallingObjects.Add(kbatchedAnimController.gameObject);
		this.SpawnPuff(vector, element, mass, temperature, disease_idx, disease_count);
		Substance substance = element.substance;
		if (substance != null && substance.fallingStartSound != null && CameraController.Instance.IsAudibleSound(vector, substance.fallingStartSound))
		{
			SoundEvent.PlayOneShot(substance.fallingStartSound, vector, 1f);
		}
	}

	private void SpawnOld(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			global::Debug.LogError("Spawning falling ground with a stable element");
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		GameComps.Gravities.Add(kbatchedAnimController.gameObject, Vector2.zero, null);
		kbatchedAnimController.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		this.fallingObjects.Add(kbatchedAnimController.gameObject);
		kbatchedAnimController.gameObject.name = "SpawnOld " + element.name;
	}

	private void SpawnPuff(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			global::Debug.LogError("Spawning sand puff with a stable element");
		}
		KBatchedAnimController kbatchedAnimController = this.Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		kbatchedAnimController.Play("sandPuff", KAnim.PlayMode.Once, 1f, 0f);
		kbatchedAnimController.gameObject.name = "Puff " + element.name;
		kbatchedAnimController.transform.SetPosition(kbatchedAnimController.transform.GetPosition() + this.spawnPuffOffset);
	}

	private KBatchedAnimController Spawn(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		UnstableGroundManager.EffectRuntimeInfo effectRuntimeInfo;
		if (!this.runtimeInfo.TryGetValue(element.id, out effectRuntimeInfo))
		{
			global::Debug.LogError(element.id.ToString() + " needs unstable ground info hookup!");
		}
		GameObject instance = effectRuntimeInfo.pool.GetInstance();
		instance.transform.SetPosition(pos);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			global::Debug.LogError("Tried to spawn unstable ground with NaN temperature");
			temperature = 293f;
		}
		UnstableGround component = instance.GetComponent<UnstableGround>();
		component.element = element.id;
		component.mass = mass;
		component.temperature = temperature;
		component.diseaseIdx = disease_idx;
		component.diseaseCount = disease_count;
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
			Vector2I vector2I;
			Grid.PosToXY(this.fallingObjects[i].transform.GetPosition(), out vector2I);
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
			if (!(gameObject == null))
			{
				Vector3 position = gameObject.transform.GetPosition();
				int cell = Grid.PosToCell(position);
				int num = Grid.CellBelow(cell);
				if (!Grid.IsValidCell(num) || Grid.Element[num].IsSolid || (Grid.Properties[num] & 4) != 0)
				{
					UnstableGround component = gameObject.GetComponent<UnstableGround>();
					this.pendingCells.Add(cell);
					HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
					{
						this.RemoveFromPending(cell);
					}, false));
					SimMessages.AddRemoveSubstance(cell, component.element, CellEventLogger.Instance.UnstableGround, component.mass, component.temperature, component.diseaseIdx, component.diseaseCount, true, handle.index);
					ListPool<ScenePartitionerEntry, UnstableGroundManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, UnstableGroundManager>.Allocate();
					Vector2I vector2I = Grid.CellToXY(cell);
					vector2I.x = Mathf.Max(0, vector2I.x - 1);
					vector2I.y = Mathf.Min(Grid.HeightInCells - 1, vector2I.y + 1);
					GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 3, 3, GameScenePartitioner.Instance.collisionLayer, pooledList);
					foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
					{
						if (scenePartitionerEntry.obj is KCollider2D)
						{
							(scenePartitionerEntry.obj as KCollider2D).gameObject.Trigger(-975551167, null);
						}
					}
					pooledList.Recycle();
					Element element = ElementLoader.FindElementByHash(component.element);
					if (element != null && element.substance != null && element.substance.fallingStopSound != null && CameraController.Instance.IsAudibleSound(position, element.substance.fallingStopSound))
					{
						SoundEvent.PlayOneShot(element.substance.fallingStopSound, position, 1f);
					}
					GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.OreAbsorbId), position + this.landEffectOffset, Grid.SceneLayer.Front, null, 0).SetActive(true);
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
			UnstableGround component = gameObject.GetComponent<UnstableGround>();
			byte diseaseIdx = component.diseaseIdx;
			int num = ((diseaseIdx != byte.MaxValue) ? Db.Get().Diseases[(int)diseaseIdx].id.HashValue : 0);
			this.serializedInfo.Add(new UnstableGroundManager.SerializedInfo
			{
				position = gameObject.transform.GetPosition(),
				element = component.element,
				mass = component.mass,
				temperature = component.temperature,
				diseaseID = num,
				diseaseCount = component.diseaseCount
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
		HashedString hashedString = default(HashedString);
		foreach (UnstableGroundManager.SerializedInfo serializedInfo in this.serializedInfo)
		{
			Element element = ElementLoader.FindElementByHash(serializedInfo.element);
			hashedString.HashValue = serializedInfo.diseaseID;
			byte index = Db.Get().Diseases.GetIndex(hashedString);
			int num = serializedInfo.diseaseCount;
			if (index == 255)
			{
				num = 0;
			}
			this.SpawnOld(serializedInfo.position, element, serializedInfo.mass, serializedInfo.temperature, index, num);
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

		public int diseaseID;

		public int diseaseCount;
	}
}
