using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipelineGlobalSettings : ScriptableObject, ISerializationCallbackReceiver
	{
		protected virtual List<IRenderPipelineGraphicsSettings> settingsList
		{
			get
			{
				Debug.LogWarning(string.Format("To be able to use {0} in your {1} you must override {2}", "IRenderPipelineGraphicsSettings", base.GetType(), "settingsList"));
				Debug.LogWarning(string.Format("Create your own '[{0}] List<{1}> m_Settings = new();' in your {2} and override {3} returning m_Settings;", new object[]
				{
					"SerializeReference",
					"IRenderPipelineGraphicsSettings",
					base.GetType(),
					"settingsList"
				}));
				return null;
			}
		}

		private Dictionary<Type, int> settingsMap { get; } = new Dictionary<Type, int>();

		private void RecreateSettingsMap()
		{
			this.settingsMap.Clear();
			bool flag = this.settingsList == null;
			if (!flag)
			{
				for (int i = 0; i < this.settingsList.Count; i++)
				{
					IRenderPipelineGraphicsSettings renderPipelineGraphicsSettings = this.settingsList[i];
					bool flag2 = renderPipelineGraphicsSettings == null;
					if (!flag2)
					{
						this.settingsMap[renderPipelineGraphicsSettings.GetType()] = i;
					}
				}
			}
		}

		protected internal bool TryGet(Type type, out IRenderPipelineGraphicsSettings settings)
		{
			settings = null;
			bool flag = this.settingsList == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num;
				bool flag3 = !this.settingsMap.TryGetValue(type, out num);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					settings = this.settingsList[num];
					flag2 = settings != null;
				}
			}
			return flag2;
		}

		protected internal bool TryGetFirstSettingsImplementingInterface<TSettingsInterfaceType>(out TSettingsInterfaceType settings) where TSettingsInterfaceType : class, IRenderPipelineGraphicsSettings
		{
			settings = default(TSettingsInterfaceType);
			bool flag = this.settingsList == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < this.settingsList.Count; i++)
				{
					TSettingsInterfaceType tsettingsInterfaceType = this.settingsList[i] as TSettingsInterfaceType;
					bool flag3 = tsettingsInterfaceType != null;
					if (flag3)
					{
						settings = tsettingsInterfaceType;
						return true;
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		protected internal bool GetSettingsImplementingInterface<TSettingsInterfaceType>(out List<TSettingsInterfaceType> settings) where TSettingsInterfaceType : class, IRenderPipelineGraphicsSettings
		{
			settings = new List<TSettingsInterfaceType>();
			bool flag = this.settingsList == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < this.settingsList.Count; i++)
				{
					TSettingsInterfaceType tsettingsInterfaceType = this.settingsList[i] as TSettingsInterfaceType;
					bool flag3 = tsettingsInterfaceType != null;
					if (flag3)
					{
						settings.Add(tsettingsInterfaceType);
					}
				}
				flag2 = settings.Count > 0;
			}
			return flag2;
		}

		protected internal bool Contains(Type type)
		{
			return this.settingsList != null && this.settingsMap.ContainsKey(type);
		}

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
			this.RecreateSettingsMap();
		}
	}
}
