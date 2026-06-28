using System;
using UnityEngine;

namespace Klei.AI
{
	public abstract class AnimatedDisease : Disease
	{
		public AnimatedDisease(string disease_id, float infection_probability, float sickness_duration, string kanim_filename, string expression_id)
			: base(disease_id, infection_probability, sickness_duration, null)
		{
			this.kanim = Assets.GetAnim(kanim_filename);
			this.expressionID = expression_id;
		}

		protected override object OnInfect(GameObject go)
		{
			go.GetComponent<KAnimControllerBase>().AddAnimOverrides(this.kanim, 10f);
			if (this.expressionID != null)
			{
				Expression expression = Db.Get().Expressions.TryGet(this.expressionID);
				go.GetComponent<FaceGraph>().AddExpression(expression);
			}
			return null;
		}

		protected override void OnCure(GameObject go, object instace_data)
		{
			if (this.expressionID != null)
			{
				Expression expression = Db.Get().Expressions.TryGet(this.expressionID);
				go.GetComponent<FaceGraph>().RemoveExpression(expression);
			}
			go.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(this.kanim);
		}

		private KAnimFile kanim;

		private string expressionID;
	}
}
