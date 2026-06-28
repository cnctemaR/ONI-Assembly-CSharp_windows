using System;
using System.Collections.Generic;

namespace System.ComponentModel
{
	public class Container : IDisposable, IContainer
	{
		public virtual ComponentCollection Components
		{
			get
			{
				IComponent[] array = this.c.ToArray();
				return new ComponentCollection(array);
			}
		}

		public virtual void Add(IComponent component)
		{
			this.Add(component, null);
		}

		public virtual void Add(IComponent component, string name)
		{
			if (component != null && (component.Site == null || component.Site.Container != this))
			{
				this.ValidateName(component, name);
				if (component.Site != null)
				{
					component.Site.Container.Remove(component);
				}
				component.Site = this.CreateSite(component, name);
				this.c.Add(component);
			}
		}

		protected virtual void ValidateName(IComponent component, string name)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (name == null)
			{
				return;
			}
			foreach (IComponent component2 in this.c)
			{
				if (!object.ReferenceEquals(component, component2))
				{
					if (component2.Site != null && string.Compare(component2.Site.Name, name, true) == 0)
					{
						throw new ArgumentException(string.Format("There already is a named component '{0}' in this container", name));
					}
				}
			}
		}

		protected virtual ISite CreateSite(IComponent component, string name)
		{
			return new Container.DefaultSite(name, component, this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				while (this.c.Count > 0)
				{
					int num = this.c.Count - 1;
					IComponent component = this.c[num];
					this.Remove(component);
					component.Dispose();
				}
			}
		}

		~Container()
		{
			this.Dispose(false);
		}

		protected virtual object GetService(Type service)
		{
			if (typeof(IContainer) != service)
			{
				return null;
			}
			return this;
		}

		public virtual void Remove(IComponent component)
		{
			this.Remove(component, true);
		}

		private void Remove(IComponent component, bool unsite)
		{
			if (component != null && component.Site != null && component.Site.Container == this)
			{
				if (unsite)
				{
					component.Site = null;
				}
				this.c.Remove(component);
			}
		}

		protected void RemoveWithoutUnsiting(IComponent component)
		{
			this.Remove(component, false);
		}

		private List<IComponent> c = new List<IComponent>();

		private class DefaultSite : IServiceProvider, ISite
		{
			public DefaultSite(string name, IComponent component, Container container)
			{
				this.component = component;
				this.container = container;
				this.name = name;
			}

			public IComponent Component
			{
				get
				{
					return this.component;
				}
			}

			public IContainer Container
			{
				get
				{
					return this.container;
				}
			}

			public bool DesignMode
			{
				get
				{
					return false;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public virtual object GetService(Type t)
			{
				if (typeof(ISite) == t)
				{
					return this;
				}
				return this.container.GetService(t);
			}

			private readonly IComponent component;

			private readonly Container container;

			private string name;
		}
	}
}
