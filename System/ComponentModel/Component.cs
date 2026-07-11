using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[DesignerCategory("Component")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	public class Component : MarshalByRefObject, IDisposable, IComponent
	{
		public Component()
		{
			this.event_handlers = null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler Disposed
		{
			add
			{
				this.Events.AddHandler(this.disposedEvent, value);
			}
			remove
			{
				this.Events.RemoveHandler(this.disposedEvent, value);
			}
		}

		protected virtual bool CanRaiseEvents
		{
			get
			{
				return false;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual ISite Site
		{
			get
			{
				return this.mySite;
			}
			set
			{
				this.mySite = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IContainer Container
		{
			get
			{
				if (this.mySite == null)
				{
					return null;
				}
				return this.mySite.Container;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected bool DesignMode
		{
			get
			{
				return this.mySite != null && this.mySite.DesignMode;
			}
		}

		protected EventHandlerList Events
		{
			get
			{
				if (this.event_handlers == null)
				{
					this.event_handlers = new EventHandlerList();
				}
				return this.event_handlers;
			}
		}

		~Component()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool release_all)
		{
			if (release_all)
			{
				if (this.mySite != null && this.mySite.Container != null)
				{
					this.mySite.Container.Remove(this);
				}
				EventHandler eventHandler = (EventHandler)this.Events[this.disposedEvent];
				if (eventHandler != null)
				{
					eventHandler(this, EventArgs.Empty);
				}
			}
		}

		protected virtual object GetService(Type service)
		{
			if (this.mySite != null)
			{
				return this.mySite.GetService(service);
			}
			return null;
		}

		public override string ToString()
		{
			if (this.mySite == null)
			{
				return base.GetType().ToString();
			}
			return string.Format("{0} [{1}]", this.mySite.Name, base.GetType().ToString());
		}

		private EventHandlerList event_handlers;

		private ISite mySite;

		private object disposedEvent = new object();
	}
}
