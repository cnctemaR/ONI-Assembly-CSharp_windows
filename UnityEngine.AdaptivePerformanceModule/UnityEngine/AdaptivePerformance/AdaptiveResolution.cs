using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveResolution : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveResolution);
			}
		}

		protected override void OnDisabled()
		{
			this.OnDestroy();
		}

		protected override void OnEnabled()
		{
		}

		private void OnValidate()
		{
			bool flag = this.MaxLevel < 1;
			if (flag)
			{
				this.MaxLevel = 1;
			}
			this.MaxBound = Mathf.Clamp(this.MaxBound, 0.25f, 1f);
			this.MinBound = Mathf.Clamp(this.MinBound, 0.25f, this.MaxBound);
		}

		private bool IsDynamicResolutionSupported()
		{
			return true;
		}

		private void Start()
		{
			AdaptiveResolution.instanceCount++;
			bool flag = AdaptiveResolution.instanceCount > 1;
			if (flag)
			{
				Debug.LogWarning("Multiple Adaptive Resolution scalers created. They will interfere with each other.");
			}
			bool flag2 = !this.IsDynamicResolutionSupported();
			if (flag2)
			{
				Debug.Log(string.Format("Dynamic resolution is not supported. Will be using fallback to Render Scale Multiplier.", Array.Empty<object>()));
			}
		}

		private void OnDestroy()
		{
			AdaptiveResolution.instanceCount--;
			bool flag = this.Scale == 1f;
			if (!flag)
			{
				APLog.Debug("Restoring dynamic resolution scale factor to 1.0", Array.Empty<object>());
				bool flag2 = this.IsDynamicResolutionSupported();
				if (flag2)
				{
					ScalableBufferManager.ResizeBuffers(1f, 1f);
				}
				else
				{
					AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 1f;
				}
			}
		}

		protected override void OnLevel()
		{
			bool flag = base.ScaleChanged();
			bool flag2 = this.IsDynamicResolutionSupported();
			if (flag2)
			{
				bool flag3 = flag;
				if (flag3)
				{
					ScalableBufferManager.ResizeBuffers(this.Scale, this.Scale);
				}
				int num = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * (float)Screen.currentResolution.width);
				int num2 = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * (float)Screen.currentResolution.height);
				APLog.Debug(string.Format("Adaptive Resolution Scale: {0:F3} Resolution: {1}x{2} ScaleFactor: {3:F3}x{4:F3} Level:{5}/{6}", new object[]
				{
					this.Scale,
					num,
					num2,
					ScalableBufferManager.widthScaleFactor,
					ScalableBufferManager.heightScaleFactor,
					base.CurrentLevel,
					this.MaxLevel
				}), Array.Empty<object>());
			}
			else
			{
				AdaptivePerformanceRenderSettings.RenderScaleMultiplier = this.Scale;
				APLog.Debug(string.Format("Dynamic resolution is not supported. Using fallback to Render Scale Multiplier : {0:F3}", this.Scale), Array.Empty<object>());
			}
		}

		private static int instanceCount;
	}
}
