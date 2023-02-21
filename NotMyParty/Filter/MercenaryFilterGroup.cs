using Dalamud.Game.Gui.PartyFinder.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotMyParty.Filter {
	internal class MercenaryFilterGroup : GenericFilterGroup {
		private readonly Regex mercRegex = new Regex(@"(\b\d+[m]\b)|(([^a-z]|^)(mil|mill|million|gil)(\b|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public MercenaryFilterGroup(bool enabled) : base(enabled) { }
		protected override string PlainTextIdentifier => "mercenary";

		protected override bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args)
			=> listing.LootRules == LootRuleFlags.Lootmaster || mercRegex.IsMatch(listing.Description.TextValue);
	}
}
