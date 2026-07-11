using System;

namespace System.Runtime.Remoting.Messaging
{
	internal class MCMDictionary : MessageDictionary
	{
		public MCMDictionary(IMethodMessage message)
			: base(message)
		{
			base.MethodKeys = MCMDictionary.InternalKeys;
		}

		public static string[] InternalKeys = new string[] { "__Uri", "__MethodName", "__TypeName", "__MethodSignature", "__Args", "__CallContext" };
	}
}
