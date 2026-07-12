using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class NestedContainer : Container, INestedContainer, IContainer, IDisposable
	{
		public NestedContainer(IComponent owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.Owner = owner;
			this.Owner.Disposed += this.OnOwnerDisposed;
		}

		public IComponent Owner { get; }

		protected virtual string OwnerName
		{
			get
			{
				string text = null;
				if (this.Owner != null && this.Owner.Site != null)
				{
					INestedSite nestedSite = this.Owner.Site as INestedSite;
					if (nestedSite != null)
					{
						text = nestedSite.FullName;
					}
					else
					{
						text = this.Owner.Site.Name;
					}
				}
				return text;
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Owner.Disposed -= this.OnOwnerDisposed;
			}
			base.Dispose(disposing);
		}

		protected override object GetService(Type service)
		{
			if (service == typeof(INestedContainer))
			{
				return this;
			}
			return base.GetService(service);
		}

		private void OnOwnerDisposed(object sender, EventArgs e)
		{
			base.Dispose();
		}

		private class Site : INestedSite, ISite, IServiceProvider
		{
			internal Site(IComponent component, NestedContainer container, string name)
			{
				this.Component = component;
				this.Container = container;
				this._name = name;
			}

			public IComponent Component { get; }

			public IContainer Container { get; }

			public object GetService(Type service)
			{
				if (!(service == typeof(ISite)))
				{
					return ((NestedContainer)this.Container).GetService(service);
				}
				return this;
			}

			public bool DesignMode
			{
				get
				{
					IComponent owner = ((NestedContainer)this.Container).Owner;
					return owner != null && owner.Site != null && owner.Site.DesignMode;
				}
			}

			public string FullName
			{
				get
				{
					if (this._name != null)
					{
						string ownerName = ((NestedContainer)this.Container).OwnerName;
						string text = this._name;
						if (ownerName != null)
						{
							text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ownerName, text);
						}
						return text;
					}
					return this._name;
				}
			}

			public string Name
			{
				get
				{
					return this._name;
				}
				set
				{
					if (value == null || this._name == null || !value.Equals(this._name))
					{
						((NestedContainer)this.Container).ValidateName(this.Component, value);
						this._name = value;
					}
				}
			}

			private string _name;
		}
	}
}
