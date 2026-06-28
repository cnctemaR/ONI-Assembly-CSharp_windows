using System;
using UnityEngine;

namespace Klei.AI
{
	public class AnimatedAttributeModifierDisease : AttributeModifierDisease
	{
		public AnimatedAttributeModifierDisease(string id, HashedString[] kanim_filenames, string expression_id, bool use_custom_effect, AttributeModifier[] attribute_modifiers, float infection_probability, float sickness_duration, Disease.EffectProbabilityDelta[] effect_probability_deltas = null)
			: base(id, use_custom_effect, attribute_modifiers, infection_probability, sickness_duration, effect_probability_deltas)
		{
			this.kanims = new KAnimFile[kanim_filenames.Length];
			for (int i = 0; i < kanim_filenames.Length; i++)
			{
				this.kanims[i] = Assets.GetAnim(kanim_filenames[i]);
			}
			this.expressionID = expression_id;
		}

		protected override object OnInfect(GameObject go)
		{
			base.OnInfect(go);
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

		protected override void OnCure(GameObject go, object instace_data)
		{
			base.OnCure(go, instace_data);
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
