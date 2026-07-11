using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public class InternalRemotingServices
	{
		[Conditional("_LOGGING")]
		public static void DebugOutChnl(string s)
		{
			throw new NotSupportedException();
		}

		public static SoapAttribute GetCachedSoapAttribute(object reflectionObject)
		{
			object syncRoot = InternalRemotingServices._soapAttributes.SyncRoot;
			SoapAttribute soapAttribute2;
			lock (syncRoot)
			{
				SoapAttribute soapAttribute = InternalRemotingServices._soapAttributes[reflectionObject] as SoapAttribute;
				if (soapAttribute != null)
				{
					soapAttribute2 = soapAttribute;
				}
				else
				{
					object[] customAttributes = ((ICustomAttributeProvider)reflectionObject).GetCustomAttributes(typeof(SoapAttribute), true);
					if (customAttributes.Length != 0)
					{
						soapAttribute = (SoapAttribute)customAttributes[0];
					}
					else if (reflectionObject is Type)
					{
						soapAttribute = new SoapTypeAttribute();
					}
					else if (reflectionObject is FieldInfo)
					{
						soapAttribute = new SoapFieldAttribute();
					}
					else if (reflectionObject is MethodBase)
					{
						soapAttribute = new SoapMethodAttribute();
					}
					else if (reflectionObject is ParameterInfo)
					{
						soapAttribute = new SoapParameterAttribute();
					}
					soapAttribute.SetReflectionObject(reflectionObject);
					InternalRemotingServices._soapAttributes[reflectionObject] = soapAttribute;
					soapAttribute2 = soapAttribute;
				}
			}
			return soapAttribute2;
		}

		[Conditional("_DEBUG")]
		public static void RemotingAssert(bool condition, string message)
		{
			throw new NotSupportedException();
		}

		[Conditional("_LOGGING")]
		public static void RemotingTrace(params object[] messages)
		{
			throw new NotSupportedException();
		}

		[CLSCompliant(false)]
		public static void SetServerIdentity(MethodCall m, object srvID)
		{
			Identity identity = srvID as Identity;
			if (identity == null)
			{
				throw new ArgumentException("srvID");
			}
			RemotingServices.SetMessageTargetIdentity(m, identity);
		}

		private static Hashtable _soapAttributes = new Hashtable();
	}
}
