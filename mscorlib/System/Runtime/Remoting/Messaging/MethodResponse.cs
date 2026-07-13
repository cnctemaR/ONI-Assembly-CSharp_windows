using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[Serializable]
	public class MethodResponse : IMethodReturnMessage, IMethodMessage, IMessage, ISerializable, IInternalMessage, ISerializationRootObject
	{
		public MethodResponse(Header[] h1, IMethodCallMessage mcm)
		{
			if (mcm != null)
			{
				this._methodName = mcm.MethodName;
				this._uri = mcm.Uri;
				this._typeName = mcm.TypeName;
				this._methodBase = mcm.MethodBase;
				this._methodSignature = (Type[])mcm.MethodSignature;
				this._args = mcm.Args;
			}
			if (h1 != null)
			{
				foreach (Header header in h1)
				{
					this.InitMethodProperty(header.Name, header.Value);
				}
			}
		}

		internal MethodResponse(Exception e, IMethodCallMessage msg)
		{
			this._callMsg = msg;
			if (msg != null)
			{
				this._uri = msg.Uri;
			}
			else
			{
				this._uri = string.Empty;
			}
			this._exception = e;
			this._returnValue = null;
			this._outArgs = new object[0];
		}

		internal MethodResponse(object returnValue, object[] outArgs, LogicalCallContext callCtx, IMethodCallMessage msg)
		{
			this._callMsg = msg;
			this._uri = msg.Uri;
			this._exception = null;
			this._returnValue = returnValue;
			this._args = outArgs;
		}

		internal MethodResponse(IMethodCallMessage msg, CADMethodReturnMessage retmsg)
		{
			this._callMsg = msg;
			this._methodBase = msg.MethodBase;
			this._uri = msg.Uri;
			this._methodName = msg.MethodName;
			ArrayList arguments = retmsg.GetArguments();
			this._exception = retmsg.GetException(arguments);
			this._returnValue = retmsg.GetReturnValue(arguments);
			this._args = retmsg.GetArgs(arguments);
			this._callContext = retmsg.GetLogicalCallContext(arguments);
			if (this._callContext == null)
			{
				this._callContext = new LogicalCallContext();
			}
			if (retmsg.PropertiesCount > 0)
			{
				CADMessageBase.UnmarshalProperties(this.Properties, retmsg.PropertiesCount, arguments);
			}
		}

		internal MethodResponse(IMethodCallMessage msg, object handlerObject, BinaryMethodReturnMessage smuggledMrm)
		{
			if (msg != null)
			{
				this._methodBase = msg.MethodBase;
				this._methodName = msg.MethodName;
				this._uri = msg.Uri;
			}
			this._returnValue = smuggledMrm.ReturnValue;
			this._args = smuggledMrm.Args;
			this._exception = smuggledMrm.Exception;
			this._callContext = smuggledMrm.LogicalCallContext;
			if (smuggledMrm.HasProperties)
			{
				smuggledMrm.PopulateMessageProperties(this.Properties);
			}
		}

		internal MethodResponse(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				this.InitMethodProperty(serializationEntry.Name, serializationEntry.Value);
			}
		}

		internal void InitMethodProperty(string key, object value)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1960967436U)
			{
				if (num <= 1201911322U)
				{
					if (num != 990701179U)
					{
						if (num == 1201911322U)
						{
							if (key == "__CallContext")
							{
								this._callContext = (LogicalCallContext)value;
								return;
							}
						}
					}
					else if (key == "__Uri")
					{
						this._uri = (string)value;
						return;
					}
				}
				else if (num != 1637783905U)
				{
					if (num == 1960967436U)
					{
						if (key == "__OutArgs")
						{
							this._args = (object[])value;
							return;
						}
					}
				}
				else if (key == "__Return")
				{
					this._returnValue = value;
					return;
				}
			}
			else if (num <= 3166241401U)
			{
				if (num != 2010141056U)
				{
					if (num == 3166241401U)
					{
						if (key == "__MethodName")
						{
							this._methodName = (string)value;
							return;
						}
					}
				}
				else if (key == "__TypeName")
				{
					this._typeName = (string)value;
					return;
				}
			}
			else if (num != 3626951189U)
			{
				if (num == 3679129400U)
				{
					if (key == "__MethodSignature")
					{
						this._methodSignature = (Type[])value;
						return;
					}
				}
			}
			else if (key == "__fault")
			{
				this._exception = (Exception)value;
				return;
			}
			this.Properties[key] = value;
		}

		public int ArgCount
		{
			[SecurityCritical]
			get
			{
				if (this._args == null)
				{
					return 0;
				}
				return this._args.Length;
			}
		}

		public object[] Args
		{
			[SecurityCritical]
			get
			{
				return this._args;
			}
		}

		public Exception Exception
		{
			[SecurityCritical]
			get
			{
				return this._exception;
			}
		}

		public bool HasVarArgs
		{
			[SecurityCritical]
			get
			{
				return (this.MethodBase.CallingConvention | CallingConventions.VarArgs) > (CallingConventions)0;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				if (this._callContext == null)
				{
					this._callContext = new LogicalCallContext();
				}
				return this._callContext;
			}
		}

		public MethodBase MethodBase
		{
			[SecurityCritical]
			get
			{
				if (null == this._methodBase)
				{
					if (this._callMsg != null)
					{
						this._methodBase = this._callMsg.MethodBase;
					}
					else if (this.MethodName != null && this.TypeName != null)
					{
						this._methodBase = RemotingServices.GetMethodBaseFromMethodMessage(this);
					}
				}
				return this._methodBase;
			}
		}

		public string MethodName
		{
			[SecurityCritical]
			get
			{
				if (this._methodName == null && this._callMsg != null)
				{
					this._methodName = this._callMsg.MethodName;
				}
				return this._methodName;
			}
		}

		public object MethodSignature
		{
			[SecurityCritical]
			get
			{
				if (this._methodSignature == null && this._callMsg != null)
				{
					this._methodSignature = (Type[])this._callMsg.MethodSignature;
				}
				return this._methodSignature;
			}
		}

		public int OutArgCount
		{
			[SecurityCritical]
			get
			{
				if (this._args == null || this._args.Length == 0)
				{
					return 0;
				}
				if (this._inArgInfo == null)
				{
					this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
				}
				return this._inArgInfo.GetInOutArgCount();
			}
		}

		public object[] OutArgs
		{
			[SecurityCritical]
			get
			{
				if (this._outArgs == null && this._args != null)
				{
					if (this._inArgInfo == null)
					{
						this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
					}
					this._outArgs = this._inArgInfo.GetInOutArgs(this._args);
				}
				return this._outArgs;
			}
		}

		public virtual IDictionary Properties
		{
			[SecurityCritical]
			get
			{
				if (this.ExternalProperties == null)
				{
					MethodReturnDictionary methodReturnDictionary = new MethodReturnDictionary(this);
					this.ExternalProperties = methodReturnDictionary;
					this.InternalProperties = methodReturnDictionary.GetInternalProperties();
				}
				return this.ExternalProperties;
			}
		}

		public object ReturnValue
		{
			[SecurityCritical]
			get
			{
				return this._returnValue;
			}
		}

		public string TypeName
		{
			[SecurityCritical]
			get
			{
				if (this._typeName == null && this._callMsg != null)
				{
					this._typeName = this._callMsg.TypeName;
				}
				return this._typeName;
			}
		}

		public string Uri
		{
			[SecurityCritical]
			get
			{
				if (this._uri == null && this._callMsg != null)
				{
					this._uri = this._callMsg.Uri;
				}
				return this._uri;
			}
			set
			{
				this._uri = value;
			}
		}

		string IInternalMessage.Uri
		{
			get
			{
				return this.Uri;
			}
			set
			{
				this.Uri = value;
			}
		}

		[SecurityCritical]
		public object GetArg(int argNum)
		{
			if (this._args == null)
			{
				return null;
			}
			return this._args[argNum];
		}

		[SecurityCritical]
		public string GetArgName(int index)
		{
			return this.MethodBase.GetParameters()[index].Name;
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (this._exception == null)
			{
				info.AddValue("__TypeName", this._typeName);
				info.AddValue("__MethodName", this._methodName);
				info.AddValue("__MethodSignature", this._methodSignature);
				info.AddValue("__Uri", this._uri);
				info.AddValue("__Return", this._returnValue);
				info.AddValue("__OutArgs", this._args);
			}
			else
			{
				info.AddValue("__fault", this._exception);
			}
			info.AddValue("__CallContext", this._callContext);
			if (this.InternalProperties != null)
			{
				foreach (object obj in this.InternalProperties)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					info.AddValue((string)dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
		}

		[SecurityCritical]
		public object GetOutArg(int argNum)
		{
			if (this._args == null)
			{
				return null;
			}
			if (this._inArgInfo == null)
			{
				this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
			}
			return this._args[this._inArgInfo.GetInOutArgIndex(argNum)];
		}

		[SecurityCritical]
		public string GetOutArgName(int index)
		{
			if (null == this._methodBase)
			{
				return "__method_" + index.ToString();
			}
			if (this._inArgInfo == null)
			{
				this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
			}
			return this._inArgInfo.GetInOutArgName(index);
		}

		[MonoTODO]
		public virtual object HeaderHandler(Header[] h)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RootSetObjectData(SerializationInfo info, StreamingContext ctx)
		{
			throw new NotImplementedException();
		}

		Identity IInternalMessage.TargetIdentity
		{
			get
			{
				return this._targetIdentity;
			}
			set
			{
				this._targetIdentity = value;
			}
		}

		bool IInternalMessage.HasProperties()
		{
			return this.ExternalProperties != null || this.InternalProperties != null;
		}

		private string _methodName;

		private string _uri;

		private string _typeName;

		private MethodBase _methodBase;

		private object _returnValue;

		private Exception _exception;

		private Type[] _methodSignature;

		private ArgInfo _inArgInfo;

		private object[] _args;

		private object[] _outArgs;

		private IMethodCallMessage _callMsg;

		private LogicalCallContext _callContext;

		private Identity _targetIdentity;

		protected IDictionary ExternalProperties;

		protected IDictionary InternalProperties;
	}
}
