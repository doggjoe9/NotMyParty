using Dalamud.ContextMenu;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using NotMyParty.Filter;

namespace NotMyParty {
	public sealed class NotMyParty : IDalamudPlugin {
		#region Fields
		public string Name => "Not My Party";
		private const string CommandName = "/nmp";

		private bool config = false;
		private bool debug = false;

		private readonly IFilterGroup[] filters = new IFilterGroup[] {
			new ManualFilterGroup(true),
			new MercenaryFilterGroup(false),
			new RPFilterGroup(false),
			new CustomRegexFilterGroup(false),
		};

		private bool[] enabledFlags;
		
		private readonly GameObjectContextMenuItem hideContextMenu;
		private readonly GameObjectContextMenuItem filterContextMenu;
		#endregion

		public NotMyParty(DalamudPluginInterface pluginInterface) {
			#region Service Init
			Services.Initialize(pluginInterface);
			#endregion
			#region Command Init
			Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				// TODO Put some helpful message here.
				HelpMessage = "Opens the main interface for NotMyParty"
			});
			#endregion

			enabledFlags = new bool[filters.Length];
			for (int i = 0; i < filters.Length; i++)
				enabledFlags[i] = filters[i].Enabled;

			hideContextMenu = new GameObjectContextMenuItem(new SeString(new TextPayload("Hide")), OnSelectHideContextMenuItem, true);
			filterContextMenu = new GameObjectContextMenuItem(new SeString(new TextPayload("Filter")), OnSelectHideContextMenuItem, true);

			Services.PartyFinderGui.ReceiveListing += OnPartyFinderListingOverride;
			Services.ContextMenu.OnOpenGameObjectContextMenu += OpenContextMenuOverride;
			Services.ChatGui.ChatMessage += ChatMessageOverride;
			Services.PluginInterface.UiBuilder.OpenConfigUi += OpenUI;
			Services.PluginInterface.UiBuilder.ShowUi += OpenUI;
			Services.PluginInterface.UiBuilder.HideUi += HideUI;
			Services.PluginInterface.UiBuilder.Draw += DrawUI;
		}

		private void DisposeAll() {
			foreach (IFilterGroup filter in filters)
				filter.Dispose();
		}

		public void Dispose() {
			Services.Configuration.Save();
			DisposeAll();
			Services.PluginInterface.UiBuilder.OpenConfigUi -= OpenUI;
			Services.PluginInterface.UiBuilder.ShowUi -= OpenUI;
			Services.PluginInterface.UiBuilder.HideUi -= HideUI;
			Services.PluginInterface.UiBuilder.Draw -= DrawUI;
			HideUI();
			Services.ChatGui.ChatMessage -= ChatMessageOverride;
			Services.PartyFinderGui.ReceiveListing -= OnPartyFinderListingOverride;
			Services.CommandManager.RemoveHandler(CommandName);
			Services.ContextMenu.OnOpenGameObjectContextMenu -= OpenContextMenuOverride;
			Services.ContextMenu.Dispose();
		}

		private void OpenUI() {
			config = !config;
		}

		private void HideUI() {
			config = false;
		}

		#region UI

		private void DrawUI() {
			if (config) {
				ImGui.Begin("NotMyParty", ref config, ImGuiWindowFlags.NoCollapse);
				ImGui.SetNextWindowSize(new Vector2(300, 500), ImGuiCond.FirstUseEver);

				

				if (ImGui.Button("Reset Hidden Items"))
					DisposeAll();

				for (int i = 0; i < filters.Length; i++) {
					IFilterGroup filter = filters[i];

					bool tmpEnabled = filter.Enabled;
					if (ImGui.Checkbox("##toggle" + i, ref tmpEnabled))
						filter.Enabled = tmpEnabled;

					ImGui.SameLine();
					ImGui.Indent();
					ImGui.SameLine();

					filter.DrawConfiguration();

					ImGui.Unindent();
				}

				ImGui.End();
			}
			
		}
		#endregion


		#region Command
		private void OnCommandDebug() {
			debug = !debug;
			string onoff = debug ? "on" : "off";
			Services.ChatGui.Print($"NotMyParty debug is now {onoff}");
		}

		private void OnCommandConfig() {
			if (config)
				HideUI();
			else
				OpenUI();
		}

		/// <summary>
		/// Executes when the player enters the command.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		private void OnCommand(string command, string args) {
			if (command == CommandName) {
				string[] argarray = args.Split(' ');
				string arg1 = argarray.Length > 0 ? argarray[0] : "";

				switch(arg1) {
					case "":
						OnCommandConfig();
						break;
					case "debug":
						OnCommandDebug();
						break;
					default:
						Services.ChatGui.PrintError($"{command} Unrecognized argument(s): {args}");
						break;
				}
				Services.ChatGui.UpdateQueue();
			}
		}
		#endregion

		#region Party Finder
		private void OnPartyFinderListingOverride(PartyFinderListing listing, PartyFinderListingEventArgs args) {
			foreach (IFilterGroup filter in filters)
				if (filter.CheckAndFilter(listing, args))
					break; // if one of them hid, then who cares what the rest think.
		}
		#endregion

		#region Context Menu
		private void OpenContextMenuOverride(GameObjectContextMenuOpenArgs args) {
			if (args.ParentAddonName != null && args.ParentAddonName.Equals("LookingForGroup") && filters[0].Enabled) {
				args.AddCustomItem(filters[0].HideInstead ? hideContextMenu : filterContextMenu);
			}
		}

		private void OnSelectHideContextMenuItem(GameObjectContextMenuItemSelectedArgs args) {
			if (args.Text != null)
				((ManualFilterGroup) filters[0]).HideEntry(args.Text.TextValue);
		}
		#endregion

		private void ChatMessageOverride(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled) {
			if (debug) {
				foreach (Payload payload in message.Payloads) {
					PluginLog.Debug(payload.ToString());
				}
			}
		}
	}
}
