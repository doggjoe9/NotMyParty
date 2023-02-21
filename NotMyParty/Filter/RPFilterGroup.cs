using Dalamud.Game.Gui.PartyFinder.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotMyParty.Filter {
	internal class RPFilterGroup : GenericFilterGroup {
		private readonly Regex rpRegex = new Regex(@"(^|[^a-z])(e?)rp([^a-z]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public RPFilterGroup(bool enabled) : base(enabled) { }

		protected override string PlainTextIdentifier => "RP";

		protected override bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args)
			=> rpRegex.IsMatch(listing.Description.TextValue);
	}
}
