using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class ObsoleteAttribute : Attribute
	{
		public ObsoleteAttribute()
		{
			this._message = null;
			this._error = false;
		}

		public ObsoleteAttribute(string message)
		{
			this._message = message;
			this._error = false;
		}

		public ObsoleteAttribute(string message, bool error)
		{
			this._message = message;
			this._error = error;
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public bool IsError
		{
			get
			{
				return this._error;
			}
		}

		private string _message;

		private bool _error;
	}
}
