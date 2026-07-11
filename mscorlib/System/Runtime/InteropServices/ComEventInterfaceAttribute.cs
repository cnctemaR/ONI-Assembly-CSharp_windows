using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComEventInterfaceAttribute : Attribute
	{
		public ComEventInterfaceAttribute(Type SourceInterface, Type EventProvider)
		{
			this._SourceInterface = SourceInterface;
			this._EventProvider = EventProvider;
		}

		public Type SourceInterface
		{
			get
			{
				return this._SourceInterface;
			}
		}

		public Type EventProvider
		{
			get
			{
				return this._EventProvider;
			}
		}

		internal Type _SourceInterface;

		internal Type _EventProvider;
	}
}
