using System;
using System.Diagnostics;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	[DebuggerTypeProxy(typeof(CompositionElementDebuggerProxy))]
	[Serializable]
	internal class CompositionElement : SerializableCompositionElement
	{
		public CompositionElement(object underlyingObject)
			: base(underlyingObject.ToString(), CompositionElement.UnknownOrigin)
		{
			this._underlyingObject = underlyingObject;
		}

		public object UnderlyingObject
		{
			get
			{
				return this._underlyingObject;
			}
		}

		private static readonly ICompositionElement UnknownOrigin = new SerializableCompositionElement(Strings.CompositionElement_UnknownOrigin, null);

		private readonly object _underlyingObject;
	}
}
