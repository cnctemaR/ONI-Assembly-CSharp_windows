using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class RenderPipelineGraphicsSettingsCollection
	{
		public List<IRenderPipelineGraphicsSettings> settingsList
		{
			get
			{
				return this.m_List;
			}
		}

		[SerializeReference]
		private List<IRenderPipelineGraphicsSettings> m_List = new List<IRenderPipelineGraphicsSettings>();
	}
}
