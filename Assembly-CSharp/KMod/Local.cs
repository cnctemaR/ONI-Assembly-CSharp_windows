using System;
using System.IO;
using Klei;
using STRINGS;

namespace KMod
{
	public class Local : IDistributionPlatform
	{
		public string folder { get; private set; }

		public Label.DistributionPlatform distribution_platform { get; private set; }

		public string GetDirectory()
		{
			return FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), this.folder));
		}

		private void Subscribe(string directoryName, long timestamp, IFileSource file_source, bool isDevMod)
		{
			Label label = new Label
			{
				id = directoryName,
				distribution_platform = this.distribution_platform,
				version = (long)directoryName.GetHashCode(),
				title = directoryName
			};
			KModHeader header = KModUtil.GetHeader(file_source, label.defaultStaticID, directoryName, directoryName, isDevMod);
			label.title = header.title;
			Mod mod = new Mod(label, header.staticID, header.description, file_source, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_LOCAL_MOD, delegate
			{
				App.OpenWebURL("file://" + file_source.GetRoot());
			});
			if (file_source.GetType() == typeof(Directory))
			{
				mod.status = Mod.Status.Installed;
			}
			Global.Instance.modManager.Subscribe(mod, this);
		}

		public Local(string folder, Label.DistributionPlatform distribution_platform, bool isDevFolder)
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
				string name = directoryInfo2.Name;
				this.Subscribe(name, directoryInfo2.LastWriteTime.ToFileTime(), new Directory(directoryInfo2.FullName), isDevFolder);
			}
		}
	}
}
