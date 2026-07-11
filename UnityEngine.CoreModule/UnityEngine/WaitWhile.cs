using System;

namespace UnityEngine
{
	public sealed class WaitWhile : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				return this.m_Predicate();
			}
		}

		public WaitWhile(Func<bool> predicate)
		{
			this.m_Predicate = predicate;
		}

		private Func<bool> m_Predicate;
	}
}
