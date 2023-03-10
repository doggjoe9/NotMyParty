using Dalamud.Configuration;
using System;

namespace NotMyParty {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public int Version { get; set; } = 0;

		public Configuration() { }

		public void Save() {
			Services.PluginInterface.SavePluginConfig(this);
		}
	}
}
