using System;
using UnityEngine;

public class KCompBuildInstance
{
	public KCompBuildInstance(KCompBuilder.BodyData data, KBatchedAnimController controller)
	{
		this.buildData = KCompBuilder.Instance.GenerateDefaultPose(data);
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
		while (num < this.buildData.animCount && (neutral_anim == null || override_anim == null))
		{
			KAnim.Anim anim = this.buildData.GetAnim(num);
			if (anim.rootSymbol == rootSymbol)
			{
				if (anim.hash == this.animOverride)
				{
					override_anim = anim;
				}
				else if (anim.hash == KCompBuilder.neutral)
				{
					neutral_anim = anim;
				}
			}
			num++;
		}
	}

	public void Refresh(KBatchedAnimController target_controller = null)
	{
		if (target_controller == null)
		{
			target_controller = this.controller;
		}
		if (this.buildData.animCount == 0)
		{
			return;
		}
		try
		{
			this.ApplyNeutralAndOverride(target_controller, KCompBuilder.head_comp);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception while applying override " + ex.Message + "\n" + ex.StackTrace);
		}
	}

	private void ApplyNeutralAndOverride(KBatchedAnimController target_controller, HashedString root)
	{
		KAnim.Anim anim = null;
		KAnim.Anim anim2 = null;
		this.GetNeutralAndOverride(root, ref anim, ref anim2);
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
				KAnim.Anim.FrameElement animFrameElement = this.buildData.GetAnimFrameElement(frame.firstElementIdx + i);
				KAnim.Build.Symbol symbol = this.buildData.build.GetSymbol(animFrameElement.symbol);
				if (symbol != null)
				{
					target_controller.RemoveSingleFrameOverride(animFrameElement.symbol);
					target_controller.AddSymbolOverride(animFrameElement.symbol, this.buildData.build.batchTag, symbol, false);
					for (int j = 0; j < num; j++)
					{
						KAnim.Anim.FrameElement animFrameElement2 = this.buildData.GetAnimFrameElement(num2 + j);
						if (animFrameElement2.symbol == animFrameElement.symbol)
						{
							if (animFrameElement.frame != animFrameElement2.frame)
							{
								target_controller.ApplySingleFrameOverride(animFrameElement.symbol, animFrameElement.frame, animFrameElement2.frame);
							}
							break;
						}
					}
					target_controller.ShowSymbol(animFrameElement.symbol);
				}
			}
		}
	}

	private HashedString animOverride;

	private KBatchedAnimController controller;

	private KAnimFileData buildData;
}
