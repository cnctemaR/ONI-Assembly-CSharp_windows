using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	[Serializable]
	internal class SerializableCompositionElement : ICompositionElement
	{
		public SerializableCompositionElement(string displayName, ICompositionElement origin)
		{
			Assumes.IsTrue(origin == null || origin.GetType().IsSerializable);
			this._displayName = displayName ?? string.Empty;
			this._origin = origin;
		}

		public string DisplayName
		{
			get
			{
				return this._displayName;
			}
		}

		public ICompositionElement Origin
		{
			get
			{
				return this._origin;
			}
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		public static ICompositionElement FromICompositionElement(ICompositionElement element)
		{
			if (element == null)
			{
				return null;
			}
			ICompositionElement compositionElement = SerializableCompositionElement.FromICompositionElement(element.Origin);
			return new SerializableCompositionElement(element.DisplayName, compositionElement);
		}

		private readonly string _displayName;

		private readonly ICompositionElement _origin;
	}
}
