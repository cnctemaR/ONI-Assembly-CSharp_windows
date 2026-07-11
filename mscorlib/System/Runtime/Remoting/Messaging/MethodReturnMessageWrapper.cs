using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class MethodReturnMessageWrapper : InternalMessageWrapper, IMethodReturnMessage, IMethodMessage, IMessage
	{
		public MethodReturnMessageWrapper(IMethodReturnMessage msg)
			: base(msg)
		{
			if (msg.Exception != null)
			{
				this._exception = msg.Exception;
				this._args = new object[0];
				return;
			}
			this._args = msg.Args;
			this._return = msg.ReturnValue;
			if (msg.MethodBase != null)
			{
				this._outArgInfo = new ArgInfo(msg.MethodBase, ArgInfoType.Out);
			}
		}

		public virtual int ArgCount
		{
			get
			{
				return this._args.Length;
			}
		}

		public virtual object[] Args
		{
			get
			{
				return this._args;
			}
			set
			{
				this._args = value;
			}
		}

		public virtual Exception Exception
		{
			get
			{
				return this._exception;
			}
			set
			{
				this._exception = value;
			}
		}

		public virtual bool HasVarArgs
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).HasVarArgs;
			}
		}

		public virtual LogicalCallContext LogicalCallContext
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).LogicalCallContext;
			}
		}

		public virtual MethodBase MethodBase
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodBase;
			}
		}

		public virtual string MethodName
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodName;
			}
		}

		public virtual object MethodSignature
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodSignature;
			}
		}

		public virtual int OutArgCount
		{
			get
			{
				if (this._outArgInfo == null)
				{
					return 0;
				}
				return this._outArgInfo.GetInOutArgCount();
			}
		}

		public virtual object[] OutArgs
		{
			get
			{
				if (this._outArgInfo == null)
				{
					return this._args;
				}
				return this._outArgInfo.GetInOutArgs(this._args);
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				if (this._properties == null)
				{
					this._properties = new MethodReturnMessageWrapper.DictionaryWrapper(this, this.WrappedMessage.Properties);
				}
				return this._properties;
			}
		}

		public virtual object ReturnValue
		{
			get
			{
				return this._return;
			}
			set
			{
				this._return = value;
			}
		}

		public virtual string TypeName
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).TypeName;
			}
		}

		public string Uri
		{
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).Uri;
			}
			set
			{
				this.Properties["__Uri"] = value;
			}
		}

		public virtual object GetArg(int argNum)
		{
			return this._args[argNum];
		}

		public virtual string GetArgName(int index)
		{
			return ((IMethodReturnMessage)this.WrappedMessage).GetArgName(index);
		}

		public virtual object GetOutArg(int argNum)
		{
			return this._args[this._outArgInfo.GetInOutArgIndex(argNum)];
		}

		public virtual string GetOutArgName(int index)
		{
			return this._outArgInfo.GetInOutArgName(index);
		}

		private object[] _args;

		private ArgInfo _outArgInfo;

		private MethodReturnMessageWrapper.DictionaryWrapper _properties;

		private Exception _exception;

		private object _return;

		private class DictionaryWrapper : MethodReturnDictionary
		{
			public DictionaryWrapper(IMethodReturnMessage message, IDictionary wrappedDictionary)
				: base(message)
			{
				this._wrappedDictionary = wrappedDictionary;
				base.MethodKeys = MethodReturnMessageWrapper.DictionaryWrapper._keys;
			}

			protected override IDictionary AllocInternalProperties()
			{
				return this._wrappedDictionary;
			}

			protected override void SetMethodProperty(string key, object value)
			{
				if (key == "__Args")
				{
					((MethodReturnMessageWrapper)this._message)._args = (object[])value;
					return;
				}
				if (key == "__Return")
				{
					((MethodReturnMessageWrapper)this._message)._return = value;
					return;
				}
				base.SetMethodProperty(key, value);
			}

			protected override object GetMethodProperty(string key)
			{
				if (key == "__Args")
				{
					return ((MethodReturnMessageWrapper)this._message)._args;
				}
				if (key == "__Return")
				{
					return ((MethodReturnMessageWrapper)this._message)._return;
				}
				return base.GetMethodProperty(key);
			}

			private IDictionary _wrappedDictionary;

			private static string[] _keys = new string[] { "__Args", "__Return" };
		}
	}
}
