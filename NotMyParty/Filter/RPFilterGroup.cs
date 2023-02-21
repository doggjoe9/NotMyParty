using Dalamud.Game.Gui.PartyFinder.Types;
using System.Text.RegularExpressions;

namespace NotMyParty.Filter {
	internal class RPFilterGroup : GenericFilterGroup {
		private readonly Regex rpRegex = new Regex(@"(^|[^a-z])(e?)rp([^a-z]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public RPFilterGroup(bool enabled) : base(enabled) { }

		protected override string PlainTextIdentifier => "RP";

		protected override bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args)
			=> rpRegex.IsMatch(listing.Description.TextValue);
	}
}
