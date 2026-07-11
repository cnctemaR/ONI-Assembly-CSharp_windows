using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace System.Configuration
{
	[Serializable]
	public sealed class ConfigurationSectionCollection : NameObjectCollectionBase
	{
		internal ConfigurationSectionCollection(Configuration config, SectionGroupInfo group)
			: base(StringComparer.Ordinal)
		{
			this.config = config;
			this.group = group;
		}

		public override NameObjectCollectionBase.KeysCollection Keys
		{
			get
			{
				return this.group.Sections.Keys;
			}
		}

		public override int Count
		{
			get
			{
				return this.group.Sections.Count;
			}
		}

		public ConfigurationSection this[string name]
		{
			get
			{
				ConfigurationSection configurationSection = base.BaseGet(name) as ConfigurationSection;
				if (configurationSection == null)
				{
					SectionInfo sectionInfo = this.group.Sections[name] as SectionInfo;
					if (sectionInfo == null)
					{
						return null;
					}
					configurationSection = this.config.GetSectionInstance(sectionInfo, true);
					if (configurationSection == null)
					{
						return null;
					}
					object obj = ConfigurationSectionCollection.lockObject;
					lock (obj)
					{
						base.BaseSet(name, configurationSection);
					}
				}
				return configurationSection;
			}
		}

		public ConfigurationSection this[int index]
		{
			get
			{
				return this[this.GetKey(index)];
			}
		}

		public void Add(string name, ConfigurationSection section)
		{
			this.config.CreateSection(this.group, name, section);
		}

		public void Clear()
		{
			if (this.group.Sections != null)
			{
				foreach (object obj in this.group.Sections)
				{
					ConfigInfo configInfo = (ConfigInfo)obj;
					this.config.RemoveConfigInfo(configInfo);
				}
			}
		}

		public void CopyTo(ConfigurationSection[] array, int index)
		{
			for (int i = 0; i < this.group.Sections.Count; i++)
			{
				array[i + index] = this[i];
			}
		}

		public ConfigurationSection Get(int index)
		{
			return this[index];
		}

		public ConfigurationSection Get(string name)
		{
			return this[name];
		}

		public override IEnumerator GetEnumerator()
		{
			foreach (object obj in this.group.Sections.AllKeys)
			{
				string text = (string)obj;
				yield return this[text];
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		public string GetKey(int index)
		{
			return this.group.Sections.GetKey(index);
		}

		public void Remove(string name)
		{
			SectionInfo sectionInfo = this.group.Sections[name] as SectionInfo;
			if (sectionInfo != null)
			{
				this.config.RemoveConfigInfo(sectionInfo);
			}
		}

		public void RemoveAt(int index)
		{
			SectionInfo sectionInfo = this.group.Sections[index] as SectionInfo;
			this.config.RemoveConfigInfo(sectionInfo);
		}

		[MonoTODO]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		private SectionGroupInfo group;

		private Configuration config;

		private static readonly object lockObject = new object();
	}
}
