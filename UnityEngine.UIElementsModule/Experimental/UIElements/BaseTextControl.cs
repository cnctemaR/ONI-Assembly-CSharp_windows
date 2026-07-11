using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Abstract base class for controls containing a text property.</para>
	/// </summary>
	public abstract class BaseTextControl<T> : BaseTextElement, INotifyValueChanged<T>
	{
		public BaseTextControl()
		{
			this.focusIndex = 0;
		}

		public new virtual string text
		{
			get
			{
				return base.text;
			}
			protected set
			{
				base.text = value;
			}
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
		///   <para>UxmlTraits for the BaseTextControl.</para>
		/// </summary>
		public class BaseTextControlUxmlTraits : BaseTextElement.BaseTextElementUxmlTraits
		{
			public BaseTextControlUxmlTraits()
			{
				this.m_FocusIndex.defaultValue = 0;
			}
		}
	}
}
