using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Serialization;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromObjectFactory]
	[Serializable]
	public abstract class TextAsset : ScriptableObject
	{
		public string version
		{
			get
			{
				return this.m_Version;
			}
			internal set
			{
				this.m_Version = value;
			}
		}

		public int instanceID
		{
			get
			{
				bool flag = this.m_InstanceID == 0;
				if (flag)
				{
					this.m_InstanceID = base.GetInstanceID();
				}
				return this.m_InstanceID;
			}
		}

		public int hashCode
		{
			get
			{
				bool flag = this.m_HashCode == 0;
				if (flag)
				{
					this.m_HashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
				}
				return this.m_HashCode;
			}
			set
			{
				this.m_HashCode = value;
			}
		}

		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		public int materialHashCode
		{
			get
			{
				bool flag = this.m_MaterialHashCode == 0;
				if (flag)
				{
					bool flag2 = this.m_Material == null;
					if (flag2)
					{
						return 0;
					}
					this.m_MaterialHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_Material.name);
				}
				return this.m_MaterialHashCode;
			}
			set
			{
				this.m_MaterialHashCode = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static TextAsset GetTextAssetByID(int id)
		{
			WeakReference<TextAsset> weakReference;
			TextAsset textAsset;
			bool flag = TextAsset.kTextAssetByInstanceId.TryGetValue(id, out weakReference) && weakReference.TryGetTarget(out textAsset);
			TextAsset textAsset2;
			if (flag)
			{
				textAsset2 = textAsset;
			}
			else
			{
				textAsset2 = null;
			}
			return textAsset2;
		}

		internal virtual void OnDestroy()
		{
			TextAsset.kTextAssetByInstanceId.Remove(this.instanceID);
		}

		internal virtual void OnEnable()
		{
			TextAsset.kTextAssetByInstanceId.TryAdd(this.instanceID, new WeakReference<TextAsset>(this));
		}

		[SerializeField]
		internal string m_Version;

		internal int m_InstanceID;

		internal int m_HashCode;

		[FormerlySerializedAs("material")]
		[SerializeField]
		internal Material m_Material;

		internal int m_MaterialHashCode;

		private static Dictionary<int, WeakReference<TextAsset>> kTextAssetByInstanceId = new Dictionary<int, WeakReference<TextAsset>>();
	}
}
