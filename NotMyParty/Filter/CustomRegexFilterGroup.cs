using Dalamud.Game.Gui.PartyFinder.Types;
using ImGuiNET;
using System.Text.RegularExpressions;

namespace NotMyParty.Filter {
	internal class CustomRegexFilterGroup : GenericFilterGroup {
		private Regex? customRegex;

		public CustomRegexFilterGroup(bool enabled) : base(enabled) { }

		protected override string PlainTextIdentifier => "ERRORCustomRegexFilterGroupYOUSHOULDNOTSEETHIS";

		protected override bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args) {
			return customRegex?.IsMatch(listing.Description.TextValue) ?? false;
		}

		public override void DrawConfiguration() {
			if (ImGui.Button((HideInstead ? "Hide" : "Filter") + "##" + PlainTextIdentifier))
				HideInstead = !HideInstead;
			ImGui.SameLine();
			ImGui.Text("using the following regular expression: ");
			ImGui.SameLine();
			string tmp = customRegex?.ToString() ?? "Insert Regex here...";
			if (ImGui.InputText("##inputtext" + PlainTextIdentifier, ref tmp, 32))
				customRegex = new Regex(tmp, RegexOptions.Compiled);
		}
	}
}
