using System;

namespace Unity.Properties
{
	public interface IPropertyBag
	{
		void Accept(ITypeVisitor visitor);

		void Accept(IPropertyBagVisitor visitor, ref object container);
	}
}
