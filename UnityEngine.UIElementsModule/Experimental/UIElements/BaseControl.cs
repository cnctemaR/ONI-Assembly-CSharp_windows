using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Abstract base class for controls.</para>
	/// </summary>
	public abstract class BaseControl<T> : VisualElement, INotifyValueChanged<T>
	{
		public BaseControl()
		{
			this.focusIndex = 0;
		}

		public abstract T value { get; set; }

		public virtual void OnValueChanged(EventCallback<ChangeEvent<T>> callback)
		{
			base.RegisterCallback<ChangeEvent<T>>(callback, Capture.NoCapture);
		}

		public virtual void SetValueAndNotify(T newValue)
		{
			if (!EqualityComparer<T>.Default.Equals(this.value, newValue))
			{
				using (ChangeEvent<T> pooled = ChangeEvent<T>.GetPooled(this.value, newValue))
				{
					pooled.target = this;
					this.value = newValue;
					UIElementsUtility.eventDispatcher.DispatchEvent(pooled, base.panel);
				}
			}
		}

		/// <summary>
		///   <para>UxmlTraits for the BaseControl.</para>
		/// </summary>
		public class BaseControlUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			public BaseControlUxmlTraits()
			{
				this.m_FocusIndex.defaultValue = 0;
			}

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}
		}
	}
}
