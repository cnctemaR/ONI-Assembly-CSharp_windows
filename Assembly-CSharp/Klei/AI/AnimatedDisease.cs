using System;
using UnityEngine;

namespace Klei.AI
{
	public class AnimatedDisease : Disease.DiseaseComponent
	{
		public AnimatedDisease(HashedString[] kanim_filenames, string expression_id)
		{
			this.kanims = new KAnimFile[kanim_filenames.Length];
			for (int i = 0; i < kanim_filenames.Length; i++)
			{
				this.kanims[i] = Assets.GetAnim(kanim_filenames[i]);
			}
			this.expressionID = expression_id;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			for (int i = 0; i < this.kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().AddAnimOverrides(this.kanims[i], 10f);
			}
			if (this.expressionID != null)
			{
				Expression expression = Db.Get().Expressions.TryGet(this.expressionID);
				go.GetComponent<FaceGraph>().AddExpression(expression);
			}
			return null;
		}

		public override void OnCure(GameObject go, object instace_data)
		{
			if (this.expressionID != null)
			{
				Expression expression = Db.Get().Expressions.TryGet(this.expressionID);
				go.GetComponent<FaceGraph>().RemoveExpression(expression);
			}
			for (int i = 0; i < this.kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(this.kanims[i]);
			}
		}

		private KAnimFile[] kanims;

		private string expressionID;
	}
}
