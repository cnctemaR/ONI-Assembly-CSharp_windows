using System;

namespace UnityEngine
{
	public sealed class WaitUntil : CustomYieldInstruction
	{
		public WaitUntil(Func<bool> predicate)
		{
			this.m_Predicate = predicate;
		}

		public override bool keepWaiting
		{
			get
			{
				return !this.m_Predicate();
			}
		}

		private Func<bool> m_Predicate;
	}
}
