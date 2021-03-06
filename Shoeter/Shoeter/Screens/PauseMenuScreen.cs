#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Shoeter
{
	/// <summary>
	/// The pause menu comes up over the top of the game,
	/// giving the player options to resume or quit.
	/// </summary>
	class PauseMenuScreen : MenuScreen
	{
		#region Initialization

		/// <summary>
		/// Constructor.
		/// </summary>
		public PauseMenuScreen() : base("PauseMenu")
		{
			// Create our menu entries.
			MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
			MenuEntry quitGameMenuEntry = new MenuEntry("Return to Main Menu");
			
			// Hook up menu event handlers.
			resumeGameMenuEntry.Selected += OnCancel;
			quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

			resumeGameMenuEntry.currentScreenBeingUsed = "PauseMenu";
			quitGameMenuEntry.currentScreenBeingUsed = "PauseMenu";

			// Add entries to the menu.
			MenuEntries.Add(resumeGameMenuEntry);
			MenuEntries.Add(quitGameMenuEntry);

			Utilities.movementLockedDueToActivePauseScreen = true;
		}


		#endregion

		#region Handle Input


		/// <summary>
		/// Event handler for when the Quit Game menu entry is selected.
		/// </summary>
		void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			const string message = "Do you want to return to the Main Menu?";

			MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, false);

			confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

			ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
		}


		/// <summary>
		/// Event handler for when the user selects ok on the "are you sure
		/// you want to quit" message box. This uses the loading screen to
		/// transition from the game back to the main menu screen.
		/// </summary>
		void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
		{
			LoadingScreen.Load(ScreenManager, false, null, "Main Menu", new BackgroundScreen(), new MainMenuScreen());
			Utilities.movementLockedDueToActivePauseScreen = false;
		}


		#endregion
	}
}
