using System;
using System.IO;
using Klei;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Local : IDistributionPlatform
	{
		public Local(string folder, Label.DistributionPlatform distribution_platform)
		{
			this.folder = folder;
			this.distribution_platform = distribution_platform;
			DirectoryInfo directoryInfo = new DirectoryInfo(this.GetDirectory());
			if (!directoryInfo.Exists)
			{
				return;
			}
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				string text = directoryInfo2.Name.ToLower();
				this.Subscribe(text, directoryInfo2.LastWriteTime.ToFileTime(), new Directory(directoryInfo2.FullName));
			}
		}

		public string folder { get; private set; }

		public Label.DistributionPlatform distribution_platform { get; private set; }

		public string GetDirectory()
		{
			return FSUtil.Normalize(Path.Combine(Manager.GetDirectory(), this.folder));
		}

		private void Subscribe(string id, long timestamp, IFileSource file_source)
		{
			string text = file_source.Read("mod.yaml");
			Local.Header header = ((!string.IsNullOrEmpty(text)) ? YamlIO<Local.Header>.Parse(text, null) : null);
			if (header == null)
			{
				header = new Local.Header
				{
					title = id,
					description = id
				};
			}
			Label label = new Label
			{
				id = id,
				distribution_platform = this.distribution_platform,
				version = (ulong)timestamp,
				title = header.title
			};
			Mod mod = new Mod(label, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, delegate
			{
				Application.OpenURL("file://" + file_source.GetRoot());
			});
			if (file_source.GetType() == typeof(Directory))
			{
				mod.status = Mod.Status.Installed;
			}
			Global.Instance.modManager.Subscribe(mod, this);
		}

		private class Header : YamlIO<Local.Header>
		{
			public string title { get; set; }

			public string description { get; set; }
		}
	}
}
