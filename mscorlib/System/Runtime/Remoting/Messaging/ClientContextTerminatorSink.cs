using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Messaging
{
	internal class ClientContextTerminatorSink : IMessageSink
	{
		public ClientContextTerminatorSink(Context ctx)
		{
			this._context = ctx;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			Context.NotifyGlobalDynamicSinks(true, msg, true, false);
			this._context.NotifyDynamicSinks(true, msg, true, false);
			IMessage message;
			if (msg is IConstructionCallMessage)
			{
				message = ActivationServices.RemoteActivate((IConstructionCallMessage)msg);
			}
			else
			{
				message = RemotingServices.GetMessageTargetIdentity(msg).ChannelSink.SyncProcessMessage(msg);
			}
			Context.NotifyGlobalDynamicSinks(false, msg, true, false);
			this._context.NotifyDynamicSinks(false, msg, true, false);
			return message;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			if (this._context.HasDynamicSinks || Context.HasGlobalDynamicSinks)
			{
				Context.NotifyGlobalDynamicSinks(true, msg, true, true);
				this._context.NotifyDynamicSinks(true, msg, true, true);
				if (replySink != null)
				{
					replySink = new ClientContextReplySink(this._context, replySink);
				}
			}
			IMessageCtrl messageCtrl = RemotingServices.GetMessageTargetIdentity(msg).ChannelSink.AsyncProcessMessage(msg, replySink);
			if (replySink == null && (this._context.HasDynamicSinks || Context.HasGlobalDynamicSinks))
			{
				Context.NotifyGlobalDynamicSinks(false, msg, true, true);
				this._context.NotifyDynamicSinks(false, msg, true, true);
			}
			return messageCtrl;
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		private Context _context;
	}
}
