using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MiniComet")]
public class MiniComet : KMonoBehaviour, ISim33ms
{
	public Vector3 TargetPosition
	{
		get
		{
			return this.anim.PositionIncludingOffset;
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
		this.RandomizeVelocity();
	}

	protected override void OnSpawn()
	{
		this.anim.Offset = this.offsetPosition;
		if (this.spawnWithOffset)
		{
			this.SetupOffset();
		}
		base.OnSpawn();
		this.StartLoopingSound();
		bool flag = this.offsetPosition.x != 0f || this.offsetPosition.y != 0f;
		this.selectable.enabled = !flag;
		this.typeID = base.GetComponent<KPrefabID>().PrefabTag;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
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
	}

	public virtual void RandomizeVelocity()
	{
		float num = global::UnityEngine.Random.Range(this.spawnAngle.x, this.spawnAngle.y);
		float num2 = num * 3.1415927f / 180f;
		float num3 = global::UnityEngine.Random.Range(this.spawnVelocity.x, this.spawnVelocity.y);
		this.velocity = new Vector2(-Mathf.Cos(num2) * num3, Mathf.Sin(num2) * num3);
		base.GetComponent<KBatchedAnimController>().Rotation = -num - 90f;
	}

	public int GetRandomNumOres()
	{
		return global::UnityEngine.Random.Range(this.explosionOreCount.x, this.explosionOreCount.y + 1);
	}

	[ContextMenu("Explode")]
	private void Explode(Vector3 pos, int cell, int prev_cell, Element element)
	{
		byte b = Grid.WorldIdx[cell];
		this.PlayImpactSound(pos);
		Vector3 vector = pos;
		vector.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
		if (this.explosionEffectHash != SpawnFXHashes.None)
		{
			Game.Instance.SpawnFX(this.explosionEffectHash, vector, 0f);
		}
		if (element != null)
		{
			Substance substance = element.substance;
			int randomNumOres = this.GetRandomNumOres();
			Vector2 vector2 = -this.velocity.normalized;
			Vector2 vector3 = new Vector2(vector2.y, -vector2.x);
			float num = ((randomNumOres > 0) ? (this.pe.Mass / (float)randomNumOres) : 1f);
			for (int i = 0; i < randomNumOres; i++)
			{
				Vector2 normalized = (vector2 + vector3 * global::UnityEngine.Random.Range(-1f, 1f)).normalized;
				Vector3 vector4 = normalized * global::UnityEngine.Random.Range(this.explosionSpeedRange.x, this.explosionSpeedRange.y);
				Vector3 vector5 = vector + normalized.normalized * 1.25f;
				GameObject gameObject = substance.SpawnResource(vector5, num, this.pe.Temperature, this.pe.DiseaseIdx, this.pe.DiseaseCount / randomNumOres, false, false, false);
				if (GameComps.Fallers.Has(gameObject))
				{
					GameComps.Fallers.Remove(gameObject);
				}
				GameComps.Fallers.Add(gameObject, vector4);
			}
		}
		if (this.OnImpact != null)
		{
			this.OnImpact();
		}
	}

	public float GetDistanceFromImpact()
	{
		float num = this.velocity.x / this.velocity.y;
		Vector3 position = base.transform.GetPosition();
		float num2 = 0f;
		while (num2 > -6f)
		{
			num2 -= 1f;
			num2 = Mathf.Ceil(position.y + num2) - 0.2f - position.y;
			float num3 = num2 * num;
			Vector3 vector = new Vector3(num3, num2, 0f);
			int num4 = Grid.PosToCell(position + vector);
			if (Grid.IsValidCell(num4) && Grid.Solid[num4])
			{
				return vector.magnitude;
			}
		}
		return 6f;
	}

	public float GetSoundDistance()
	{
		return this.GetDistanceFromImpact();
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
		}
		else
		{
			if (this.anim.Offset != Vector3.zero)
			{
				this.anim.Offset = Vector3.zero;
			}
			if (!this.selectable.enabled)
			{
				this.selectable.enabled = true;
			}
			Vector2 vector3 = new Vector2((float)Grid.WidthInCells, (float)Grid.HeightInCells) * -0.1f;
			Vector2 vector4 = new Vector2((float)Grid.WidthInCells, (float)Grid.HeightInCells) * 1.1f;
			Vector3 position = base.transform.GetPosition();
			Vector3 vector5 = position + new Vector3(this.velocity.x * dt, this.velocity.y * dt, 0f);
			Grid.PosToCell(vector5);
			this.loopingSounds.UpdateVelocity(this.flyingSound, vector5 - position);
			if (vector5.x < vector3.x || vector4.x < vector5.x || vector5.y < vector3.y)
			{
				global::Util.KDestroyGameObject(base.gameObject);
			}
			int num = Grid.PosToCell(this);
			int num2 = Grid.PosToCell(this.previousPosition);
			if (num != num2 && Grid.IsValidCell(num) && Grid.Solid[num])
			{
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				this.Explode(position, num, num2, component.Element);
				this.hasExploded = true;
				global::Util.KDestroyGameObject(base.gameObject);
				return;
			}
			this.previousPosition = position;
			base.transform.SetPosition(vector5);
		}
		this.age += dt;
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

	private void StartLoopingSound()
	{
		this.loopingSounds.StartSound(this.flyingSound);
		this.loopingSounds.UpdateFirstParameter(this.flyingSound, this.FLYING_SOUND_ID_PARAMETER, (float)this.flyingSoundID);
	}

	public void Explode()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Vector3 position = base.transform.GetPosition();
		int num = Grid.PosToCell(position);
		this.Explode(position, num, num, component.Element);
		this.hasExploded = true;
		global::Util.KDestroyGameObject(base.gameObject);
	}

	[MyCmpGet]
	private PrimaryElement pe;

	public Vector2 spawnVelocity = new Vector2(7f, 9f);

	public Vector2 spawnAngle = new Vector2(30f, 150f);

	public SpawnFXHashes explosionEffectHash;

	public int addDiseaseCount;

	public byte diseaseIdx = byte.MaxValue;

	public Vector2I explosionOreCount = new Vector2I(1, 1);

	public Vector2 explosionSpeedRange = new Vector2(0f, 0f);

	public string impactSound;

	public string flyingSound;

	public int flyingSoundID;

	private HashedString FLYING_SOUND_ID_PARAMETER = "meteorType";

	public bool Targeted;

	[Serialize]
	protected Vector3 offsetPosition;

	[Serialize]
	protected Vector2 velocity;

	private Vector3 previousPosition;

	private bool hasExploded;

	public string[] craterPrefabs;

	public bool spawnWithOffset;

	private float age;

	public global::System.Action OnImpact;

	public Ref<KPrefabID> ignoreObstacleForDamage = new Ref<KPrefabID>();

	[MyCmpGet]
	private KBatchedAnimController anim;

	[MyCmpGet]
	private KSelectable selectable;

	public Tag typeID;

	private LoopingSounds loopingSounds;

	private List<GameObject> damagedEntities = new List<GameObject>();

	private List<int> destroyedCells = new List<int>();

	private const float MAX_DISTANCE_TEST = 6f;
}
