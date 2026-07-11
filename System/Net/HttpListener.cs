using System;
using System.Collections;
using System.Threading;

namespace System.Net
{
	public sealed class HttpListener : IDisposable
	{
		public HttpListener()
		{
			this.prefixes = new HttpListenerPrefixCollection(this);
			this.registry = new Hashtable();
			this.ctx_queue = new ArrayList();
			this.wait_queue = new ArrayList();
			this.auth_schemes = AuthenticationSchemes.Anonymous;
		}

		void IDisposable.Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.Close(true);
			this.disposed = true;
		}

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this.auth_schemes;
			}
			set
			{
				this.CheckDisposed();
				this.auth_schemes = value;
			}
		}

		public AuthenticationSchemeSelector AuthenticationSchemeSelectorDelegate
		{
			get
			{
				return this.auth_selector;
			}
			set
			{
				this.CheckDisposed();
				this.auth_selector = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				return this.ignore_write_exceptions;
			}
			set
			{
				this.CheckDisposed();
				this.ignore_write_exceptions = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return this.listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				this.CheckDisposed();
				return this.prefixes;
			}
		}

		public string Realm
		{
			get
			{
				return this.realm;
			}
			set
			{
				this.CheckDisposed();
				this.realm = value;
			}
		}

		[global::System.MonoTODO("Support for NTLM needs some loving.")]
		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				return this.unsafe_ntlm_auth;
			}
			set
			{
				this.CheckDisposed();
				this.unsafe_ntlm_auth = value;
			}
		}

		public void Abort()
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.listening)
			{
				return;
			}
			this.Close(true);
		}

		public void Close()
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.listening)
			{
				this.disposed = true;
				return;
			}
			this.Close(false);
			this.disposed = true;
		}

		private void Close(bool force)
		{
			this.CheckDisposed();
			EndPointManager.RemoveListener(this);
			this.Cleanup(force);
		}

		private void Cleanup(bool close_existing)
		{
			Hashtable hashtable = this.registry;
			lock (hashtable)
			{
				if (close_existing)
				{
					foreach (object obj in this.registry.Keys)
					{
						HttpListenerContext httpListenerContext = (HttpListenerContext)obj;
						httpListenerContext.Connection.Close();
					}
					this.registry.Clear();
				}
				ArrayList arrayList = this.ctx_queue;
				lock (arrayList)
				{
					foreach (object obj2 in this.ctx_queue)
					{
						HttpListenerContext httpListenerContext2 = (HttpListenerContext)obj2;
						httpListenerContext2.Connection.Close();
					}
					this.ctx_queue.Clear();
				}
				ArrayList arrayList2 = this.wait_queue;
				lock (arrayList2)
				{
					foreach (object obj3 in this.wait_queue)
					{
						ListenerAsyncResult listenerAsyncResult = (ListenerAsyncResult)obj3;
						listenerAsyncResult.Complete("Listener was closed.");
					}
					this.wait_queue.Clear();
				}
			}
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (!this.listening)
			{
				throw new InvalidOperationException("Please, call Start before using this method.");
			}
			ListenerAsyncResult listenerAsyncResult = new ListenerAsyncResult(callback, state);
			ArrayList arrayList = this.wait_queue;
			lock (arrayList)
			{
				ArrayList arrayList2 = this.ctx_queue;
				lock (arrayList2)
				{
					HttpListenerContext contextFromQueue = this.GetContextFromQueue();
					if (contextFromQueue != null)
					{
						listenerAsyncResult.Complete(contextFromQueue, true);
						return listenerAsyncResult;
					}
				}
				this.wait_queue.Add(listenerAsyncResult);
			}
			return listenerAsyncResult;
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			ListenerAsyncResult listenerAsyncResult = asyncResult as ListenerAsyncResult;
			if (listenerAsyncResult == null)
			{
				throw new ArgumentException("Wrong IAsyncResult.", "asyncResult");
			}
			if (!listenerAsyncResult.IsCompleted)
			{
				listenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			ArrayList arrayList = this.wait_queue;
			lock (arrayList)
			{
				int num = this.wait_queue.IndexOf(listenerAsyncResult);
				if (num >= 0)
				{
					this.wait_queue.RemoveAt(num);
				}
			}
			HttpListenerContext context = listenerAsyncResult.GetContext();
			if (this.auth_schemes != AuthenticationSchemes.Anonymous)
			{
				context.ParseAuthentication(this.auth_schemes);
			}
			return context;
		}

		internal AuthenticationSchemes SelectAuthenticationScheme(HttpListenerContext context)
		{
			if (this.AuthenticationSchemeSelectorDelegate != null)
			{
				return this.AuthenticationSchemeSelectorDelegate(context.Request);
			}
			return this.auth_schemes;
		}

		public HttpListenerContext GetContext()
		{
			if (this.prefixes.Count == 0)
			{
				throw new InvalidOperationException("Please, call AddPrefix before using this method.");
			}
			IAsyncResult asyncResult = this.BeginGetContext(null, null);
			return this.EndGetContext(asyncResult);
		}

		public void Start()
		{
			this.CheckDisposed();
			if (this.listening)
			{
				return;
			}
			EndPointManager.AddListener(this);
			this.listening = true;
		}

		public void Stop()
		{
			this.CheckDisposed();
			this.listening = false;
			this.Close(false);
		}

		internal void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private HttpListenerContext GetContextFromQueue()
		{
			if (this.ctx_queue.Count == 0)
			{
				return null;
			}
			HttpListenerContext httpListenerContext = (HttpListenerContext)this.ctx_queue[0];
			this.ctx_queue.RemoveAt(0);
			return httpListenerContext;
		}

		internal void RegisterContext(HttpListenerContext context)
		{
			try
			{
				Monitor.Enter(this.registry);
				this.registry[context] = context;
				Monitor.Enter(this.wait_queue);
				Monitor.Enter(this.ctx_queue);
				if (this.wait_queue.Count == 0)
				{
					this.ctx_queue.Add(context);
				}
				else
				{
					ListenerAsyncResult listenerAsyncResult = (ListenerAsyncResult)this.wait_queue[0];
					this.wait_queue.RemoveAt(0);
					listenerAsyncResult.Complete(context);
				}
			}
			finally
			{
				Monitor.Exit(this.ctx_queue);
				Monitor.Exit(this.wait_queue);
				Monitor.Exit(this.registry);
			}
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			try
			{
				Monitor.Enter(this.registry);
				Monitor.Enter(this.ctx_queue);
				int num = this.ctx_queue.IndexOf(context);
				if (num >= 0)
				{
					this.ctx_queue.RemoveAt(num);
				}
				this.registry.Remove(context);
			}
			finally
			{
				Monitor.Exit(this.ctx_queue);
				Monitor.Exit(this.registry);
			}
		}

		private AuthenticationSchemes auth_schemes;

		private HttpListenerPrefixCollection prefixes;

		private AuthenticationSchemeSelector auth_selector;

		private string realm;

		private bool ignore_write_exceptions;

		private bool unsafe_ntlm_auth;

		private bool listening;

		private bool disposed;

		private Hashtable registry;

		private ArrayList ctx_queue;

		private ArrayList wait_queue;
	}
}
