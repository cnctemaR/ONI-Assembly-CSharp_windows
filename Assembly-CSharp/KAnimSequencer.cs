using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class KAnimSequencer : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.kbac = base.GetComponent<KBatchedAnimController>();
		this.mb = base.GetComponent<MinionBrain>();
		if (this.animFiles != null && this.animFiles.Length > 0)
		{
			KAnimFile[] array = new KAnimFile[this.animFiles.Length];
			for (int i = 0; i < this.animFiles.Length; i++)
			{
				array[i] = Assets.GetAnim(this.animFiles[i]);
			}
			this.kbac.AddAnims(array);
		}
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
		if (this.sequence != null && this.sequence.Length > 0)
		{
			if (this.mb != null)
			{
				this.mb.Suspend("AnimSequencer");
				if (this.setBody)
				{
					KAnimFileData kanimFileData = KCompBuilder.Instance.GenerateDefaultPose(this.bodyData);
					this.kbac.AddBuildOverride(kanimFileData, true, true);
				}
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
		}
		else
		{
			this.kbac.onAnimComplete -= this.PlayNext;
			if (this.mb != null)
			{
				this.mb.Resume("AnimSequencer");
			}
		}
	}

	[Serialize]
	public KCompBuilder.BodyData bodyData = default(KCompBuilder.BodyData);

	[Serialize]
	public bool setBody;

	[Serialize]
	public bool autoRun;

	[Serialize]
	public string[] animFiles = new string[0];

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
