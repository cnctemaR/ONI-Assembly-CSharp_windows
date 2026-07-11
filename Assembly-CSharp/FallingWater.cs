using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using FMOD.Studio;
using FMODUnity;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/FallingWater")]
public class FallingWater : KMonoBehaviour, ISim200ms
{
	public static FallingWater instance
	{
		get
		{
			return FallingWater._instance;
		}
		private set
		{
		}
	}

	public static void DestroyInstance()
	{
		FallingWater._instance = null;
	}

	protected override void OnPrefabInit()
	{
		FallingWater._instance = this;
		base.OnPrefabInit();
		this.mistEffect.SetActive(false);
		this.mistPool = new ObjectPool(new Func<GameObject>(this.InstantiateMist), 16);
	}

	protected override void OnSpawn()
	{
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		this.mesh.name = "FallingWater";
		this.lastSpawnTime = new float[Grid.WidthInCells * Grid.HeightInCells];
		for (int i = 0; i < this.lastSpawnTime.Length; i++)
		{
			this.lastSpawnTime[i] = 0f;
		}
		this.propertyBlock = new MaterialPropertyBlock();
		this.propertyBlock.SetTexture("_MainTex", this.texture);
		this.uvFrameSize = new Vector2(1f / (float)this.numFrames, 1f);
	}

	protected override void OnCleanUp()
	{
		FallingWater.instance = null;
		base.OnCleanUp();
	}

	private float GetTime()
	{
		return Time.time % 360f;
	}

	public void AddParticle(int cell, byte elementIdx, float base_mass, float temperature, byte disease_idx, int base_disease_count, bool skip_sound = false, bool skip_decor = false, bool debug_track = false, bool disable_randomness = false)
	{
		Vector2 vector = Grid.CellToPos2D(cell);
		this.AddParticle(vector, elementIdx, base_mass, temperature, disease_idx, base_disease_count, skip_sound, skip_decor, debug_track, disable_randomness);
	}

	public void AddParticle(Vector2 root_pos, byte elementIdx, float base_mass, float temperature, byte disease_idx, int base_disease_count, bool skip_sound = false, bool skip_decor = false, bool debug_track = false, bool disable_randomness = false)
	{
		int num = Grid.PosToCell(root_pos);
		if (!Grid.IsValidCell(num))
		{
			KCrashReporter.Assert(false, "Trying to add falling water outside of the scene");
			return;
		}
		if (temperature <= 0f || base_mass <= 0f)
		{
			global::Debug.LogError(string.Format("Unexpected water mass/temperature values added to the falling water manager T({0}) M({1})", temperature, base_mass));
		}
		float time = this.GetTime();
		if (!skip_sound)
		{
			FallingWater.SoundInfo soundInfo;
			if (!this.topSounds.TryGetValue(num, out soundInfo))
			{
				soundInfo = default(FallingWater.SoundInfo);
				soundInfo.handle = LoopingSoundManager.StartSound(this.liquid_top_loop, root_pos, true, true);
			}
			soundInfo.startTime = time;
			LoopingSoundManager.Get().UpdateSecondParameter(soundInfo.handle, FallingWater.HASH_LIQUIDVOLUME, SoundUtil.GetLiquidVolume(base_mass));
			this.topSounds[num] = soundInfo;
		}
		int num2 = base_disease_count;
		while (base_mass > 0f)
		{
			float num3 = global::UnityEngine.Random.value * 2f * this.particleMassVariation - this.particleMassVariation;
			float num4 = Mathf.Max(0f, Mathf.Min(base_mass, this.particleMassToSplit + num3));
			float num5 = num4 / base_mass;
			base_mass -= num4;
			int num6 = (int)(num5 * (float)base_disease_count);
			num6 = Mathf.Min(num2, num6);
			num2 = Mathf.Max(0, num2 - num6);
			int num7 = global::UnityEngine.Random.Range(0, this.numFrames);
			Vector2 vector = (disable_randomness ? Vector2.zero : new Vector2(this.jitterStep * Mathf.Sin(this.offset), this.jitterStep * Mathf.Sin(this.offset + 17f)));
			Vector2 vector2 = (disable_randomness ? Vector2.zero : new Vector2(global::UnityEngine.Random.Range(-this.multipleOffsetRange.x, this.multipleOffsetRange.x), global::UnityEngine.Random.Range(-this.multipleOffsetRange.y, this.multipleOffsetRange.y)));
			Element element = ElementLoader.elements[(int)elementIdx];
			Vector2 vector3 = root_pos;
			bool flag = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellLeft(num), false, element);
			bool flag2 = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellRight(num), true, element);
			Vector2 vector4 = Vector2.ClampMagnitude(this.initialOffset + vector + vector2, 1f);
			if (flag || flag2)
			{
				if (flag && flag2)
				{
					vector3 += vector4;
					vector3.x += 0.5f;
				}
				else if (flag)
				{
					vector3 += vector4;
				}
				else
				{
					vector3.x += 1f - vector4.x;
					vector3.y += vector4.y;
				}
			}
			else
			{
				vector3 += vector4;
				vector3.x += 0.5f;
			}
			int num8 = Grid.PosToCell(vector3);
			if ((Grid.Element[num8].state & Element.State.Solid) == Element.State.Solid || (Grid.Properties[num8] & 2) != 0)
			{
				vector3.y = Mathf.Floor(vector3.y + 1f);
			}
			this.physics.Add(new FallingWater.ParticlePhysics(vector3, Vector2.zero, num7, elementIdx));
			this.particleProperties.Add(new FallingWater.ParticleProperties(elementIdx, num4, temperature, disease_idx, num6, debug_track));
		}
	}

	private bool SpawnLiquidTopDecor(float time, int cell, bool flip, Element element)
	{
		if (Grid.IsValidCell(cell) && Grid.Element[cell] == element)
		{
			Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.TileMain);
			if (CameraController.Instance.IsVisiblePos(vector))
			{
				Pair<int, bool> pair = new Pair<int, bool>(cell, flip);
				FallingWater.MistInfo mistInfo;
				if (!this.mistAlive.TryGetValue(pair, out mistInfo))
				{
					mistInfo = default(FallingWater.MistInfo);
					mistInfo.fx = this.SpawnMist();
					mistInfo.fx.TintColour = element.substance.colour;
					Vector3 vector2 = vector + (flip ? (-Vector3.right) : Vector3.right) * 0.5f;
					mistInfo.fx.transform.SetPosition(vector2);
					mistInfo.fx.FlipX = flip;
				}
				mistInfo.deathTime = Time.time + this.mistEffectMinAliveTime;
				this.mistAlive[pair] = mistInfo;
				return true;
			}
		}
		return false;
	}

	public void SpawnLiquidSplash(float x, int cell, byte elementIdx, bool forceSplash = false)
	{
		float time = this.GetTime();
		float num = this.lastSpawnTime[cell];
		if (time - num >= this.minSpawnDelay || forceSplash)
		{
			this.lastSpawnTime[cell] = time;
			Vector2 vector = Grid.CellToPos2D(cell);
			vector.x = x - 0.5f;
			int num2 = global::UnityEngine.Random.Range(0, this.liquid_splash.names.Length);
			Vector2 vector2 = vector + new Vector2(this.liquid_splash.offset.x, this.liquid_splash.offset.y);
			SpriteSheetAnimManager.instance.Play(this.liquid_splash.names[num2], new Vector3(vector2.x, vector2.y, this.renderOffset.z), new Vector2(this.liquid_splash.size.x, this.liquid_splash.size.y), Color.white);
		}
	}

	public void UpdateParticles(float dt)
	{
		if (dt <= 0f || this.simUpdateDelay >= 0)
		{
			return;
		}
		this.offset = (this.offset + dt) % 360f;
		int count = this.physics.Count;
		Vector2 vector = Physics.gravity * dt * this.gravityScale;
		for (int i = 0; i < count; i++)
		{
			FallingWater.ParticlePhysics particlePhysics = this.physics[i];
			Vector3 vector2 = particlePhysics.position;
			int num;
			int num2;
			Grid.PosToXY(vector2, out num, out num2);
			particlePhysics.velocity += vector;
			Vector3 vector3 = particlePhysics.velocity * dt;
			Vector3 vector4 = vector2 + vector3;
			particlePhysics.position = vector4;
			this.physics[i] = particlePhysics;
			int num3;
			int num4;
			Grid.PosToXY(particlePhysics.position, out num3, out num4);
			int num5 = ((num2 > num4) ? num2 : num4);
			int num6 = ((num2 > num4) ? num4 : num2);
			int j = num5;
			while (j >= num6)
			{
				int num7 = j * Grid.WidthInCells + num;
				int num8 = (j + 1) * Grid.WidthInCells + num;
				if (Grid.IsValidCell(num7))
				{
					Element element = Grid.Element[num7];
					Element.State state = element.state & Element.State.Solid;
					bool flag = false;
					if (state == Element.State.Solid || (Grid.Properties[num7] & 2) != 0)
					{
						this.AddToSim(num8, i, ref count);
					}
					else
					{
						switch (state)
						{
						case Element.State.Vacuum:
							if (element.id == SimHashes.Vacuum)
							{
								flag = true;
							}
							else
							{
								this.RemoveParticle(i, ref count);
							}
							break;
						case Element.State.Gas:
							flag = true;
							break;
						case Element.State.Liquid:
						{
							FallingWater.ParticleProperties particleProperties = this.particleProperties[i];
							Element element2 = ElementLoader.elements[(int)particleProperties.elementIdx];
							if (element2.id == element.id)
							{
								if (Grid.Mass[num7] <= element.defaultValues.mass)
								{
									flag = true;
								}
								else
								{
									this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties.elementIdx, false);
									this.AddToSim(num7, i, ref count);
								}
							}
							else if (element2.molarMass > element.molarMass)
							{
								flag = true;
							}
							else
							{
								this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties.elementIdx, false);
								this.AddToSim(num8, i, ref count);
							}
							break;
						}
						}
					}
					if (!flag)
					{
						break;
					}
					j--;
				}
				else
				{
					if (Grid.IsValidCell(num8))
					{
						FallingWater.ParticleProperties particleProperties2 = this.particleProperties[i];
						this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties2.elementIdx, false);
						this.AddToSim(num8, i, ref count);
						break;
					}
					this.RemoveParticle(i, ref count);
					break;
				}
			}
		}
		float time = this.GetTime();
		this.UpdateSounds(time);
		this.UpdateMistFX(Time.time);
	}

	private void UpdateMistFX(float t)
	{
		this.mistClearList.Clear();
		foreach (KeyValuePair<Pair<int, bool>, FallingWater.MistInfo> keyValuePair in this.mistAlive)
		{
			if (t > keyValuePair.Value.deathTime)
			{
				keyValuePair.Value.fx.Play("end", KAnim.PlayMode.Once, 1f, 0f);
				this.mistClearList.Add(keyValuePair.Key);
			}
		}
		foreach (Pair<int, bool> pair in this.mistClearList)
		{
			this.mistAlive.Remove(pair);
		}
		this.mistClearList.Clear();
	}

	private void UpdateSounds(float t)
	{
		this.clearList.Clear();
		foreach (KeyValuePair<int, FallingWater.SoundInfo> keyValuePair in this.topSounds)
		{
			FallingWater.SoundInfo value = keyValuePair.Value;
			if (t - value.startTime >= this.stopTopLoopDelay)
			{
				if (value.handle != HandleVector<int>.InvalidHandle)
				{
					LoopingSoundManager.StopSound(value.handle);
				}
				this.clearList.Add(keyValuePair.Key);
			}
		}
		foreach (int num in this.clearList)
		{
			this.topSounds.Remove(num);
		}
		this.clearList.Clear();
		foreach (KeyValuePair<int, FallingWater.SoundInfo> keyValuePair2 in this.splashSounds)
		{
			FallingWater.SoundInfo value2 = keyValuePair2.Value;
			if (t - value2.startTime >= this.stopSplashLoopDelay)
			{
				if (value2.handle != HandleVector<int>.InvalidHandle)
				{
					LoopingSoundManager.StopSound(value2.handle);
				}
				this.clearList.Add(keyValuePair2.Key);
			}
		}
		foreach (int num2 in this.clearList)
		{
			this.splashSounds.Remove(num2);
		}
		this.clearList.Clear();
	}

	public Dictionary<int, float> GetInfo(int cell)
	{
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		int count = this.physics.Count;
		for (int i = 0; i < count; i++)
		{
			if (Grid.PosToCell(this.physics[i].position) == cell)
			{
				FallingWater.ParticleProperties particleProperties = this.particleProperties[i];
				float num = 0f;
				dictionary.TryGetValue((int)particleProperties.elementIdx, out num);
				num += particleProperties.mass;
				dictionary[(int)particleProperties.elementIdx] = num;
			}
		}
		return dictionary;
	}

	private float GetParticleVolume(float mass)
	{
		return Mathf.Clamp01((mass - (this.particleMassToSplit - this.particleMassVariation)) / (2f * this.particleMassVariation));
	}

	private void AddToSim(int cell, int particleIdx, ref int num_particles)
	{
		bool flag = false;
		for (;;)
		{
			if ((Grid.Element[cell].state & Element.State.Solid) == Element.State.Solid || (Grid.Properties[cell] & 2) != 0)
			{
				cell += Grid.WidthInCells;
				if (!Grid.IsValidCell(cell))
				{
					break;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				goto Block_3;
			}
		}
		return;
		Block_3:
		FallingWater.ParticleProperties particleProperties = this.particleProperties[particleIdx];
		SimMessages.AddRemoveSubstance(cell, (int)particleProperties.elementIdx, CellEventLogger.Instance.FallingWaterAddToSim, particleProperties.mass, particleProperties.temperature, particleProperties.diseaseIdx, particleProperties.diseaseCount, true, -1);
		this.RemoveParticle(particleIdx, ref num_particles);
		float time = this.GetTime();
		float num = this.lastSpawnTime[cell];
		if (time - num >= this.minSpawnDelay)
		{
			this.lastSpawnTime[cell] = time;
			Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
			vector.z = 0f;
			if (CameraController.Instance.IsAudibleSound(vector))
			{
				bool flag2 = true;
				FallingWater.SoundInfo soundInfo;
				if (this.splashSounds.TryGetValue(cell, out soundInfo))
				{
					soundInfo.splashCount++;
					if (soundInfo.splashCount > this.splashCountLoopThreshold)
					{
						if (soundInfo.handle == HandleVector<int>.InvalidHandle)
						{
							soundInfo.handle = LoopingSoundManager.StartSound(this.liquid_splash_loop, vector, true, true);
						}
						LoopingSoundManager.Get().UpdateFirstParameter(soundInfo.handle, FallingWater.HASH_LIQUIDDEPTH, SoundUtil.GetLiquidDepth(cell));
						LoopingSoundManager.Get().UpdateSecondParameter(soundInfo.handle, FallingWater.HASH_LIQUIDVOLUME, this.GetParticleVolume(particleProperties.mass));
						flag2 = false;
					}
				}
				else
				{
					soundInfo = default(FallingWater.SoundInfo);
					soundInfo.handle = HandleVector<int>.InvalidHandle;
				}
				soundInfo.startTime = time;
				this.splashSounds[cell] = soundInfo;
				if (flag2)
				{
					EventInstance eventInstance = SoundEvent.BeginOneShot(this.liquid_splash_initial, vector, 1f, false);
					eventInstance.setParameterByName("liquidDepth", SoundUtil.GetLiquidDepth(cell), false);
					eventInstance.setParameterByName("liquidVolume", this.GetParticleVolume(particleProperties.mass), false);
					SoundEvent.EndOneShot(eventInstance);
				}
			}
		}
	}

	private void RemoveParticle(int particleIdx, ref int num_particles)
	{
		num_particles--;
		this.physics[particleIdx] = this.physics[num_particles];
		this.particleProperties[particleIdx] = this.particleProperties[num_particles];
		this.physics.RemoveAt(num_particles);
		this.particleProperties.RemoveAt(num_particles);
	}

	public void Render()
	{
		List<Vector3> vertices = MeshUtil.vertices;
		List<Color32> colours = MeshUtil.colours32;
		List<Vector2> uvs = MeshUtil.uvs;
		List<int> indices = MeshUtil.indices;
		uvs.Clear();
		vertices.Clear();
		indices.Clear();
		colours.Clear();
		float num = this.particleSize.x * 0.5f;
		float num2 = this.particleSize.y * 0.5f;
		Vector2 vector = new Vector2(-num, -num2);
		Vector2 vector2 = new Vector2(num, -num2);
		Vector2 vector3 = new Vector2(num, num2);
		Vector2 vector4 = new Vector2(-num, num2);
		float num3 = 1f;
		float num4 = 0f;
		int num5 = Mathf.Min(this.physics.Count, 16249);
		if (num5 < this.physics.Count)
		{
			DebugUtil.LogWarningArgs(new object[]
			{
				"Too many water particles to render. Wanted",
				this.physics.Count,
				"but truncating to limit"
			});
		}
		for (int i = 0; i < num5; i++)
		{
			Vector2 position = this.physics[i].position;
			float num6 = Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(this.particleProperties[i].mass / this.particleMassToSplit));
			vertices.Add(position + vector * num6);
			vertices.Add(position + vector2 * num6);
			vertices.Add(position + vector3 * num6);
			vertices.Add(position + vector4 * num6);
			int frame = this.physics[i].frame;
			float num7 = (float)frame * this.uvFrameSize.x;
			float num8 = (float)(frame + 1) * this.uvFrameSize.x;
			uvs.Add(new Vector2(num7, num4));
			uvs.Add(new Vector2(num8, num4));
			uvs.Add(new Vector2(num8, num3));
			uvs.Add(new Vector2(num7, num3));
			Color32 colour = this.physics[i].colour;
			colours.Add(colour);
			colours.Add(colour);
			colours.Add(colour);
			colours.Add(colour);
			int num9 = i * 4;
			indices.Add(num9);
			indices.Add(num9 + 1);
			indices.Add(num9 + 2);
			indices.Add(num9);
			indices.Add(num9 + 2);
			indices.Add(num9 + 3);
		}
		this.mesh.Clear();
		this.mesh.SetVertices(vertices);
		this.mesh.SetUVs(0, uvs);
		this.mesh.SetColors(colours);
		this.mesh.SetTriangles(indices, 0);
		int num10 = LayerMask.NameToLayer("Water");
		Graphics.DrawMesh(this.mesh, this.renderOffset, Quaternion.identity, this.material, num10, null, 0, this.propertyBlock);
	}

	private KBatchedAnimController SpawnMist()
	{
		GameObject instance = this.mistPool.GetInstance();
		instance.SetActive(true);
		KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
		component.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		return component;
	}

	private GameObject InstantiateMist()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.mistEffect, Grid.SceneLayer.BuildingBack, null, 0);
		gameObject.SetActive(false);
		gameObject.GetComponent<KBatchedAnimController>().onDestroySelf = new Action<GameObject>(this.ReleaseMist);
		return gameObject;
	}

	private void ReleaseMist(GameObject go)
	{
		go.SetActive(false);
		this.mistPool.ReleaseInstance(go);
	}

	public void Sim200ms(float dt)
	{
		if (this.simUpdateDelay >= 0)
		{
			this.simUpdateDelay--;
			return;
		}
		SimAndRenderScheduler.instance.Remove(this);
	}

	[OnSerializing]
	private void OnSerializing()
	{
		List<Element> elements = ElementLoader.elements;
		Diseases diseases = Db.Get().Diseases;
		this.serializedParticleProperties = new List<FallingWater.SerializedParticleProperties>();
		foreach (FallingWater.ParticleProperties particleProperties in this.particleProperties)
		{
			FallingWater.SerializedParticleProperties serializedParticleProperties = default(FallingWater.SerializedParticleProperties);
			serializedParticleProperties.elementID = elements[(int)particleProperties.elementIdx].id;
			serializedParticleProperties.diseaseID = ((particleProperties.diseaseIdx != byte.MaxValue) ? diseases[(int)particleProperties.diseaseIdx].IdHash : HashedString.Invalid);
			serializedParticleProperties.mass = particleProperties.mass;
			serializedParticleProperties.temperature = particleProperties.temperature;
			serializedParticleProperties.diseaseCount = particleProperties.diseaseCount;
			this.serializedParticleProperties.Add(serializedParticleProperties);
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		this.serializedParticleProperties = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializedParticleProperties != null)
		{
			Diseases diseases = Db.Get().Diseases;
			this.particleProperties.Clear();
			using (List<FallingWater.SerializedParticleProperties>.Enumerator enumerator = this.serializedParticleProperties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FallingWater.SerializedParticleProperties serializedParticleProperties = enumerator.Current;
					FallingWater.ParticleProperties particleProperties = default(FallingWater.ParticleProperties);
					particleProperties.elementIdx = (byte)ElementLoader.GetElementIndex(serializedParticleProperties.elementID);
					particleProperties.diseaseIdx = ((serializedParticleProperties.diseaseID != HashedString.Invalid) ? diseases.GetIndex(serializedParticleProperties.diseaseID) : byte.MaxValue);
					particleProperties.mass = serializedParticleProperties.mass;
					particleProperties.temperature = serializedParticleProperties.temperature;
					particleProperties.diseaseCount = serializedParticleProperties.diseaseCount;
					this.particleProperties.Add(particleProperties);
				}
				goto IL_00DC;
			}
		}
		this.particleProperties = this.properties;
		IL_00DC:
		this.properties = null;
	}

	private const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

	private const byte FORCED_ALPHA = 191;

	private int simUpdateDelay = 2;

	[SerializeField]
	private Vector2 particleSize;

	[SerializeField]
	private Vector2 initialOffset;

	[SerializeField]
	private float jitterStep;

	[SerializeField]
	private Vector3 renderOffset;

	[SerializeField]
	private float minSpawnDelay;

	[SerializeField]
	private float gravityScale = 0.05f;

	[SerializeField]
	private float particleMassToSplit = 75f;

	[SerializeField]
	private float particleMassVariation = 15f;

	[SerializeField]
	private Vector2 multipleOffsetRange;

	[SerializeField]
	private GameObject mistEffect;

	[SerializeField]
	private float mistEffectMinAliveTime = 2f;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Texture2D texture;

	[SerializeField]
	private int numFrames;

	[SerializeField]
	private FallingWater.DecorInfo liquid_splash;

	[SerializeField]
	[EventRef]
	private string liquid_top_loop;

	[SerializeField]
	[EventRef]
	private string liquid_splash_initial;

	[SerializeField]
	[EventRef]
	private string liquid_splash_loop;

	[SerializeField]
	private float stopTopLoopDelay = 0.2f;

	[SerializeField]
	private float stopSplashLoopDelay = 1f;

	[SerializeField]
	private int splashCountLoopThreshold = 10;

	[Serialize]
	private List<FallingWater.ParticlePhysics> physics = new List<FallingWater.ParticlePhysics>();

	private List<FallingWater.ParticleProperties> particleProperties = new List<FallingWater.ParticleProperties>();

	[Serialize]
	private List<FallingWater.SerializedParticleProperties> serializedParticleProperties;

	[Serialize]
	private List<FallingWater.ParticleProperties> properties = new List<FallingWater.ParticleProperties>();

	private Dictionary<int, FallingWater.SoundInfo> topSounds = new Dictionary<int, FallingWater.SoundInfo>();

	private Dictionary<int, FallingWater.SoundInfo> splashSounds = new Dictionary<int, FallingWater.SoundInfo>();

	private ObjectPool mistPool;

	private Mesh mesh;

	private float offset;

	private float[] lastSpawnTime;

	private Dictionary<Pair<int, bool>, FallingWater.MistInfo> mistAlive = new Dictionary<Pair<int, bool>, FallingWater.MistInfo>();

	private Vector2 uvFrameSize;

	private MaterialPropertyBlock propertyBlock;

	private static FallingWater _instance;

	private List<int> clearList = new List<int>();

	private List<Pair<int, bool>> mistClearList = new List<Pair<int, bool>>();

	private static HashedString HASH_LIQUIDDEPTH = "liquidDepth";

	private static HashedString HASH_LIQUIDVOLUME = "liquidVolume";

	[Serializable]
	private struct DecorInfo
	{
		public string[] names;

		public Vector2 offset;

		public Vector2 size;
	}

	private struct SoundInfo
	{
		public float startTime;

		public int splashCount;

		public HandleVector<int>.Handle handle;
	}

	private struct MistInfo
	{
		public KBatchedAnimController fx;

		public float deathTime;
	}

	private struct ParticlePhysics
	{
		public ParticlePhysics(Vector2 position, Vector2 velocity, int frame, byte elementIdx)
		{
			this.position = position;
			this.velocity = velocity;
			this.frame = frame;
			this.colour = ElementLoader.elements[(int)elementIdx].substance.colour;
			this.colour.a = 191;
		}

		public Vector2 position;

		public Vector2 velocity;

		public int frame;

		public Color32 colour;
	}

	private struct SerializedParticleProperties
	{
		public SimHashes elementID;

		public HashedString diseaseID;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	private struct ParticleProperties
	{
		public ParticleProperties(byte elementIdx, float mass, float temperature, byte disease_idx, int disease_count, bool debug_track)
		{
			this.elementIdx = elementIdx;
			this.diseaseIdx = disease_idx;
			this.mass = mass;
			this.temperature = temperature;
			this.diseaseCount = disease_count;
		}

		public byte elementIdx;

		public byte diseaseIdx;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}
}
