using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class BaseField<T> : BindableElement, INotifyValueChanged<T>
	{
		public BaseField()
		{
			this.focusIndex = 0;
			this.m_Value = default(T);
		}

		public virtual T value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (!EqualityComparer<T>.Default.Equals(this.m_Value, value))
				{
					if (base.panel != null)
					{
						using (ChangeEvent<T> pooled = ChangeEvent<T>.GetPooled(this.m_Value, value))
						{
							pooled.target = this;
							this.SetValueWithoutNotify(value);
							this.SendEvent(pooled);
						}
					}
					else
					{
						this.SetValueWithoutNotify(value);
					}
				}
			}
		}

		[Obsolete("This method is replaced by simply using this.value. The default behaviour has been changed to notify when changed. If the behaviour is not to be notified, SetValueWithoutNotify() must be used.", false)]
		public virtual void SetValueAndNotify(T newValue)
		{
			this.value = newValue;
		}

		public override void OnPersistentDataReady()
		{
			base.OnPersistentDataReady();
			string fullHierarchicalPersistenceKey = base.GetFullHierarchicalPersistenceKey();
			T value = this.m_Value;
			base.OverwriteFromPersistedData(this, fullHierarchicalPersistenceKey);
			if (!EqualityComparer<T>.Default.Equals(value, this.m_Value))
			{
				using (ChangeEvent<T> pooled = ChangeEvent<T>.GetPooled(value, this.m_Value))
				{
					pooled.target = this;
					this.SendEvent(pooled);
				}
			}
		}

		public void OnValueChanged(EventCallback<ChangeEvent<T>> callback)
		{
			base.RegisterCallback<ChangeEvent<T>>(callback, TrickleDown.NoTrickleDown);
		}

		public void RemoveOnValueChanged(EventCallback<ChangeEvent<T>> callback)
		{
			base.UnregisterCallback<ChangeEvent<T>>(callback, TrickleDown.NoTrickleDown);
		}

		public virtual void SetValueWithoutNotify(T newValue)
		{
			this.m_Value = newValue;
			if (!string.IsNullOrEmpty(base.persistenceKey))
			{
				base.SavePersistentData();
			}
			base.MarkDirtyRepaint();
		}

		[SerializeField]
		protected T m_Value;

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public UxmlTraits()
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
