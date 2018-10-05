using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using GitUIPluginInterfaces;
using OpenSolution.Properties;
using ResourceManager;

namespace OpenSolution
{
	[Export(typeof(IGitPlugin))]
	public sealed class OpenSolutionPlugin : GitPluginBase, IGitPluginForRepository
	{
		private readonly StringSetting _preferedSolutonsSetting = new StringSetting("Prefered files", "");

		public OpenSolutionPlugin()
		{
			SetNameAndDescription("Open Default Solution");
			Translate();
			Icon = Resources.IconFindLargeFiles;
		}

		public override IEnumerable<ISetting> GetSettings()
		{
			yield return _preferedSolutonsSetting;
		}

		public override bool Execute(GitUIEventArgs args)
		{
			var workingDir = args.GitModule.WorkingDir;

			string slnFilePath;
			var foundSlnFile = TryFindSlnFile(workingDir, out slnFilePath);
			if (foundSlnFile)
				System.Diagnostics.Process.Start(slnFilePath);

			return foundSlnFile;
		}

		private bool TryFindSlnFile(string dirPath, out string slnFilePath)
		{
			var files = Directory.GetFiles(dirPath, "*.sln");

			slnFilePath = string.Empty;

			string defaultFile = string.Empty;

			var preferedSOlutions = _preferedSolutonsSetting.ValueOrDefault(Settings).Split(',', ';', '|');

			foreach (var preferedSolution in preferedSOlutions)
			{
				var file = files.FirstOrDefault(f => f.Contains(preferedSolution));

				defaultFile = file;

				if (string.IsNullOrEmpty(file))
					continue;

				slnFilePath = file;

				return true;
			}

			if (string.IsNullOrEmpty(defaultFile))
				return false;

			slnFilePath = defaultFile;

			return true;
		}
	}
}