using System;
using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance
{
	[RequireDerived]
	public abstract class AdaptivePerformanceScaler : ScriptableObject
	{
		public virtual string Name
		{
			get
			{
				return this.m_defaultSetting.name;
			}
			set
			{
				bool flag = this.m_defaultSetting.name == value;
				if (!flag)
				{
					this.m_defaultSetting.name = value;
				}
			}
		}

		public virtual bool Enabled
		{
			get
			{
				return this.m_defaultSetting.enabled;
			}
			set
			{
				bool flag = this.m_defaultSetting.enabled == value;
				if (!flag)
				{
					this.m_defaultSetting.enabled = value;
				}
			}
		}

		public virtual float Scale
		{
			get
			{
				return this.m_defaultSetting.scale;
			}
			set
			{
				bool flag = this.m_defaultSetting.scale == value;
				if (!flag)
				{
					this.m_defaultSetting.scale = value;
				}
			}
		}

		public virtual ScalerVisualImpact VisualImpact
		{
			get
			{
				return this.m_defaultSetting.visualImpact;
			}
			set
			{
				bool flag = this.m_defaultSetting.visualImpact == value;
				if (!flag)
				{
					this.m_defaultSetting.visualImpact = value;
				}
			}
		}

		public virtual ScalerTarget Target
		{
			get
			{
				return this.m_defaultSetting.target;
			}
			set
			{
				bool flag = this.m_defaultSetting.target == value;
				if (!flag)
				{
					this.m_defaultSetting.target = value;
				}
			}
		}

		public virtual int MaxLevel
		{
			get
			{
				return this.m_defaultSetting.maxLevel;
			}
			set
			{
				bool flag = this.m_defaultSetting.maxLevel == value;
				if (!flag)
				{
					this.m_defaultSetting.maxLevel = value;
				}
			}
		}

		public virtual float MinBound
		{
			get
			{
				return this.m_defaultSetting.minBound;
			}
			set
			{
				bool flag = this.m_defaultSetting.minBound == value;
				if (!flag)
				{
					this.m_defaultSetting.minBound = value;
				}
			}
		}

		public virtual float MaxBound
		{
			get
			{
				return this.m_defaultSetting.maxBound;
			}
			set
			{
				bool flag = this.m_defaultSetting.maxBound == value;
				if (!flag)
				{
					this.m_defaultSetting.maxBound = value;
				}
			}
		}

		public int CurrentLevel { get; private set; }

		public bool IsMaxLevel
		{
			get
			{
				return this.CurrentLevel == this.MaxLevel;
			}
		}

		public bool NotLeveled
		{
			get
			{
				return this.CurrentLevel == 0;
			}
		}

		public int GpuImpact { get; internal set; }

		public int CpuImpact { get; internal set; }

		public int OverrideLevel
		{
			get
			{
				return this.m_OverrideLevel;
			}
			set
			{
				this.m_OverrideLevel = value;
				this.m_Indexer.UpdateOverrideLevel(this);
			}
		}

		public int CalculateCost()
		{
			PerformanceBottleneck performanceBottleneck = Holder.Instance.PerformanceStatus.PerformanceMetrics.PerformanceBottleneck;
			int num = 0;
			switch (this.VisualImpact)
			{
			case ScalerVisualImpact.Low:
				num += this.CurrentLevel;
				break;
			case ScalerVisualImpact.Medium:
				num += this.CurrentLevel * 2;
				break;
			case ScalerVisualImpact.High:
				num += this.CurrentLevel * 3;
				break;
			}
			bool flag = performanceBottleneck == PerformanceBottleneck.CPU && (this.Target & ScalerTarget.CPU) == (ScalerTarget)0;
			if (flag)
			{
				num = 6;
			}
			bool flag2 = performanceBottleneck == PerformanceBottleneck.GPU && (this.Target & ScalerTarget.GPU) == (ScalerTarget)0;
			if (flag2)
			{
				num = 6;
			}
			bool flag3 = performanceBottleneck == PerformanceBottleneck.TargetFrameRate && (this.Target & ScalerTarget.FillRate) == (ScalerTarget)0;
			if (flag3)
			{
				num = 6;
			}
			return num;
		}

		protected virtual void Awake()
		{
			bool flag = Holder.Instance == null;
			if (!flag)
			{
				this.m_Settings = Holder.Instance.Settings;
				this.m_Indexer = Holder.Instance.Indexer;
			}
		}

		private void OnEnable()
		{
			bool flag = this.m_Indexer == null;
			if (!flag)
			{
				this.m_Indexer.AddScaler(this);
				this.OnEnabled();
			}
		}

		private void OnDisable()
		{
			bool flag = this.m_Indexer == null;
			if (!flag)
			{
				this.m_Indexer.RemoveScaler(this);
				this.OnDisabled();
			}
		}

		internal void IncreaseLevel()
		{
			bool isMaxLevel = this.IsMaxLevel;
			if (isMaxLevel)
			{
				Debug.LogError("Cannot increase scaler level as it is already max.");
			}
			else
			{
				int currentLevel = this.CurrentLevel;
				this.CurrentLevel = currentLevel + 1;
				this.OnLevelIncrease();
				this.OnLevel();
			}
		}

		internal void DecreaseLevel()
		{
			bool notLeveled = this.NotLeveled;
			if (notLeveled)
			{
				Debug.LogError("Cannot decrease scaler level as it is already 0.");
			}
			else
			{
				int currentLevel = this.CurrentLevel;
				this.CurrentLevel = currentLevel - 1;
				this.OnLevelDecrease();
				this.OnLevel();
			}
		}

		internal void Activate()
		{
			this.OnEnabled();
		}

		internal void Deactivate()
		{
			this.OnDisabled();
		}

		public void ApplyDefaultSetting(AdaptivePerformanceScalerSettingsBase defaultSetting)
		{
			this.m_defaultSetting = defaultSetting;
		}

		protected bool ScaleChanged()
		{
			float scale = this.Scale;
			float num = (this.MaxBound - this.MinBound) / (float)this.MaxLevel;
			this.Scale = num * (float)(this.MaxLevel - this.CurrentLevel) + this.MinBound;
			return this.Scale != scale;
		}

		protected virtual void OnLevelIncrease()
		{
		}

		protected virtual void OnLevelDecrease()
		{
		}

		protected virtual void OnLevel()
		{
		}

		protected virtual void OnEnabled()
		{
		}

		protected virtual void OnDisabled()
		{
		}

		private AdaptivePerformanceIndexer m_Indexer;

		private int m_OverrideLevel = -1;

		private AdaptivePerformanceScalerSettingsBase m_defaultSetting = new AdaptivePerformanceScalerSettingsBase();

		protected IAdaptivePerformanceSettings m_Settings;
	}
}
