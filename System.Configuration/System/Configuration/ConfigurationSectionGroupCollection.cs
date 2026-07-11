using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Configuration
{
	[Serializable]
	public sealed class ConfigurationSectionGroupCollection : NameObjectCollectionBase
	{
		internal ConfigurationSectionGroupCollection(Configuration config, SectionGroupInfo group)
			: base(StringComparer.Ordinal)
		{
			this.config = config;
			this.group = group;
		}

		public override NameObjectCollectionBase.KeysCollection Keys
		{
			get
			{
				return this.group.Groups.Keys;
			}
		}

		public override int Count
		{
			get
			{
				return this.group.Groups.Count;
			}
		}

		public ConfigurationSectionGroup this[string name]
		{
			get
			{
				ConfigurationSectionGroup configurationSectionGroup = base.BaseGet(name) as ConfigurationSectionGroup;
				if (configurationSectionGroup == null)
				{
					SectionGroupInfo sectionGroupInfo = this.group.Groups[name] as SectionGroupInfo;
					if (sectionGroupInfo == null)
					{
						return null;
					}
					configurationSectionGroup = this.config.GetSectionGroupInstance(sectionGroupInfo);
					base.BaseSet(name, configurationSectionGroup);
				}
				return configurationSectionGroup;
			}
		}

		public ConfigurationSectionGroup this[int index]
		{
			get
			{
				return this[this.GetKey(index)];
			}
		}

		public void Add(string name, ConfigurationSectionGroup sectionGroup)
		{
			this.config.CreateSectionGroup(this.group, name, sectionGroup);
		}

		public void Clear()
		{
			if (this.group.Groups != null)
			{
				foreach (object obj in this.group.Groups)
				{
					ConfigInfo configInfo = (ConfigInfo)obj;
					this.config.RemoveConfigInfo(configInfo);
				}
			}
		}

		public void CopyTo(ConfigurationSectionGroup[] array, int index)
		{
			for (int i = 0; i < this.group.Groups.Count; i++)
			{
				array[i + index] = this[i];
			}
		}

		public ConfigurationSectionGroup Get(int index)
		{
			return this[index];
		}

		public ConfigurationSectionGroup Get(string name)
		{
			return this[name];
		}

		public override IEnumerator GetEnumerator()
		{
			return this.group.Groups.AllKeys.GetEnumerator();
		}

		public string GetKey(int index)
		{
			return this.group.Groups.GetKey(index);
		}

		public void Remove(string name)
		{
			SectionGroupInfo sectionGroupInfo = this.group.Groups[name] as SectionGroupInfo;
			if (sectionGroupInfo != null)
			{
				this.config.RemoveConfigInfo(sectionGroupInfo);
			}
		}

		public void RemoveAt(int index)
		{
			SectionGroupInfo sectionGroupInfo = this.group.Groups[index] as SectionGroupInfo;
			this.config.RemoveConfigInfo(sectionGroupInfo);
		}

		[MonoTODO]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		internal ConfigurationSectionGroupCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SectionGroupInfo group;

		private Configuration config;
	}
}
