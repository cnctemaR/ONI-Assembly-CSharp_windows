using System;

namespace System.Runtime.Remoting.Messaging
{
	internal class MethodReturnDictionary : MessageDictionary
	{
		public MethodReturnDictionary(IMethodReturnMessage message)
			: base(message)
		{
			if (message.Exception == null)
			{
				base.MethodKeys = MethodReturnDictionary.InternalReturnKeys;
				return;
			}
			base.MethodKeys = MethodReturnDictionary.InternalExceptionKeys;
		}

		public static string[] InternalReturnKeys = new string[] { "__Uri", "__MethodName", "__TypeName", "__MethodSignature", "__OutArgs", "__Return", "__CallContext" };

		public static string[] InternalExceptionKeys = new string[] { "__CallContext" };
	}
}
