#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Shoeter
{
	/// <summary>
	/// Helper class represents a single entry in a MenuScreen. By default this
	/// just draws the entry text string, but it can be customized to display menu
	/// entries in different ways. This also provides an event that will be raised
	/// when the menu entry is selected.
	/// </summary>
	class MenuEntry : Game
	{
		#region Fields

		/// <summary>
		/// The text rendered for this entry.
		/// </summary>
		string text;

		/// <summary>
		/// Tracks a fading selection effect on the entry.
		/// </summary>
		/// <remarks>
		/// The entries transition out of the selection effect when they are deselected.
		/// </remarks>
		float selectionFade;

		/// <summary>
		/// The position at which the entry is drawn. This is set by the MenuScreen
		/// each frame in Update.
		/// </summary>
		Vector2 position;

		/// <summary>
		/// The color of the text for the MenuEntry.
		/// </summary>
		public Color color;

		#endregion

		#region Properties


		/// <summary>
		/// Gets or sets the text of this menu entry.
		/// </summary>
		public string Text
		{
			get { return text; }
			set { text = value; }
		}


		/// <summary>
		/// Gets or sets the position at which to draw this menu entry.
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}


		#endregion

		#region Events


		/// <summary>
		/// Event raised when the menu entry is selected.
		/// </summary>
		public event EventHandler<PlayerIndexEventArgs> Selected;


		/// <summary>
		/// Method for raising the Selected event.
		/// </summary>
		protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
		{
			if (Selected != null)
			{
				Selected(this, new PlayerIndexEventArgs(playerIndex));
			}
		}


		#endregion

		#region Initialization

		//ContentManager content;
		public AnimatedSprite menuHead;
		public String currentScreenBeingUsed;

		/// <summary>
		/// Constructs a new menu entry with the specified text.
		/// </summary>
		public MenuEntry(string text)
		{
			this.text = text;
			this.color = Color.Black;
		}

		public void prepareToDraw(ContentManager contentManager, SpriteBatch spriteBatch, String screenBeingUsed)
		{
			menuHead = new AnimatedSprite(contentManager.Load<Texture2D>("Sprites/Guy Animations/Guy_Head_Menu_Spritesheet"), new Vector2(100, 100), 0, 48, 60, 17, spriteBatch, 100f, MathHelper.ToRadians(0));
			menuHead.RotatedRect = new RotatedRectangle(new Rectangle(100, 100, 46, 60), 0, AnimatedSprite.AnimationState.Guy_Falling_Right.ToString());
			currentScreenBeingUsed = screenBeingUsed;
		}

		#endregion

		#region Update and Draw


		/// <summary>
		/// Updates the menu entry.
		/// </summary>
		public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			// When the menu selection changes, entries gradually fade between
			// their selected and deselected appearance, rather than instantly
			// popping to the new state.
			float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

			if (isSelected)
			{
				selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
			}
			else
			{
				selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
			}

			if (isSelected && currentScreenBeingUsed == "MainMenu")
			{
				//menuHead.Position = new Vector2(position.X - 80, position.Y - 35);
				//menuHead.Animate(gameTime);
			}
		}

		/// <summary>
		/// Draws the menu entry. This can be overridden to customize the appearance.
		/// </summary>
		public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			// there is no such thing as a selected item on Windows Phone, so we always
			// force isSelected to be false
#if WINDOWS_PHONE
			isSelected = false;
#endif

			// Draw the selected entry in yellow, otherwise white. // ******* CHANGE PAUSE MENU COLORS AND FONT
			Color color = setMenuEntryColor(isSelected);

			// Pulsate the size of the selected menu entry.
			double time = gameTime.TotalGameTime.TotalSeconds;
			
			float pulsate = (float)Math.Sin(time * 1) + 1;

			float scale = 1 + pulsate * 0.05f * selectionFade;

			// Modify the alpha to fade text out during transitions.
			color *= screen.TransitionAlpha;

			// Draw text, centered on the middle of each line.
			ScreenManager screenManager = screen.ScreenManager;
			SpriteBatch spriteBatch = screenManager.SpriteBatch;
			SpriteFont font = screenManager.Font;

			if (currentScreenBeingUsed == "MainMenu")
			{
				font = screenManager.MainMenuFont;
			}
			else
			{
				font = screenManager.PauseMenuFont;
			}

			Vector2 origin = new Vector2(0, font.LineSpacing / 2);

			//spriteBatch.DrawString(font, text, new Vector2(position.X + 2, position.Y), (isSelected ? Color.Black : Color.White), 0, origin, (float)(scale + 0.05), SpriteEffects.None, 0); // Outline
			spriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0); // OG

			if (isSelected && currentScreenBeingUsed == "MainMenu")
			{
				//menuHead.Draw();
			}
		}

		private Color setMenuEntryColor(Boolean isSelected)
		{
			if (currentScreenBeingUsed == "PauseMenu")
			{
				return isSelected ? Color.YellowGreen : Color.DarkOliveGreen;
			}
			else
			{
				return isSelected ? Color.Bisque : Color.DimGray;
			}
		}


		/// <summary>
		/// Queries how much space this menu entry requires.
		/// </summary>
		public virtual int GetHeight(MenuScreen screen)
		{
			return screen.ScreenManager.Font.LineSpacing;
		}


		/// <summary>
		/// Queries how wide the entry is, used for centering on the screen.
		/// </summary>
		public virtual int GetWidth(MenuScreen screen)
		{
			return (int)screen.ScreenManager.Font.MeasureString(Text).X;
		}


		#endregion
	}
}
