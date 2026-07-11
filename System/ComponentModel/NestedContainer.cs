using System;

namespace System.ComponentModel
{
	public class NestedContainer : Container, IDisposable, IContainer, INestedContainer
	{
		public NestedContainer(IComponent owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this._owner = owner;
			this._owner.Disposed += this.OnOwnerDisposed;
		}

		public IComponent Owner
		{
			get
			{
				return this._owner;
			}
		}

		protected virtual string OwnerName
		{
			get
			{
				if (this._owner.Site is INestedSite)
				{
					return ((INestedSite)this._owner.Site).FullName;
				}
				if (this._owner == null || this._owner.Site == null)
				{
					return null;
				}
				return this._owner.Site.Name;
			}
		}

		protected override ISite CreateSite(IComponent component, string name)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			return new NestedContainer.Site(component, this, name);
		}

		protected override object GetService(Type service)
		{
			if (service == typeof(INestedContainer))
			{
				return this;
			}
			return base.GetService(service);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._owner.Disposed -= this.OnOwnerDisposed;
			}
			base.Dispose(disposing);
		}

		private void OnOwnerDisposed(object sender, EventArgs e)
		{
			this.Dispose();
		}

		private IComponent _owner;

		private class Site : IServiceProvider, INestedSite, ISite
		{
			public Site(IComponent component, NestedContainer container, string name)
			{
				this._component = component;
				this._nestedContainer = container;
				this._siteName = name;
			}

			public IComponent Component
			{
				get
				{
					return this._component;
				}
			}

			public IContainer Container
			{
				get
				{
					return this._nestedContainer;
				}
			}

			public bool DesignMode
			{
				get
				{
					return this._nestedContainer.Owner != null && this._nestedContainer.Owner.Site != null && this._nestedContainer.Owner.Site.DesignMode;
				}
			}

			public string Name
			{
				get
				{
					return this._siteName;
				}
				set
				{
					this._siteName = value;
				}
			}

			public string FullName
			{
				get
				{
					if (this._siteName == null)
					{
						return null;
					}
					if (this._nestedContainer.OwnerName == null)
					{
						return this._siteName;
					}
					return this._nestedContainer.OwnerName + "." + this._siteName;
				}
			}

			public virtual object GetService(Type service)
			{
				if (service == typeof(ISite))
				{
					return this;
				}
				return this._nestedContainer.GetService(service);
			}

			private IComponent _component;

			private NestedContainer _nestedContainer;

			private string _siteName;
		}
	}
}
