using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using STRINGS;
using UnityEngine;

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
		int gameCell = damage_info.gameCell;
		float num = this.damageAmount;
		int damageSourceOffset = damage_info.damageSourceOffset;
		string text = BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE;
		return this.ApplyDamage(gameCell, num, damageSourceOffset, -1, text, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE);
	}

	public float ApplyDamage(int cell, float amount, int src_cell, int destroy_cb_index = -1, string source_name = null, string pop_text = null)
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
						float num3 = (float)component.HitPoints - (1f - num2) * (float)component.MaxHitPoints;
						int num4 = Mathf.RoundToInt(Mathf.Max(num3, 0f));
						gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
						{
							damage = num4,
							source = source_name,
							popString = pop_text
						});
					}
				}
			}
			Grid.Damage[cell] = Mathf.Min(1f, num2);
			if (Grid.Damage[cell] >= 1f)
			{
				this.DestroyCell(cell, destroy_cb_index);
			}
			else if (Grid.IsValidCell(src_cell) && flag)
			{
				Element element = Grid.Element[src_cell];
				if (element.IsLiquid && Grid.Mass[src_cell] > 1f)
				{
					int num5 = cell - src_cell;
					if (num5 == 1 || num5 == -1 || num5 == Grid.WidthInCells || num5 == -Grid.WidthInCells)
					{
						int num6 = cell + num5;
						if (Grid.IsValidCell(num6))
						{
							Element element2 = Grid.Element[num6];
							if (!element2.IsSolid && (!element2.IsLiquid || (element2.id == element.id && Grid.Mass[num6] <= 100f)) && (Grid.Properties[num6] & 2) == 0 && !this.spawnTimes.ContainsKey(num6))
							{
								this.spawnTimes[num6] = Time.realtimeSinceStartup;
								int idx = (int)element.idx;
								float num7 = Grid.Temperature[src_cell];
								base.StartCoroutine(this.DelayedSpawnFX(src_cell, num6, num5, element, idx, num7));
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

	private IEnumerator DelayedSpawnFX(int src_cell, int dest_cell, int offset, Element elem, int idx, float temperature)
	{
		float random_delay = global::UnityEngine.Random.value * 0.25f;
		yield return new WaitForSeconds(random_delay);
		Vector3 p = Grid.CellToPosCCC(dest_cell, Grid.SceneLayer.Front);
		GameObject fx = GameUtil.KInstantiate(this.leakEffect.gameObject, p, Grid.SceneLayer.Front, null, 0);
		KBatchedAnimController kanim = fx.GetComponent<KBatchedAnimController>();
		kanim.TintColour = elem.substance.colour;
		kanim.onDestroySelf = new Action<GameObject>(this.ReleaseGO);
		SimMessages.AddRemoveSubstance(src_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, -1f, temperature, byte.MaxValue, 0, true, -1);
		if (offset == -1)
		{
			kanim.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			kanim.FlipX = true;
			kanim.enabled = false;
			kanim.enabled = true;
			fx.transform.SetPosition(fx.transform.GetPosition() + Vector3.right * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		else if (offset == Grid.WidthInCells)
		{
			fx.transform.SetPosition(fx.transform.GetPosition() - Vector3.up * 0.5f);
			kanim.Play("floor", KAnim.PlayMode.Once, 1f, 0f);
			kanim.enabled = false;
			kanim.enabled = true;
			SimMessages.AddRemoveSubstance(dest_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, 1f, temperature, byte.MaxValue, 0, true, -1);
		}
		else if (offset == -Grid.WidthInCells)
		{
			kanim.Play("ceiling", KAnim.PlayMode.Once, 1f, 0f);
			kanim.enabled = false;
			kanim.enabled = true;
			fx.transform.SetPosition(fx.transform.GetPosition() + Vector3.up * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		else
		{
			kanim.Play("side", KAnim.PlayMode.Once, 1f, 0f);
			kanim.enabled = false;
			kanim.enabled = true;
			fx.transform.SetPosition(fx.transform.GetPosition() - Vector3.right * 0.5f);
			FallingWater.instance.AddParticle(dest_cell, (byte)idx, 1f, temperature, byte.MaxValue, 0, true, false, false, false);
		}
		if (CameraController.Instance.IsAudibleSound(fx.transform.GetPosition(), this.leakSoundMigrated))
		{
			SoundEvent.PlayOneShot(this.leakSoundMigrated, fx.transform.GetPosition());
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

	public void DestroyCell(int cell, int cb_index = -1)
	{
		if (Grid.Solid[cell])
		{
			if (cb_index == -1)
			{
				if (!this.queuedDigCallbackCells.Contains(cell))
				{
					this.queuedDigCallbackCells.Add(cell);
					SimMessages.Dig(cell, -1);
				}
			}
			else
			{
				SimMessages.Dig(cell, cb_index);
			}
		}
	}

	public void OnSolidStateChanged(int cell)
	{
		Grid.Damage[cell] = 0f;
	}

	public void OnDigComplete(int cell, float mass, float temperature, byte element_idx, byte disease_idx, int disease_count)
	{
		if (this.queuedDigCallbackCells.Contains(cell))
		{
			this.queuedDigCallbackCells.Remove(cell);
			Vector3 vector = Grid.CellToPos(cell, CellAlignment.RandomInternal, Grid.SceneLayer.Ore);
			Element element = ElementLoader.elements[(int)element_idx];
			Grid.Damage[cell] = 0f;
			WorldDamage.Instance.PlaySoundForSubstance(element, vector);
			float num = mass * 0.5f;
			if (num <= 0f)
			{
				return;
			}
			GameObject gameObject = element.substance.SpawnResource(vector, num, temperature, disease_idx, disease_count, false, false);
			Pickupable component = gameObject.GetComponent<Pickupable>();
			if (component != null && WorldInventory.Instance.IsReachable(gameObject.GetComponent<Pickupable>()))
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, Mathf.RoundToInt(num).ToString() + " " + element.name, gameObject.transform, 1.5f, false);
			}
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
			KFMOD.PlayOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(pos));
		}
	}

	public KBatchedAnimController leakEffect;

	[SerializeField]
	private FMODAsset leakSound;

	[SerializeField]
	[EventRef]
	private string leakSoundMigrated;

	private List<int> queuedDigCallbackCells = new List<int>();

	private float damageAmount = 0.00083333335f;

	private const float SPAWN_DELAY = 1f;

	private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();

	private List<int> expiredCells = new List<int>();
}
