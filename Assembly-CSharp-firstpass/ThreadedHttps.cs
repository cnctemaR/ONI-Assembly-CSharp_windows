using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
		string text = string.Empty;
		if (sslPolicyErrors == SslPolicyErrors.None)
		{
			this.certFail = false;
		}
		else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
		{
			this.certFail = false;
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"[",
					i,
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
			}), null);
		}
		return !this.certFail;
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
		if (!this.updateThread.Join(TimeSpan.FromSeconds(2.0)))
		{
			this.updateThread.Abort();
		}
		this.updateThread = null;
	}

	protected virtual void OnReplyRecieved(WebResponse response)
	{
	}

	protected string Send(byte[] byteArray, bool isForce = false)
	{
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(this.RemoteCertificateValidationCallback));
		string text = string.Empty;
		int num = 0;
		for (;;)
		{
			try
			{
				string text2 = "https://" + this.LIVE_ENDPOINT;
				Stream stream = null;
				WebResponse webResponse = null;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text2);
				httpWebRequest.AllowAutoRedirect = false;
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = (long)byteArray.Length;
				try
				{
					stream = httpWebRequest.GetRequestStream();
				}
				catch (WebException ex)
				{
					string message = ex.Message;
					text = string.Concat(new string[]
					{
						DateTime.Now.ToLongTimeString(),
						" ",
						this.serviceName,
						": Exception getting Request Stream:",
						message
					});
					Debug.LogWarning(text, null);
					throw;
				}
				try
				{
					stream.Write(byteArray, 0, byteArray.Length);
				}
				catch (WebException ex2)
				{
					string message2 = ex2.Message;
					text = string.Concat(new string[]
					{
						DateTime.Now.ToLongTimeString(),
						" ",
						this.serviceName,
						": Exception writing data to Stream:",
						message2
					});
					Debug.LogWarning(text, null);
					throw;
				}
				stream.Close();
				try
				{
					webResponse = httpWebRequest.GetResponse();
				}
				catch (WebException ex3)
				{
					string message3 = ex3.Message;
					WebResponse response = ex3.Response;
					if (response != null)
					{
						using (Stream responseStream = response.GetResponseStream())
						{
							StreamReader streamReader = new StreamReader(responseStream);
							text = streamReader.ReadToEnd();
						}
					}
					else
					{
						text = " -- we.Response is NULL";
					}
					text = string.Concat(new string[]
					{
						DateTime.Now.ToLongTimeString(),
						" ",
						this.serviceName,
						": Exception getting response:",
						message3,
						text
					});
					Debug.LogWarning(text, null);
					throw;
				}
				text = ((HttpWebResponse)webResponse).StatusDescription;
				if (text != "OK")
				{
					stream = webResponse.GetResponseStream();
					StreamReader streamReader2 = new StreamReader(stream);
					string text3 = streamReader2.ReadToEnd();
					streamReader2.Close();
					stream.Close();
					text = string.Concat(new string[]
					{
						string.Empty,
						this.serviceName,
						": Server Responded with Status: [",
						text,
						"] Response: ",
						text3
					});
				}
				else
				{
					this.OnReplyRecieved(webResponse);
				}
				webResponse.Close();
				break;
			}
			catch (Exception ex4)
			{
				if (!this.shouldQuit)
				{
					if (this.certFail)
					{
						Debug.LogWarning(this.serviceName + ": Cert fail, quitting", null);
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
					if (num > 3)
					{
						text = string.Concat(new object[]
						{
							DateTime.Now.ToLongTimeString(),
							" ",
							this.serviceName,
							": Max Retries (",
							3,
							") reached. Disabling ",
							this.serviceName,
							"..."
						});
						Debug.LogWarning(text, null);
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
					string message4 = ex4.Message;
					string stackTrace = ex4.StackTrace;
					TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Pow(2.0, (double)(num + 3)));
					text = string.Concat(new object[]
					{
						DateTime.Now.ToLongTimeString(),
						" ",
						this.serviceName,
						": Exception (retrying in ",
						timeSpan.TotalSeconds,
						" seconds): ",
						message4,
						"\n",
						stackTrace
					});
					Debug.LogWarning(text, null);
					if (isForce)
					{
						Debug.LogWarning(ex4.StackTrace, null);
						break;
					}
					Thread.Sleep(timeSpan);
				}
			}
		}
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(this.RemoteCertificateValidationCallback));
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
		object obj = this.packets;
		lock (obj)
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
		object obj = this.packets;
		lock (obj)
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

	private const int retryCount = 3;

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
