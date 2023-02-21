using Dalamud.Game.Gui.PartyFinder.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
