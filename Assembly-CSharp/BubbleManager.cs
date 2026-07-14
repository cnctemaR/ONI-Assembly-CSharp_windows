using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/BubbleManager")]
public class BubbleManager : KMonoBehaviour, ISim33ms, IRenderEveryTick
{
	public static void DestroyInstance()
	{
		BubbleManager.instance = null;
	}

	protected override void OnPrefabInit()
	{
		BubbleManager.instance = this;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		this.mesh.name = "BubbleManager Mesh";
		this.propertyBlock = new MaterialPropertyBlock();
		this.propertyBlock.SetTexture("_MainTex", this.texture);
		Game.Instance.Subscribe(-880408538, new Action<object>(this.OnTemperatureOverlayInfraredUpdate));
		Game.Instance.Subscribe(972756592, new Action<object>(this.OnTemperatureOverlayInfraredClear));
	}

	private BubbleManager.Archetype.Id RegisterArchetype(BubbleManager.Archetype archetype)
	{
		BubbleManager.Archetype.Id id = archetype.GetId();
		this.archetypes.TryAdd(id, archetype);
		return id;
	}

	private void OnTemperatureOverlayInfraredClear(object obj)
	{
		this.isInfraredON = false;
	}

	private void OnTemperatureOverlayInfraredUpdate(object obj)
	{
		this.isInfraredON = true;
	}

	[OnDeserialized]
	public void OnDeserialized()
	{
		ListPool<BubbleManager.Archetype.Id, BubbleManager>.PooledList pooledList = ListPool<BubbleManager.Archetype.Id, BubbleManager>.Allocate();
		foreach (BubbleManager.Archetype.Id id in this.archetypes.Keys)
		{
			int num = 0;
			foreach (KeyValuePair<BubbleManager.WorldArchetype, BubbleManager.InstanceData> keyValuePair in this.bubbles)
			{
				BubbleManager.WorldArchetype worldArchetype;
				BubbleManager.InstanceData instanceData;
				keyValuePair.Deconstruct(out worldArchetype, out instanceData);
				ref BubbleManager.WorldArchetype ptr = worldArchetype;
				BubbleManager.InstanceData instanceData2 = instanceData;
				if (ptr.archetype.hashCode == id.hashCode)
				{
					num += instanceData2.Count;
				}
			}
			if (num == 0)
			{
				pooledList.Add(id);
			}
		}
		foreach (BubbleManager.Archetype.Id id2 in pooledList)
		{
			this.archetypes.Remove(id2);
		}
		pooledList.Recycle();
		ListPool<BubbleManager.WorldArchetype, BubbleManager>.PooledList pooledList2 = ListPool<BubbleManager.WorldArchetype, BubbleManager>.Allocate();
		foreach (BubbleManager.WorldArchetype worldArchetype2 in this.bubbles.Keys)
		{
			if (!this.archetypes.ContainsKey(worldArchetype2.archetype))
			{
				pooledList2.Add(worldArchetype2);
			}
		}
		if (pooledList2.Count != 0)
		{
			DebugUtil.LogWarningArgs(new object[] { "BubbleManager.OnDeserialized is deleting bubbles" });
		}
		foreach (BubbleManager.WorldArchetype worldArchetype3 in pooledList2)
		{
			this.bubbles.Remove(worldArchetype3);
		}
		pooledList2.Recycle();
	}

	public void SpawnBubble(SimHashes element, Vector2 position, float mass, float temperature, BubbleManager.Disease disease, Vector2? velocity = null)
	{
		if (mass < 1E-09f)
		{
			global::Debug.LogFormat("BubbleManager.SpawnBubble: Attempted to spawn a bubble with mass {0} which is below the sim's minimum mass threshold of {1}. Bubble will not be spawned.", new object[] { mass, 1E-09f });
			return;
		}
		int num = global::UnityEngine.Random.Range(0, this.numFrames);
		int num2 = Grid.PosToCell(position);
		byte b = Grid.WorldIdx[num2];
		BubbleManager.WorldArchetype worldArchetype = new BubbleManager.WorldArchetype
		{
			worldIdx = (int)b,
			archetype = this.RegisterArchetype(new BubbleManager.Archetype(velocity ?? BubbleManager.DEFAULT_VELOCITY, element))
		};
		this.ManifestBucket(worldArchetype).Add(position, mass, temperature, num, disease);
	}

	private BubbleManager.InstanceData ManifestBucket(BubbleManager.WorldArchetype worldArchetype)
	{
		BubbleManager.InstanceData instanceData;
		if (!this.bubbles.TryGetValue(worldArchetype, out instanceData))
		{
			instanceData = new BubbleManager.InstanceData();
			this.bubbles[worldArchetype] = instanceData;
		}
		return instanceData;
	}

	private static bool ShouldPop(Vector2 position, SimHashes bubbleElement, out int cell)
	{
		cell = Grid.PosToCell(position);
		return Grid.Element[cell].id == bubbleElement || !UnderwaterSoundEvent.IsVisiblyInLiquid(position);
	}

	public void Sim33ms(float dt)
	{
		ListPool<int, BubbleManager>.PooledList pooledList = ListPool<int, BubbleManager>.Allocate();
		ListPool<BubbleManager.WorldArchetype, BubbleManager>.PooledList pooledList2 = ListPool<BubbleManager.WorldArchetype, BubbleManager>.Allocate();
		foreach (KeyValuePair<BubbleManager.WorldArchetype, BubbleManager.InstanceData> keyValuePair in this.bubbles)
		{
			BubbleManager.WorldArchetype worldArchetype;
			BubbleManager.InstanceData instanceData;
			keyValuePair.Deconstruct(out worldArchetype, out instanceData);
			BubbleManager.WorldArchetype worldArchetype2 = worldArchetype;
			BubbleManager.InstanceData instanceData2 = instanceData;
			BubbleManager.Archetype archetype;
			if (!this.archetypes.TryGetValue(worldArchetype2.archetype, out archetype))
			{
				DebugUtil.LogWarningArgs(new object[] { "BubbleManager.Sim33ms: Unknown archetype id. Skipping this bubble type for this world" });
				pooledList2.Add(worldArchetype2);
			}
			else
			{
				Vector2 vector = archetype.velocity;
				vector.Normalize();
				vector *= Grid.HalfCellSizeInMeters;
				foreach (BubbleManager.InstanceData.Subscript subscript in instanceData2)
				{
					subscript.Position += archetype.velocity * dt;
					Vector2 vector2 = subscript.Position + vector;
					int num;
					bool flag = BubbleManager.ShouldPop(vector2, archetype.element, out num);
					if (!subscript.Visible || flag)
					{
						if (Grid.Solid[num] && Grid.Element[num].IsSolid)
						{
							num = Grid.PosToCell(subscript.Position);
						}
						else if (Grid.Element[num].IsLiquid && Grid.Element[num].id != archetype.element)
						{
							int num2 = Grid.CellAbove(num);
							if (Grid.IsValidCell(num2) && (Grid.IsGas(num2) || Grid.Element[num2].IsVacuum))
							{
								num = num2;
							}
						}
						SimMessages.AddRemoveSubstance(num, archetype.element, CellEventLogger.Instance.FallingWaterAddToSim, subscript.Mass, subscript.Temperature, subscript.Disease.Idx, subscript.Disease.Count, true, -1);
						pooledList.Add(subscript.Index);
					}
					if (!subscript.FadingOut)
					{
						int num3;
						if (BubbleManager.ShouldPop(vector2 + vector * 2f, archetype.element, out num3))
						{
							subscript.FadingOut = true;
						}
					}
					else
					{
						subscript.Alpha = Mathf.Max(0f, subscript.Alpha - archetype.alphaFadeSpeed * dt);
					}
					subscript.ElapsedTime += dt;
				}
				instanceData2.Destroy(pooledList);
				pooledList.Clear();
			}
		}
		pooledList.Recycle();
		foreach (BubbleManager.WorldArchetype worldArchetype3 in pooledList2)
		{
			this.bubbles.Remove(worldArchetype3);
		}
		pooledList2.Recycle();
	}

	public void GetBubblesInCell(int cell, List<BubbleManager.CellBubbleInfo> results)
	{
		results.Clear();
		int num = (int)Grid.WorldIdx[cell];
		foreach (KeyValuePair<BubbleManager.WorldArchetype, BubbleManager.InstanceData> keyValuePair in this.bubbles)
		{
			BubbleManager.WorldArchetype worldArchetype;
			BubbleManager.InstanceData instanceData;
			keyValuePair.Deconstruct(out worldArchetype, out instanceData);
			BubbleManager.WorldArchetype worldArchetype2 = worldArchetype;
			BubbleManager.InstanceData instanceData2 = instanceData;
			BubbleManager.Archetype archetype;
			if (worldArchetype2.worldIdx == num && this.archetypes.TryGetValue(worldArchetype2.archetype, out archetype))
			{
				float num2 = 0f;
				float num3 = 0f;
				foreach (BubbleManager.InstanceData.Subscript subscript in instanceData2)
				{
					if (Grid.PosToCell(subscript.Position) == cell)
					{
						num2 += subscript.Mass;
						num3 += subscript.Mass * subscript.Temperature;
					}
				}
				if (num2 > 0f)
				{
					bool flag = false;
					for (int i = 0; i < results.Count; i++)
					{
						if (results[i].element == archetype.element)
						{
							BubbleManager.CellBubbleInfo cellBubbleInfo = results[i];
							float num4 = cellBubbleInfo.totalMass + num2;
							cellBubbleInfo.averageTemperature = (cellBubbleInfo.averageTemperature * cellBubbleInfo.totalMass + num3) / num4;
							cellBubbleInfo.totalMass = num4;
							results[i] = cellBubbleInfo;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						results.Add(new BubbleManager.CellBubbleInfo
						{
							element = archetype.element,
							totalMass = num2,
							averageTemperature = num3 / num2
						});
					}
				}
			}
		}
	}

	public void RenderEveryTick(float dt)
	{
		List<Vector3> vertices = MeshUtil.vertices;
		List<Color32> colours = MeshUtil.colours32;
		List<Vector2> uvs = MeshUtil.uvs;
		List<Vector2> uv2s = MeshUtil.uv2s;
		List<Vector4> uv4s = MeshUtil.uv4s;
		List<int> indices = MeshUtil.indices;
		float num = this.particleSize.x * 0.5f;
		float num2 = this.particleSize.y * 0.5f;
		Vector2 vector = new Vector2(-num, -num2);
		Vector2 vector2 = new Vector2(num, -num2);
		Vector2 vector3 = new Vector2(num, num2);
		Vector2 vector4 = new Vector2(-num, num2);
		uvs.Clear();
		uv2s.Clear();
		uv4s.Clear();
		vertices.Clear();
		indices.Clear();
		colours.Clear();
		int num3 = 0;
		foreach (KeyValuePair<BubbleManager.WorldArchetype, BubbleManager.InstanceData> keyValuePair in this.bubbles)
		{
			BubbleManager.WorldArchetype worldArchetype;
			BubbleManager.InstanceData instanceData;
			keyValuePair.Deconstruct(out worldArchetype, out instanceData);
			BubbleManager.WorldArchetype worldArchetype2 = worldArchetype;
			BubbleManager.InstanceData instanceData2 = instanceData;
			if (worldArchetype2.worldIdx == ClusterManager.Instance.activeWorldId)
			{
				BubbleManager.Archetype archetype;
				if (!this.archetypes.TryGetValue(worldArchetype2.archetype, out archetype))
				{
					DebugUtil.LogWarningArgs(new object[] { "BubbleManager.RenderEveryTick: Unknown archetype id, likely dynamically registered" });
				}
				else
				{
					int num4 = instanceData2.CountVisible();
					if (num4 != 0)
					{
						int num5 = 16249 - num3;
						if (num5 <= 0)
						{
							DebugUtil.LogWarningArgs(new object[] { "BubbleManager.RenderEveryTick: Particle capacity reached, skipping remaining archetypes" });
							break;
						}
						int num6 = Mathf.Min(num4, num5);
						bool flag = num6 == num4;
						if (!flag)
						{
							DebugUtil.LogWarningArgs(new object[] { "Too many bubbles to render. Wanted", num4, "but truncating to", num6 });
						}
						int num7 = 0;
						foreach (BubbleManager.InstanceData.Subscript subscript in instanceData2)
						{
							if (subscript.Visible)
							{
								vertices.Add(subscript.Position + vector);
								vertices.Add(subscript.Position + vector2);
								vertices.Add(subscript.Position + vector3);
								vertices.Add(subscript.Position + vector4);
								uvs.Add(new Vector2(0f, 0f));
								uvs.Add(new Vector2(1f, 0f));
								uvs.Add(new Vector2(1f, 1f));
								uvs.Add(new Vector2(0f, 1f));
								Color32 colour = archetype.Colour;
								colour.a = (byte)((float)colour.a * subscript.Alpha);
								Vector2 vector5 = new Vector2((float)subscript.SizeLevel, (float)subscript.Frame);
								uv2s.Add(vector5);
								uv2s.Add(vector5);
								uv2s.Add(vector5);
								uv2s.Add(vector5);
								Vector4 vector6 = (this.isInfraredON ? SimDebugView.Instance.NormalizedTemperature(subscript.Temperature) : Vector4.zero);
								uv4s.Add(vector6);
								uv4s.Add(vector6);
								uv4s.Add(vector6);
								uv4s.Add(vector6);
								colours.Add(colour);
								colours.Add(colour);
								colours.Add(colour);
								colours.Add(colour);
								int num8 = (num3 + num7) * 4;
								indices.Add(num8);
								indices.Add(num8 + 1);
								indices.Add(num8 + 2);
								indices.Add(num8);
								indices.Add(num8 + 2);
								indices.Add(num8 + 3);
								num7++;
								if (!flag && num7 == num6)
								{
									break;
								}
							}
						}
						DebugUtil.DevAssert(num7 == num6, "Rendered bubble count does not match expected", null);
						num3 += num7;
					}
				}
			}
		}
		if (num3 > 0)
		{
			this.mesh.Clear();
			this.mesh.SetVertices(vertices);
			this.mesh.SetUVs(0, uvs);
			this.mesh.SetUVs(1, uv2s);
			this.mesh.SetUVs(2, uv4s);
			this.mesh.SetColors(colours);
			this.mesh.SetTriangles(indices, 0);
			int num9 = LayerMask.NameToLayer("Default");
			Vector4 vector7 = PropertyTextures.CalculateClusterWorldSize();
			this.material.SetVector("_ClusterWorldSizeInfo", vector7);
			Graphics.DrawMesh(this.mesh, new Vector3(0f, 0f, Grid.GetLayerZ(this.sceneLayer)), Quaternion.identity, this.material, num9, null, 0, this.propertyBlock);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-880408538, new Action<object>(this.OnTemperatureOverlayInfraredUpdate));
		Game.Instance.Unsubscribe(972756592, new Action<object>(this.OnTemperatureOverlayInfraredClear));
		base.OnCleanUp();
	}

	public static BubbleManager instance;

	[Serialize]
	private readonly Dictionary<BubbleManager.WorldArchetype, BubbleManager.InstanceData> bubbles = new Dictionary<BubbleManager.WorldArchetype, BubbleManager.InstanceData>();

	[Serialize]
	private readonly Dictionary<BubbleManager.Archetype.Id, BubbleManager.Archetype> archetypes = new Dictionary<BubbleManager.Archetype.Id, BubbleManager.Archetype>();

	private Mesh mesh;

	private MaterialPropertyBlock propertyBlock;

	[SerializeField]
	private Texture2D texture;

	[SerializeField]
	private int numFrames;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Grid.SceneLayer sceneLayer;

	[SerializeField]
	private Vector2 particleSize;

	private bool isInfraredON;

	private static Vector2 DEFAULT_VELOCITY = new Vector2(0f, 1f);

	[Serializable]
	public struct Disease
	{
		public static readonly BubbleManager.Disease None = new BubbleManager.Disease
		{
			Idx = byte.MaxValue,
			Count = 0
		};

		public byte Idx;

		public int Count;
	}

	[Serializable]
	private readonly struct Archetype
	{
		public Color32 Colour
		{
			get
			{
				DebugUtil.DevAssert(ElementLoader.elementTable != null, "Elements are not loaded yet", null);
				ushort elementIndex = ElementLoader.GetElementIndex(this.element);
				DebugUtil.DevAssert(ElementLoader.elements != null, "Elements are not loaded yet", null);
				Element element = ElementLoader.elements[(int)elementIndex];
				Color color = (element.IsMoltenMetal ? WaterCubes.MOLTEN_METAL_COLOR : element.substance.colour);
				color.a = 255f;
				return color;
			}
		}

		public Archetype(Vector2 velocity, SimHashes elementId)
		{
			this.velocity = velocity;
			this.element = elementId;
			this.alphaFadeSpeed = velocity.magnitude;
		}

		public BubbleManager.Archetype.Id GetId()
		{
			return new BubbleManager.Archetype.Id
			{
				hashCode = this.GetHashCode()
			};
		}

		public readonly Vector2 velocity;

		public readonly SimHashes element;

		public readonly float alphaFadeSpeed;

		[Serializable]
		public struct Id
		{
			public int hashCode;
		}
	}

	[Serializable]
	private struct WorldArchetype
	{
		public int worldIdx;

		public BubbleManager.Archetype.Id archetype;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	private class InstanceData : IEnumerable<BubbleManager.InstanceData.Subscript>, IEnumerable
	{
		public int Add(Vector2 position, float mass, float temperature, int frame, BubbleManager.Disease disease)
		{
			int num = this.ManifestIndex();
			BubbleManager.InstanceData.Subscript subscript = this[num];
			subscript.Position = position;
			subscript.ElapsedTime = 0f;
			subscript.Frame = frame;
			subscript.Mass = mass;
			subscript.SizeLevel = this.CalculateAndGetSizeLevel(mass);
			subscript.Temperature = temperature;
			subscript.Alpha = -1f;
			subscript.Disease = disease;
			return num;
		}

		private byte CalculateAndGetSizeLevel(float mass)
		{
			DebugUtil.DevAssert(BubbleManager.InstanceData.MassTresholds != null, "MassTresholds should be statically initialized", null);
			DebugUtil.DevAssert(BubbleManager.InstanceData.MassTresholds.Length != 0, "MassTresholds should be statically initialized", null);
			for (int i = 0; i < BubbleManager.InstanceData.MassTresholds.Length; i++)
			{
				if (BubbleManager.InstanceData.MassTresholds[i] - mass >= 0f)
				{
					return (byte)i;
				}
			}
			return (byte)BubbleManager.InstanceData.MassTresholds.Length;
		}

		public void Destroy(int index)
		{
			this.freeList.Add(index);
			this.frame[index] = -1;
		}

		public void Destroy(List<int> indices)
		{
			this.freeList.AddRange(indices);
			foreach (int num in indices)
			{
				this.frame[num] = -1;
			}
		}

		private int ManifestIndex()
		{
			if (this.freeList.Count > 0)
			{
				List<int> list = this.freeList;
				int num = list[list.Count - 1];
				this.freeList.RemoveAt(this.freeList.Count - 1);
				return num;
			}
			int count = this.position.Count;
			this.position.Add(default(Vector2));
			this.elapsedTime.Add(0f);
			this.frame.Add(0);
			this.temperature.Add(0f);
			this.mass.Add(0f);
			this.sizeLevel.Add(0);
			this.alpha.Add(-1f);
			this.disease.Add(BubbleManager.Disease.None);
			return count;
		}

		public int Begin
		{
			get
			{
				return this.Next(-1);
			}
		}

		public int End
		{
			get
			{
				return this.position.Count;
			}
		}

		public int Next(int index)
		{
			if (index == this.End)
			{
				return this.End;
			}
			for (;;)
			{
				index++;
				if (index == this.End)
				{
					break;
				}
				if (this.frame[index] != -1)
				{
					return index;
				}
			}
			return this.End;
		}

		public int Count
		{
			get
			{
				return this.position.Count - this.freeList.Count;
			}
		}

		[OnDeserialized]
		public void OnDeserialized()
		{
			if (this.disease.Count < this.position.Count)
			{
				this.disease.Capacity = Math.Max(this.disease.Capacity, this.position.Count);
				for (int num = this.disease.Count; num != this.position.Count; num++)
				{
					this.disease.Add(BubbleManager.Disease.None);
				}
			}
			if (this.alpha.Count < this.position.Count)
			{
				this.alpha.Capacity = Math.Max(this.alpha.Capacity, this.position.Count);
				for (int num2 = this.alpha.Count; num2 != this.position.Count; num2++)
				{
					this.alpha.Add(-1f);
				}
			}
		}

		public int CountVisible()
		{
			int num = 0;
			foreach (BubbleManager.InstanceData.Subscript subscript in this)
			{
				if (subscript.Visible)
				{
					num++;
				}
			}
			return num;
		}

		public BubbleManager.InstanceData.Subscript this[int index]
		{
			get
			{
				return new BubbleManager.InstanceData.Subscript(this, index);
			}
		}

		public IEnumerator<BubbleManager.InstanceData.Subscript> GetEnumerator()
		{
			return new BubbleManager.InstanceData.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private const int INVALID_ENTRY = -1;

		private const float FULLY_OPAQUE = -1f;

		public static readonly float[] MassTresholds = new float[] { 0.1f, 0.3f };

		[Serialize]
		private readonly List<Vector2> position = new List<Vector2>();

		[Serialize]
		private readonly List<float> elapsedTime = new List<float>();

		[Serialize]
		private readonly List<int> frame = new List<int>();

		[Serialize]
		private readonly List<float> temperature = new List<float>();

		[Serialize]
		private readonly List<float> mass = new List<float>();

		[Serialize]
		private readonly List<byte> sizeLevel = new List<byte>();

		[Serialize]
		private readonly List<float> alpha = new List<float>();

		[Serialize]
		private readonly List<BubbleManager.Disease> disease = new List<BubbleManager.Disease>();

		[Serialize]
		private readonly List<int> freeList = new List<int>();

		public readonly struct Subscript
		{
			public Subscript(BubbleManager.InstanceData data, int index)
			{
				this.data = data;
				this.index = index;
			}

			public int Index
			{
				get
				{
					return this.index;
				}
			}

			public Vector2 Position
			{
				get
				{
					return this.data.position[this.index];
				}
				set
				{
					this.data.position[this.index] = value;
				}
			}

			public float ElapsedTime
			{
				get
				{
					return this.data.elapsedTime[this.index];
				}
				set
				{
					this.data.elapsedTime[this.index] = value;
				}
			}

			public int Frame
			{
				get
				{
					return this.data.frame[this.index];
				}
				set
				{
					this.data.frame[this.index] = value;
				}
			}

			public float Temperature
			{
				get
				{
					return this.data.temperature[this.index];
				}
				set
				{
					this.data.temperature[this.index] = value;
				}
			}

			public float Mass
			{
				get
				{
					return this.data.mass[this.index];
				}
				set
				{
					this.data.mass[this.index] = value;
				}
			}

			public byte SizeLevel
			{
				get
				{
					return this.data.sizeLevel[this.index];
				}
				set
				{
					this.data.sizeLevel[this.index] = value;
				}
			}

			public bool Visible
			{
				get
				{
					return this.data.alpha[this.index] != 0f;
				}
			}

			public bool FadingOut
			{
				get
				{
					return this.data.alpha[this.index] != -1f;
				}
				set
				{
					DebugUtil.DevAssert(value, "Cannot set FadingOut to false. Once the fade out is begun, it cannot be stopped", null);
					if (value && this.data.alpha[this.index] == -1f)
					{
						this.data.alpha[this.index] = 1f;
					}
				}
			}

			public float Alpha
			{
				get
				{
					float num = this.data.alpha[this.index];
					if (num != -1f)
					{
						return num;
					}
					return 1f;
				}
				set
				{
					this.data.alpha[this.index] = value;
				}
			}

			public BubbleManager.Disease Disease
			{
				get
				{
					return this.data.disease[this.index];
				}
				set
				{
					this.data.disease[this.index] = value;
				}
			}

			private readonly BubbleManager.InstanceData data;

			private readonly int index;
		}

		public struct Enumerator : IEnumerator<BubbleManager.InstanceData.Subscript>, IEnumerator, IDisposable
		{
			public Enumerator(BubbleManager.InstanceData outer)
			{
				this.outer = outer;
				this.index = -1;
			}

			public bool MoveNext()
			{
				this.index = ((this.index == -1) ? this.outer.Begin : this.outer.Next(this.index));
				return this.index != this.outer.End;
			}

			public void Reset()
			{
				this.index = this.outer.Begin;
			}

			readonly void IDisposable.Dispose()
			{
			}

			public readonly BubbleManager.InstanceData.Subscript Current
			{
				get
				{
					return new BubbleManager.InstanceData.Subscript(this.outer, this.index);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private int index;

			private readonly BubbleManager.InstanceData outer;
		}
	}

	public struct CellBubbleInfo
	{
		public SimHashes element;

		public float totalMass;

		public float averageTemperature;
	}
}
