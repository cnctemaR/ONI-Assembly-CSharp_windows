using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace System.Diagnostics
{
	public class TraceSource
	{
		public TraceSource(string name)
			: this(name, SourceLevels.Off)
		{
		}

		public TraceSource(string name, SourceLevels defaultLevel)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("name");
			}
			this.sourceName = name;
			this.switchLevel = defaultLevel;
			List<WeakReference> list = TraceSource.tracesources;
			lock (list)
			{
				TraceSource._pruneCachedTraceSources();
				TraceSource.tracesources.Add(new WeakReference(this));
			}
		}

		private static void _pruneCachedTraceSources()
		{
			List<WeakReference> list = TraceSource.tracesources;
			lock (list)
			{
				if (TraceSource.s_LastCollectionCount != GC.CollectionCount(2))
				{
					List<WeakReference> list2 = new List<WeakReference>(TraceSource.tracesources.Count);
					for (int i = 0; i < TraceSource.tracesources.Count; i++)
					{
						if ((TraceSource)TraceSource.tracesources[i].Target != null)
						{
							list2.Add(TraceSource.tracesources[i]);
						}
					}
					if (list2.Count < TraceSource.tracesources.Count)
					{
						TraceSource.tracesources.Clear();
						TraceSource.tracesources.AddRange(list2);
						TraceSource.tracesources.TrimExcess();
					}
					TraceSource.s_LastCollectionCount = GC.CollectionCount(2);
				}
			}
		}

		private void Initialize()
		{
			if (!this._initCalled)
			{
				lock (this)
				{
					if (!this._initCalled)
					{
						SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
						if (sources != null)
						{
							SourceElement sourceElement = sources[this.sourceName];
							if (sourceElement != null)
							{
								if (!string.IsNullOrEmpty(sourceElement.SwitchName))
								{
									this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
								}
								else
								{
									this.CreateSwitch(sourceElement.SwitchType, this.sourceName);
									if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
									{
										this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
									}
								}
								this.listeners = sourceElement.Listeners.GetRuntimeObject();
								this.attributes = new StringDictionary();
								TraceUtils.VerifyAttributes(sourceElement.Attributes, this.GetSupportedAttributes(), this);
								this.attributes.ReplaceHashtable(sourceElement.Attributes);
							}
							else
							{
								this.NoConfigInit();
							}
						}
						else
						{
							this.NoConfigInit();
						}
						this._initCalled = true;
					}
				}
			}
		}

		private void NoConfigInit()
		{
			this.internalSwitch = new SourceSwitch(this.sourceName, this.switchLevel.ToString());
			this.listeners = new TraceListenerCollection();
			this.listeners.Add(new DefaultTraceListener());
			this.attributes = null;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Close()
		{
			if (this.listeners != null)
			{
				object critSec = TraceInternal.critSec;
				lock (critSec)
				{
					foreach (object obj in this.listeners)
					{
						((TraceListener)obj).Close();
					}
				}
			}
		}

		public void Flush()
		{
			if (this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						using (IEnumerator enumerator = this.listeners.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								((TraceListener)obj).Flush();
							}
							return;
						}
					}
				}
				foreach (object obj2 in this.listeners)
				{
					TraceListener traceListener = (TraceListener)obj2;
					if (!traceListener.IsThreadSafe)
					{
						TraceListener traceListener2 = traceListener;
						lock (traceListener2)
						{
							traceListener.Flush();
							continue;
						}
					}
					traceListener.Flush();
				}
			}
		}

		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		internal static void RefreshAll()
		{
			List<WeakReference> list = TraceSource.tracesources;
			lock (list)
			{
				TraceSource._pruneCachedTraceSources();
				for (int i = 0; i < TraceSource.tracesources.Count; i++)
				{
					TraceSource traceSource = (TraceSource)TraceSource.tracesources[i].Target;
					if (traceSource != null)
					{
						traceSource.Refresh();
					}
				}
			}
		}

		internal void Refresh()
		{
			if (!this._initCalled)
			{
				this.Initialize();
				return;
			}
			SourceElementsCollection sources = DiagnosticsConfiguration.Sources;
			if (sources != null)
			{
				SourceElement sourceElement = sources[this.Name];
				if (sourceElement != null)
				{
					if ((string.IsNullOrEmpty(sourceElement.SwitchType) && this.internalSwitch.GetType() != typeof(SourceSwitch)) || sourceElement.SwitchType != this.internalSwitch.GetType().AssemblyQualifiedName)
					{
						if (!string.IsNullOrEmpty(sourceElement.SwitchName))
						{
							this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
						}
						else
						{
							this.CreateSwitch(sourceElement.SwitchType, this.Name);
							if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
							{
								this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
							}
						}
					}
					else if (!string.IsNullOrEmpty(sourceElement.SwitchName))
					{
						if (sourceElement.SwitchName != this.internalSwitch.DisplayName)
						{
							this.CreateSwitch(sourceElement.SwitchType, sourceElement.SwitchName);
						}
						else
						{
							this.internalSwitch.Refresh();
						}
					}
					else if (!string.IsNullOrEmpty(sourceElement.SwitchValue))
					{
						this.internalSwitch.Level = (SourceLevels)Enum.Parse(typeof(SourceLevels), sourceElement.SwitchValue);
					}
					else
					{
						this.internalSwitch.Level = SourceLevels.Off;
					}
					TraceListenerCollection traceListenerCollection = new TraceListenerCollection();
					foreach (object obj in sourceElement.Listeners)
					{
						ListenerElement listenerElement = (ListenerElement)obj;
						TraceListener traceListener = this.listeners[listenerElement.Name];
						if (traceListener != null)
						{
							traceListenerCollection.Add(listenerElement.RefreshRuntimeObject(traceListener));
						}
						else
						{
							traceListenerCollection.Add(listenerElement.GetRuntimeObject());
						}
					}
					TraceUtils.VerifyAttributes(sourceElement.Attributes, this.GetSupportedAttributes(), this);
					this.attributes = new StringDictionary();
					this.attributes.ReplaceHashtable(sourceElement.Attributes);
					this.listeners = traceListenerCollection;
					return;
				}
				this.internalSwitch.Level = this.switchLevel;
				this.listeners.Clear();
				this.attributes = null;
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(traceEventCache, this.Name, eventType, id);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_010F;
						}
						goto IL_00F1;
					}
					goto IL_00F1;
					IL_010F:
					j++;
					continue;
					IL_00F1:
					traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_010F;
					}
					goto IL_010F;
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string message)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(traceEventCache, this.Name, eventType, id, message);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id, message);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_0112;
						}
						goto IL_00F3;
					}
					goto IL_00F3;
					IL_0112:
					j++;
					continue;
					IL_00F3:
					traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id, message);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_0112;
					}
					goto IL_0112;
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceEvent(traceEventCache, this.Name, eventType, id, format, args);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id, format, args);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_0118;
						}
						goto IL_00F7;
					}
					goto IL_00F7;
					IL_0118:
					j++;
					continue;
					IL_00F7:
					traceListener2.TraceEvent(traceEventCache, this.Name, eventType, id, format, args);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_0118;
					}
					goto IL_0118;
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, object data)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceData(traceEventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceData(traceEventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_0112;
						}
						goto IL_00F3;
					}
					goto IL_00F3;
					IL_0112:
					j++;
					continue;
					IL_00F3:
					traceListener2.TraceData(traceEventCache, this.Name, eventType, id, data);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_0112;
					}
					goto IL_0112;
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(eventType) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceData(traceEventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceData(traceEventCache, this.Name, eventType, id, data);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_0112;
						}
						goto IL_00F3;
					}
					goto IL_00F3;
					IL_0112:
					j++;
					continue;
					IL_00F3:
					traceListener2.TraceData(traceEventCache, this.Name, eventType, id, data);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_0112;
					}
					goto IL_0112;
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceInformation(string message)
		{
		}

		[Conditional("TRACE")]
		public void TraceInformation(string format, params object[] args)
		{
		}

		[Conditional("TRACE")]
		public void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			this.Initialize();
			TraceEventCache traceEventCache = new TraceEventCache();
			if (this.internalSwitch.ShouldTrace(TraceEventType.Transfer) && this.listeners != null)
			{
				if (TraceInternal.UseGlobalLock)
				{
					object critSec = TraceInternal.critSec;
					lock (critSec)
					{
						for (int i = 0; i < this.listeners.Count; i++)
						{
							TraceListener traceListener = this.listeners[i];
							traceListener.TraceTransfer(traceEventCache, this.Name, id, message, relatedActivityId);
							if (Trace.AutoFlush)
							{
								traceListener.Flush();
							}
						}
						return;
					}
				}
				int j = 0;
				while (j < this.listeners.Count)
				{
					TraceListener traceListener2 = this.listeners[j];
					if (!traceListener2.IsThreadSafe)
					{
						TraceListener traceListener3 = traceListener2;
						lock (traceListener3)
						{
							traceListener2.TraceTransfer(traceEventCache, this.Name, id, message, relatedActivityId);
							if (Trace.AutoFlush)
							{
								traceListener2.Flush();
							}
							goto IL_0116;
						}
						goto IL_00F7;
					}
					goto IL_00F7;
					IL_0116:
					j++;
					continue;
					IL_00F7:
					traceListener2.TraceTransfer(traceEventCache, this.Name, id, message, relatedActivityId);
					if (Trace.AutoFlush)
					{
						traceListener2.Flush();
						goto IL_0116;
					}
					goto IL_0116;
				}
			}
		}

		private void CreateSwitch(string typename, string name)
		{
			if (!string.IsNullOrEmpty(typename))
			{
				this.internalSwitch = (SourceSwitch)TraceUtils.GetRuntimeObject(typename, typeof(SourceSwitch), name);
				return;
			}
			this.internalSwitch = new SourceSwitch(name, this.switchLevel.ToString());
		}

		public StringDictionary Attributes
		{
			get
			{
				this.Initialize();
				if (this.attributes == null)
				{
					this.attributes = new StringDictionary();
				}
				return this.attributes;
			}
		}

		public string Name
		{
			get
			{
				return this.sourceName;
			}
		}

		public TraceListenerCollection Listeners
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				this.Initialize();
				return this.listeners;
			}
		}

		public SourceSwitch Switch
		{
			get
			{
				this.Initialize();
				return this.internalSwitch;
			}
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Switch");
				}
				this.Initialize();
				this.internalSwitch = value;
			}
		}

		private static List<WeakReference> tracesources = new List<WeakReference>();

		private static int s_LastCollectionCount;

		private volatile SourceSwitch internalSwitch;

		private volatile TraceListenerCollection listeners;

		private StringDictionary attributes;

		private SourceLevels switchLevel;

		private volatile string sourceName;

		internal volatile bool _initCalled;
	}
}
