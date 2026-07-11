using System;
using System.ComponentModel;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XTypeDescriptionProvider<T> : TypeDescriptionProvider
	{
		public XTypeDescriptionProvider()
			: base(TypeDescriptor.GetProvider(typeof(T)))
		{
		}

		public override ICustomTypeDescriptor GetTypeDescriptor(Type type, object instance)
		{
			return new XTypeDescriptor<T>(base.GetTypeDescriptor(type, instance));
		}
	}
}
