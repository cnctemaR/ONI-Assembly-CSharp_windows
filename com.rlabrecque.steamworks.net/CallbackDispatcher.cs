using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Steamworks
{
	public static class CallbackDispatcher
	{
		public static void ExceptionHandler(Exception e)
		{
			Debug.LogException(e);
		}

		public static bool IsInitialized
		{
			get
			{
				return CallbackDispatcher.m_initCount > 0;
			}
		}

		internal static void Initialize()
		{
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				if (CallbackDispatcher.m_initCount == 0)
				{
					NativeMethods.SteamAPI_ManualDispatch_Init();
					CallbackDispatcher.m_pCallbackMsg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CallbackMsg_t)));
				}
				CallbackDispatcher.m_initCount++;
			}
		}

		internal static void Shutdown()
		{
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				CallbackDispatcher.m_initCount--;
				if (CallbackDispatcher.m_initCount == 0)
				{
					CallbackDispatcher.UnregisterAll();
					Marshal.FreeHGlobal(CallbackDispatcher.m_pCallbackMsg);
					CallbackDispatcher.m_pCallbackMsg = IntPtr.Zero;
				}
			}
		}

		internal static void Register(Callback cb)
		{
			int callbackIdentity = CallbackIdentities.GetCallbackIdentity(cb.GetCallbackType());
			Dictionary<int, List<Callback>> dictionary = (cb.IsGameServer ? CallbackDispatcher.m_registeredGameServerCallbacks : CallbackDispatcher.m_registeredCallbacks);
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				List<Callback> list;
				if (!dictionary.TryGetValue(callbackIdentity, out list))
				{
					list = new List<Callback>();
					dictionary.Add(callbackIdentity, list);
				}
				list.Add(cb);
			}
		}

		internal static void Register(SteamAPICall_t asyncCall, CallResult cr)
		{
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				List<CallResult> list;
				if (!CallbackDispatcher.m_registeredCallResults.TryGetValue((ulong)asyncCall, out list))
				{
					list = new List<CallResult>();
					CallbackDispatcher.m_registeredCallResults.Add((ulong)asyncCall, list);
				}
				list.Add(cr);
			}
		}

		internal static void Unregister(Callback cb)
		{
			int callbackIdentity = CallbackIdentities.GetCallbackIdentity(cb.GetCallbackType());
			Dictionary<int, List<Callback>> dictionary = (cb.IsGameServer ? CallbackDispatcher.m_registeredGameServerCallbacks : CallbackDispatcher.m_registeredCallbacks);
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				List<Callback> list;
				if (dictionary.TryGetValue(callbackIdentity, out list))
				{
					list.Remove(cb);
					if (list.Count == 0)
					{
						dictionary.Remove(callbackIdentity);
					}
				}
			}
		}

		internal static void Unregister(SteamAPICall_t asyncCall, CallResult cr)
		{
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				List<CallResult> list;
				if (CallbackDispatcher.m_registeredCallResults.TryGetValue((ulong)asyncCall, out list))
				{
					list.Remove(cr);
					if (list.Count == 0)
					{
						CallbackDispatcher.m_registeredCallResults.Remove((ulong)asyncCall);
					}
				}
			}
		}

		private static void UnregisterAll()
		{
			List<Callback> list = new List<Callback>();
			List<CallResult> list2 = new List<CallResult>();
			object sync = CallbackDispatcher.m_sync;
			lock (sync)
			{
				foreach (KeyValuePair<int, List<Callback>> keyValuePair in CallbackDispatcher.m_registeredCallbacks)
				{
					list.AddRange(keyValuePair.Value);
				}
				CallbackDispatcher.m_registeredCallbacks.Clear();
				foreach (KeyValuePair<int, List<Callback>> keyValuePair2 in CallbackDispatcher.m_registeredGameServerCallbacks)
				{
					list.AddRange(keyValuePair2.Value);
				}
				CallbackDispatcher.m_registeredGameServerCallbacks.Clear();
				foreach (KeyValuePair<ulong, List<CallResult>> keyValuePair3 in CallbackDispatcher.m_registeredCallResults)
				{
					list2.AddRange(keyValuePair3.Value);
				}
				CallbackDispatcher.m_registeredCallResults.Clear();
				foreach (Callback callback in list)
				{
					callback.SetUnregistered();
				}
				foreach (CallResult callResult in list2)
				{
					callResult.SetUnregistered();
				}
			}
		}

		internal static void RunFrame(bool isGameServer)
		{
			if (!CallbackDispatcher.IsInitialized)
			{
				throw new InvalidOperationException("Callback dispatcher is not initialized.");
			}
			HSteamPipe hsteamPipe = (HSteamPipe)(isGameServer ? NativeMethods.SteamGameServer_GetHSteamPipe() : NativeMethods.SteamAPI_GetHSteamPipe());
			NativeMethods.SteamAPI_ManualDispatch_RunFrame(hsteamPipe);
			Dictionary<int, List<Callback>> dictionary = (isGameServer ? CallbackDispatcher.m_registeredGameServerCallbacks : CallbackDispatcher.m_registeredCallbacks);
			while (NativeMethods.SteamAPI_ManualDispatch_GetNextCallback(hsteamPipe, CallbackDispatcher.m_pCallbackMsg))
			{
				CallbackMsg_t callbackMsg_t = (CallbackMsg_t)Marshal.PtrToStructure(CallbackDispatcher.m_pCallbackMsg, typeof(CallbackMsg_t));
				try
				{
					List<Callback> list2;
					if (callbackMsg_t.m_iCallback == 703)
					{
						SteamAPICallCompleted_t steamAPICallCompleted_t = (SteamAPICallCompleted_t)Marshal.PtrToStructure(callbackMsg_t.m_pubParam, typeof(SteamAPICallCompleted_t));
						IntPtr intPtr = Marshal.AllocHGlobal((int)steamAPICallCompleted_t.m_cubParam);
						bool flag;
						if (NativeMethods.SteamAPI_ManualDispatch_GetAPICallResult(hsteamPipe, steamAPICallCompleted_t.m_hAsyncCall, intPtr, (int)steamAPICallCompleted_t.m_cubParam, steamAPICallCompleted_t.m_iCallback, out flag))
						{
							object obj = CallbackDispatcher.m_sync;
							lock (obj)
							{
								List<CallResult> list;
								if (CallbackDispatcher.m_registeredCallResults.TryGetValue((ulong)steamAPICallCompleted_t.m_hAsyncCall, out list))
								{
									CallbackDispatcher.m_registeredCallResults.Remove((ulong)steamAPICallCompleted_t.m_hAsyncCall);
									foreach (CallResult callResult in list)
									{
										callResult.OnRunCallResult(intPtr, flag, (ulong)steamAPICallCompleted_t.m_hAsyncCall);
										callResult.SetUnregistered();
									}
								}
							}
						}
						Marshal.FreeHGlobal(intPtr);
					}
					else if (dictionary.TryGetValue(callbackMsg_t.m_iCallback, out list2))
					{
						object obj = CallbackDispatcher.m_sync;
						List<Callback> list3;
						lock (obj)
						{
							list3 = new List<Callback>(list2);
						}
						foreach (Callback callback in list3)
						{
							callback.OnRunCallback(callbackMsg_t.m_pubParam);
						}
					}
				}
				catch (Exception ex)
				{
					CallbackDispatcher.ExceptionHandler(ex);
				}
				finally
				{
					NativeMethods.SteamAPI_ManualDispatch_FreeLastCallback(hsteamPipe);
				}
			}
		}

		private static Dictionary<int, List<Callback>> m_registeredCallbacks = new Dictionary<int, List<Callback>>();

		private static Dictionary<int, List<Callback>> m_registeredGameServerCallbacks = new Dictionary<int, List<Callback>>();

		private static Dictionary<ulong, List<CallResult>> m_registeredCallResults = new Dictionary<ulong, List<CallResult>>();

		private static object m_sync = new object();

		private static IntPtr m_pCallbackMsg;

		private static int m_initCount;
	}
}
