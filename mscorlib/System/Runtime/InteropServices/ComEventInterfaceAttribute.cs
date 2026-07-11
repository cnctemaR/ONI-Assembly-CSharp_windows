using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComEventInterfaceAttribute : Attribute
	{
		public ComEventInterfaceAttribute(Type SourceInterface, Type EventProvider)
		{
			this.si = SourceInterface;
			this.ep = EventProvider;
		}

		public Type EventProvider
		{
			get
			{
				return this.ep;
			}
		}

		public Type SourceInterface
		{
			get
			{
				return this.si;
			}
		}

		private Type si;

		private Type ep;
	}
}
