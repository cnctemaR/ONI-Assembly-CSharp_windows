using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class SignalReceiver : MonoBehaviour, INotificationReceiver
	{
		public void OnNotify(Playable origin, INotification notification, object context)
		{
			SignalEmitter signalEmitter = notification as SignalEmitter;
			UnityEvent unityEvent;
			if (signalEmitter != null && signalEmitter.asset != null && this.m_Events.TryGetValue(signalEmitter.asset, out unityEvent) && unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		public void AddReaction(SignalAsset asset, UnityEvent reaction)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
			}
			if (this.m_Events.signals.Contains(asset))
			{
				throw new ArgumentException("SignalAsset already used.");
			}
			this.m_Events.Append(asset, reaction);
		}

		public int AddEmptyReaction(UnityEvent reaction)
		{
			this.m_Events.Append(null, reaction);
			return this.m_Events.events.Count - 1;
		}

		public void Remove(SignalAsset asset)
		{
			if (!this.m_Events.signals.Contains(asset))
			{
				throw new ArgumentException("The SignalAsset is not registered with this receiver.");
			}
			this.m_Events.Remove(asset);
		}

		public IEnumerable<SignalAsset> GetRegisteredSignals()
		{
			return this.m_Events.signals;
		}

		public UnityEvent GetReaction(SignalAsset key)
		{
			UnityEvent unityEvent;
			if (this.m_Events.TryGetValue(key, out unityEvent))
			{
				return unityEvent;
			}
			return null;
		}

		public int Count()
		{
			return this.m_Events.signals.Count;
		}

		public void ChangeSignalAtIndex(int idx, SignalAsset newKey)
		{
			if (idx < 0 || idx > this.m_Events.signals.Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
			if (this.m_Events.signals[idx] == newKey)
			{
				return;
			}
			bool flag = this.m_Events.signals.Contains(newKey);
			if (newKey == null || this.m_Events.signals[idx] == null || !flag)
			{
				this.m_Events.signals[idx] = newKey;
			}
			if (flag)
			{
				throw new ArgumentException("SignalAsset already used.");
			}
		}

		public void RemoveAtIndex(int idx)
		{
			if (idx < 0 || idx > this.m_Events.signals.Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
			this.m_Events.Remove(idx);
		}

		public void ChangeReactionAtIndex(int idx, UnityEvent reaction)
		{
			if (idx < 0 || idx > this.m_Events.events.Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
			this.m_Events.events[idx] = reaction;
		}

		public UnityEvent GetReactionAtIndex(int idx)
		{
			if (idx < 0 || idx > this.m_Events.events.Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
			return this.m_Events.events[idx];
		}

		public SignalAsset GetSignalAssetAtIndex(int idx)
		{
			if (idx < 0 || idx > this.m_Events.signals.Count - 1)
			{
				throw new IndexOutOfRangeException();
			}
			return this.m_Events.signals[idx];
		}

		private void OnEnable()
		{
		}

		[SerializeField]
		private SignalReceiver.EventKeyValue m_Events = new SignalReceiver.EventKeyValue();

		[Serializable]
		private class EventKeyValue
		{
			public bool TryGetValue(SignalAsset key, out UnityEvent value)
			{
				int num = this.m_Signals.IndexOf(key);
				if (num != -1)
				{
					value = this.m_Events[num];
					return true;
				}
				value = null;
				return false;
			}

			public void Append(SignalAsset key, UnityEvent value)
			{
				this.m_Signals.Add(key);
				this.m_Events.Add(value);
			}

			public void Remove(int idx)
			{
				if (idx != -1)
				{
					this.m_Signals.RemoveAt(idx);
					this.m_Events.RemoveAt(idx);
				}
			}

			public void Remove(SignalAsset key)
			{
				int num = this.m_Signals.IndexOf(key);
				if (num != -1)
				{
					this.m_Signals.RemoveAt(num);
					this.m_Events.RemoveAt(num);
				}
			}

			public List<SignalAsset> signals
			{
				get
				{
					return this.m_Signals;
				}
			}

			public List<UnityEvent> events
			{
				get
				{
					return this.m_Events;
				}
			}

			[SerializeField]
			private List<SignalAsset> m_Signals = new List<SignalAsset>();

			[SerializeField]
			[CustomSignalEventDrawer]
			private List<UnityEvent> m_Events = new List<UnityEvent>();
		}
	}
}
