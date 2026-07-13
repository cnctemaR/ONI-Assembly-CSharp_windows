using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/CO2")]
public class CO2 : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void StartLoop()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play(this.anim_name_pre, KAnim.PlayMode.Once, 1f, 0f);
		component.Play(this.anim_name_loop, KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void TriggerDestroy()
	{
		base.GetComponent<KBatchedAnimController>().Play(this.anim_name_pst, KAnim.PlayMode.Once, 1f, 0f);
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

	[Serialize]
	[NonSerialized]
	public string kAnimFileName = "exhale_kanim";

	[Serialize]
	[NonSerialized]
	public string anim_name_pre = "exhale_pre";

	[Serialize]
	[NonSerialized]
	public string anim_name_loop = "exhale_loop";

	[Serialize]
	[NonSerialized]
	public string anim_name_pst = "exhale_pst";

	[Serialize]
	[NonSerialized]
	public bool affectedByGravity = true;
}
