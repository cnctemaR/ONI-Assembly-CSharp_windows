using System;
using UnityEngine;

public class OreSizeVisualizer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-2064133523, new EventSystem.EventHandler(this.OnMassChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnMassChanged(null);
	}

	private void OnMassChanged(object data)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float num = component.Mass;
		OreSizeVisualizer.MassTier massTier = default(OreSizeVisualizer.MassTier);
		if (data != null)
		{
			GameObject gameObject = (GameObject)data;
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			num += component2.Mass;
		}
		for (int i = 0; i < this.tiers.Length; i++)
		{
			if (num <= this.tiers[i].massRequired)
			{
				massTier = this.tiers[i];
				break;
			}
		}
		KBatchedAnimController component3 = base.GetComponent<KBatchedAnimController>();
		component3.Play(massTier.animName, KAnim.PlayMode.Once, 1f, 0f);
		CircleCollider2D component4 = base.GetComponent<CircleCollider2D>();
		if (component4 != null)
		{
			component4.radius = massTier.colliderRadius;
		}
		this.Trigger(1807976145, null);
	}

	[SerializeField]
	private OreSizeVisualizer.MassTier[] tiers;

	[Serializable]
	private struct MassTier
	{
		public string animName;

		public float massRequired;

		public float colliderRadius;
	}
}
