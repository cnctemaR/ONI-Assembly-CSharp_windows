using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Specialized
{
	[TypeForwardedFrom("WindowsBase, Version=3.0.0.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
	public class NotifyCollectionChangedEventArgs : EventArgs
	{
		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
		{
			if (action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Reset }), "action");
			}
			this.InitializeAdd(action, null, -1);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
		{
			if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException(global::SR.GetString("Constructor only supports either a Reset, Add, or Remove action."), "action");
			}
			if (action != NotifyCollectionChangedAction.Reset)
			{
				this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
				return;
			}
			if (changedItem != null)
			{
				throw new ArgumentException(global::SR.GetString("Reset action must be initialized with no changed items."), "action");
			}
			this.InitializeAdd(action, null, -1);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
		{
			if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException(global::SR.GetString("Constructor only supports either a Reset, Add, or Remove action."), "action");
			}
			if (action != NotifyCollectionChangedAction.Reset)
			{
				this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
				return;
			}
			if (changedItem != null)
			{
				throw new ArgumentException(global::SR.GetString("Reset action must be initialized with no changed items."), "action");
			}
			if (index != -1)
			{
				throw new ArgumentException(global::SR.GetString("Reset action must be initialized with index -1."), "action");
			}
			this.InitializeAdd(action, null, -1);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
		{
			if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException(global::SR.GetString("Constructor only supports either a Reset, Add, or Remove action."), "action");
			}
			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItems != null)
				{
					throw new ArgumentException(global::SR.GetString("Reset action must be initialized with no changed items."), "action");
				}
				this.InitializeAdd(action, null, -1);
				return;
			}
			else
			{
				if (changedItems == null)
				{
					throw new ArgumentNullException("changedItems");
				}
				this.InitializeAddOrRemove(action, changedItems, -1);
				return;
			}
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
		{
			if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove && action != NotifyCollectionChangedAction.Reset)
			{
				throw new ArgumentException(global::SR.GetString("Constructor only supports either a Reset, Add, or Remove action."), "action");
			}
			if (action == NotifyCollectionChangedAction.Reset)
			{
				if (changedItems != null)
				{
					throw new ArgumentException(global::SR.GetString("Reset action must be initialized with no changed items."), "action");
				}
				if (startingIndex != -1)
				{
					throw new ArgumentException(global::SR.GetString("Reset action must be initialized with index -1."), "action");
				}
				this.InitializeAdd(action, null, -1);
				return;
			}
			else
			{
				if (changedItems == null)
				{
					throw new ArgumentNullException("changedItems");
				}
				if (startingIndex < -1)
				{
					throw new ArgumentException(global::SR.GetString("Index cannot be negative."), "startingIndex");
				}
				this.InitializeAddOrRemove(action, changedItems, startingIndex);
				return;
			}
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Replace }), "action");
			}
			this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Replace }), "action");
			}
			this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Replace }), "action");
			}
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			if (oldItems == null)
			{
				throw new ArgumentNullException("oldItems");
			}
			this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
		{
			if (action != NotifyCollectionChangedAction.Replace)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Replace }), "action");
			}
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			if (oldItems == null)
			{
				throw new ArgumentNullException("oldItems");
			}
			this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
		{
			if (action != NotifyCollectionChangedAction.Move)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Move }), "action");
			}
			if (index < 0)
			{
				throw new ArgumentException(global::SR.GetString("Index cannot be negative."), "index");
			}
			object[] array = new object[] { changedItem };
			this.InitializeMoveOrReplace(action, array, array, index, oldIndex);
		}

		public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
		{
			if (action != NotifyCollectionChangedAction.Move)
			{
				throw new ArgumentException(global::SR.GetString("Constructor supports only the '{0}' action.", new object[] { NotifyCollectionChangedAction.Move }), "action");
			}
			if (index < 0)
			{
				throw new ArgumentException(global::SR.GetString("Index cannot be negative."), "index");
			}
			this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
		}

		internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newIndex, int oldIndex)
		{
			this._action = action;
			this._newItems = ((newItems == null) ? null : ArrayList.ReadOnly(newItems));
			this._oldItems = ((oldItems == null) ? null : ArrayList.ReadOnly(oldItems));
			this._newStartingIndex = newIndex;
			this._oldStartingIndex = oldIndex;
		}

		private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
		{
			if (action == NotifyCollectionChangedAction.Add)
			{
				this.InitializeAdd(action, changedItems, startingIndex);
				return;
			}
			if (action == NotifyCollectionChangedAction.Remove)
			{
				this.InitializeRemove(action, changedItems, startingIndex);
			}
		}

		private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
		{
			this._action = action;
			this._newItems = ((newItems == null) ? null : ArrayList.ReadOnly(newItems));
			this._newStartingIndex = newStartingIndex;
		}

		private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
		{
			this._action = action;
			this._oldItems = ((oldItems == null) ? null : ArrayList.ReadOnly(oldItems));
			this._oldStartingIndex = oldStartingIndex;
		}

		private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
		{
			this.InitializeAdd(action, newItems, startingIndex);
			this.InitializeRemove(action, oldItems, oldStartingIndex);
		}

		public NotifyCollectionChangedAction Action
		{
			get
			{
				return this._action;
			}
		}

		public IList NewItems
		{
			get
			{
				return this._newItems;
			}
		}

		public IList OldItems
		{
			get
			{
				return this._oldItems;
			}
		}

		public int NewStartingIndex
		{
			get
			{
				return this._newStartingIndex;
			}
		}

		public int OldStartingIndex
		{
			get
			{
				return this._oldStartingIndex;
			}
		}

		private NotifyCollectionChangedAction _action;

		private IList _newItems;

		private IList _oldItems;

		private int _newStartingIndex = -1;

		private int _oldStartingIndex = -1;
	}
}
