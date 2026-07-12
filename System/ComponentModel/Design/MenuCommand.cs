using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.ComponentModel.Design
{
	public class MenuCommand
	{
		public MenuCommand(EventHandler handler, CommandID command)
		{
			this._execHandler = handler;
			this.CommandID = command;
			this._status = 3;
		}

		public virtual bool Checked
		{
			get
			{
				return (this._status & 4) != 0;
			}
			set
			{
				this.SetStatus(4, value);
			}
		}

		public virtual bool Enabled
		{
			get
			{
				return (this._status & 2) != 0;
			}
			set
			{
				this.SetStatus(2, value);
			}
		}

		private void SetStatus(int mask, bool value)
		{
			int num = this._status;
			if (value)
			{
				num |= mask;
			}
			else
			{
				num &= ~mask;
			}
			if (num != this._status)
			{
				this._status = num;
				this.OnCommandChanged(EventArgs.Empty);
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				IDictionary dictionary;
				if ((dictionary = this._properties) == null)
				{
					dictionary = (this._properties = new HybridDictionary());
				}
				return dictionary;
			}
		}

		public virtual bool Supported
		{
			get
			{
				return (this._status & 1) != 0;
			}
			set
			{
				this.SetStatus(1, value);
			}
		}

		public virtual bool Visible
		{
			get
			{
				return (this._status & 16) == 0;
			}
			set
			{
				this.SetStatus(16, !value);
			}
		}

		public event EventHandler CommandChanged;

		public virtual CommandID CommandID { get; }

		public virtual void Invoke()
		{
			if (this._execHandler != null)
			{
				try
				{
					this._execHandler(this, EventArgs.Empty);
				}
				catch (CheckoutException ex)
				{
					if (ex != CheckoutException.Canceled)
					{
						throw;
					}
				}
			}
		}

		public virtual void Invoke(object arg)
		{
			this.Invoke();
		}

		public virtual int OleStatus
		{
			get
			{
				return this._status;
			}
		}

		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHandler commandChanged = this.CommandChanged;
			if (commandChanged == null)
			{
				return;
			}
			commandChanged(this, e);
		}

		public override string ToString()
		{
			string text = this.CommandID.ToString() + " : ";
			if ((this._status & 1) != 0)
			{
				text += "Supported";
			}
			if ((this._status & 2) != 0)
			{
				text += "|Enabled";
			}
			if ((this._status & 16) == 0)
			{
				text += "|Visible";
			}
			if ((this._status & 4) != 0)
			{
				text += "|Checked";
			}
			return text;
		}

		private EventHandler _execHandler;

		private int _status;

		private IDictionary _properties;

		private const int ENABLED = 2;

		private const int INVISIBLE = 16;

		private const int CHECKED = 4;

		private const int SUPPORTED = 1;
	}
}
