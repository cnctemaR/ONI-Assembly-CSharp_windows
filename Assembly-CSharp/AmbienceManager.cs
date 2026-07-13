using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AmbienceManager")]
public class AmbienceManager : KMonoBehaviour
{
	public static float BoilingTreshold { get; private set; } = 1f;

	protected override void OnSpawn()
	{
		if (!RuntimeManager.IsInitialized)
		{
			base.enabled = false;
			return;
		}
		AmbienceManager.BoilingTreshold = this.LiquidMaterial.GetFloat("_BoilingTreshold");
		for (int i = 0; i < this.quadrants.Length; i++)
		{
			this.quadrants[i] = new AmbienceManager.Quadrant(this.quadrantDefs[i]);
		}
	}

	protected override void OnForcedCleanUp()
	{
		AmbienceManager.Quadrant[] array = this.quadrants;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (AmbienceManager.Layer layer in array[i].GetAllLayers())
			{
				layer.Stop();
			}
		}
	}

	private void LateUpdate()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I min = visibleArea.Min;
		Vector2I max = visibleArea.Max;
		Vector2I vector2I = min + (max - min) / 2;
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		Vector3 vector3 = vector2 + (vector - vector2) / 2f;
		Vector3 vector4 = vector - vector2;
		if (vector4.x > vector4.y)
		{
			vector4.y = vector4.x;
		}
		else
		{
			vector4.x = vector4.y;
		}
		vector = vector3 + vector4 / 2f;
		vector2 = vector3 - vector4 / 2f;
		Vector3 vector5 = vector4 / 2f / 2f;
		this.quadrants[0].Update(new Vector2I(min.x, min.y), new Vector2I(vector2I.x, vector2I.y), new Vector3(vector2.x + vector5.x, vector2.y + vector5.y, this.emitterZPosition));
		this.quadrants[1].Update(new Vector2I(vector2I.x, min.y), new Vector2I(max.x, vector2I.y), new Vector3(vector3.x + vector5.x, vector2.y + vector5.y, this.emitterZPosition));
		this.quadrants[2].Update(new Vector2I(min.x, vector2I.y), new Vector2I(vector2I.x, max.y), new Vector3(vector2.x + vector5.x, vector3.y + vector5.y, this.emitterZPosition));
		this.quadrants[3].Update(new Vector2I(vector2I.x, vector2I.y), new Vector2I(max.x, max.y), new Vector3(vector3.x + vector5.x, vector3.y + vector5.y, this.emitterZPosition));
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < this.quadrants.Length; i++)
		{
			num += (float)this.quadrants[i].spaceLayer.tileCount;
			num2 += (float)this.quadrants[i].facilityLayer.tileCount;
			num3 += (float)this.quadrants[i].totalTileCount;
		}
		AudioMixer.instance.UpdateSpaceVisibleSnapshot(num / num3);
		AudioMixer.instance.UpdateFacilityVisibleSnapshot(num2 / num3);
	}

	public Material LiquidMaterial;

	private float emitterZPosition;

	public AmbienceManager.QuadrantDef[] quadrantDefs;

	public AmbienceManager.Quadrant[] quadrants = new AmbienceManager.Quadrant[4];

	public class Tuning : TuningData<AmbienceManager.Tuning>
	{
		public int backwallTileValue = 1;

		public int foundationTileValue = 2;

		public int buildingTileValue = 3;
	}

	public class LiquidLayer : AmbienceManager.Layer
	{
		public LiquidLayer(EventReference sound, EventReference one_shot_sound = default(EventReference))
			: base(sound, one_shot_sound)
		{
		}

		public override void Reset()
		{
			base.Reset();
			this.boilingTileCount = 0;
			this.averageBoilIntensity = 0f;
		}

		public override void UpdatePercentage(int cell_count)
		{
			base.UpdatePercentage(cell_count);
			this.boilTilePercentage = (float)this.boilingTileCount / (float)cell_count;
		}

		public override void UpdateParameters(Vector3 emitter_position)
		{
			base.UpdateParameters(emitter_position);
			this.soundEvent.setParameterByName("Boiling_Tile_Percentage", this.boilTilePercentage, false);
		}

		public override void UpdateAverageTemperature()
		{
			base.UpdateAverageTemperature();
			this.UpdateAverageBoilIntensity();
		}

		public void UpdateAverageBoilIntensity()
		{
			this.averageBoilIntensity = ((this.tileCount > 0) ? (this.averageBoilIntensity / (float)this.tileCount) : 0f);
			this.soundEvent.setParameterByName("Boiling_Intensity", this.averageBoilIntensity, false);
		}

		private const string BOILING_INTENSITY_ID = "Boiling_Intensity";

		private const string BOILING_TILE_PERCENTAGE_ID = "Boiling_Tile_Percentage";

		public int boilingTileCount;

		public float boilTilePercentage;

		public float averageBoilIntensity;
	}

	public class Layer : IComparable<AmbienceManager.Layer>
	{
		public Layer(EventReference sound, EventReference one_shot_sound = default(EventReference))
		{
			this.sound = sound;
			this.oneShotSound = one_shot_sound;
		}

		public virtual void Reset()
		{
			this.tileCount = 0;
			this.averageTemperature = 0f;
			this.averageRadiation = 0f;
		}

		public virtual void UpdatePercentage(int cell_count)
		{
			this.tilePercentage = (float)this.tileCount / (float)cell_count;
		}

		public virtual void UpdateAverageTemperature()
		{
			this.averageTemperature /= (float)this.tileCount;
			this.soundEvent.setParameterByName("averageTemperature", this.averageTemperature, false);
		}

		public void UpdateAverageRadiation()
		{
			this.averageRadiation = ((this.tileCount > 0) ? (this.averageRadiation / (float)this.tileCount) : 0f);
			this.soundEvent.setParameterByName("averageRadiation", this.averageRadiation, false);
		}

		public virtual void UpdateParameters(Vector3 emitter_position)
		{
			if (!this.soundEvent.isValid())
			{
				return;
			}
			Vector3 vector = new Vector3(emitter_position.x, emitter_position.y, 0f);
			this.soundEvent.set3DAttributes(vector.To3DAttributes());
			this.soundEvent.setParameterByName("tilePercentage", this.tilePercentage, false);
		}

		public void SetCustomParameter(string parameterName, float value)
		{
			this.soundEvent.setParameterByName(parameterName, value, false);
		}

		public int CompareTo(AmbienceManager.Layer layer)
		{
			return layer.tileCount - this.tileCount;
		}

		public void SetVolume(float volume)
		{
			if (this.volume != volume)
			{
				this.volume = volume;
				if (this.soundEvent.isValid())
				{
					this.soundEvent.setVolume(volume);
				}
			}
		}

		public void Stop()
		{
			if (this.soundEvent.isValid())
			{
				this.soundEvent.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.soundEvent.release();
			}
			this.isRunning = false;
		}

		public void Start(Vector3 emitter_position)
		{
			if (!this.isRunning)
			{
				if (!this.oneShotSound.IsNull)
				{
					EventInstance eventInstance = KFMOD.CreateInstance(this.oneShotSound);
					if (!eventInstance.isValid())
					{
						string text = "Could not find event: ";
						EventReference eventReference = this.oneShotSound;
						global::Debug.LogWarning(text + eventReference.ToString());
						return;
					}
					ATTRIBUTES_3D attributes_3D = new Vector3(emitter_position.x, emitter_position.y, 0f).To3DAttributes();
					eventInstance.set3DAttributes(attributes_3D);
					eventInstance.setVolume(this.tilePercentage * 2f);
					eventInstance.start();
					eventInstance.release();
					return;
				}
				else
				{
					this.soundEvent = KFMOD.CreateInstance(this.sound);
					if (this.soundEvent.isValid())
					{
						this.soundEvent.start();
					}
					this.isRunning = true;
				}
			}
		}

		private const string TILE_PERCENTAGE_ID = "tilePercentage";

		private const string AVERAGE_TEMPERATURE_ID = "averageTemperature";

		private const string AVERAGE_RADIATION_ID = "averageRadiation";

		public EventReference sound;

		public EventReference oneShotSound;

		public int tileCount;

		public float tilePercentage;

		public float volume;

		public bool isRunning;

		protected EventInstance soundEvent;

		public float averageTemperature;

		public float averageRadiation;
	}

	[Serializable]
	public class QuadrantDef
	{
		public string name;

		public EventReference[] liquidSounds;

		public EventReference[] gasSounds;

		public EventReference[] solidSounds;

		public EventReference fogSound;

		public EventReference spaceSound;

		public EventReference rocketInteriorSound;

		public EventReference facilitySound;

		public EventReference radiationSound;
	}

	public class Quadrant
	{
		public Quadrant(AmbienceManager.QuadrantDef def)
		{
			this.name = def.name;
			this.fogLayer = new AmbienceManager.Layer(def.fogSound, default(EventReference));
			this.allLayers.Add(this.fogLayer);
			this.loopingLayers.Add(this.fogLayer);
			this.spaceLayer = new AmbienceManager.Layer(def.spaceSound, default(EventReference));
			this.allLayers.Add(this.spaceLayer);
			this.loopingLayers.Add(this.spaceLayer);
			this.m_isClusterSpaceEnabled = DlcManager.FeatureClusterSpaceEnabled();
			if (this.m_isClusterSpaceEnabled)
			{
				this.rocketInteriorLayer = new AmbienceManager.Layer(def.rocketInteriorSound, default(EventReference));
				this.allLayers.Add(this.rocketInteriorLayer);
			}
			this.facilityLayer = new AmbienceManager.Layer(def.facilitySound, default(EventReference));
			this.allLayers.Add(this.facilityLayer);
			this.loopingLayers.Add(this.facilityLayer);
			this.m_isRadiationEnabled = Sim.IsRadiationEnabled();
			if (this.m_isRadiationEnabled)
			{
				this.radiationLayer = new AmbienceManager.Layer(def.radiationSound, default(EventReference));
				this.allLayers.Add(this.radiationLayer);
			}
			for (int i = 0; i < 4; i++)
			{
				this.gasLayers[i] = new AmbienceManager.Layer(def.gasSounds[i], default(EventReference));
				this.liquidLayers[i] = new AmbienceManager.LiquidLayer(def.liquidSounds[i], default(EventReference));
				this.allLayers.Add(this.gasLayers[i]);
				this.allLayers.Add(this.liquidLayers[i]);
				this.loopingLayers.Add(this.gasLayers[i]);
				this.loopingLayers.Add(this.liquidLayers[i]);
			}
			for (int j = 0; j < this.solidLayers.Length; j++)
			{
				if (j >= def.solidSounds.Length)
				{
					string text = "Missing solid layer: ";
					SolidAmbienceType solidAmbienceType = (SolidAmbienceType)j;
					global::Debug.LogError(text + solidAmbienceType.ToString());
				}
				this.solidLayers[j] = new AmbienceManager.Layer(default(EventReference), def.solidSounds[j]);
				this.allLayers.Add(this.solidLayers[j]);
				this.oneShotLayers.Add(this.solidLayers[j]);
			}
			this.solidTimers = new AmbienceManager.Quadrant.SolidTimer[AmbienceManager.Quadrant.activeSolidLayerCount];
			for (int k = 0; k < AmbienceManager.Quadrant.activeSolidLayerCount; k++)
			{
				this.solidTimers[k] = new AmbienceManager.Quadrant.SolidTimer();
			}
		}

		public void Update(Vector2I min, Vector2I max, Vector3 emitter_position)
		{
			this.emitterPosition = emitter_position;
			this.totalTileCount = 0;
			for (int i = 0; i < this.allLayers.Count; i++)
			{
				this.allLayers[i].Reset();
			}
			float num = 1f - AmbienceManager.BoilingTreshold;
			for (int j = min.y; j < max.y; j++)
			{
				if (j % 2 != 1)
				{
					for (int k = min.x; k < max.x; k++)
					{
						if (k % 2 != 0)
						{
							int num2 = Grid.XYToCell(k, j);
							if (Grid.IsValidCell(num2))
							{
								this.totalTileCount++;
								if (Grid.IsVisible(num2))
								{
									if (Grid.GravitasFacility[num2])
									{
										this.facilityLayer.tileCount += 8;
									}
									else
									{
										Element element = Grid.Element[num2];
										if (element != null)
										{
											if (element.IsLiquid && Grid.IsSubstantialLiquid(num2, 0.35f))
											{
												AmbienceType ambience = element.substance.GetAmbience();
												if (ambience != AmbienceType.None)
												{
													this.liquidLayers[(int)ambience].tileCount++;
													this.liquidLayers[(int)ambience].averageTemperature += Grid.Temperature[num2];
													float num3 = Mathf.Clamp01(element.GetRelativeHeatLevel(Grid.Temperature[num2]) - AmbienceManager.BoilingTreshold) / num;
													this.liquidLayers[(int)ambience].boilingTileCount += ((num3 > 0f) ? 1 : 0);
													this.liquidLayers[(int)ambience].averageBoilIntensity += num3;
												}
											}
											else if (element.IsGas)
											{
												AmbienceType ambience2 = element.substance.GetAmbience();
												if (ambience2 != AmbienceType.None)
												{
													this.gasLayers[(int)ambience2].tileCount++;
													this.gasLayers[(int)ambience2].averageTemperature += Grid.Temperature[num2];
												}
											}
											else if (element.IsSolid)
											{
												SolidAmbienceType solidAmbienceType = element.substance.GetSolidAmbience();
												if (Grid.Foundation[num2])
												{
													solidAmbienceType = SolidAmbienceType.Tile;
													this.solidLayers[(int)solidAmbienceType].tileCount += TuningData<AmbienceManager.Tuning>.Get().foundationTileValue;
													this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().foundationTileValue;
												}
												else if (Grid.Objects[num2, 2] != null)
												{
													solidAmbienceType = SolidAmbienceType.Tile;
													this.solidLayers[(int)solidAmbienceType].tileCount += TuningData<AmbienceManager.Tuning>.Get().backwallTileValue;
													this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().backwallTileValue;
												}
												else if (solidAmbienceType != SolidAmbienceType.None)
												{
													this.solidLayers[(int)solidAmbienceType].tileCount++;
												}
												else if (element.id == SimHashes.Regolith || element.id == SimHashes.MaficRock)
												{
													this.spaceLayer.tileCount++;
												}
											}
											else if (element.id == SimHashes.Vacuum && CellSelectionObject.IsExposedToSpace(num2))
											{
												if (Grid.Objects[num2, 1] != null)
												{
													this.spaceLayer.tileCount -= TuningData<AmbienceManager.Tuning>.Get().buildingTileValue;
												}
												this.spaceLayer.tileCount++;
											}
										}
									}
									if (Grid.Radiation[num2] > 0f)
									{
										this.radiationLayer.averageRadiation += Grid.Radiation[num2];
										this.radiationLayer.tileCount++;
									}
								}
								else
								{
									this.fogLayer.tileCount++;
								}
							}
						}
					}
				}
			}
			Vector2I vector2I = max - min;
			int num4 = vector2I.x * vector2I.y;
			for (int l = 0; l < this.allLayers.Count; l++)
			{
				this.allLayers[l].UpdatePercentage(num4);
			}
			this.loopingLayers.Sort();
			this.topLayers.Clear();
			for (int m = 0; m < this.loopingLayers.Count; m++)
			{
				AmbienceManager.Layer layer = this.loopingLayers[m];
				if (m < 3 && layer.tilePercentage > 0f)
				{
					layer.Start(emitter_position);
					layer.UpdateAverageTemperature();
					layer.UpdateParameters(emitter_position);
					this.topLayers.Add(layer);
				}
				else
				{
					layer.Stop();
				}
			}
			if (this.m_isClusterSpaceEnabled)
			{
				float num5 = 0f;
				if (ClusterManager.Instance != null && ClusterManager.Instance.activeWorld != null && ClusterManager.Instance.activeWorld.IsModuleInterior)
				{
					num5 = 1f;
				}
				this.rocketInteriorLayer.Start(emitter_position);
				this.rocketInteriorLayer.SetCustomParameter("RocketState", (float)ClusterManager.RocketInteriorState);
				this.rocketInteriorLayer.SetVolume(num5);
			}
			if (this.m_isRadiationEnabled)
			{
				this.radiationLayer.Start(emitter_position);
				this.radiationLayer.UpdateAverageRadiation();
				this.radiationLayer.UpdateParameters(emitter_position);
			}
			this.oneShotLayers.Sort();
			for (int n = 0; n < AmbienceManager.Quadrant.activeSolidLayerCount; n++)
			{
				if (this.solidTimers[n].ShouldPlay() && this.oneShotLayers[n].tilePercentage > 0f)
				{
					this.oneShotLayers[n].Start(emitter_position);
				}
			}
		}

		public List<AmbienceManager.Layer> GetAllLayers()
		{
			return this.allLayers;
		}

		public string name;

		public Vector3 emitterPosition;

		public AmbienceManager.Layer[] gasLayers = new AmbienceManager.Layer[4];

		public AmbienceManager.LiquidLayer[] liquidLayers = new AmbienceManager.LiquidLayer[4];

		public AmbienceManager.Layer fogLayer;

		public AmbienceManager.Layer spaceLayer;

		public AmbienceManager.Layer rocketInteriorLayer;

		public AmbienceManager.Layer facilityLayer;

		public AmbienceManager.Layer radiationLayer;

		public AmbienceManager.Layer[] solidLayers = new AmbienceManager.Layer[21];

		private List<AmbienceManager.Layer> allLayers = new List<AmbienceManager.Layer>();

		private List<AmbienceManager.Layer> loopingLayers = new List<AmbienceManager.Layer>();

		private List<AmbienceManager.Layer> oneShotLayers = new List<AmbienceManager.Layer>();

		private List<AmbienceManager.Layer> topLayers = new List<AmbienceManager.Layer>();

		public static int activeSolidLayerCount = 2;

		public int totalTileCount;

		private bool m_isRadiationEnabled;

		private bool m_isClusterSpaceEnabled;

		private const string ROCKET_STATE_FOR_AMBIENCE = "RocketState";

		private AmbienceManager.Quadrant.SolidTimer[] solidTimers;

		public class SolidTimer
		{
			public SolidTimer()
			{
				this.solidTargetTime = Time.unscaledTime + global::UnityEngine.Random.value * AmbienceManager.Quadrant.SolidTimer.solidMinTime;
			}

			public bool ShouldPlay()
			{
				if (Time.unscaledTime > this.solidTargetTime)
				{
					this.solidTargetTime = Time.unscaledTime + AmbienceManager.Quadrant.SolidTimer.solidMinTime + global::UnityEngine.Random.value * (AmbienceManager.Quadrant.SolidTimer.solidMaxTime - AmbienceManager.Quadrant.SolidTimer.solidMinTime);
					return true;
				}
				return false;
			}

			public static float solidMinTime = 9f;

			public static float solidMaxTime = 15f;

			public float solidTargetTime;
		}
	}
}
