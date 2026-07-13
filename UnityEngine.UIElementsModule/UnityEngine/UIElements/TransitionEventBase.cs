using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.StyleTransition)]
	public abstract class TransitionEventBase<T> : EventBase<T>, ITransitionEvent where T : TransitionEventBase<T>, new()
	{
		public StylePropertyNameCollection stylePropertyNames { get; }

		public double elapsedTime { get; protected set; }

		protected TransitionEventBase()
		{
			this.stylePropertyNames = new StylePropertyNameCollection(new List<StylePropertyName>());
			this.LocalInit();
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles;
			this.stylePropertyNames.propertiesList.Clear();
			this.elapsedTime = 0.0;
		}

		public static T GetPooled(StylePropertyName stylePropertyName, double elapsedTime)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.stylePropertyNames.propertiesList.Add(stylePropertyName);
			pooled.elapsedTime = elapsedTime;
			return pooled;
		}

		public bool AffectsProperty(StylePropertyName stylePropertyName)
		{
			return this.stylePropertyNames.Contains(stylePropertyName);
		}
	}
}
