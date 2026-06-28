using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	public class ServiceContainer : IDisposable, IServiceProvider, IServiceContainer
	{
		public ServiceContainer()
			: this(null)
		{
		}

		public ServiceContainer(IServiceProvider parentProvider)
		{
			this.parentProvider = parentProvider;
		}

		private Hashtable Services
		{
			get
			{
				if (this.services == null)
				{
					this.services = new Hashtable();
				}
				return this.services;
			}
		}

		public void AddService(Type serviceType, object serviceInstance)
		{
			this.AddService(serviceType, serviceInstance, false);
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			this.AddService(serviceType, callback, false);
		}

		public virtual void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (promote && this.parentProvider != null)
			{
				IServiceContainer serviceContainer = (IServiceContainer)this.parentProvider.GetService(typeof(IServiceContainer));
				serviceContainer.AddService(serviceType, serviceInstance, promote);
				return;
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}
			if (this.Services.Contains(serviceType))
			{
				throw new ArgumentException(string.Format("The service {0} already exists in the service container.", serviceType.ToString()), "serviceType");
			}
			this.Services.Add(serviceType, serviceInstance);
		}

		public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			if (promote && this.parentProvider != null)
			{
				IServiceContainer serviceContainer = (IServiceContainer)this.parentProvider.GetService(typeof(IServiceContainer));
				serviceContainer.AddService(serviceType, callback, promote);
				return;
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (this.Services.Contains(serviceType))
			{
				throw new ArgumentException(string.Format("The service {0} already exists in the service container.", serviceType.ToString()), "serviceType");
			}
			this.Services.Add(serviceType, callback);
		}

		public void RemoveService(Type serviceType)
		{
			this.RemoveService(serviceType, false);
		}

		public virtual void RemoveService(Type serviceType, bool promote)
		{
			if (promote && this.parentProvider != null)
			{
				IServiceContainer serviceContainer = (IServiceContainer)this.parentProvider.GetService(typeof(IServiceContainer));
				serviceContainer.RemoveService(serviceType, promote);
				return;
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}
			this.Services.Remove(serviceType);
		}

		public virtual object GetService(Type serviceType)
		{
			object obj = null;
			Type[] defaultServices = this.DefaultServices;
			for (int i = 0; i < defaultServices.Length; i++)
			{
				if (defaultServices[i] == serviceType)
				{
					obj = this;
					break;
				}
			}
			if (obj == null)
			{
				obj = this.Services[serviceType];
			}
			if (obj == null && this.parentProvider != null)
			{
				obj = this.parentProvider.GetService(serviceType);
			}
			if (obj != null)
			{
				ServiceCreatorCallback serviceCreatorCallback = obj as ServiceCreatorCallback;
				if (serviceCreatorCallback != null)
				{
					obj = serviceCreatorCallback(this, serviceType);
					this.Services[serviceType] = obj;
				}
			}
			return obj;
		}

		protected virtual Type[] DefaultServices
		{
			get
			{
				return new Type[]
				{
					typeof(IServiceContainer),
					typeof(ServiceContainer)
				};
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing && this.services != null)
				{
					foreach (object obj in this.services)
					{
						if (obj is IDisposable)
						{
							((IDisposable)obj).Dispose();
						}
					}
					this.services = null;
				}
				this._disposed = true;
			}
		}

		private IServiceProvider parentProvider;

		private Hashtable services;

		private bool _disposed;
	}
}
