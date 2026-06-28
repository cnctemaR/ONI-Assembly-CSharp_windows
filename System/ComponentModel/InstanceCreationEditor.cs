using System;

namespace System.ComponentModel
{
	public abstract class InstanceCreationEditor
	{
		public virtual string Text
		{
			get
			{
				return global::Locale.GetText("(New ...)");
			}
		}

		public abstract object CreateInstance(ITypeDescriptorContext context, Type type);
	}
}
