using System;
using System.Collections.Generic;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public readonly struct AttributesScope : IDisposable
	{
		public AttributesScope(IProperty target, IProperty source)
		{
			this.m_Target = target as IAttributes;
			IAttributes attributes = target as IAttributes;
			this.m_Previous = ((attributes != null) ? attributes.Attributes : null);
			bool flag = this.m_Target != null;
			if (flag)
			{
				IAttributes target2 = this.m_Target;
				IAttributes attributes2 = source as IAttributes;
				target2.Attributes = ((attributes2 != null) ? attributes2.Attributes : null);
			}
		}

		internal AttributesScope(IAttributes target, List<Attribute> attributes)
		{
			this.m_Target = target;
			this.m_Previous = target.Attributes;
			target.Attributes = attributes;
		}

		public void Dispose()
		{
			bool flag = this.m_Target != null;
			if (flag)
			{
				this.m_Target.Attributes = this.m_Previous;
			}
		}

		private readonly IAttributes m_Target;

		private readonly List<Attribute> m_Previous;
	}
}
