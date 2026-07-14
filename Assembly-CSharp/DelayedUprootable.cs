using System;

public class DelayedUprootable : Uprootable
{
	public override void Uproot()
	{
		KBatchedAnimController kbatchedAnimController;
		if (this.deathAnimation.IsValid && base.TryGetComponent<KBatchedAnimController>(out kbatchedAnimController))
		{
			kbatchedAnimController.Play(this.deathAnimation, KAnim.PlayMode.Once, 1f, 0f);
			kbatchedAnimController.onAnimComplete += delegate(HashedString anim)
			{
				this.FinalizeUproot();
			};
			return;
		}
		this.FinalizeUproot();
	}

	private void FinalizeUproot()
	{
		base.Uproot();
	}

	public HashedString deathAnimation;
}
