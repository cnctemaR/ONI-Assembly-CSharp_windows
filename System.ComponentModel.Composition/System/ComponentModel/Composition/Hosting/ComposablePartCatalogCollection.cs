using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	internal class ComposablePartCatalogCollection : ICollection<ComposablePartCatalog>, IEnumerable<ComposablePartCatalog>, IEnumerable, INotifyComposablePartCatalogChanged, IDisposable
	{
		public ComposablePartCatalogCollection(IEnumerable<ComposablePartCatalog> catalogs, Action<ComposablePartCatalogChangeEventArgs> onChanged, Action<ComposablePartCatalogChangeEventArgs> onChanging)
		{
			catalogs = catalogs ?? Enumerable.Empty<ComposablePartCatalog>();
			this._catalogs = new List<ComposablePartCatalog>(catalogs);
			this._onChanged = onChanged;
			this._onChanging = onChanging;
			this.SubscribeToCatalogNotifications(catalogs);
		}

		public void Add(ComposablePartCatalog item)
		{
			Requires.NotNull<ComposablePartCatalog>(item, "item");
			this.ThrowIfDisposed();
			Lazy<IEnumerable<ComposablePartDefinition>> lazy = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.ToArray<ComposablePartDefinition>(), LazyThreadSafetyMode.PublicationOnly);
			using (AtomicComposition atomicComposition = new AtomicComposition())
			{
				this.RaiseChangingEvent(lazy, null, atomicComposition);
				using (new WriteLock(this._lock))
				{
					if (this._isCopyNeeded)
					{
						this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
						this._isCopyNeeded = false;
					}
					this._hasChanged = true;
					this._catalogs.Add(item);
				}
				this.SubscribeToCatalogNotifications(item);
				atomicComposition.Complete();
			}
			this.RaiseChangedEvent(lazy, null);
		}

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		public void Clear()
		{
			this.ThrowIfDisposed();
			ComposablePartCatalog[] catalogs = null;
			using (new ReadLock(this._lock))
			{
				if (this._catalogs.Count == 0)
				{
					return;
				}
				catalogs = this._catalogs.ToArray();
			}
			Lazy<IEnumerable<ComposablePartDefinition>> lazy = new Lazy<IEnumerable<ComposablePartDefinition>>(() => catalogs.SelectMany<ComposablePartCatalog, ComposablePartDefinition>((ComposablePartCatalog catalog) => catalog).ToArray<ComposablePartDefinition>(), LazyThreadSafetyMode.PublicationOnly);
			using (AtomicComposition atomicComposition = new AtomicComposition())
			{
				this.RaiseChangingEvent(null, lazy, atomicComposition);
				this.UnsubscribeFromCatalogNotifications(catalogs);
				using (new WriteLock(this._lock))
				{
					this._catalogs = new List<ComposablePartCatalog>();
					this._isCopyNeeded = false;
					this._hasChanged = true;
				}
				atomicComposition.Complete();
			}
			this.RaiseChangedEvent(null, lazy);
		}

		public bool Contains(ComposablePartCatalog item)
		{
			Requires.NotNull<ComposablePartCatalog>(item, "item");
			this.ThrowIfDisposed();
			bool flag;
			using (new ReadLock(this._lock))
			{
				flag = this._catalogs.Contains(item);
			}
			return flag;
		}

		public void CopyTo(ComposablePartCatalog[] array, int arrayIndex)
		{
			this.ThrowIfDisposed();
			using (new ReadLock(this._lock))
			{
				this._catalogs.CopyTo(array, arrayIndex);
			}
		}

		public int Count
		{
			get
			{
				this.ThrowIfDisposed();
				int count;
				using (new ReadLock(this._lock))
				{
					count = this._catalogs.Count;
				}
				return count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				this.ThrowIfDisposed();
				return false;
			}
		}

		public bool Remove(ComposablePartCatalog item)
		{
			Requires.NotNull<ComposablePartCatalog>(item, "item");
			this.ThrowIfDisposed();
			using (new ReadLock(this._lock))
			{
				if (!this._catalogs.Contains(item))
				{
					return false;
				}
			}
			bool flag = false;
			Lazy<IEnumerable<ComposablePartDefinition>> lazy = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.ToArray<ComposablePartDefinition>(), LazyThreadSafetyMode.PublicationOnly);
			using (AtomicComposition atomicComposition = new AtomicComposition())
			{
				this.RaiseChangingEvent(null, lazy, atomicComposition);
				using (new WriteLock(this._lock))
				{
					if (this._isCopyNeeded)
					{
						this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
						this._isCopyNeeded = false;
					}
					flag = this._catalogs.Remove(item);
					if (flag)
					{
						this._hasChanged = true;
					}
				}
				this.UnsubscribeFromCatalogNotifications(item);
				atomicComposition.Complete();
			}
			this.RaiseChangedEvent(null, lazy);
			return flag;
		}

		internal bool HasChanged
		{
			get
			{
				this.ThrowIfDisposed();
				bool hasChanged;
				using (new ReadLock(this._lock))
				{
					hasChanged = this._hasChanged;
				}
				return hasChanged;
			}
		}

		public IEnumerator<ComposablePartCatalog> GetEnumerator()
		{
			this.ThrowIfDisposed();
			IEnumerator<ComposablePartCatalog> enumerator2;
			using (new WriteLock(this._lock))
			{
				IEnumerator<ComposablePartCatalog> enumerator = this._catalogs.GetEnumerator();
				this._isCopyNeeded = true;
				enumerator2 = enumerator;
			}
			return enumerator2;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				bool flag = false;
				IEnumerable<ComposablePartCatalog> enumerable = null;
				try
				{
					using (new WriteLock(this._lock))
					{
						if (!this._isDisposed)
						{
							flag = true;
							enumerable = this._catalogs;
							this._catalogs = null;
							this._isDisposed = true;
						}
					}
				}
				finally
				{
					if (enumerable != null)
					{
						this.UnsubscribeFromCatalogNotifications(enumerable);
						enumerable.ForEach<ComposablePartCatalog>(delegate(ComposablePartCatalog catalog)
						{
							catalog.Dispose();
						});
					}
					if (flag)
					{
						this._lock.Dispose();
					}
				}
			}
		}

		private void RaiseChangedEvent(Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions, Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions)
		{
			if (this._onChanged == null || this.Changed == null)
			{
				return;
			}
			IEnumerable<ComposablePartDefinition> enumerable = ((addedDefinitions == null) ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
			IEnumerable<ComposablePartDefinition> enumerable2 = ((removedDefinitions == null) ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);
			this._onChanged(new ComposablePartCatalogChangeEventArgs(enumerable, enumerable2, null));
		}

		public void OnChanged(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
			if (changed != null)
			{
				changed(sender, e);
			}
		}

		private void RaiseChangingEvent(Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions, Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions, AtomicComposition atomicComposition)
		{
			if (this._onChanging == null || this.Changing == null)
			{
				return;
			}
			IEnumerable<ComposablePartDefinition> enumerable = ((addedDefinitions == null) ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
			IEnumerable<ComposablePartDefinition> enumerable2 = ((removedDefinitions == null) ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);
			this._onChanging(new ComposablePartCatalogChangeEventArgs(enumerable, enumerable2, atomicComposition));
		}

		public void OnChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changing = this.Changing;
			if (changing != null)
			{
				changing(sender, e);
			}
		}

		private void OnContainedCatalogChanged(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			if (this._onChanged == null || this.Changed == null)
			{
				return;
			}
			this._onChanged(e);
		}

		private void OnContainedCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			if (this._onChanging == null || this.Changing == null)
			{
				return;
			}
			this._onChanging(e);
		}

		private void SubscribeToCatalogNotifications(ComposablePartCatalog catalog)
		{
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = catalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changed += this.OnContainedCatalogChanged;
				notifyComposablePartCatalogChanged.Changing += this.OnContainedCatalogChanging;
			}
		}

		private void SubscribeToCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
		{
			foreach (ComposablePartCatalog composablePartCatalog in catalogs)
			{
				this.SubscribeToCatalogNotifications(composablePartCatalog);
			}
		}

		private void UnsubscribeFromCatalogNotifications(ComposablePartCatalog catalog)
		{
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = catalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changed -= this.OnContainedCatalogChanged;
				notifyComposablePartCatalogChanged.Changing -= this.OnContainedCatalogChanging;
			}
		}

		private void UnsubscribeFromCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
		{
			foreach (ComposablePartCatalog composablePartCatalog in catalogs)
			{
				this.UnsubscribeFromCatalogNotifications(composablePartCatalog);
			}
		}

		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private readonly Microsoft.Internal.Lock _lock = new Microsoft.Internal.Lock();

		private Action<ComposablePartCatalogChangeEventArgs> _onChanged;

		private Action<ComposablePartCatalogChangeEventArgs> _onChanging;

		private List<ComposablePartCatalog> _catalogs = new List<ComposablePartCatalog>();

		private volatile bool _isCopyNeeded;

		private volatile bool _isDisposed;

		private bool _hasChanged;
	}
}
