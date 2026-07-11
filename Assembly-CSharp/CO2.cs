using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/CO2")]
public class CO2 : KMonoBehaviour
{
	public void StartLoop()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play("exhale_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Play("exhale_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void TriggerDestroy()
	{
		base.GetComponent<KBatchedAnimController>().Play("exhale_pst", KAnim.PlayMode.Once, 1f, 0f);
	}

	[Serialize]
	[NonSerialized]
	public Vector3 velocity = Vector3.zero;

	[Serialize]
	[NonSerialized]
	public float mass;

	[Serialize]
	[NonSerialized]
	public float temperature;

	[Serialize]
	[NonSerialized]
	public float lifetimeRemaining;
}
