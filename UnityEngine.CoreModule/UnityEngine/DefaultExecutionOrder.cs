using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[AttributeUsage(AttributeTargets.Class)]
	public class DefaultExecutionOrder : Attribute
	{
		public DefaultExecutionOrder(int order)
		{
			this.m_Order = order;
		}

		public int order
		{
			get
			{
				return this.m_Order;
			}
		}

		private int m_Order;
	}
}
