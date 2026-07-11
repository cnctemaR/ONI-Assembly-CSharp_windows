using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class MonoMethodMessage : IMethodCallMessage, IMethodMessage, IMessage, IMethodReturnMessage, IInternalMessage
	{
		internal void InitMessage(MonoMethod method, object[] out_args)
		{
			this.method = method;
			ParameterInfo[] parametersInternal = method.GetParametersInternal();
			int num = parametersInternal.Length;
			this.args = new object[num];
			this.arg_types = new byte[num];
			this.asyncResult = null;
			this.call_type = CallType.Sync;
			this.names = new string[num];
			for (int i = 0; i < num; i++)
			{
				this.names[i] = parametersInternal[i].Name;
			}
			bool flag = out_args != null;
			int num2 = 0;
			for (int j = 0; j < num; j++)
			{
				bool isOut = parametersInternal[j].IsOut;
				byte b;
				if (parametersInternal[j].ParameterType.IsByRef)
				{
					if (flag)
					{
						this.args[j] = out_args[num2++];
					}
					b = 2;
					if (!isOut)
					{
						b |= 1;
					}
				}
				else
				{
					b = 1;
					if (isOut)
					{
						b |= 4;
					}
				}
				this.arg_types[j] = b;
			}
		}

		public MonoMethodMessage(MethodBase method, object[] out_args)
		{
			if (method != null)
			{
				this.InitMessage((MonoMethod)method, out_args);
				return;
			}
			this.args = null;
		}

		internal MonoMethodMessage(MethodInfo minfo, object[] in_args, object[] out_args)
		{
			this.InitMessage((MonoMethod)minfo, out_args);
			int num = in_args.Length;
			for (int i = 0; i < num; i++)
			{
				this.args[i] = in_args[i];
			}
		}

		private static MethodInfo GetMethodInfo(Type type, string methodName)
		{
			MethodInfo methodInfo = type.GetMethod(methodName);
			if (methodInfo == null)
			{
				throw new ArgumentException(string.Format("Could not find '{0}' in {1}", methodName, type), "methodName");
			}
			return methodInfo;
		}

		public MonoMethodMessage(Type type, string methodName, object[] in_args)
			: this(MonoMethodMessage.GetMethodInfo(type, methodName), in_args, null)
		{
		}

		public IDictionary Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new MCMDictionary(this);
				}
				return this.properties;
			}
		}

		public int ArgCount
		{
			get
			{
				if (this.CallType == CallType.EndInvoke)
				{
					return -1;
				}
				if (this.args == null)
				{
					return 0;
				}
				return this.args.Length;
			}
		}

		public object[] Args
		{
			get
			{
				return this.args;
			}
		}

		public bool HasVarArgs
		{
			get
			{
				return false;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			get
			{
				return this.ctx;
			}
			set
			{
				this.ctx = value;
			}
		}

		public MethodBase MethodBase
		{
			get
			{
				return this.method;
			}
		}

		public string MethodName
		{
			get
			{
				if (null == this.method)
				{
					return string.Empty;
				}
				return this.method.Name;
			}
		}

		public object MethodSignature
		{
			get
			{
				if (this.methodSignature == null)
				{
					ParameterInfo[] parameters = this.method.GetParameters();
					this.methodSignature = new Type[parameters.Length];
					for (int i = 0; i < parameters.Length; i++)
					{
						this.methodSignature[i] = parameters[i].ParameterType;
					}
				}
				return this.methodSignature;
			}
		}

		public string TypeName
		{
			get
			{
				if (null == this.method)
				{
					return string.Empty;
				}
				return this.method.DeclaringType.AssemblyQualifiedName;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public object GetArg(int arg_num)
		{
			if (this.args == null)
			{
				return null;
			}
			return this.args[arg_num];
		}

		public string GetArgName(int arg_num)
		{
			if (this.args == null)
			{
				return string.Empty;
			}
			return this.names[arg_num];
		}

		public int InArgCount
		{
			get
			{
				if (this.CallType == CallType.EndInvoke)
				{
					return -1;
				}
				if (this.args == null)
				{
					return 0;
				}
				int num = 0;
				byte[] array = this.arg_types;
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i] & 1) != 0)
					{
						num++;
					}
				}
				return num;
			}
		}

		public object[] InArgs
		{
			get
			{
				object[] array = new object[this.InArgCount];
				int num2;
				int num = (num2 = 0);
				byte[] array2 = this.arg_types;
				for (int i = 0; i < array2.Length; i++)
				{
					if ((array2[i] & 1) != 0)
					{
						array[num++] = this.args[num2];
					}
					num2++;
				}
				return array;
			}
		}

		public object GetInArg(int arg_num)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = this.arg_types;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] & 1) != 0 && num2++ == arg_num)
				{
					return this.args[num];
				}
				num++;
			}
			return null;
		}

		public string GetInArgName(int arg_num)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = this.arg_types;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] & 1) != 0 && num2++ == arg_num)
				{
					return this.names[num];
				}
				num++;
			}
			return null;
		}

		public Exception Exception
		{
			get
			{
				return this.exc;
			}
		}

		public int OutArgCount
		{
			get
			{
				if (this.args == null)
				{
					return 0;
				}
				int num = 0;
				byte[] array = this.arg_types;
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i] & 2) != 0)
					{
						num++;
					}
				}
				return num;
			}
		}

		public object[] OutArgs
		{
			get
			{
				if (this.args == null)
				{
					return null;
				}
				object[] array = new object[this.OutArgCount];
				int num2;
				int num = (num2 = 0);
				byte[] array2 = this.arg_types;
				for (int i = 0; i < array2.Length; i++)
				{
					if ((array2[i] & 2) != 0)
					{
						array[num++] = this.args[num2];
					}
					num2++;
				}
				return array;
			}
		}

		public object ReturnValue
		{
			get
			{
				return this.rval;
			}
		}

		public object GetOutArg(int arg_num)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = this.arg_types;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] & 2) != 0 && num2++ == arg_num)
				{
					return this.args[num];
				}
				num++;
			}
			return null;
		}

		public string GetOutArgName(int arg_num)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = this.arg_types;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] & 2) != 0 && num2++ == arg_num)
				{
					return this.names[num];
				}
				num++;
			}
			return null;
		}

		Identity IInternalMessage.TargetIdentity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		bool IInternalMessage.HasProperties()
		{
			return this.properties != null;
		}

		public bool IsAsync
		{
			get
			{
				return this.asyncResult != null;
			}
		}

		public AsyncResult AsyncResult
		{
			get
			{
				return this.asyncResult;
			}
		}

		internal CallType CallType
		{
			get
			{
				if (this.call_type == CallType.Sync && RemotingServices.IsOneWay(this.method))
				{
					this.call_type = CallType.OneWay;
				}
				return this.call_type;
			}
		}

		public bool NeedsOutProcessing(out int outCount)
		{
			bool flag = false;
			outCount = 0;
			foreach (byte b in this.arg_types)
			{
				if ((b & 2) != 0)
				{
					outCount++;
				}
				else if ((b & 4) != 0)
				{
					flag = true;
				}
			}
			return outCount > 0 || flag;
		}

		private MonoMethod method;

		private object[] args;

		private string[] names;

		private byte[] arg_types;

		public LogicalCallContext ctx;

		public object rval;

		public Exception exc;

		private AsyncResult asyncResult;

		private CallType call_type;

		private string uri;

		private MCMDictionary properties;

		private Type[] methodSignature;

		private Identity identity;

		internal static string CallContextKey = "__CallContext";

		internal static string UriKey = "__Uri";
	}
}
