using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public sealed class Callback<T> : Callback, IDisposable
	{
		private event Callback<T>.DispatchDelegate m_Func;

		public static Callback<T> Create(Callback<T>.DispatchDelegate func)
		{
			return new Callback<T>(func, false);
		}

		public static Callback<T> CreateGameServer(Callback<T>.DispatchDelegate func)
		{
			return new Callback<T>(func, true);
		}

		public Callback(Callback<T>.DispatchDelegate func, bool bGameServer = false)
		{
			this.m_bGameServer = bGameServer;
			this.Register(func);
		}

		~Callback()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.m_bDisposed)
			{
				return;
			}
			GC.SuppressFinalize(this);
			if (this.m_bIsRegistered)
			{
				this.Unregister();
			}
			this.m_bDisposed = true;
		}

		public void Register(Callback<T>.DispatchDelegate func)
		{
			if (func == null)
			{
				throw new Exception("Callback function must not be null.");
			}
			if (this.m_bIsRegistered)
			{
				this.Unregister();
			}
			this.m_Func = func;
			CallbackDispatcher.Register(this);
			this.m_bIsRegistered = true;
		}

		public void Unregister()
		{
			CallbackDispatcher.Unregister(this);
			this.m_bIsRegistered = false;
		}

		public override bool IsGameServer
		{
			get
			{
				return this.m_bGameServer;
			}
		}

		internal override Type GetCallbackType()
		{
			return typeof(T);
		}

		internal override void OnRunCallback(IntPtr pvParam)
		{
			try
			{
				this.m_Func((T)((object)Marshal.PtrToStructure(pvParam, typeof(T))));
			}
			catch (Exception ex)
			{
				CallbackDispatcher.ExceptionHandler(ex);
			}
		}

		internal override void SetUnregistered()
		{
			this.m_bIsRegistered = false;
		}

		private bool m_bGameServer;

		private bool m_bIsRegistered;

		private bool m_bDisposed;

		public delegate void DispatchDelegate(T param);
	}
}
