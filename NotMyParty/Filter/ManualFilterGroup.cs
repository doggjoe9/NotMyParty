using Dalamud.Game.Gui.PartyFinder.Types;
using System.Collections.Generic;

namespace NotMyParty.Filter {
	internal class ManualFilterGroup : GenericFilterGroup {
		private HashSet<string> awaitingHide = new();
		private Dictionary<string, string> selectedHides = new();

		public ManualFilterGroup(bool enabled) : base(enabled) {
			HideInstead = true;
		}

		protected override string PlainTextIdentifier => "user selected";

		protected override bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args) {
			string name = listing.Name.TextValue;

			if (awaitingHide.Contains(name)) {
				awaitingHide.Remove(name);
				selectedHides[name] = listing.Description.TextValue;
			}

			return selectedHides.ContainsKey(name) && selectedHides[name].Equals(listing.Description.TextValue);
		}

		public void HideEntry(string name) {
			awaitingHide.Add(name);
		}

		public override void Dispose() {
			base.Dispose();
			selectedHides = new();
		}
	}
}
