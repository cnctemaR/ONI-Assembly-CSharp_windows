using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[Designer("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(global::System.ComponentModel.Design.IRootDesigner))]
	[DesignerCategory("Component")]
	[TypeConverter(typeof(ComponentConverter))]
	[ComVisible(true)]
	public class MarshalByValueComponent : IDisposable, IServiceProvider, IComponent
	{
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

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[global::System.MonoTODO]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		~MarshalByValueComponent()
		{
			this.Dispose(false);
		}

		public virtual object GetService(Type service)
		{
			if (this.mySite != null)
			{
				return this.mySite.GetService(service);
			}
			return null;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IContainer Container
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
		public virtual bool DesignMode
		{
			get
			{
				return this.mySite != null && this.mySite.DesignMode;
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

		public override string ToString()
		{
			if (this.mySite == null)
			{
				return base.GetType().ToString();
			}
			return string.Format("{0} [{1}]", this.mySite.Name, base.GetType().ToString());
		}

		protected EventHandlerList Events
		{
			get
			{
				if (this.eventList == null)
				{
					this.eventList = new EventHandlerList();
				}
				return this.eventList;
			}
		}

		private EventHandlerList eventList;

		private ISite mySite;

		private object disposedEvent = new object();
	}
}
