using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/WorldDamage")]
public class WorldDamage : KMonoBehaviour
{
	public static WorldDamage Instance { get; private set; }

	public static void DestroyInstance()
	{
		WorldDamage.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		WorldDamage.Instance = this;
	}

	public void RestoreDamageToValue(int cell, float amount)
	{
		if (Grid.Damage[cell] > amount)
		{
			Grid.Damage[cell] = amount;
		}
	}

	public float ApplyDamage(Sim.WorldDamageInfo damage_info)
	{
		return this.ApplyDamage(damage_info.gameCell, this.damageAmount, damage_info.damageSourceOffset, BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE);
	}

	public float ApplyDamage(int cell, float amount, int src_cell, string source_name = null, string pop_text = null)
	{
		float num = 0f;
		if (Grid.Solid[cell])
		{
			float num2 = Grid.Damage[cell];
			num = Mathf.Min(amount, 1f - num2);
			num2 += amount;
			bool flag = num2 > 0.15f;
			if (flag)
			{
				GameObject gameObject = Grid.Objects[cell, 9];
				if (gameObject != null)
				{
					BuildingHP component = gameObject.GetComponent<BuildingHP>();
					if (component != null)
					{
						if (!component.invincible)
						{
							int num3 = Mathf.RoundToInt(Mathf.Max((float)component.HitPoints - (1f - num2) * (float)component.MaxHitPoints, 0f));
							gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
							{
								damage = num3,
								source = source_name,
								popString = pop_text
							});
						}
						else
						{
							num2 = 0f;
						}
					}
				}
			}
			Grid.Damage[cell] = Mathf.Min(1f, num2);
			if (Grid.Damage[cell] >= 1f)
			{
				this.DestroyCell(cell);
			}
			else if (Grid.IsValidCell(src_cell) && flag)
			{
				Element element = Grid.Element[src_cell];
				if (element.IsLiquid && Grid.Mass[src_cell] > 1f)
				{
					int num4 = cell - src_cell;
					if (num4 == 1 || num4 == -1 || num4 == Grid.WidthInCells || num4 == -Grid.WidthInCells)
					{
						int num5 = cell + num4;
						if (Grid.IsValidCell(num5))
						{
							Element element2 = Grid.Element[num5];
							if (!element2.IsSolid && (!element2.IsLiquid || (element2.id == element.id && Grid.Mass[num5] <= 100f)) && (Grid.Properties[num5] & 2) == 0 && !this.spawnTimes.ContainsKey(num5))
							{
								this.spawnTimes[num5] = Time.realtimeSinceStartup;
								ushort idx = element.idx;
								float num6 = Grid.Temperature[src_cell];
								base.StartCoroutine(this.DelayedSpawnFX(src_cell, num5, num4, element, idx, num6));
							}
						}
					}
				}
			}
		}
		return num;
	}

	private void ReleaseGO(GameObject go)
	{
		go.DeleteObject();
	}

	private IEnumerator DelayedSpawnFX(int src_cell, int dest_cell, int offset, Element elem, ushort idx, float temperature)
	{
		float num = global::UnityEngine.Random.value * 0.25f;
		yield return new WaitForSeconds(num);
		Vector3 vector = Grid.CellToPosCCC(dest_cell, Grid.SceneLayer.Front);
		GameObject gameObject = GameUtil.KInstantiate(this.leakEffect.gameObject, vector, Grid.SceneLayer.Front, null, 0);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.TintColour = elem.substance.colour;
		component.onDestroySelf = new Action<GameObject>(this.ReleaseGO);
		SimMessages.AddRemoveSubstance(src_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, -1f, temperature, byte.MaxValue, 0, true, -1);
		if (offset == -1)
		{
			component.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			component.FlipX = true;
			component.enabled = false;
			component.enabled = true;
			gameObject.transform.SetPosition(gameObject.transform.GetPosition() + Vector3.right * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		else if (offset == Grid.WidthInCells)
		{
			gameObject.transform.SetPosition(gameObject.transform.GetPosition() - Vector3.up * 0.5f);
			component.Play("floor", KAnim.PlayMode.Once, 1f, 0f);
			component.enabled = false;
			component.enabled = true;
			SimMessages.AddRemoveSubstance(dest_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, 1f, temperature, byte.MaxValue, 0, true, -1);
		}
		else if (offset == -Grid.WidthInCells)
		{
			component.Play("ceiling", KAnim.PlayMode.Once, 1f, 0f);
			component.enabled = false;
			component.enabled = true;
			gameObject.transform.SetPosition(gameObject.transform.GetPosition() + Vector3.up * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		else
		{
			component.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			component.enabled = false;
			component.enabled = true;
			gameObject.transform.SetPosition(gameObject.transform.GetPosition() - Vector3.right * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		if (CameraController.Instance.IsAudibleSound(gameObject.transform.GetPosition(), this.leakSoundMigrated))
		{
			SoundEvent.PlayOneShot(this.leakSoundMigrated, gameObject.transform.GetPosition(), 1f);
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
			SimMessages.Dig(cell, -1, false);
		}
	}

	public void OnSolidStateChanged(int cell)
	{
		Grid.Damage[cell] = 0f;
	}

	public void OnDigComplete(int cell, float mass, float temperature, ushort element_idx, byte disease_idx, int disease_count)
	{
		Vector3 vector = Grid.CellToPos(cell, CellAlignment.RandomInternal, Grid.SceneLayer.Ore);
		Element element = ElementLoader.elements[(int)element_idx];
		Grid.Damage[cell] = 0f;
		WorldDamage.Instance.PlaySoundForSubstance(element, vector);
		float num = mass * 0.5f;
		if (num <= 0f)
		{
			return;
		}
		GameObject gameObject = element.substance.SpawnResource(vector, num, temperature, disease_idx, disease_count, false, false, false);
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component != null && component.GetMyWorld() != null && component.GetMyWorld().worldInventory.IsReachable(component))
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
			KFMOD.PlayOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(pos, false), 1f);
		}
	}

	public KBatchedAnimController leakEffect;

	[SerializeField]
	private FMODAsset leakSound;

	[SerializeField]
	private EventReference leakSoundMigrated;

	private float damageAmount = 0.00083333335f;

	private const float SPAWN_DELAY = 1f;

	private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();

	private List<int> expiredCells = new List<int>();
}
