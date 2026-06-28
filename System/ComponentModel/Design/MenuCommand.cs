using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class MenuCommand
	{
		public MenuCommand(EventHandler handler, CommandID command)
		{
			this.handler = handler;
			this.command = command;
		}

		public event EventHandler CommandChanged;

		public virtual bool Checked
		{
			get
			{
				return this.ischecked;
			}
			set
			{
				if (this.ischecked != value)
				{
					this.ischecked = value;
					this.OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		public virtual CommandID CommandID
		{
			get
			{
				return this.command;
			}
		}

		public virtual bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.enabled != value)
				{
					this.enabled = value;
					this.OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		[global::System.MonoTODO]
		public virtual int OleStatus
		{
			get
			{
				return 3;
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new Hashtable();
				}
				return this.properties;
			}
		}

		public virtual bool Supported
		{
			get
			{
				return this.issupported;
			}
			set
			{
				this.issupported = value;
			}
		}

		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
			}
		}

		public virtual void Invoke()
		{
			if (this.handler != null)
			{
				this.handler(this, EventArgs.Empty);
			}
		}

		public virtual void Invoke(object arg)
		{
			this.Invoke();
		}

		protected virtual void OnCommandChanged(EventArgs e)
		{
			if (this.CommandChanged != null)
			{
				this.CommandChanged(this, e);
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (this.command != null)
			{
				text = this.command.ToString();
			}
			text += " : ";
			if (this.Supported)
			{
				text += "Supported";
			}
			if (this.Enabled)
			{
				text += "|Enabled";
			}
			if (this.Visible)
			{
				text += "|Visible";
			}
			if (this.Checked)
			{
				text += "|Checked";
			}
			return text;
		}

		private EventHandler handler;

		private CommandID command;

		private bool ischecked;

		private bool enabled = true;

		private bool issupported = true;

		private bool visible = true;

		private Hashtable properties;
	}
}
