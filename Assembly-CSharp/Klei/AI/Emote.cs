using System;
using System.Collections.Generic;

namespace Klei.AI
{
	public class Emote : Resource
	{
		public int StepCount
		{
			get
			{
				if (this.emoteSteps != null)
				{
					return this.emoteSteps.Count;
				}
				return 0;
			}
		}

		public KAnimFile AnimSet
		{
			get
			{
				if (this.animSetName != HashedString.Invalid && this.animSet == null)
				{
					this.animSet = Assets.GetAnim(this.animSetName);
				}
				return this.animSet;
			}
		}

		public Emote(ResourceSet parent, string emoteId, EmoteStep[] defaultSteps, string animSetName = null)
			: base(emoteId, parent, null)
		{
			this.emoteSteps.AddRange(defaultSteps);
			this.animSetName = animSetName;
		}

		public bool IsValidForController(KBatchedAnimController animController)
		{
			bool flag = true;
			int num = 0;
			while (flag && num < this.StepCount)
			{
				flag = animController.HasAnimation(this.emoteSteps[num].anim);
				num++;
			}
			KAnimFileData kanimFileData = ((this.animSet == null) ? null : this.animSet.GetData());
			int num2 = 0;
			while (kanimFileData != null && flag && num2 < this.StepCount)
			{
				bool flag2 = false;
				int num3 = 0;
				while (!flag2 && num3 < kanimFileData.animCount)
				{
					flag2 = kanimFileData.GetAnim(num2).id == this.emoteSteps[num2].anim;
					num3++;
				}
				flag = flag2;
				num2++;
			}
			return flag;
		}

		public void ApplyAnimOverrides(KBatchedAnimController animController, KAnimFile overrideSet)
		{
			KAnimFile kanimFile = ((overrideSet != null) ? overrideSet : this.AnimSet);
			if (kanimFile == null || animController == null)
			{
				return;
			}
			animController.AddAnimOverrides(kanimFile, 0f);
		}

		public void RemoveAnimOverrides(KBatchedAnimController animController, KAnimFile overrideSet)
		{
			KAnimFile kanimFile = ((overrideSet != null) ? overrideSet : this.AnimSet);
			if (kanimFile == null || animController == null)
			{
				return;
			}
			animController.RemoveAnimOverrides(kanimFile);
		}

		public void CollectStepAnims(out HashedString[] emoteAnims, int iterations)
		{
			emoteAnims = new HashedString[this.emoteSteps.Count * iterations];
			for (int i = 0; i < emoteAnims.Length; i++)
			{
				emoteAnims[i] = this.emoteSteps[i % this.emoteSteps.Count].anim;
			}
		}

		public bool IsValidStep(int stepIdx)
		{
			return stepIdx >= 0 && stepIdx < this.emoteSteps.Count;
		}

		public EmoteStep this[int stepIdx]
		{
			get
			{
				if (!this.IsValidStep(stepIdx))
				{
					return null;
				}
				return this.emoteSteps[stepIdx];
			}
		}

		public int GetStepIndex(HashedString animName)
		{
			int i = 0;
			bool flag = false;
			while (i < this.emoteSteps.Count)
			{
				if (this.emoteSteps[i].anim == animName)
				{
					flag = true;
					break;
				}
				i++;
			}
			Debug.Assert(flag, string.Format("Could not find emote step {0} for emote {1}!", animName, this.Id));
			return i;
		}

		private HashedString animSetName = null;

		private KAnimFile animSet;

		private List<EmoteStep> emoteSteps = new List<EmoteStep>();
	}
}
