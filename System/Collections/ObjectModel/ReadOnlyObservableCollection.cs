using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Collections.ObjectModel
{
	[TypeForwardedFrom("WindowsBase, Version=3.0.0.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
	[Serializable]
	public class ReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public ReadOnlyObservableCollection(ObservableCollection<T> list)
			: base(list)
		{
			((INotifyCollectionChanged)base.Items).CollectionChanged += this.HandleCollectionChanged;
			((INotifyPropertyChanged)base.Items).PropertyChanged += this.HandlePropertyChanged;
		}

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add
			{
				this.CollectionChanged += value;
			}
			remove
			{
				this.CollectionChanged -= value;
			}
		}

		[field: NonSerialized]
		protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, args);
			}
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this.PropertyChanged += value;
			}
			remove
			{
				this.PropertyChanged -= value;
			}
		}

		[field: NonSerialized]
		protected virtual event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, args);
			}
		}

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnCollectionChanged(e);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e);
		}
	}
}
