﻿#region File Description
//-----------------------------------------------------------------------------
// TutorialScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Shoeter
{
	/// <summary>
	/// This screen implements the actual game logic. It is just a
	/// placeholder to get the idea across: you'll probably want to
	/// put some more interesting gameplay in here!
	/// </summary>
	class TutorialScreen : GameScreen
	{
		#region Fields

		ContentManager content;
		Texture2D backgroundTexture;

		float pauseAlpha;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public TutorialScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}


		/// <summary>
		/// Load graphics content for the game.
		/// </summary>
		public override void LoadContent()
		{
			if (content == null)
			{
				content = new ContentManager(ScreenManager.Game.Services, "Content");
			}

			backgroundTexture = content.Load<Texture2D>("Backgrounds/TutorialScreen");

			// A real game would probably have more content than this sample, so
			// it would take longer to load. We simulate that by delaying for a
			// while, giving you a chance to admire the beautiful loading screen.
			Thread.Sleep(1000);

			// once the load has finished, we use ResetElapsedTime to tell the game's
			// timing mechanism that we have just finished a very long frame, and that
			// it should not try to catch up.
			ScreenManager.Game.ResetElapsedTime();
		}


		/// <summary>
		/// Unload graphics content used by the game.
		/// </summary>
		public override void UnloadContent()
		{
			content.Unload();
		}


		#endregion

		#region Update and Draw


		/// <summary>
		/// Updates the state of the game. This method checks the GameScreen.IsActive
		/// property, so the game will stop updating when the pause menu is active,
		/// or if you tab away to a different application.
		/// </summary>
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
			{
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			}

			else
			{
				if (Level.currentLevel != -2)
				{
					Level.currentLevel = -2;
					MusicHandler.PlayMusic(-2, ref content);
				}

				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
			}
		}


		/// <summary>
		/// Lets the game respond to player input. Unlike the Update method,
		/// this will only be called when the gameplay screen is active.
		/// </summary>
		public override void HandleInput(InputState input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}

			// Look up inputs for the active player profile.
			int playerIndex = (int)ControllingPlayer.Value;

			if (input.IsPauseGame(ControllingPlayer))
			{
				ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
			}
		}


		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Our player and enemy are both actually just text strings.
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			//Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
			Rectangle fullscreen = new Rectangle(0, 0, 1280, 720);

			spriteBatch.Begin();

			spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				ScreenManager.FadeBackBufferToBlack(alpha);
			}
		}


		#endregion
	}
}