using System;
using KSerialization;
using UnityEngine;

public class SelfChargingElectrobank : Electrobank
{
	public float LifetimeRemaining
	{
		get
		{
			return this.lifetimeRemaining;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.selectable = base.GetComponent<KSelectable>();
		this.selectable.AddStatusItem(Db.Get().MiscStatusItems.ElectrobankSelfCharging, 60f);
		this.lifetimeStatus = this.selectable.AddStatusItem(Db.Get().MiscStatusItems.ElectrobankLifetimeRemaining, this);
		Components.SelfChargingElectrobanks.Add(base.gameObject.GetMyWorldId(), this);
		if (this.lifetimeRemaining <= 0f)
		{
			this.Delete();
		}
	}

	public override void Sim200ms(float dt)
	{
		base.Sim200ms(dt);
		if (this.lifetimeRemaining > 0f)
		{
			base.AddPower(dt * 60f);
			this.lifetimeRemaining -= dt;
			return;
		}
		this.Explode();
	}

	public override void Explode()
	{
		Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactMetal, base.gameObject.transform.position, 0f);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Battery_explode", false), base.gameObject.transform.position, 1f);
		base.LaunchNearbyStuff();
		SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.position), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, 10f, 3000f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt(5000000f), true, -1);
		if (base.transform.parent != null)
		{
			Storage component = base.transform.parent.GetComponent<Storage>();
			if (component != null)
			{
				Health component2 = component.GetComponent<Health>();
				if (component2 != null)
				{
					component2.Damage(500f);
				}
			}
		}
		this.Delete();
	}

	private void Delete()
	{
		if (!this.IsNullOrDestroyed() && !base.gameObject.IsNullOrDestroyed())
		{
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.SelfChargingElectrobanks.Remove(base.gameObject.GetMyWorldId(), this);
	}

	[Serialize]
	private float lifetimeRemaining = 90000f;

	private KSelectable selectable;

	private Guid lifetimeStatus = Guid.Empty;
}
