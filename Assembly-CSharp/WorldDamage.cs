using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class WorldDamage : KMonoBehaviour
{
	public static WorldDamage Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		WorldDamage.Instance = this;
	}

	public bool ApplyDamage(Sim.WorldDamageInfo damage_info)
	{
		return this.ApplyDamage(damage_info.gameCell, this.damageAmount, damage_info.damageSourceOffset);
	}

	public bool ApplyDamage(int cell, float amount, int src_cell)
	{
		if (Grid.Solid[cell])
		{
			float num = Grid.Damage[cell];
			num += amount;
			Grid.Damage[cell] = num;
			GameObject gameObject = Grid.Objects[cell, 8];
			if (gameObject != null)
			{
				gameObject.Trigger(-184635526, num >= 1f);
			}
			if (Grid.Damage[cell] >= 1f)
			{
				this.DestroyCell(cell);
				return true;
			}
			if (src_cell != -1 && num > 0.15f)
			{
				Element element = Grid.Element[src_cell];
				if (element.IsLiquid && Grid.Cell[src_cell].mass > 1f)
				{
					int elementIndex = ElementLoader.GetElementIndex(element.id);
					float temperature = Grid.Cell[src_cell].temperature;
					int num2 = cell - src_cell;
					if (num2 == 1 || num2 == -1 || num2 == Grid.WidthInCells || num2 == -Grid.WidthInCells)
					{
						int num3 = cell + num2;
						Element element2 = Grid.Element[num3];
						if (!element2.IsSolid && (!element2.IsLiquid || Grid.Cell[num3].mass <= 100f) && !this.spawnTimes.ContainsKey(num3))
						{
							this.spawnTimes[num3] = Time.realtimeSinceStartup;
							base.StartCoroutine(this.DelayedSpawnFX(src_cell, num3, num2, element, elementIndex, temperature));
						}
					}
				}
			}
		}
		return false;
	}

	private void ReleaseGO(GameObject go)
	{
		go.DeleteObject();
	}

	private IEnumerator DelayedSpawnFX(int src_cell, int dest_cell, int offset, Element elem, int idx, float temperature)
	{
		float random_delay = global::UnityEngine.Random.value * 0.25f;
		yield return new WaitForSeconds(random_delay);
		Vector3 p = Grid.CellToPosCCC(dest_cell, Grid.SceneLayer.Front);
		GameObject fx = GameUtil.KInstantiate(this.leakEffect.gameObject, p, Grid.SceneLayer.Front, Folder.FX, null, 0);
		KBatchedAnimController kanim = fx.GetComponent<KBatchedAnimController>();
		kanim.TintColour = elem.substance.colour;
		kanim.onDestroySelf = new Action<GameObject>(this.ReleaseGO);
		SimMessages.AddRemoveSubstance(src_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, -1f, temperature, -1);
		if (offset == -1)
		{
			kanim.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			kanim.Flip = true;
			fx.transform.position += Vector3.right * 0.5f;
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, true, false);
		}
		else if (offset == Grid.WidthInCells)
		{
			fx.transform.position -= Vector3.up * 0.5f;
			kanim.Play("floor", KAnim.PlayMode.Once, 1f, 0f);
			SimMessages.AddRemoveSubstance(dest_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, 1f, temperature, -1);
		}
		else if (offset == -Grid.WidthInCells)
		{
			kanim.Play("ceiling", KAnim.PlayMode.Once, 1f, 0f);
			fx.transform.position += Vector3.up * 0.5f;
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, true, false);
		}
		else
		{
			kanim.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			fx.transform.position -= Vector3.right * 0.5f;
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, true, false);
		}
		if (CameraController.Instance.IsAudibleSound(fx.transform.position, this.leakSoundMigrated))
		{
			SoundEvent.PlayOneShot(this.leakSoundMigrated, fx.transform.position);
		}
		yield return null;
		yield break;
	}

	private void Update()
	{
		this.expiredCells.Clear();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		foreach (KeyValuePair<int, float> keyValuePair in this.spawnTimes)
		{
			if (realtimeSinceStartup - keyValuePair.Value > 1f)
			{
				this.expiredCells.Add(keyValuePair.Key);
			}
		}
		foreach (int num in this.expiredCells)
		{
			this.spawnTimes.Remove(num);
		}
		this.expiredCells.Clear();
	}

	public void DestroyCell(int cell)
	{
		if (Grid.Solid[cell])
		{
			float mass = Grid.Cell[cell].mass;
			Element element = Grid.Element[cell];
			float temperature = Grid.Temperature[cell];
			global::System.Action action = delegate
			{
				WorldDamage.OnDigComplete(cell, mass, temperature, element);
			};
			HandleVector<global::System.Action>.Handle handle = Game.Instance.callbackManager.Add(action, "WorldDamage");
			SimMessages.Dig(cell, handle.index);
		}
	}

	public void OnSolidStateChanged(int cell)
	{
		Grid.Damage[cell] = 0f;
	}

	private static void OnDigComplete(int cell, float mass, float temperature, Element element)
	{
		Vector3 vector = Grid.CellToPos(cell, CellAlignment.RandomInternal, Grid.SceneLayer.Use);
		Grid.Damage[cell] = 0f;
		WorldDamage.Instance.PlaySoundForSubstance(element, vector);
		float num = mass * 0.5f;
		if (num <= 0f)
		{
			return;
		}
		GameObject gameObject = element.substance.SpawnResource(vector, num, temperature, false, false);
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component != null && WorldInventory.Instance.IsReachable(gameObject.GetComponent<Pickupable>()))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, Mathf.RoundToInt(num).ToString() + " " + element.name, gameObject.transform, 1.5f, false);
		}
	}

	private void PlaySoundForSubstance(Element element, Vector3 pos)
	{
		string text = element.substance.GetMiningBreakSound();
		if (text == null)
		{
			if (element.HasTag(GameTags.RefinedMetal))
			{
				text = "RefinedMetal";
			}
			else if (element.HasTag(GameTags.Metal))
			{
				text = "RawMetal";
			}
			else
			{
				text = "Rock";
			}
		}
		text = "Break_" + text;
		text = GlobalAssets.GetSound(text, false);
		if (CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, text))
		{
			KFMOD.PlayOneShot(text, pos);
		}
	}

	private const float SPAWN_DELAY = 1f;

	public KBatchedAnimController leakEffect;

	[SerializeField]
	private FMODAsset leakSound;

	[SerializeField]
	[EventRef]
	private string leakSoundMigrated;

	private float damageAmount = 0.00083333335f;

	private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();

	private List<int> expiredCells = new List<int>();

	[Serializable]
	public struct SoundData : ISerializationCallbackReceiver
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		public string name;
	}
}
