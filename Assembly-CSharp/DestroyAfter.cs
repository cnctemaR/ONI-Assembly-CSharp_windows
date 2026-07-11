using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DestroyAfter")]
public class DestroyAfter : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.particleSystems = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
	}

	private bool IsAlive()
	{
		for (int i = 0; i < this.particleSystems.Length; i++)
		{
			if (this.particleSystems[i].IsAlive(false))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (this.particleSystems != null && !this.IsAlive())
		{
			this.DeleteObject();
		}
	}

	private ParticleSystem[] particleSystems;
}
