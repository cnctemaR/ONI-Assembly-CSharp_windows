using System;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Bank Loader")]
	public class StudioBankLoader : MonoBehaviour
	{
		private void HandleGameEvent(LoaderGameEvent gameEvent)
		{
			if (this.LoadEvent == gameEvent)
			{
				this.Load();
			}
			if (this.UnloadEvent == gameEvent)
			{
				this.Unload();
			}
		}

		private void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			this.HandleGameEvent(LoaderGameEvent.ObjectStart);
		}

		private void OnApplicationQuit()
		{
			this.isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!this.isQuitting)
			{
				this.HandleGameEvent(LoaderGameEvent.ObjectDestroy);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(LoaderGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(LoaderGameEvent.TriggerExit);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(LoaderGameEvent.TriggerEnter2D);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(LoaderGameEvent.TriggerExit2D);
			}
		}

		private void OnEnable()
		{
			this.HandleGameEvent(LoaderGameEvent.ObjectEnable);
		}

		private void OnDisable()
		{
			this.HandleGameEvent(LoaderGameEvent.ObjectDisable);
		}

		public void Load()
		{
			foreach (string text in this.Banks)
			{
				try
				{
					RuntimeManager.LoadBank(text, this.PreloadSamples);
				}
				catch (BankLoadException ex)
				{
					RuntimeUtils.DebugLogException(ex);
				}
			}
			if (this.PreloadSamples)
			{
				RuntimeManager.WaitForAllSampleLoading();
			}
		}

		public void Unload()
		{
			foreach (string text in this.Banks)
			{
				RuntimeManager.UnloadBank(text);
			}
		}

		public LoaderGameEvent LoadEvent;

		public LoaderGameEvent UnloadEvent;

		[BankRef]
		public List<string> Banks;

		public string CollisionTag;

		public bool PreloadSamples;

		private bool isQuitting;
	}
}
