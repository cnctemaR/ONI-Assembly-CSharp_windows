using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;

namespace System.Runtime.Remoting.Messaging
{
	internal class CADMethodCallMessage : CADMessageBase
	{
		internal string Uri
		{
			get
			{
				return this._uri;
			}
		}

		internal static CADMethodCallMessage Create(IMessage callMsg)
		{
			IMethodCallMessage methodCallMessage = callMsg as IMethodCallMessage;
			if (methodCallMessage == null)
			{
				return null;
			}
			return new CADMethodCallMessage(methodCallMessage);
		}

		internal CADMethodCallMessage(IMethodCallMessage callMsg)
			: base(callMsg)
		{
			this._uri = callMsg.Uri;
			ArrayList arrayList = null;
			this._propertyCount = CADMessageBase.MarshalProperties(callMsg.Properties, ref arrayList);
			this._args = base.MarshalArguments(callMsg.Args, ref arrayList);
			base.SaveLogicalCallContext(callMsg, ref arrayList);
			if (arrayList != null)
			{
				MemoryStream memoryStream = CADSerializer.SerializeObject(arrayList.ToArray());
				this._serializedArgs = memoryStream.GetBuffer();
			}
		}

		internal ArrayList GetArguments()
		{
			ArrayList arrayList = null;
			if (this._serializedArgs != null)
			{
				arrayList = new ArrayList((object[])CADSerializer.DeserializeObject(new MemoryStream(this._serializedArgs)));
				this._serializedArgs = null;
			}
			return arrayList;
		}

		internal object[] GetArgs(ArrayList args)
		{
			return base.UnmarshalArguments(this._args, args);
		}

		internal int PropertiesCount
		{
			get
			{
				return this._propertyCount;
			}
		}

		private string _uri;
	}
}
