using System;
using UnityEngine;

namespace Klei.AI
{
	public class AnimatedSickness : Sickness.SicknessComponent
	{
		public AnimatedSickness(HashedString[] kanim_filenames, Expression expression)
		{
			this.kanims = new KAnimFile[kanim_filenames.Length];
			for (int i = 0; i < kanim_filenames.Length; i++)
			{
				this.kanims[i] = Assets.GetAnim(kanim_filenames[i]);
			}
			this.expression = expression;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			for (int i = 0; i < this.kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().AddAnimOverrides(this.kanims[i], 10f);
			}
			if (this.expression != null)
			{
				go.GetComponent<FaceGraph>().AddExpression(this.expression);
			}
			return null;
		}

		public override void OnCure(GameObject go, object instace_data)
		{
			if (this.expression != null)
			{
				go.GetComponent<FaceGraph>().RemoveExpression(this.expression);
			}
			for (int i = 0; i < this.kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(this.kanims[i]);
			}
		}

		private KAnimFile[] kanims;

		private Expression expression;
	}
}
