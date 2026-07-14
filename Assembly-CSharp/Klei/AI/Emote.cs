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

		public bool IsValid { get; private set; }

		public KAnimFile ManifestSwimAnimSet()
		{
			if (this.swimAnimSetName != null && this.swimAnimSet == null)
			{
				this.swimAnimSet = Assets.GetAnim(this.swimAnimSetName);
			}
			return this.swimAnimSet;
		}

		public Emote(ResourceSet parent, string emoteId, EmoteStep[] defaultSteps, string animSetName = null, string swimAnimSetName = null)
			: base(emoteId, parent, null)
		{
			this.emoteSteps.AddRange(defaultSteps);
			this.animSetName = animSetName;
			this.swimAnimSetName = swimAnimSetName;
			this.IsValid = this.Validate();
		}

		private bool Validate()
		{
			KAnimFileData kanimFileData = ((this.AnimSet == null) ? null : this.AnimSet.GetData());
			if (kanimFileData == null)
			{
				return false;
			}
			for (int i = 0; i < this.StepCount; i++)
			{
				bool flag = false;
				for (int j = 0; j < kanimFileData.animCount; j++)
				{
					if (kanimFileData.GetAnim(j).name == this.emoteSteps[i].anim)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.LogWarningFormat("Emote AnimFile [{0}] does not have animations for emote step [{1}]", new object[]
					{
						this.animSetName,
						this.emoteSteps[i].anim
					});
					return false;
				}
			}
			return true;
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

		private HashedString swimAnimSetName = null;

		private KAnimFile swimAnimSet;

		private List<EmoteStep> emoteSteps = new List<EmoteStep>();
	}
}
