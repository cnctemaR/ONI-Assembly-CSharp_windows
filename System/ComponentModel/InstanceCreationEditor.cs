using System;

namespace System.ComponentModel
{
	public abstract class InstanceCreationEditor
	{
		public virtual string Text
		{
			get
			{
				return "(New...)";
			}
		}

		public abstract object CreateInstance(ITypeDescriptorContext context, Type instanceType);
	}
}
