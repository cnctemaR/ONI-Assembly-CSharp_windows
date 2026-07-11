using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	[MonoTODO("Handle domain unloading?")]
	internal class CrossAppDomainSink : IMessageSink
	{
		internal CrossAppDomainSink(int domainID)
		{
			this._domainID = domainID;
		}

		internal static CrossAppDomainSink GetSink(int domainID)
		{
			object syncRoot = CrossAppDomainSink.s_sinks.SyncRoot;
			CrossAppDomainSink crossAppDomainSink;
			lock (syncRoot)
			{
				if (CrossAppDomainSink.s_sinks.ContainsKey(domainID))
				{
					crossAppDomainSink = (CrossAppDomainSink)CrossAppDomainSink.s_sinks[domainID];
				}
				else
				{
					CrossAppDomainSink crossAppDomainSink2 = new CrossAppDomainSink(domainID);
					CrossAppDomainSink.s_sinks[domainID] = crossAppDomainSink2;
					crossAppDomainSink = crossAppDomainSink2;
				}
			}
			return crossAppDomainSink;
		}

		internal int TargetDomainId
		{
			get
			{
				return this._domainID;
			}
		}

		private static CrossAppDomainSink.ProcessMessageRes ProcessMessageInDomain(byte[] arrRequest, CADMethodCallMessage cadMsg)
		{
			CrossAppDomainSink.ProcessMessageRes processMessageRes = default(CrossAppDomainSink.ProcessMessageRes);
			try
			{
				AppDomain.CurrentDomain.ProcessMessageInDomain(arrRequest, cadMsg, out processMessageRes.arrResponse, out processMessageRes.cadMrm);
			}
			catch (Exception ex)
			{
				IMessage message = new MethodResponse(ex, new ErrorMessage());
				processMessageRes.arrResponse = CADSerializer.SerializeMessage(message).GetBuffer();
			}
			return processMessageRes;
		}

		public virtual IMessage SyncProcessMessage(IMessage msgRequest)
		{
			IMessage message = null;
			try
			{
				byte[] array = null;
				byte[] array2 = null;
				CADMethodReturnMessage cadmethodReturnMessage = null;
				CADMethodCallMessage cadmethodCallMessage = CADMethodCallMessage.Create(msgRequest);
				if (cadmethodCallMessage == null)
				{
					MemoryStream memoryStream = CADSerializer.SerializeMessage(msgRequest);
					array2 = memoryStream.GetBuffer();
				}
				Context currentContext = Thread.CurrentContext;
				try
				{
					CrossAppDomainSink.ProcessMessageRes processMessageRes = (CrossAppDomainSink.ProcessMessageRes)AppDomain.InvokeInDomainByID(this._domainID, CrossAppDomainSink.processMessageMethod, null, new object[] { array2, cadmethodCallMessage });
					array = processMessageRes.arrResponse;
					cadmethodReturnMessage = processMessageRes.cadMrm;
				}
				finally
				{
					AppDomain.InternalSetContext(currentContext);
				}
				if (array != null)
				{
					MemoryStream memoryStream2 = new MemoryStream(array);
					message = CADSerializer.DeserializeMessage(memoryStream2, msgRequest as IMethodCallMessage);
				}
				else
				{
					message = new MethodResponse(msgRequest as IMethodCallMessage, cadmethodReturnMessage);
				}
			}
			catch (Exception ex)
			{
				try
				{
					message = new ReturnMessage(ex, msgRequest as IMethodCallMessage);
				}
				catch (Exception)
				{
				}
			}
			return message;
		}

		public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
		{
			AsyncRequest asyncRequest = new AsyncRequest(reqMsg, replySink);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendAsyncMessage), asyncRequest);
			return null;
		}

		public void SendAsyncMessage(object data)
		{
			AsyncRequest asyncRequest = (AsyncRequest)data;
			IMessage message = this.SyncProcessMessage(asyncRequest.MsgRequest);
			asyncRequest.ReplySink.SyncProcessMessage(message);
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		private static Hashtable s_sinks = new Hashtable();

		private static MethodInfo processMessageMethod = typeof(CrossAppDomainSink).GetMethod("ProcessMessageInDomain", BindingFlags.Static | BindingFlags.NonPublic);

		private int _domainID;

		private struct ProcessMessageRes
		{
			public byte[] arrResponse;

			public CADMethodReturnMessage cadMrm;
		}
	}
}
