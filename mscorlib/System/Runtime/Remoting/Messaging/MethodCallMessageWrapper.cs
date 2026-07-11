using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class MethodCallMessageWrapper : InternalMessageWrapper, IMethodCallMessage, IMethodMessage, IMessage
	{
		public MethodCallMessageWrapper(IMethodCallMessage msg)
			: base(msg)
		{
			this._args = ((IMethodCallMessage)this.WrappedMessage).Args;
			this._inArgInfo = new ArgInfo(msg.MethodBase, ArgInfoType.In);
		}

		public virtual int ArgCount
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).ArgCount;
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

		public virtual bool HasVarArgs
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).HasVarArgs;
			}
		}

		public virtual int InArgCount
		{
			get
			{
				return this._inArgInfo.GetInOutArgCount();
			}
		}

		public virtual object[] InArgs
		{
			get
			{
				return this._inArgInfo.GetInOutArgs(this._args);
			}
		}

		public virtual LogicalCallContext LogicalCallContext
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).LogicalCallContext;
			}
		}

		public virtual MethodBase MethodBase
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).MethodBase;
			}
		}

		public virtual string MethodName
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).MethodName;
			}
		}

		public virtual object MethodSignature
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).MethodSignature;
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				if (this._properties == null)
				{
					this._properties = new MethodCallMessageWrapper.DictionaryWrapper(this, this.WrappedMessage.Properties);
				}
				return this._properties;
			}
		}

		public virtual string TypeName
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).TypeName;
			}
		}

		public virtual string Uri
		{
			get
			{
				return ((IMethodCallMessage)this.WrappedMessage).Uri;
			}
			set
			{
				IInternalMessage internalMessage = this.WrappedMessage as IInternalMessage;
				if (internalMessage != null)
				{
					internalMessage.Uri = value;
					return;
				}
				this.Properties["__Uri"] = value;
			}
		}

		public virtual object GetArg(int argNum)
		{
			return this._args[argNum];
		}

		public virtual string GetArgName(int index)
		{
			return ((IMethodCallMessage)this.WrappedMessage).GetArgName(index);
		}

		public virtual object GetInArg(int argNum)
		{
			return this._args[this._inArgInfo.GetInOutArgIndex(argNum)];
		}

		public virtual string GetInArgName(int index)
		{
			return this._inArgInfo.GetInOutArgName(index);
		}

		private object[] _args;

		private ArgInfo _inArgInfo;

		private MethodCallMessageWrapper.DictionaryWrapper _properties;

		private class DictionaryWrapper : MCMDictionary
		{
			public DictionaryWrapper(IMethodMessage message, IDictionary wrappedDictionary)
				: base(message)
			{
				this._wrappedDictionary = wrappedDictionary;
				base.MethodKeys = MethodCallMessageWrapper.DictionaryWrapper._keys;
			}

			protected override IDictionary AllocInternalProperties()
			{
				return this._wrappedDictionary;
			}

			protected override void SetMethodProperty(string key, object value)
			{
				if (key == "__Args")
				{
					((MethodCallMessageWrapper)this._message)._args = (object[])value;
					return;
				}
				base.SetMethodProperty(key, value);
			}

			protected override object GetMethodProperty(string key)
			{
				if (key == "__Args")
				{
					return ((MethodCallMessageWrapper)this._message)._args;
				}
				return base.GetMethodProperty(key);
			}

			private IDictionary _wrappedDictionary;

			private static string[] _keys = new string[] { "__Args" };
		}
	}
}
