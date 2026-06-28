using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class FallingWater : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		FallingWater.instance = this;
		base.OnPrefabInit();
		this.mistEffect.SetActive(false);
		this.mistPool = new ObjectPool(new Func<GameObject>(this.InstantiateMist), 16);
	}

	protected override void OnCleanUp()
	{
		FallingWater.instance = null;
		base.OnCleanUp();
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

	private float GetTime()
	{
		return Time.time % 360f;
	}

	public void AddParticle(int cell, byte elementIdx, float base_mass, float temperature, bool skip_sound = false, bool skip_decor = false, bool debug_track = false)
	{
		if (!Grid.IsValidCell(cell))
		{
			KCrashReporter.Assert(false, "Trying to add falling water outside of the scene");
			return;
		}
		if (temperature <= 0f || base_mass <= 0f)
		{
			Output.LogError(new object[] { "Unexpected water mass/temperature values added to the falling water manager" });
		}
		Vector2 vector = Grid.CellToPos2D(cell);
		float time = this.GetTime();
		if (!skip_sound)
		{
			FallingWater.SoundInfo soundInfo;
			if (!this.topSounds.TryGetValue(cell, out soundInfo))
			{
				soundInfo = default(FallingWater.SoundInfo);
				soundInfo.eventInstance = LoopingSoundManager.StartSound(this.liquid_top_loop, vector, true);
			}
			soundInfo.startTime = time;
			soundInfo.eventInstance.setParameterValue("liquidVolume", SoundUtil.GetLiquidVolume(base_mass));
			this.topSounds[cell] = soundInfo;
		}
		while (base_mass > 0f)
		{
			float num = global::UnityEngine.Random.value * 2f * this.particleMassVariation - this.particleMassVariation;
			float num2 = Mathf.Max(0f, Mathf.Min(base_mass, this.particleMassToSplit + num));
			base_mass -= num2;
			int num3 = global::UnityEngine.Random.Range(0, this.numFrames);
			Vector2 vector2 = new Vector2(this.jitterStep * Mathf.Sin(this.offset), this.jitterStep * Mathf.Sin(this.offset + 17f));
			Vector2 vector3 = new Vector2(global::UnityEngine.Random.Range(-this.multipleOffsetRange.x, this.multipleOffsetRange.x), global::UnityEngine.Random.Range(-this.multipleOffsetRange.y, this.multipleOffsetRange.y));
			Element element = ElementLoader.elements[(int)elementIdx];
			Vector2 vector4 = vector;
			bool flag = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellLeft(cell), false, element, num2);
			bool flag2 = !skip_decor && this.SpawnLiquidTopDecor(time, Grid.CellRight(cell), true, element, num2);
			Vector2 vector5 = Vector2.ClampMagnitude(this.initialOffset + vector2 + vector3, 1f);
			if (flag || flag2)
			{
				if (flag && flag2)
				{
					vector4 += vector5;
					vector4.x += 0.5f;
				}
				else if (flag)
				{
					vector4 += vector5;
				}
				else
				{
					vector4.x += 1f - vector5.x;
					vector4.y += vector5.y;
				}
			}
			else
			{
				vector4 += vector5;
				vector4.x += 0.5f;
			}
			int num4 = Grid.PosToCell(vector4);
			Element element2 = Grid.Element[num4];
			Element.State state = element2.state & Element.State.Solid;
			if (state == Element.State.Solid || (Grid.Cell[num4].properties & 2) != 0)
			{
				vector4.y = Mathf.Floor(vector4.y + 1f);
			}
			this.physics.Add(new FallingWater.ParticlePhysics(vector4, Vector2.zero, num3, elementIdx));
			this.properties.Add(new FallingWater.ParticleProperties(elementIdx, num2, temperature, debug_track));
		}
	}

	private bool SpawnLiquidTopDecor(float time, int cell, bool flip, Element element, float mass)
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
					Vector3 vector2 = vector + ((!flip) ? Vector3.right : (-Vector3.right)) * 0.5f;
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
			int num5 = ((num2 <= num4) ? num4 : num2);
			int num6 = ((num2 <= num4) ? num2 : num4);
			for (int j = num5; j >= num6; j--)
			{
				int num7 = j * Grid.WidthInCells + num;
				int num8 = (j + 1) * Grid.WidthInCells + num;
				if (!Grid.IsValidCell(num7))
				{
					if (Grid.IsValidCell(num8))
					{
						FallingWater.ParticleProperties particleProperties = this.properties[i];
						this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties.elementIdx, false);
						this.AddToSim(num8, i, ref count);
					}
					else
					{
						this.RemoveParticle(i, ref count);
						Output.LogError(new object[] { "WTF" });
					}
					break;
				}
				Element element = Grid.Element[num7];
				Element.State state = element.state & Element.State.Solid;
				bool flag = false;
				if (state == Element.State.Solid || (Grid.Cell[num7].properties & 2) != 0)
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
						FallingWater.ParticleProperties particleProperties2 = this.properties[i];
						Element element2 = ElementLoader.elements[(int)particleProperties2.elementIdx];
						if (element2.id == element.id)
						{
							if (Grid.Cell[num7].mass <= element.defaultValues.mass)
							{
								flag = true;
							}
							else
							{
								this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties2.elementIdx, false);
								this.AddToSim(num7, i, ref count);
							}
						}
						else if (element2.molarMass > element.molarMass)
						{
							flag = true;
						}
						else
						{
							this.SpawnLiquidSplash(particlePhysics.position.x, num8, particleProperties2.elementIdx, false);
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
			float num = t - value.startTime;
			if (num >= this.stopTopLoopDelay)
			{
				LoopingSoundManager.StopSound(this.liquid_top_loop, value.eventInstance);
				this.clearList.Add(keyValuePair.Key);
			}
		}
		foreach (int num2 in this.clearList)
		{
			this.topSounds.Remove(num2);
		}
		this.clearList.Clear();
		foreach (KeyValuePair<int, FallingWater.SoundInfo> keyValuePair2 in this.splashSounds)
		{
			FallingWater.SoundInfo value2 = keyValuePair2.Value;
			if (value2.eventInstance != null)
			{
				float num3 = t - value2.startTime;
				if (num3 >= this.stopSplashLoopDelay)
				{
					LoopingSoundManager.StopSound(this.liquid_splash_loop, value2.eventInstance);
					this.clearList.Add(keyValuePair2.Key);
				}
			}
		}
		foreach (int num4 in this.clearList)
		{
			this.splashSounds.Remove(num4);
		}
		this.clearList.Clear();
	}

	public Dictionary<int, float> GetInfo(int cell)
	{
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		int count = this.physics.Count;
		for (int i = 0; i < count; i++)
		{
			int num = Grid.PosToCell(this.physics[i].position);
			if (num == cell)
			{
				FallingWater.ParticleProperties particleProperties = this.properties[i];
				float num2 = 0f;
				dictionary.TryGetValue((int)particleProperties.elementIdx, out num2);
				num2 += particleProperties.mass;
				dictionary[(int)particleProperties.elementIdx] = num2;
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
		FallingWater.ParticleProperties particleProperties = this.properties[particleIdx];
		SimMessages.AddRemoveSubstance(cell, (int)particleProperties.elementIdx, CellEventLogger.Instance.FallingWaterAddToSim, particleProperties.mass, particleProperties.temperature, -1);
		this.RemoveParticle(particleIdx, ref num_particles);
		float time = this.GetTime();
		float num = this.lastSpawnTime[cell];
		if (time - num >= this.minSpawnDelay)
		{
			this.lastSpawnTime[cell] = time;
			Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
			if (CameraController.Instance.IsAudibleSound(vector, 0f))
			{
				bool flag = true;
				FallingWater.SoundInfo soundInfo;
				if (this.splashSounds.TryGetValue(cell, out soundInfo))
				{
					soundInfo.splashCount++;
					if (soundInfo.splashCount > this.splashCountLoopThreshold)
					{
						if (soundInfo.eventInstance == null)
						{
							soundInfo.eventInstance = LoopingSoundManager.StartSound(this.liquid_splash_loop, vector, true);
						}
						soundInfo.eventInstance.setParameterValue("liquidDepth", SoundUtil.GetLiquidDepth(cell));
						soundInfo.eventInstance.setParameterValue("liquidVolume", this.GetParticleVolume(particleProperties.mass));
						flag = false;
					}
				}
				else
				{
					soundInfo = default(FallingWater.SoundInfo);
				}
				soundInfo.startTime = time;
				this.splashSounds[cell] = soundInfo;
				if (flag)
				{
					EventInstance eventInstance = SoundEvent.BeginOneShot(this.liquid_splash_initial, vector);
					eventInstance.setParameterValue("liquidDepth", SoundUtil.GetLiquidDepth(cell));
					eventInstance.setParameterValue("liquidVolume", this.GetParticleVolume(particleProperties.mass));
					SoundEvent.EndOneShot(eventInstance);
				}
			}
		}
	}

	private void RemoveParticle(int particleIdx, ref int num_particles)
	{
		num_particles--;
		this.physics[particleIdx] = this.physics[num_particles];
		this.properties[particleIdx] = this.properties[num_particles];
		this.physics.RemoveAt(num_particles);
		this.properties.RemoveAt(num_particles);
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
			Output.LogWarning(new object[]
			{
				"Too many water particles to render. Wanted",
				this.physics.Count,
				"but truncating to limit"
			});
		}
		for (int i = 0; i < num5; i++)
		{
			Vector2 position = this.physics[i].position;
			float num6 = Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(this.properties[i].mass / this.particleMassToSplit));
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
		GameObject gameObject = this.mistPool.GetInstance();
		gameObject.SetActive(true);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.Play("loop", KAnim.PlayMode.Loop, 1f, 0f);
		return component;
	}

	private GameObject InstantiateMist()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.mistEffect, Grid.SceneLayer.BuildingBack, Folder.FX, null, 0);
		gameObject.SetActive(false);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.onDestroySelf = new Action<GameObject>(this.ReleaseMist);
		return gameObject;
	}

	private void ReleaseMist(GameObject go)
	{
		go.SetActive(false);
		this.mistPool.ReleaseInstance(go);
	}

	private void SimUpdate(float dt)
	{
		if (this.simUpdateDelay >= 0)
		{
			this.simUpdateDelay--;
		}
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

	public static FallingWater instance;

	private List<int> clearList = new List<int>();

	private List<Pair<int, bool>> mistClearList = new List<Pair<int, bool>>();

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

		public EventInstance eventInstance;
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

	private struct ParticleProperties
	{
		public ParticleProperties(byte elementIdx, float mass, float temperature, bool debug_track)
		{
			this.elementIdx = elementIdx;
			this.mass = mass;
			this.temperature = temperature;
		}

		public byte elementIdx;

		public float mass;

		public float temperature;
	}
}
