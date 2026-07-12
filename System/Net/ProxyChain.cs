using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Net
{
	internal abstract class ProxyChain : IEnumerable<Uri>, IEnumerable, IDisposable
	{
		protected ProxyChain(Uri destination)
		{
			this.m_Destination = destination;
		}

		public IEnumerator<Uri> GetEnumerator()
		{
			ProxyChain.ProxyEnumerator proxyEnumerator = new ProxyChain.ProxyEnumerator(this);
			if (this.m_MainEnumerator == null)
			{
				this.m_MainEnumerator = proxyEnumerator;
			}
			return proxyEnumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual void Dispose()
		{
		}

		internal IEnumerator<Uri> Enumerator
		{
			get
			{
				if (this.m_MainEnumerator != null)
				{
					return this.m_MainEnumerator;
				}
				return this.GetEnumerator();
			}
		}

		internal Uri Destination
		{
			get
			{
				return this.m_Destination;
			}
		}

		internal virtual void Abort()
		{
		}

		internal bool HttpAbort(HttpWebRequest request, WebException webException)
		{
			this.Abort();
			return true;
		}

		internal HttpAbortDelegate HttpAbortDelegate
		{
			get
			{
				if (this.m_HttpAbortDelegate == null)
				{
					this.m_HttpAbortDelegate = new HttpAbortDelegate(this.HttpAbort);
				}
				return this.m_HttpAbortDelegate;
			}
		}

		protected abstract bool GetNextProxy(out Uri proxy);

		private List<Uri> m_Cache = new List<Uri>();

		private bool m_CacheComplete;

		private ProxyChain.ProxyEnumerator m_MainEnumerator;

		private Uri m_Destination;

		private HttpAbortDelegate m_HttpAbortDelegate;

		private class ProxyEnumerator : IEnumerator<Uri>, IDisposable, IEnumerator
		{
			internal ProxyEnumerator(ProxyChain chain)
			{
				this.m_Chain = chain;
			}

			public Uri Current
			{
				get
				{
					if (this.m_Finished || this.m_CurrentIndex < 0)
					{
						throw new InvalidOperationException(SR.GetString("Enumeration has either not started or has already finished."));
					}
					return this.m_Chain.m_Cache[this.m_CurrentIndex];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (this.m_Finished)
				{
					return false;
				}
				checked
				{
					this.m_CurrentIndex++;
					if (this.m_Chain.m_Cache.Count > this.m_CurrentIndex)
					{
						return true;
					}
					if (this.m_Chain.m_CacheComplete)
					{
						this.m_Finished = true;
						return false;
					}
					List<Uri> cache = this.m_Chain.m_Cache;
					bool flag2;
					lock (cache)
					{
						if (this.m_Chain.m_Cache.Count > this.m_CurrentIndex)
						{
							flag2 = true;
						}
						else if (this.m_Chain.m_CacheComplete)
						{
							this.m_Finished = true;
							flag2 = false;
						}
						else
						{
							Uri uri;
							while (this.m_Chain.GetNextProxy(out uri))
							{
								if (uri == null)
								{
									if (this.m_TriedDirect)
									{
										continue;
									}
									this.m_TriedDirect = true;
								}
								this.m_Chain.m_Cache.Add(uri);
								return true;
							}
							this.m_Finished = true;
							this.m_Chain.m_CacheComplete = true;
							flag2 = false;
						}
					}
					return flag2;
				}
			}

			public void Reset()
			{
				this.m_Finished = false;
				this.m_CurrentIndex = -1;
			}

			public void Dispose()
			{
			}

			private ProxyChain m_Chain;

			private bool m_Finished;

			private int m_CurrentIndex = -1;

			private bool m_TriedDirect;
		}
	}
}
