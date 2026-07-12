using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class HighEnergyParticleStorage : KMonoBehaviour, IStorage
{
	public float Particles
	{
		get
		{
			return this.particles;
		}
	}

	public bool allowUIItemRemoval { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.autoStore)
		{
			HighEnergyParticlePort component = base.gameObject.GetComponent<HighEnergyParticlePort>();
			component.onParticleCapture = (HighEnergyParticlePort.OnParticleCapture)Delegate.Combine(component.onParticleCapture, new HighEnergyParticlePort.OnParticleCapture(this.OnParticleCapture));
			component.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Combine(component.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(this.OnParticleCaptureAllowed));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.autoStore)
		{
			HighEnergyParticlePort component = base.gameObject.GetComponent<HighEnergyParticlePort>();
			component.onParticleCapture = (HighEnergyParticlePort.OnParticleCapture)Delegate.Remove(component.onParticleCapture, new HighEnergyParticlePort.OnParticleCapture(this.OnParticleCapture));
		}
	}

	private void OnParticleCapture(HighEnergyParticle particle)
	{
		float num = Mathf.Min(particle.payload, this.capacity - this.particles);
		this.Store(num);
		particle.payload -= num;
		if (particle.payload > 0f)
		{
			base.gameObject.GetComponent<HighEnergyParticlePort>().Uncapture(particle);
		}
	}

	private bool OnParticleCaptureAllowed(HighEnergyParticle particle)
	{
		return this.particles < this.capacity && this.receiverOpen;
	}

	public void Store(float amount)
	{
		DebugUtil.Assert(amount >= 0f, string.Format("Storing negative amount ({0}) of particles", amount));
		this.particles += amount;
		base.Trigger(-1837862626, base.transform.gameObject);
	}

	public float ConsumeAndGet(float amount)
	{
		if (amount > this.Particles)
		{
			amount = this.Particles;
			this.particles = 0f;
			base.Trigger(155636535, base.transform.gameObject);
		}
		else
		{
			this.particles -= amount;
		}
		base.Trigger(-1837862626, base.transform.gameObject);
		return amount;
	}

	[ContextMenu("Trigger Stored Event")]
	public void DEBUG_TriggerStorageEvent()
	{
		base.Trigger(-1837862626, base.transform.gameObject);
	}

	[ContextMenu("Trigger Zero Event")]
	public void DEBUG_TriggerZeroEvent()
	{
		this.ConsumeAndGet(this.particles + 1f);
	}

	public float ConsumeAll()
	{
		return this.ConsumeAndGet(this.particles);
	}

	public bool HasRadiation()
	{
		return this.Particles > 0f;
	}

	public GameObject Drop(GameObject go, bool do_disease_transfer = true)
	{
		return null;
	}

	public List<GameObject> GetItems()
	{
		return new List<GameObject> { base.gameObject };
	}

	public bool IsFull()
	{
		return this.RemainingCapacity() <= 0f;
	}

	public bool IsEmpty()
	{
		return this.Particles == 0f;
	}

	public float Capacity()
	{
		return this.capacity;
	}

	public float RemainingCapacity()
	{
		return Mathf.Max(this.capacity - this.Particles, 0f);
	}

	public bool ShouldShowInUI()
	{
		return this.showInUI;
	}

	public float GetAmountAvailable(Tag tag)
	{
		if (tag != GameTags.HighEnergyParticle)
		{
			return 0f;
		}
		return this.Particles;
	}

	public void ConsumeIgnoringDisease(Tag tag, float amount)
	{
		DebugUtil.DevAssert(tag == GameTags.HighEnergyParticle, "Consuming non-particle tag as amount", null);
		this.ConsumeAndGet(amount);
	}

	[Serialize]
	[SerializeField]
	private float particles;

	public float capacity;

	public bool showInUI = true;

	public bool autoStore;

	[Serialize]
	public bool receiverOpen = true;
}
