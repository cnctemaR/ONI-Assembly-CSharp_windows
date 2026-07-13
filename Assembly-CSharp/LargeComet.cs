using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Comet")]
public class LargeComet : KMonoBehaviour, ISim33ms
{
	public float LandingProgress { get; private set; }

	public Vector3 VisualPosition
	{
		get
		{
			return base.transform.position + this.anim.Offset;
		}
	}

	public Vector3 VisualPositionCentredImage
	{
		get
		{
			return this.VisualPosition + new Vector3(0f, (float)Mathf.Abs(this.lowestTemplateYLocalPosition), 0f);
		}
	}

	public Vector2 Velocity
	{
		get
		{
			return this.velocity;
		}
		set
		{
			this.velocity = value;
		}
	}

	private float GetVolume(GameObject gameObject)
	{
		float num = 1f;
		if (gameObject != null && this.selectable != null && this.selectable.IsSelected)
		{
			num = 1f;
		}
		return num;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.loopingSounds = base.gameObject.GetComponent<LoopingSounds>();
		this.flyingSound = GlobalAssets.GetSound("Meteor_LP", false);
		this.SetVelocity();
	}

	protected override void OnSpawn()
	{
		this.anim.Offset = this.offsetPosition;
		this.SetupOffset();
		this.child_controllers = base.GetComponents<KBatchedAnimController>();
		KBatchedAnimController[] array = this.child_controllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Offset = this.anim.Offset;
		}
		base.OnSpawn();
		this.StartLoopingSound();
		bool flag = this.offsetPosition.x != 0f || this.offsetPosition.y != 0f;
		this.selectable.enabled = !flag;
		Vector3 position = base.gameObject.transform.position;
		foreach (KeyValuePair<string, string> keyValuePair in this.additionalAnimFiles)
		{
			this.additionalAnimControllers.Add(this.AddEffectAnim(keyValuePair.Key, keyValuePair.Value, position));
			position.z -= 0.001f;
		}
		KBatchedAnimController kbatchedAnimController = this.AddEffectAnim(this.mainAnimFile.Key, this.mainAnimFile.Value, position);
		this.additionalAnimControllers.Add(kbatchedAnimController);
		this.mainChildrenAnimController = kbatchedAnimController;
		this.mainChildrenAnimController.materialType = KAnimBatchGroup.MaterialType.Invisible;
		this.initialPosition = this.VisualPosition;
		this.lowestTemplateYLocalPosition = this.asteroidTemplate.GetTemplateBounds(0).yMin;
		this.templateWidth = this.asteroidTemplate.GetTemplateBounds(0).width;
		this.InitializeMaterial();
		CameraController.Instance.RegisterCustomScreenPostProcessingEffect(new Func<RenderTexture, Material>(this.DrawComet));
		this.fromStampToCrashPosition = this.stampLocation - this.crashPosition;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		CameraController.Instance.UnregisterCustomScreenPostProcessingEffect(new Func<RenderTexture, Material>(this.DrawComet));
	}

	private KBatchedAnimController AddEffectAnim(string anim_file, string anim_name, Vector3 startPosition)
	{
		KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect(anim_file, startPosition, null, false, Grid.SceneLayer.Front, false);
		kbatchedAnimController.Play(anim_name, KAnim.PlayMode.Loop, 1f, 0f);
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		kbatchedAnimController.animScale = 0.1f;
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.Offset = this.anim.Offset;
		return kbatchedAnimController;
	}

	protected void SetupOffset()
	{
		Vector3 position = base.transform.GetPosition();
		Vector3 position2 = base.transform.GetPosition();
		position2.z = 0f;
		Vector3 vector = new Vector3(this.velocity.x, this.velocity.y, 0f);
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		float num = (float)(myWorld.WorldOffset.y + myWorld.Height + MissileLauncher.Def.launchRange.y) * Grid.CellSizeInMeters - position2.y;
		float num2 = Vector3.Angle(Vector3.up, -vector) * 0.017453292f;
		float num3 = Mathf.Abs(num / Mathf.Cos(num2));
		Vector3 vector2 = position2 - vector.normalized * num3;
		float num4 = (float)(myWorld.WorldOffset.x + myWorld.Width) * Grid.CellSizeInMeters;
		if (vector2.x < (float)myWorld.WorldOffset.x * Grid.CellSizeInMeters || vector2.x > num4)
		{
			float num5 = ((vector.x < 0f) ? (num4 - position2.x) : (position2.x - (float)myWorld.WorldOffset.x * Grid.CellSizeInMeters));
			num2 = Vector3.Angle((vector.x < 0f) ? Vector3.right : Vector3.left, -vector) * 0.017453292f;
			num3 = Mathf.Abs(num5 / Mathf.Cos(num2));
		}
		Vector3 vector3 = -vector.normalized * num3;
		(position2 + vector3).z = position.z;
		this.offsetPosition = vector3;
		this.anim.Offset = this.offsetPosition;
		this.worldID = myWorld.id;
		this.previousVisualPosition = this.VisualPosition;
	}

	public void SetVelocity()
	{
		int num = -90;
		float num2 = (float)num * 3.1415927f / 180f;
		int num3 = 12;
		this.velocity = new Vector2(-Mathf.Cos(num2) * (float)num3, Mathf.Sin(num2) * (float)num3);
		base.GetComponent<KBatchedAnimController>().Rotation = (float)(-(float)num) - 90f;
	}

	private void Explode(Vector3 pos)
	{
		this.PlayImpactSound(pos);
		if (this.OnImpact != null)
		{
			this.OnImpact();
		}
		foreach (KAnimControllerBase kanimControllerBase in this.additionalAnimControllers)
		{
			global::Util.KDestroyGameObject(kanimControllerBase);
		}
		global::Util.KDestroyGameObject(base.gameObject);
	}

	public void Sim33ms(float dt)
	{
		if (this.hasExploded)
		{
			return;
		}
		if (this.offsetPosition.y > 0f)
		{
			Vector3 vector = new Vector3(this.velocity.x * dt, this.velocity.y * dt, 0f);
			Vector3 vector2 = this.offsetPosition + vector;
			this.offsetPosition = vector2;
			this.anim.Offset = this.offsetPosition;
			using (List<KAnimControllerBase>.Enumerator enumerator = this.additionalAnimControllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KAnimControllerBase kanimControllerBase = enumerator.Current;
					kanimControllerBase.Offset = this.offsetPosition;
				}
				goto IL_01E2;
			}
		}
		if (this.anim.Offset != Vector3.zero)
		{
			this.anim.Offset = Vector3.zero;
			foreach (KAnimControllerBase kanimControllerBase2 in this.additionalAnimControllers)
			{
				kanimControllerBase2.Offset = this.anim.Offset;
			}
		}
		Vector3 position = base.transform.GetPosition();
		Vector3 vector3 = position + new Vector3(this.velocity.x * dt, this.velocity.y * dt, 0f);
		this.loopingSounds.UpdateVelocity(this.flyingSound, vector3 - position);
		base.transform.SetPosition(vector3);
		Vector3 vector4 = vector3;
		foreach (KAnimControllerBase kanimControllerBase3 in this.additionalAnimControllers)
		{
			kanimControllerBase3.transform.SetPosition(vector4);
			vector4.z -= 0.001f;
		}
		if (vector3.y < (float)this.crashPosition.y)
		{
			this.Explode(vector3);
		}
		IL_01E2:
		Vector2I vector2I = Grid.PosToXY(this.previousVisualPosition);
		Vector2I vector2I2 = Grid.PosToXY(this.VisualPosition);
		vector2I.y = Mathf.Clamp(vector2I.y, this.crashPosition.y, int.MaxValue);
		vector2I2.y = Mathf.Clamp(vector2I2.y, this.crashPosition.y, int.MaxValue);
		if (vector2I2.y != vector2I.y)
		{
			Grid.CollectCellsInLine(Grid.XYToCell(vector2I.x, vector2I.y), Grid.XYToCell(vector2I2.x, vector2I2.y), this.cellsCentrePassedThrough);
			bool flag = false;
			Vector3 vector5 = Vector3.zero;
			foreach (int num in this.cellsCentrePassedThrough)
			{
				foreach (CellOffset cellOffset in this.bottomCellsOffsetOfTemplate.Values)
				{
					int num2 = Grid.OffsetCell(Grid.OffsetCell(num, 0, Mathf.Abs(this.lowestTemplateYLocalPosition)), cellOffset.x, cellOffset.y);
					if (Grid.IsValidCellInWorld(num2, this.worldID) && this.DestroyCell(num2) && !flag)
					{
						Vector3 vector6 = Grid.CellToPos(num2);
						if (this.IsPositionFarAwayFromOtherExplosions(vector6))
						{
							flag = true;
							vector5 = Grid.CellToPos(num2);
						}
					}
				}
			}
			if (flag)
			{
				this.PlayExplosionEffectOnPosition(vector5);
			}
		}
		float num3 = Mathf.Clamp(1f - (this.VisualPosition.y - (float)this.crashPosition.y) / (this.initialPosition.y - (float)this.crashPosition.y), 0f, 1f);
		this.mainChildrenAnimController.postProcessingParameters = Mathf.Clamp(Mathf.Ceil(num3 * (Mathf.Pow(10f, 3f) - 1f)), 0f, float.MaxValue);
		this.LandingProgress = num3;
		this.previousVisualPosition = this.VisualPosition;
		this.age += dt;
	}

	private bool IsPositionFarAwayFromOtherExplosions(Vector3 position)
	{
		this.activeExplosionPosition.z = position.z;
		for (int i = 0; i < 30; i++)
		{
			if (this.ShaderExplosions[i].z >= 0f && Time.timeSinceLevelLoad - this.ShaderExplosions[i].z < 1.2333333f)
			{
				this.activeExplosionPosition.x = this.ShaderExplosions[i].x;
				this.activeExplosionPosition.y = this.ShaderExplosions[i].y;
				if ((this.activeExplosionPosition - position).magnitude < this.minSeparationBetweenExplosions)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void PlayExplosionEffectOnPosition(Vector3 position)
	{
		for (int i = 0; i < 30; i++)
		{
			if (this.ShaderExplosions[i].z < 0f || Time.timeSinceLevelLoad - this.ShaderExplosions[i].z > 1.2333333f)
			{
				this.ShaderExplosions[i].x = position.x;
				this.ShaderExplosions[i].y = position.y;
				this.ShaderExplosions[i].z = Time.timeSinceLevelLoad;
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Battery_explode", false), position, 1f);
				this.lastExplosionPosition = position;
				return;
			}
		}
	}

	private void PlayImpactSound(Vector3 pos)
	{
		if (this.impactSound == null)
		{
			this.impactSound = "Meteor_Large_Impact";
		}
		this.loopingSounds.StopSound(this.flyingSound);
		string sound = GlobalAssets.GetSound(this.impactSound, false);
		int num = Grid.PosToCell(pos);
		if (Grid.IsValidCell(num) && (int)Grid.WorldIdx[num] == ClusterManager.Instance.activeWorldId)
		{
			float volume = this.GetVolume(base.gameObject);
			pos.z = 0f;
			EventInstance eventInstance = KFMOD.BeginOneShot(sound, pos, volume);
			eventInstance.setParameterByName("userVolume_SFX", KPlayerPrefs.GetFloat("Volume_SFX"), false);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public bool DestroyCell(int cell)
	{
		bool flag = false;
		ListPool<GameObject, LargeComet>.PooledList pooledList = ListPool<GameObject, LargeComet>.Allocate();
		GameObject gameObject = Grid.Objects[cell, 1];
		flag = gameObject != null;
		pooledList.Add(gameObject);
		pooledList.Add(Grid.Objects[cell, 2]);
		pooledList.Add(Grid.Objects[cell, 12]);
		pooledList.Add(Grid.Objects[cell, 15]);
		pooledList.Add(Grid.Objects[cell, 16]);
		pooledList.Add(Grid.Objects[cell, 19]);
		pooledList.Add(Grid.Objects[cell, 20]);
		pooledList.Add(Grid.Objects[cell, 23]);
		pooledList.Add(Grid.Objects[cell, 26]);
		pooledList.Add(Grid.Objects[cell, 29]);
		pooledList.Add(Grid.Objects[cell, 31]);
		pooledList.Add(Grid.Objects[cell, 30]);
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (Grid.PosToCell(minionIdentity) == cell)
			{
				pooledList.Add(minionIdentity.gameObject);
				SaveGame.Instance.ColonyAchievementTracker.deadDupeCounter++;
			}
		}
		foreach (GameObject gameObject2 in pooledList)
		{
			if (gameObject2 != null)
			{
				global::Util.KDestroyGameObject(gameObject2);
			}
		}
		this.ClearCellPickupables(cell);
		Element element = ElementLoader.elements[(int)Grid.ElementIdx[cell]];
		if (element.id == SimHashes.Void)
		{
			SimMessages.ReplaceElement(cell, SimHashes.Void, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
		else
		{
			SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
		flag = flag || element.IsSolid;
		pooledList.Recycle();
		return flag;
	}

	public void ClearCellPickupables(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (gameObject != null)
		{
			ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
			while (objectLayerListItem != null)
			{
				GameObject gameObject2 = objectLayerListItem.gameObject;
				objectLayerListItem = objectLayerListItem.nextItem;
				if (!(gameObject2 == null))
				{
					global::Util.KDestroyGameObject(gameObject2);
				}
			}
		}
	}

	private void InitializeMaterial()
	{
		this.largeCometMaterial = new Material(Shader.Find("Klei/DLC4/LargeImpactorCometShader"));
		this.largeCometTexture = Assets.GetSprite("Demolior_final_broken");
		this.explosionTexture = Assets.GetSprite("contact_explode_fx_animationSheet");
		for (int i = 0; i < 30; i++)
		{
			this.ShaderExplosions[i] = Vector4.one * -1f;
			this.ShaderExplosions[i].w = (this.minSeparationBetweenExplosions - 1f) * 2f;
		}
	}

	private Material DrawComet(RenderTexture source)
	{
		this.largeCometMaterial.SetTexture("_CometTex", this.largeCometTexture.texture);
		this.largeCometMaterial.SetTexture("_ExplosionTex", this.explosionTexture.texture);
		this.largeCometMaterial.SetVector("_CometWorldPosition", this.VisualPositionCentredImage);
		this.largeCometMaterial.SetFloat("_LandingProgress", this.LandingProgress);
		this.largeCometMaterial.SetFloat("_CometWidth", (float)this.templateWidth);
		this.largeCometMaterial.SetFloat("_CometRatio", (float)this.largeCometTexture.texture.height / (float)this.largeCometTexture.texture.width);
		this.largeCometMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
		this.largeCometMaterial.SetVectorArray("_ExplosionLocations", this.ShaderExplosions);
		return this.largeCometMaterial;
	}

	private void StartLoopingSound()
	{
		this.loopingSounds.StartSound(this.flyingSound);
		this.loopingSounds.UpdateFirstParameter(this.flyingSound, LargeComet.FLYING_SOUND_ID_PARAMETER, (float)this.flyingSoundID);
	}

	private static HashedString FLYING_SOUND_ID_PARAMETER = "meteorType";

	public string impactSound;

	public string flyingSound;

	public int flyingSoundID;

	public List<KeyValuePair<string, string>> additionalAnimFiles = new List<KeyValuePair<string, string>>();

	public KeyValuePair<string, string> mainAnimFile;

	public bool affectedByDifficulty = true;

	public bool destroyOnExplode = true;

	public bool spawnWithOffset;

	public Vector2I stampLocation;

	public Vector2I crashPosition;

	public Dictionary<int, CellOffset> bottomCellsOffsetOfTemplate;

	public TemplateContainer asteroidTemplate;

	public Ref<KPrefabID> ignoreObstacleForDamage = new Ref<KPrefabID>();

	private bool hasExploded;

	private float age;

	private int lowestTemplateYLocalPosition;

	private int templateWidth;

	private int worldID;

	private Vector3 previousVisualPosition;

	private Vector3 initialPosition;

	private Vector2I prevCell;

	public global::System.Action OnImpact;

	[Serialize]
	protected Vector3 offsetPosition;

	[Serialize]
	protected Vector2 velocity;

	[MyCmpGet]
	private KBatchedAnimController anim;

	[MyCmpGet]
	private KSelectable selectable;

	private LoopingSounds loopingSounds;

	private KBatchedAnimController[] child_controllers;

	private List<KAnimControllerBase> additionalAnimControllers = new List<KAnimControllerBase>();

	private KBatchedAnimController mainChildrenAnimController;

	private Vector2I fromStampToCrashPosition;

	private HashSet<int> cellsCentrePassedThrough = new HashSet<int>();

	private Vector3 activeExplosionPosition = Vector3.zero;

	private Material largeCometMaterial;

	private Sprite largeCometTexture;

	private Sprite explosionTexture;

	private float minSeparationBetweenExplosions = 8f;

	private Vector3 lastExplosionPosition;

	private const string LARGE_COMET_SHADER_NAME = "Klei/DLC4/LargeImpactorCometShader";

	private const int MAX_SHADER_EXPLOSION_COUNT = 30;

	private const float EXPLOSION_ANIMATION_FRAME_COUNT = 37f;

	private const float EXPLOSION_ANIMATION_DURATION = 1.2333333f;

	private Vector4[] ShaderExplosions = new Vector4[30];
}
