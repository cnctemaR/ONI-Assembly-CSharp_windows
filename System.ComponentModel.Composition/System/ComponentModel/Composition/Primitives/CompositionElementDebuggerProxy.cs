using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	internal class CompositionElementDebuggerProxy
	{
		public CompositionElementDebuggerProxy(CompositionElement element)
		{
			Requires.NotNull<CompositionElement>(element, "element");
			this._element = element;
		}

		public string DisplayName
		{
			get
			{
				return this._element.DisplayName;
			}
		}

		public ICompositionElement Origin
		{
			get
			{
				return this._element.Origin;
			}
		}

		public object UnderlyingObject
		{
			get
			{
				return this._element.UnderlyingObject;
			}
		}

		private readonly CompositionElement _element;
	}
}
