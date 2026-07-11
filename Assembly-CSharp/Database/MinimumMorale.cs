using System;
using System.Collections;
using System.IO;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class MinimumMorale : VictoryColonyAchievementRequirement
	{
		public MinimumMorale(int minimumMorale = 16)
		{
			this.minimumMorale = minimumMorale;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE, this.minimumMorale);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE_DESCRIPTION, this.minimumMorale);
		}

		public override bool Success()
		{
			bool flag = true;
			IEnumerator enumerator = Components.MinionAssignablesProxy.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)obj;
					GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
					if (targetGameObject != null)
					{
						if (!targetGameObject.HasTag(GameTags.Dead))
						{
							AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
							flag = attributeInstance != null && attributeInstance.GetTotalValue() >= (float)this.minimumMorale && flag;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.minimumMorale);
		}

		public override void Deserialize(IReader reader)
		{
			this.minimumMorale = reader.ReadInt32();
		}

		private int minimumMorale;
	}
}
