using System;

public class KCompBuildInstance
{
	public KCompBuildInstance(int eyes, int hair, int headshape, int mouth, KBatchedAnimController controller)
	{
		this.buildData = KCompBuilder.Instance.GenerateDefaultPose(eyes, hair, headshape, mouth);
		this.controller = controller;
	}

	public KAnimFileData GetData()
	{
		return this.buildData;
	}

	public HashedString GetAnimOverride()
	{
		return this.animOverride;
	}

	public void SetAnimOverride(HashedString anim_override)
	{
		this.animOverride = anim_override;
		this.Refresh(this.controller);
	}

	public void ClearAnimOverride()
	{
		this.animOverride = default(HashedString);
		this.Refresh(this.controller);
	}

	private void GetNeutralAndOverride(HashedString rootSymbol, ref KAnim.Anim neutral_anim, ref KAnim.Anim override_anim)
	{
		neutral_anim = null;
		override_anim = null;
		int num = 0;
		while (num < this.buildData.anims.Length && (neutral_anim == null || override_anim == null))
		{
			if (this.buildData.anims[num].rootSymbol == rootSymbol)
			{
				if (this.buildData.anims[num].hash == this.animOverride)
				{
					override_anim = this.buildData.anims[num];
				}
				else if (this.buildData.anims[num].hash == KCompBuilder.neutral)
				{
					neutral_anim = this.buildData.anims[num];
				}
			}
			num++;
		}
	}

	public void Refresh(KBatchedAnimController controller)
	{
		if (this.buildData.anims == null)
		{
			return;
		}
		KAnim.Anim anim = null;
		KAnim.Anim anim2 = null;
		this.GetNeutralAndOverride(this.root, ref anim, ref anim2);
		if (anim == null)
		{
			anim = anim2;
			anim2 = null;
		}
		if (anim != null)
		{
			KAnim.Anim.Frame frame = anim.GetFrame(this.buildData.batchTag, 0);
			int num = -1;
			int num2 = -1;
			if (anim2 != null)
			{
				KAnim.Anim.Frame frame2 = anim2.GetFrame(this.buildData.batchTag, 0);
				num = frame2.numElements;
				num2 = frame2.firstElementIdx;
			}
			for (int i = 0; i < frame.numElements; i++)
			{
				KAnim.Anim.FrameElement frameElement = this.buildData.animFrameElements[frame.firstElementIdx + i];
				KAnim.Build.Symbol symbol = this.buildData.build.GetSymbol(frameElement.symbol);
				if (symbol != null)
				{
					controller.RemoveSingleFrameOverride(frameElement.symbol);
					controller.AddSymbolOverride(frameElement.symbol, this.buildData.build.batchTag, symbol);
					for (int j = 0; j < num; j++)
					{
						KAnim.Anim.FrameElement frameElement2 = this.buildData.animFrameElements[num2 + j];
						if (frameElement2.symbol == frameElement.symbol)
						{
							if (frameElement.frame != frameElement2.frame)
							{
								controller.ApplySingleFrameOverride(frameElement.symbol, frameElement.frame, frameElement2.frame);
							}
							break;
						}
					}
					controller.ShowSymbol(frameElement.symbol);
				}
			}
		}
	}

	private HashedString animOverride;

	private KBatchedAnimController controller;

	private KAnimFileData buildData;

	public HashedString root = KCompBuilder.head_comp;
}
