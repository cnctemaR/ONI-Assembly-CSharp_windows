using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimSequencer")]
public class KAnimSequencer : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.kbac = base.GetComponent<KBatchedAnimController>();
		this.mb = base.GetComponent<MinionBrain>();
		if (this.autoRun)
		{
			this.PlaySequence();
		}
	}

	public void Reset()
	{
		this.currentIndex = 0;
	}

	public void PlaySequence()
	{
		if (this.sequence != null && this.sequence.Length != 0)
		{
			if (this.mb != null)
			{
				this.mb.Suspend("AnimSequencer");
			}
			this.kbac.onAnimComplete += this.PlayNext;
			this.PlayNext(null);
		}
	}

	private void PlayNext(HashedString name)
	{
		if (this.sequence.Length > this.currentIndex)
		{
			this.kbac.Play(new HashedString(this.sequence[this.currentIndex].anim), this.sequence[this.currentIndex].mode, this.sequence[this.currentIndex].speed, 0f);
			this.currentIndex++;
			return;
		}
		this.kbac.onAnimComplete -= this.PlayNext;
		if (this.mb != null)
		{
			this.mb.Resume("AnimSequencer");
		}
	}

	[Serialize]
	public bool autoRun;

	[Serialize]
	public KAnimSequencer.KAnimSequence[] sequence = new KAnimSequencer.KAnimSequence[0];

	private int currentIndex;

	private KBatchedAnimController kbac;

	private MinionBrain mb;

	[SerializationConfig(MemberSerialization.OptOut)]
	[Serializable]
	public class KAnimSequence
	{
		public string anim;

		public float speed = 1f;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;
	}
}
