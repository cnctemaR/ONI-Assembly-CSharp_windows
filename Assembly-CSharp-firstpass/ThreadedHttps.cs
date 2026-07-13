using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class ThreadedHttps<T> where T : class, new()
{
	public static T Instance
	{
		get
		{
			return ThreadedHttps<T>.Singleton.instance;
		}
	}

	public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		if (this.certFail)
		{
			return false;
		}
		this.certFail = true;
		string text = "";
		if (sslPolicyErrors == SslPolicyErrors.None)
		{
			this.certFail = false;
		}
		else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
		{
			this.certFail = false;
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				text = string.Concat(new string[]
				{
					text,
					"[",
					i.ToString(),
					"] ",
					chain.ChainStatus[i].Status.ToString(),
					"\n"
				});
				if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!chain.Build((X509Certificate2)certificate))
					{
						this.certFail = true;
					}
				}
			}
		}
		else
		{
			this.certFail = true;
		}
		if (this.certFail)
		{
			X509Certificate2 x509Certificate = new X509Certificate2(certificate);
			Debug.LogWarning(string.Concat(new string[]
			{
				this.serviceName,
				": ",
				sslPolicyErrors.ToString(),
				"\n",
				text,
				"\n",
				x509Certificate.ToString()
			}));
		}
		return !this.certFail;
	}

	private HttpClient GetOrCreateHttpClient()
	{
		if (this.httpClient == null)
		{
			HttpClientHandler httpClientHandler = new HttpClientHandler
			{
				AllowAutoRedirect = false,
				ServerCertificateCustomValidationCallback = (HttpRequestMessage message, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors) => this.RemoteCertificateValidationCallback(message, cert, chain, errors)
			};
			this.httpClient = new HttpClient(httpClientHandler);
		}
		return this.httpClient;
	}

	public void Start()
	{
		if (this.updateThread != null)
		{
			this.End();
		}
		if (this.certFail)
		{
			return;
		}
		this.packets = new List<byte[]>();
		this.shouldQuit = false;
		this.updateThread = new Thread(new ThreadStart(this.SendData));
		this.updateThread.Start();
	}

	public void End()
	{
		this.Quit();
		if (this.updateThread == null)
		{
			return;
		}
		if (this.updateThread != Thread.CurrentThread && !this.updateThread.Join(TimeSpan.FromSeconds(2.0)))
		{
			this.updateThread.Abort();
		}
		this.updateThread = null;
	}

	protected virtual void OnReplyRecieved(HttpResponseMessage response)
	{
	}

	protected string Send(byte[] byteArray, bool isForce = false)
	{
		HttpClient orCreateHttpClient = this.GetOrCreateHttpClient();
		string text = "";
		int num = 0;
		for (;;)
		{
			try
			{
				string text2 = "https://" + this.LIVE_ENDPOINT;
				HttpResponseMessage result = orCreateHttpClient.PostAsync(text2, new ByteArrayContent(byteArray)
				{
					Headers = 
					{
						ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
					}
				}).Result;
				text = result.ReasonPhrase;
				if (result.StatusCode != HttpStatusCode.OK)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					text = string.Concat(new string[] { this.serviceName, ": Server Responded with Status: [", text, "] Response: ", result2 });
				}
				else
				{
					this.OnReplyRecieved(result);
				}
				result.Dispose();
			}
			catch (Exception ex)
			{
				if (!this.shouldQuit)
				{
					if (this.certFail)
					{
						Debug.LogWarning(this.serviceName + ": Cert fail, quitting");
						try
						{
							this.OnReplyRecieved(null);
						}
						catch
						{
						}
						this.QuitOnError();
						break;
					}
					num++;
					if (num > this.RetryCount)
					{
						text = string.Concat(new string[]
						{
							DateTime.Now.ToLongTimeString(),
							" ",
							this.serviceName,
							": Max Retries (",
							this.RetryCount.ToString(),
							") reached. Disabling ",
							this.serviceName,
							"..."
						});
						Debug.LogWarning(text);
						try
						{
							this.OnReplyRecieved(null);
						}
						catch
						{
						}
						this.QuitOnError();
						break;
					}
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Pow(2.0, (double)(num + 3)));
					text = string.Concat(new string[]
					{
						DateTime.Now.ToLongTimeString(),
						" ",
						this.serviceName,
						": Exception (retrying in ",
						timeSpan.TotalSeconds.ToString(),
						" seconds): ",
						message,
						"\n",
						stackTrace
					});
					Debug.LogWarning(text);
					if (isForce)
					{
						Debug.LogWarning(ex.StackTrace);
						break;
					}
					Thread.Sleep(timeSpan);
				}
				continue;
			}
			break;
		}
		return text;
	}

	protected bool ShouldQuit()
	{
		bool flag = false;
		object quitLock = this._quitLock;
		lock (quitLock)
		{
			flag = (this.shouldQuit && this.packets.Count == 0) || this.quitOnError;
		}
		return flag;
	}

	protected void QuitOnError()
	{
		object quitLock = this._quitLock;
		lock (quitLock)
		{
			this.quitOnError = true;
			this.shouldQuit = true;
		}
	}

	protected void Quit()
	{
		object quitLock = this._quitLock;
		lock (quitLock)
		{
			this.shouldQuit = true;
		}
	}

	protected byte[] GetPacket()
	{
		byte[] array = null;
		List<byte[]> list = this.packets;
		lock (list)
		{
			if (this.packets.Count > 0)
			{
				array = this.packets[0];
				this.packets.RemoveAt(0);
			}
		}
		return array;
	}

	protected void PutPacket(byte[] packet, bool infront = false)
	{
		List<byte[]> list = this.packets;
		lock (list)
		{
			if (infront)
			{
				this.packets.Insert(0, packet);
			}
			else
			{
				this.packets.Add(packet);
				this._waitHandle.Set();
			}
		}
	}

	public void ForceSendData()
	{
		for (byte[] array = this.GetPacket(); array != null; array = this.GetPacket())
		{
			if (this.Send(array, true) != "OK")
			{
				this.PutPacket(array, true);
				return;
			}
		}
	}

	protected void SendData()
	{
		while (!this.ShouldQuit())
		{
			byte[] packet = this.GetPacket();
			if (packet != null)
			{
				if (this.Send(packet, false) != "OK")
				{
					this.PutPacket(packet, true);
				}
			}
			else
			{
				this._waitHandle.WaitOne();
			}
			if (this.singleSend)
			{
				return;
			}
		}
	}

	protected string serviceName;

	protected string CLIENT_KEY;

	protected string LIVE_ENDPOINT;

	private bool certFail;

	private HttpClient httpClient;

	protected int RetryCount = 3;

	protected Thread updateThread;

	protected List<byte[]> packets = new List<byte[]>();

	private EventWaitHandle _waitHandle = new AutoResetEvent(false);

	protected bool shouldQuit;

	protected bool quitOnError;

	private object _quitLock = new object();

	protected bool singleSend;

	private class Singleton
	{
		internal static readonly T instance = new T();
	}
}
