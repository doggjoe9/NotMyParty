using Dalamud.Game.Gui.PartyFinder.Types;

namespace NotMyParty {
	internal class HiddenEntry {
		public string Description { get; }
		public PartyFinderListingEventArgs Args { get; }

		public HiddenEntry(string description, PartyFinderListingEventArgs args) {
			Description = description;
			Args = args;
		}
	}
}
