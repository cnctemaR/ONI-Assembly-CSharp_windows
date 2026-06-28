using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public class Context
	{
		public Context()
		{
			this.domain_id = Thread.GetDomainID();
			this.context_id = 1 + Context.global_count++;
		}

		~Context()
		{
		}

		public static Context DefaultContext
		{
			get
			{
				return AppDomain.InternalGetDefaultContext();
			}
		}

		public virtual int ContextID
		{
			get
			{
				return this.context_id;
			}
		}

		public virtual IContextProperty[] ContextProperties
		{
			get
			{
				if (this.context_properties == null)
				{
					return new IContextProperty[0];
				}
				return (IContextProperty[])this.context_properties.ToArray(typeof(IContextProperty[]));
			}
		}

		internal bool IsDefaultContext
		{
			get
			{
				return this.context_id == 0;
			}
		}

		internal bool NeedsContextSink
		{
			get
			{
				return this.context_id != 0 || (Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties) || (this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties);
			}
		}

		public static bool RegisterDynamicProperty(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
		{
			DynamicPropertyCollection dynamicPropertyCollection = Context.GetDynamicPropertyCollection(obj, ctx);
			return dynamicPropertyCollection.RegisterDynamicProperty(prop);
		}

		public static bool UnregisterDynamicProperty(string name, ContextBoundObject obj, Context ctx)
		{
			DynamicPropertyCollection dynamicPropertyCollection = Context.GetDynamicPropertyCollection(obj, ctx);
			return dynamicPropertyCollection.UnregisterDynamicProperty(name);
		}

		private static DynamicPropertyCollection GetDynamicPropertyCollection(ContextBoundObject obj, Context ctx)
		{
			if (ctx == null && obj != null)
			{
				if (RemotingServices.IsTransparentProxy(obj))
				{
					RealProxy realProxy = RemotingServices.GetRealProxy(obj);
					return realProxy.ObjectIdentity.ClientDynamicProperties;
				}
				return obj.ObjectIdentity.ServerDynamicProperties;
			}
			else
			{
				if (ctx != null && obj == null)
				{
					if (ctx.context_dynamic_properties == null)
					{
						ctx.context_dynamic_properties = new DynamicPropertyCollection();
					}
					return ctx.context_dynamic_properties;
				}
				if (ctx == null && obj == null)
				{
					if (Context.global_dynamic_properties == null)
					{
						Context.global_dynamic_properties = new DynamicPropertyCollection();
					}
					return Context.global_dynamic_properties;
				}
				throw new ArgumentException("Either obj or ctx must be null");
			}
		}

		internal static void NotifyGlobalDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties)
			{
				Context.global_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		internal static bool HasGlobalDynamicSinks
		{
			get
			{
				return Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties;
			}
		}

		internal void NotifyDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties)
			{
				this.context_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		internal bool HasDynamicSinks
		{
			get
			{
				return this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties;
			}
		}

		internal bool HasExitSinks
		{
			get
			{
				return !(this.GetClientContextSinkChain() is ClientContextTerminatorSink) || this.HasDynamicSinks || Context.HasGlobalDynamicSinks;
			}
		}

		public virtual IContextProperty GetProperty(string name)
		{
			if (this.context_properties == null)
			{
				return null;
			}
			foreach (object obj in this.context_properties)
			{
				IContextProperty contextProperty = (IContextProperty)obj;
				if (contextProperty.Name == name)
				{
					return contextProperty;
				}
			}
			return null;
		}

		public virtual void SetProperty(IContextProperty prop)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("IContextProperty");
			}
			if (this == Context.DefaultContext)
			{
				throw new InvalidOperationException("Can not add properties to default context");
			}
			if (this.frozen)
			{
				throw new InvalidOperationException("Context is Frozen");
			}
			if (this.context_properties == null)
			{
				this.context_properties = new ArrayList();
			}
			this.context_properties.Add(prop);
		}

		public virtual void Freeze()
		{
			if (this.context_properties != null)
			{
				foreach (object obj in this.context_properties)
				{
					IContextProperty contextProperty = (IContextProperty)obj;
					contextProperty.Freeze(this);
				}
			}
		}

		public override string ToString()
		{
			return "ContextID: " + this.context_id;
		}

		internal IMessageSink GetServerContextSinkChain()
		{
			if (this.server_context_sink_chain == null)
			{
				if (Context.default_server_context_sink == null)
				{
					Context.default_server_context_sink = new ServerContextTerminatorSink();
				}
				this.server_context_sink_chain = Context.default_server_context_sink;
				if (this.context_properties != null)
				{
					for (int i = this.context_properties.Count - 1; i >= 0; i--)
					{
						IContributeServerContextSink contributeServerContextSink = this.context_properties[i] as IContributeServerContextSink;
						if (contributeServerContextSink != null)
						{
							this.server_context_sink_chain = contributeServerContextSink.GetServerContextSink(this.server_context_sink_chain);
						}
					}
				}
			}
			return this.server_context_sink_chain;
		}

		internal IMessageSink GetClientContextSinkChain()
		{
			if (this.client_context_sink_chain == null)
			{
				this.client_context_sink_chain = new ClientContextTerminatorSink(this);
				if (this.context_properties != null)
				{
					foreach (object obj in this.context_properties)
					{
						IContextProperty contextProperty = (IContextProperty)obj;
						IContributeClientContextSink contributeClientContextSink = contextProperty as IContributeClientContextSink;
						if (contributeClientContextSink != null)
						{
							this.client_context_sink_chain = contributeClientContextSink.GetClientContextSink(this.client_context_sink_chain);
						}
					}
				}
			}
			return this.client_context_sink_chain;
		}

		internal IMessageSink CreateServerObjectSinkChain(MarshalByRefObject obj, bool forceInternalExecute)
		{
			IMessageSink messageSink = new StackBuilderSink(obj, forceInternalExecute);
			messageSink = new ServerObjectTerminatorSink(messageSink);
			messageSink = new LeaseSink(messageSink);
			if (this.context_properties != null)
			{
				for (int i = this.context_properties.Count - 1; i >= 0; i--)
				{
					IContextProperty contextProperty = (IContextProperty)this.context_properties[i];
					IContributeObjectSink contributeObjectSink = contextProperty as IContributeObjectSink;
					if (contributeObjectSink != null)
					{
						messageSink = contributeObjectSink.GetObjectSink(obj, messageSink);
					}
				}
			}
			return messageSink;
		}

		internal IMessageSink CreateEnvoySink(MarshalByRefObject serverObject)
		{
			IMessageSink messageSink = EnvoyTerminatorSink.Instance;
			if (this.context_properties != null)
			{
				foreach (object obj in this.context_properties)
				{
					IContextProperty contextProperty = (IContextProperty)obj;
					IContributeEnvoySink contributeEnvoySink = contextProperty as IContributeEnvoySink;
					if (contributeEnvoySink != null)
					{
						messageSink = contributeEnvoySink.GetEnvoySink(serverObject, messageSink);
					}
				}
			}
			return messageSink;
		}

		internal static Context SwitchToContext(Context newContext)
		{
			return AppDomain.InternalSetContext(newContext);
		}

		internal static Context CreateNewContext(IConstructionCallMessage msg)
		{
			Context context = new Context();
			foreach (object obj in msg.ContextProperties)
			{
				IContextProperty contextProperty = (IContextProperty)obj;
				if (context.GetProperty(contextProperty.Name) == null)
				{
					context.SetProperty(contextProperty);
				}
			}
			context.Freeze();
			foreach (object obj2 in msg.ContextProperties)
			{
				IContextProperty contextProperty2 = (IContextProperty)obj2;
				if (!contextProperty2.IsNewContextOK(context))
				{
					throw new RemotingException("A context property did not approve the candidate context for activating the object");
				}
			}
			return context;
		}

		public void DoCallBack(CrossContextDelegate deleg)
		{
			lock (this)
			{
				if (this.callback_object == null)
				{
					Context context = Context.SwitchToContext(this);
					this.callback_object = new ContextCallbackObject();
					Context.SwitchToContext(context);
				}
			}
			this.callback_object.DoCallBack(deleg);
		}

		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return new LocalDataStoreSlot(false);
		}

		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			object syncRoot = Context.namedSlots.SyncRoot;
			LocalDataStoreSlot localDataStoreSlot2;
			lock (syncRoot)
			{
				LocalDataStoreSlot localDataStoreSlot = Context.AllocateDataSlot();
				Context.namedSlots.Add(name, localDataStoreSlot);
				localDataStoreSlot2 = localDataStoreSlot;
			}
			return localDataStoreSlot2;
		}

		public static void FreeNamedDataSlot(string name)
		{
			object syncRoot = Context.namedSlots.SyncRoot;
			lock (syncRoot)
			{
				Context.namedSlots.Remove(name);
			}
		}

		public static object GetData(LocalDataStoreSlot slot)
		{
			Context currentContext = Thread.CurrentContext;
			Context context = currentContext;
			object obj;
			lock (context)
			{
				if (currentContext.datastore != null && slot.slot < currentContext.datastore.Length)
				{
					obj = currentContext.datastore[slot.slot];
				}
				else
				{
					obj = null;
				}
			}
			return obj;
		}

		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			object syncRoot = Context.namedSlots.SyncRoot;
			LocalDataStoreSlot localDataStoreSlot2;
			lock (syncRoot)
			{
				LocalDataStoreSlot localDataStoreSlot = Context.namedSlots[name] as LocalDataStoreSlot;
				if (localDataStoreSlot == null)
				{
					localDataStoreSlot2 = Context.AllocateNamedDataSlot(name);
				}
				else
				{
					localDataStoreSlot2 = localDataStoreSlot;
				}
			}
			return localDataStoreSlot2;
		}

		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			Context currentContext = Thread.CurrentContext;
			Context context = currentContext;
			lock (context)
			{
				if (currentContext.datastore == null)
				{
					currentContext.datastore = new object[slot.slot + 2];
				}
				else if (slot.slot >= currentContext.datastore.Length)
				{
					object[] array = new object[slot.slot + 2];
					currentContext.datastore.CopyTo(array, 0);
					currentContext.datastore = array;
				}
				currentContext.datastore[slot.slot] = data;
			}
		}

		private int domain_id;

		private int context_id;

		private UIntPtr static_data;

		private static IMessageSink default_server_context_sink;

		private IMessageSink server_context_sink_chain;

		private IMessageSink client_context_sink_chain;

		private object[] datastore;

		private ArrayList context_properties;

		private bool frozen;

		private static int global_count;

		private static Hashtable namedSlots = new Hashtable();

		private static DynamicPropertyCollection global_dynamic_properties;

		private DynamicPropertyCollection context_dynamic_properties;

		private ContextCallbackObject callback_object;
	}
}
