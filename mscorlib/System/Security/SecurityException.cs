using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public class SecurityException : SystemException
	{
		[ComVisible(false)]
		public SecurityAction Action
		{
			get
			{
				return this._action;
			}
			set
			{
				this._action = value;
			}
		}

		[ComVisible(false)]
		public object DenySetInstance
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._denyset;
			}
			set
			{
				this._denyset = value;
			}
		}

		[ComVisible(false)]
		public AssemblyName FailedAssemblyInfo
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._assembly;
			}
			set
			{
				this._assembly = value;
			}
		}

		[ComVisible(false)]
		public MethodInfo Method
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._method;
			}
			set
			{
				this._method = value;
			}
		}

		[ComVisible(false)]
		public object PermitOnlySetInstance
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._permitset;
			}
			set
			{
				this._permitset = value;
			}
		}

		public string Url
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._url;
			}
			set
			{
				this._url = value;
			}
		}

		public SecurityZone Zone
		{
			get
			{
				return this._zone;
			}
			set
			{
				this._zone = value;
			}
		}

		[ComVisible(false)]
		public object Demanded
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._demanded;
			}
			set
			{
				this._demanded = value;
			}
		}

		public IPermission FirstPermissionThatFailed
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._firstperm;
			}
			set
			{
				this._firstperm = value;
			}
		}

		public string PermissionState
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this.permissionState;
			}
			set
			{
				this.permissionState = value;
			}
		}

		public Type PermissionType
		{
			get
			{
				return this.permissionType;
			}
			set
			{
				this.permissionType = value;
			}
		}

		public string GrantedSet
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._granted;
			}
			set
			{
				this._granted = value;
			}
		}

		public string RefusedSet
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true, ControlPolicy = true)]
			get
			{
				return this._refused;
			}
			set
			{
				this._refused = value;
			}
		}

		public SecurityException()
			: this(Locale.GetText("A security error has been detected."))
		{
		}

		public SecurityException(string message)
			: base(message)
		{
			base.HResult = -2146233078;
		}

		protected SecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			base.HResult = -2146233078;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Name == "PermissionState")
				{
					this.permissionState = (string)enumerator.Value;
					return;
				}
			}
		}

		public SecurityException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233078;
		}

		public SecurityException(string message, Type type)
			: base(message)
		{
			base.HResult = -2146233078;
			this.permissionType = type;
		}

		public SecurityException(string message, Type type, string state)
			: base(message)
		{
			base.HResult = -2146233078;
			this.permissionType = type;
			this.permissionState = state;
		}

		internal SecurityException(string message, PermissionSet granted, PermissionSet refused)
			: base(message)
		{
			base.HResult = -2146233078;
			this._granted = granted.ToString();
			this._refused = refused.ToString();
		}

		public SecurityException(string message, object deny, object permitOnly, MethodInfo method, object demanded, IPermission permThatFailed)
			: base(message)
		{
			base.HResult = -2146233078;
			this._denyset = deny;
			this._permitset = permitOnly;
			this._method = method;
			this._demanded = demanded;
			this._firstperm = permThatFailed;
		}

		public SecurityException(string message, AssemblyName assemblyName, PermissionSet grant, PermissionSet refused, MethodInfo method, SecurityAction action, object demanded, IPermission permThatFailed, Evidence evidence)
			: base(message)
		{
			base.HResult = -2146233078;
			this._assembly = assemblyName;
			this._granted = ((grant == null) ? string.Empty : grant.ToString());
			this._refused = ((refused == null) ? string.Empty : refused.ToString());
			this._method = method;
			this._action = action;
			this._demanded = demanded;
			this._firstperm = permThatFailed;
			if (this._firstperm != null)
			{
				this.permissionType = this._firstperm.GetType();
			}
			this._evidence = evidence;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			try
			{
				info.AddValue("PermissionState", this.permissionState);
			}
			catch (SecurityException)
			{
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			try
			{
				if (this.permissionType != null)
				{
					stringBuilder.AppendFormat("{0}Type: {1}", Environment.NewLine, this.PermissionType);
				}
				if (this._method != null)
				{
					string text = this._method.ToString();
					int num = text.IndexOf(" ") + 1;
					stringBuilder.AppendFormat("{0}Method: {1} {2}.{3}", new object[]
					{
						Environment.NewLine,
						this._method.ReturnType.Name,
						this._method.ReflectedType,
						text.Substring(num)
					});
				}
				if (this.permissionState != null)
				{
					stringBuilder.AppendFormat("{0}State: {1}", Environment.NewLine, this.PermissionState);
				}
				if (this._granted != null && this._granted.Length > 0)
				{
					stringBuilder.AppendFormat("{0}Granted: {1}", Environment.NewLine, this.GrantedSet);
				}
				if (this._refused != null && this._refused.Length > 0)
				{
					stringBuilder.AppendFormat("{0}Refused: {1}", Environment.NewLine, this.RefusedSet);
				}
				if (this._demanded != null)
				{
					stringBuilder.AppendFormat("{0}Demanded: {1}", Environment.NewLine, this.Demanded);
				}
				if (this._firstperm != null)
				{
					stringBuilder.AppendFormat("{0}Failed Permission: {1}", Environment.NewLine, this.FirstPermissionThatFailed);
				}
				if (this._evidence != null)
				{
					stringBuilder.AppendFormat("{0}Evidences:", Environment.NewLine);
					foreach (object obj in this._evidence)
					{
						if (!(obj is Hash))
						{
							stringBuilder.AppendFormat("{0}\t{1}", Environment.NewLine, obj);
						}
					}
				}
			}
			catch (SecurityException)
			{
			}
			return stringBuilder.ToString();
		}

		private string permissionState;

		private Type permissionType;

		private string _granted;

		private string _refused;

		private object _demanded;

		private IPermission _firstperm;

		private MethodInfo _method;

		private Evidence _evidence;

		private SecurityAction _action;

		private object _denyset;

		private object _permitset;

		private AssemblyName _assembly;

		private string _url;

		private SecurityZone _zone;
	}
}
