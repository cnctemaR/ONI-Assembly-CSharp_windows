using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class ReturnMessage : IMethodReturnMessage, IMethodMessage, IMessage, IInternalMessage
	{
		public ReturnMessage(object ret, object[] outArgs, int outArgsCount, LogicalCallContext callCtx, IMethodCallMessage mcm)
		{
			this._returnValue = ret;
			this._args = outArgs;
			this._callCtx = callCtx;
			if (mcm != null)
			{
				this._uri = mcm.Uri;
				this._methodBase = mcm.MethodBase;
			}
			if (this._args == null)
			{
				this._args = new object[outArgsCount];
			}
		}

		public ReturnMessage(Exception e, IMethodCallMessage mcm)
		{
			this._exception = e;
			if (mcm != null)
			{
				this._methodBase = mcm.MethodBase;
				this._callCtx = mcm.LogicalCallContext;
			}
			this._args = new object[0];
		}

		public int ArgCount
		{
			[SecurityCritical]
			get
			{
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

		public bool HasVarArgs
		{
			[SecurityCritical]
			get
			{
				return !(this._methodBase == null) && (this._methodBase.CallingConvention | CallingConventions.VarArgs) > (CallingConventions)0;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				if (this._callCtx == null)
				{
					this._callCtx = new LogicalCallContext();
				}
				return this._callCtx;
			}
		}

		public MethodBase MethodBase
		{
			[SecurityCritical]
			get
			{
				return this._methodBase;
			}
		}

		public string MethodName
		{
			[SecurityCritical]
			get
			{
				if (this._methodBase != null && this._methodName == null)
				{
					this._methodName = this._methodBase.Name;
				}
				return this._methodName;
			}
		}

		public object MethodSignature
		{
			[SecurityCritical]
			get
			{
				if (this._methodBase != null && this._methodSignature == null)
				{
					ParameterInfo[] parameters = this._methodBase.GetParameters();
					this._methodSignature = new Type[parameters.Length];
					for (int i = 0; i < parameters.Length; i++)
					{
						this._methodSignature[i] = parameters[i].ParameterType;
					}
				}
				return this._methodSignature;
			}
		}

		public virtual IDictionary Properties
		{
			[SecurityCritical]
			get
			{
				if (this._properties == null)
				{
					this._properties = new MethodReturnDictionary(this);
				}
				return this._properties;
			}
		}

		public string TypeName
		{
			[SecurityCritical]
			get
			{
				if (this._methodBase != null && this._typeName == null)
				{
					this._typeName = this._methodBase.DeclaringType.AssemblyQualifiedName;
				}
				return this._typeName;
			}
		}

		public string Uri
		{
			[SecurityCritical]
			get
			{
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
			return this._args[argNum];
		}

		[SecurityCritical]
		public string GetArgName(int index)
		{
			return this._methodBase.GetParameters()[index].Name;
		}

		public Exception Exception
		{
			[SecurityCritical]
			get
			{
				return this._exception;
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

		public virtual object ReturnValue
		{
			[SecurityCritical]
			get
			{
				return this._returnValue;
			}
		}

		[SecurityCritical]
		public object GetOutArg(int argNum)
		{
			if (this._inArgInfo == null)
			{
				this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
			}
			return this._args[this._inArgInfo.GetInOutArgIndex(argNum)];
		}

		[SecurityCritical]
		public string GetOutArgName(int index)
		{
			if (this._inArgInfo == null)
			{
				this._inArgInfo = new ArgInfo(this.MethodBase, ArgInfoType.Out);
			}
			return this._inArgInfo.GetInOutArgName(index);
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
			return this._properties != null;
		}

		internal bool HasProperties()
		{
			return this._properties != null;
		}

		private object[] _outArgs;

		private object[] _args;

		private LogicalCallContext _callCtx;

		private object _returnValue;

		private string _uri;

		private Exception _exception;

		private MethodBase _methodBase;

		private string _methodName;

		private Type[] _methodSignature;

		private string _typeName;

		private MethodReturnDictionary _properties;

		private Identity _targetIdentity;

		private ArgInfo _inArgInfo;
	}
}
