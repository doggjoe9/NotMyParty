using Dalamud.Game.Gui.PartyFinder.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotMyParty.Filter {
	internal interface IFilterGroup : IDisposable {
		bool HideInstead { get; set; }

		bool Enabled { get; set; }

		void DrawConfiguration();

		/// <summary>
		/// Check if the given entry should be filtered out of the results, and hide it if it should be.
		/// </summary>
		/// <param name="listing"></param>
		/// <param name="args"></param>
		/// <returns>true if the entry was hidden</returns>
		bool CheckAndFilter(PartyFinderListing listing, PartyFinderListingEventArgs args);
	}
}
