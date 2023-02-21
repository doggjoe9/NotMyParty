using Dalamud.Game.Gui.PartyFinder.Types;
using ImGuiNET;
using System.Collections.Generic;

namespace NotMyParty.Filter {
	internal abstract class GenericFilterGroup : IFilterGroup {
		protected Dictionary<string, HiddenEntry> hiddenEntries = new();

		protected bool enabled;
		protected bool hideInstead = false;

		public GenericFilterGroup(bool enabled) {
			this.enabled = enabled;
		}

		public bool HideInstead {
			get {
				return hideInstead;
			}
			set {
				bool wasEnabled = enabled;
				Enabled = false;
				hideInstead = value;
				if (wasEnabled)
					Enabled = true;
			}
		}

		public bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
				if (!value)
					foreach (HiddenEntry entry in hiddenEntries.Values)
						entry.Args.Visible = true;
			}
		}

		protected abstract string PlainTextIdentifier { get; }

		protected abstract bool Predicate(PartyFinderListing listing, PartyFinderListingEventArgs args);

		public virtual void DrawConfiguration() {
			if (ImGui.Button((HideInstead ? "Hide" : "Filter for") + "##" + PlainTextIdentifier))
				HideInstead = !HideInstead;
			ImGui.SameLine();
			ImGui.Text($" {PlainTextIdentifier} listings.");
		}

		public bool CheckAndFilter(PartyFinderListing listing, PartyFinderListingEventArgs args) {
			if (enabled) {
				bool predicate = Predicate(listing, args);

				if (HideInstead)
					predicate = !predicate;

				if (!predicate) {
					args.Visible = false;

					string name = listing.Name.TextValue;
					HiddenEntry newEntry = new HiddenEntry(listing.Description.TextValue, args);
					hiddenEntries![name] = newEntry;

					return true;
				}
			}

			return false;
		}

		public virtual void Dispose() {
			Enabled = false;
			hiddenEntries = new();
		}
	}
}
