﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shoeter
{
	public abstract class Character
	{
		protected SpriteBatch spriteBatch;
		protected GameTime gameTime;

		// Animation
		public AnimatedSprite sprite;
		Rectangle sourceRect;   // The rectangle in which the animated sprite will be drawn.
		float timer = 0f;       // The amount of time it takes before the sprite moves to the next frame.
		float interval = 100f;  // The amount of time a frame is shown on screen.
		protected int currentFrame = 0;   // The current frame we are drawing.
		protected int totalFrames = 0;    // Stores the total amount of frames in the sprite sheet.

		// Character information
		protected Texture2D spriteTexture;      // The image of our animated sprite.
		protected Vector2 position;             // The position of the animated sprite.
		Vector2 center;                         // The center of the animated sprite.
		public Vector2 velocity;				// The velocity that changes the player's position.
		SpriteEffects facingRight;              // Stores if the player is facing right or not.
		protected bool isFalling = false;       // Stores if the player is falling or not.
		protected bool isJumping = false;       // Stores if we're jumping or not.

		public float gravity;                   // Gravity CHANGE BACK TO PROTECTED LATER

		protected float spriteSpeed = 600f;     // This is how fast the sprite moves.
		public int spriteWidth;              // The width of the individual sprite.
		public int spriteHeight;             // The height of the individual sprite.

		// State
		public State state;         // The current state of the player.
		public State oldState;      // The old state of the player.
		public Mode currentMode;    // The current mode of the player.

		// Collision
		Rectangle futurePositionRec;    // The player's future position rectangle.
		Rectangle positionRect;         // The rectangle around the player.
		Rectangle tileCollRect;         // The current tile's position the player is colliding with.
		Vector2 tileArrayCoordinates;   // The current tile's coordinates into the level array.
		Tile currentTileCollidingWith;	// The current tile that the Character is currently colliding with.
		Tile previousTileCollidingWith;	// The previous tile that the Character collided with.

		// Important: Be sure to reset the appropriate border collision variable to false after it has been found that is has been set to true.
		// Flag that says whether or not the Character has collided with that particular border of the screen.
		protected bool didCharacterCollideWithLeftBorderOfScreen = false;
		protected bool didCharacterCollideWithRightBorderOfScreen = false;
		protected bool didCharacterCollideWithTopBorderOfScreen = false;
		protected bool didCharacterCollideWithBottomBorderOfScreen = false;

		// Window Information
		protected int screenHeight;
		protected int screenWidth;
		public static string charDebug;

		/// <summary>
		/// Property for the position of the Character.
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public int SpriteWidth
		{
			get { return spriteWidth; }
			set { spriteWidth = value; }
		}

		public int SpriteHeight
		{
			get { return spriteWidth; }
			set { spriteWidth = value; }
		}

		/// <summary>
		/// Property for the center of the sprite.
		/// </summary>
		public Vector2 Center
		{
			get { return center; }
			set { center = value; }
		}

		/// <summary>
		/// Property for the Texture of the Character.
		/// </summary>
		public Texture2D Texture
		{
			get { return spriteTexture; }
			set { spriteTexture = value; }
		}

		/// <summary>
		/// Property for the Rectangle that is drawn around the Character.
		/// </summary>
		public Rectangle SourceRect
		{
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>
		/// Property for the Rectangle that represents the position of the Character.
		/// </summary>
		public Rectangle PositionRect
		{
			get { return positionRect; }
			set { positionRect = value; }
		}

		/// <summary>
		/// Property for the Tile Source Rectangle that the Character has collided with.
		/// </summary>
		public Rectangle TileCollisionRectangle
		{
			get { return tileCollRect; }
			set { tileCollRect = value; }
		}

		/// <summary>
		/// Property for the Tile that the Character is currently colliding with.
		/// </summary>
		public Tile CurrentCollidingTile
		{
			get { return currentTileCollidingWith; }
			set { currentTileCollidingWith = value; }
		}

		/// <summary>
		/// Property for the Tile that the Character was previously colliding with.
		/// </summary>
		public Tile PreviousCollidingTile
		{
			get { return previousTileCollidingWith; }
			set { previousTileCollidingWith = value; }
		}

		/// <summary>
		/// Property for the Future Rectangle position of the Chracter.
		/// </summary>
		public Rectangle FutureRectangleRect
		{
			get { return futurePositionRec; }
			set { futurePositionRec = value; }
		}

		/// <summary>
		/// Property for the coodinates of the Tile in the Level that the Character has collided with.
		/// </summary>
		public Vector2 TileArrayCoordinates
		{
			get { return tileArrayCoordinates; }
			set { tileArrayCoordinates = value; }
		}

		/// <summary>
		/// The possible states a Character can be in.
		/// </summary>
		public enum State
		{
			Idle_Left = 0,
			Idle_Right = 1,
			Running_Right = 2,
			Running_Left = 3,
			Jumping = 4,
			Decending = 5,
		}

		/// <summary>
		/// Property for the State of the Character.
		/// </summary>
		public State CurrentState
		{
			get { return state; }
			set { state = value; }
		}

		/// <summary>
		/// These are the modes the player can be in.
		/// </summary>
		public enum Mode
		{
			Guy = 0,
			Shoes = 1,
		}

		/// <summary>
		/// Property for the Mode of the Character.
		/// </summary>
		public Mode PlayerMode
		{
			get { return currentMode; }
			set { currentMode = value; }
		}

		/// <summary>
		/// Stores if the sprite is facing right or not.
		/// </summary>
		public SpriteEffects FacingRight
		{
			get { return facingRight; }
			set { facingRight = value; }
		}

		/// <summary>
		/// Stores the total amount of frames in the current sprite sheet.
		/// </summary>
		public int TotalFrames
		{
			get { return totalFrames; }
			set { totalFrames = value; }
		}

		/// <summary>
		/// The Animated Sprite that represents the Character.
		/// </summary>
		public AnimatedSprite Sprite
		{
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>
		/// Constructor for the Character class.
		/// </summary>
		public Character() 
		{ 
			charDebug = "";
		}

		/// <summary>
		/// Determines if there is a Tile below the Character.
		/// </summary>
		/// <returns>A boolean saying whether or not there is a Tile below the Character.</returns>
		protected bool standingOnGround()
		{
			updateRectangles(0, 1);

			for (int i = 0; i < Level.impassableTileRecs.Count; i++)
			{
				if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
				{
					position.Y = Level.impassableTilePos[i].Y - Sprite.RotatedRect.Height;
					updateRectangles(0, -1);
					tileCollRect = Level.impassableTileRecs[i];
					setTileArrayCoordinates(Level.impassableTilePos[i].X, Level.impassableTilePos[i].Y);

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets the tile coordinates (between (0-45, 0-80)) of the tile at (xPosition, yPosition)
		/// </summary>
		/// <param name="xPosition">X coordinate of the tile in the level.</param>
		/// <param name="yPosition">Y coordinate of the tile in the level.</param>
		public void setTileArrayCoordinates(float xPosition, float yPosition)
		{
			for (int x = 0; x < Level.numberOfTileColumns; x++)
			{
				for (int y = 0; y < Level.numberOfTilesInRow; y++)
				{
					if (Level.tiles[x, y].Position.X == xPosition && Level.tiles[x, y].Position.Y == yPosition)
					{
						TileArrayCoordinates = new Vector2(x, y);
					}
				}
			}
		}

		/// <summary>
		/// Determines if there is a Tile above the Chracter.
		/// </summary>
		/// <returns>A boolean saying whether or not there is a Tile above the Character.</returns>
		public bool underTile()
		{
			updateRectangles(0, -1);

			for (int i = 0; i < Level.impassableTileRecs.Count; i++)
			{
				if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
				{
					tileCollRect = Level.impassableTileRecs[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines if there is a Tile to the right of the Chracter.
		/// </summary>
		/// <returns>A boolean saying whether or not there is a Tile to the right the Character.</returns>
		public bool tileToTheRight()
		{
			updateRectangles(1, 0);

			for (int i = 0; i < Level.impassableTileRecs.Count; i++)
			{
				if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
				{
					tileCollRect = Level.impassableTileRecs[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines if there is a Tile to the left the Chracter.
		/// </summary>
		/// <returns>A boolean saying whether or not there is a Tile to the left of the Character.</returns>
		public bool tileToTheLeft()
		{
			updateRectangles(-1, 0);

			for (int i = 0; i < Level.impassableTileRecs.Count; i++)
			{
				if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
				{
					tileCollRect = Level.impassableTileRecs[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Updates the Characters bounding Rectangles, depending on where the parameters specify the Rectangles to be shifted to.
		/// </summary>
		/// <param name="xOffset">Offset to modify the X coordinate of the future Rectangle.</param>
		/// <param name="yOffset">Offset to modify the Y coordinate of the future Rectangle.</param>
		/// <remarks>The offsets are used so that it is always known what's "in front" of the Character. Used for collision detection.</remarks>
		protected void updateRectangles(int xOffset, int yOffset)
		{
			Vector2 shiftedHitboxPosition = getShiftedPositionOfHitbox();

			RotatedRectangle oldHbox = Sprite.RotatedRect;
			Sprite.RotatedRect = new RotatedRectangle(new Rectangle((int)position.X, (int)position.Y, oldHbox.Width, oldHbox.Height), oldHbox.Rotation, oldHbox.Tag);
			Sprite.RotatedRect.PreviousTag = oldHbox.PreviousTag;
			futurePositionRec = new Rectangle((int)Sprite.RotatedRect.X + xOffset, (int)Sprite.RotatedRect.Y + yOffset, Math.Abs(Sprite.RotatedRect.Right - Sprite.RotatedRect.Left), Math.Abs(Sprite.RotatedRect.Top - Sprite.RotatedRect.Bottom));
		}

		private Vector2 getShiftedPositionOfHitbox()
		{
			if (Sprite.RotatedRect.Tag == AnimatedSprite.AnimationState.Guy_BeingShot_Left.ToString())
			{
				return new Vector2(position.X, position.Y);
			}
			else if (Sprite.RotatedRect.Tag == AnimatedSprite.AnimationState.Guy_BeingShot_Right.ToString())
			{
				return new Vector2(position.X, position.Y);
			}
			else
			{
				return new Vector2(position.X, position.Y);
			}
		}

		/// <summary>
		/// Changes the State of the Character.
		/// </summary>
		/// <param name="newState">The new State of the Character.</param>
		protected void changeState(State newState)
		{
			// Only change the state if it's a new state.
			if (newState != state)
			{
				oldState = state;
				state = newState;
			}
		}

		/// <summary>
		/// Checks to see if a future position change will result in a collision with a Tile.
		/// </summary>
		/// <param name="potentialState">The State that the Character is in.</param>
		/// <remarks>When a collision is detected, execution is shifted to specializedCollision. This is an overloaded method for a derived class.</remarks>
		protected void handleCollisions(State potentialState)
		{
			int leftTile = (int)Math.Floor((float)Sprite.RotatedRect.Left / Level.impassableTileRecs[0].Width);
			int rightTile = (int)Math.Ceiling(((float)Sprite.RotatedRect.Right / Level.impassableTileRecs[0].Width)) - 1;
			int topTile = (int)Math.Floor((float)Sprite.RotatedRect.Top / Level.impassableTileRecs[0].Height);
			int bottomTile = (int)Math.Ceiling(((float)Sprite.RotatedRect.Bottom / Level.impassableTileRecs[0].Height)) - 1;

			for (int y = topTile; y <= bottomTile; ++y)
			{
				for (int x = leftTile; x <= rightTile; ++x)
				{
					// Keeps the player in bounds of the screen.
					if (x < 0)
					{
						velocity.X = 0.0f;
						position.X = 0f;
						didCharacterCollideWithLeftBorderOfScreen = true;
					}
					else if (x > 79)
					{
						position.X = screenWidth - Sprite.RotatedRect.Width;
						velocity.X = 0f;
						didCharacterCollideWithRightBorderOfScreen = true;
					}
					else if (y < 0)
					{
						position.Y = 0f;
						velocity.Y = 0f;
						didCharacterCollideWithTopBorderOfScreen = true;
					}
					else if (y > 44)
					{
						//velocity.Y = 0f;
					}
					else if (futurePositionRec.Intersects(Level.tiles[y, x].SourceRect) && (Level.tiles[y, x].CollProperties == Tile.CollisionProperty.Impassable || Level.tiles[y, x].IsAirCannonSwitch))
					{
						setFlagsForBorderCollision(false);
						if (potentialState == State.Running_Right)
						{
							specializedCollision(potentialState, y, x);
							updateRectangles(1, 0);
							tileCollRect = Level.tiles[y, x].SourceRect;
						}
						else if (potentialState == State.Running_Left)
						{
							specializedCollision(potentialState, y, x);
							updateRectangles(-1, 0);
							tileCollRect = Level.tiles[y, x].SourceRect;
						}
						else if (potentialState == State.Jumping)
						{
							specializedCollision(potentialState, y, x);
							updateRectangles(0, 1);
							tileCollRect = Level.tiles[y, x].SourceRect;
						}
						else if (potentialState == State.Decending)
						{
							specializedCollision(potentialState, y, x);
							updateRectangles(0, -1);
							tileCollRect = Level.tiles[y, x].SourceRect;
						}
					}
				}
			}
		}

		/// <summary>
		/// This method is overloaded for derived classes. Used to handle collisions on a class by class basis (i.e. for the Shoes and Guy).
		/// </summary>
		/// <param name="currentState">The current State of the Character.</param>
		/// <param name="y">The Y coordinate of the tile in the Level that the Character has collided with.</param>
		/// <param name="x">The X coordinate of the tile in the Level that the Character has collided with.</param>
		protected abstract void specializedCollision(State currentState, int y, int x);

		/// <summary>
		/// Sets the flags for collisions with the border of the screen to true or false.
		/// </summary>
		/// <param name="setToTrue">Flag that says to set the border flags to true or false.</param>
		protected void setFlagsForBorderCollision(bool setToTrue)
		{
			if (setToTrue)
			{
				didCharacterCollideWithLeftBorderOfScreen = true;
				didCharacterCollideWithRightBorderOfScreen = true;
				didCharacterCollideWithTopBorderOfScreen = true;
				didCharacterCollideWithBottomBorderOfScreen = true;
			}
			else
			{
				didCharacterCollideWithLeftBorderOfScreen = false;
				didCharacterCollideWithRightBorderOfScreen = false;
				didCharacterCollideWithTopBorderOfScreen = false;
				didCharacterCollideWithBottomBorderOfScreen = false;
			}
		}

		/// <summary>
		/// Sets the current and previous Tile that the Character is colliding with.
		/// </summary>
		protected void setCurrentAndPreviousCollisionTiles()
		{
			PreviousCollidingTile = CurrentCollidingTile;

			int leftTile = (int)Math.Floor((float)Sprite.RotatedRect.Left / Level.impassableTileRecs[0].Width);
			int rightTile = (int)Math.Ceiling(((float)Sprite.RotatedRect.Right / Level.impassableTileRecs[0].Width)) - 1;
			int topTile = (int)Math.Floor((float)Sprite.RotatedRect.Top / Level.impassableTileRecs[0].Height);
			int bottomTile = (int)Math.Ceiling(((float)Sprite.RotatedRect.Bottom / Level.impassableTileRecs[0].Height)) - 1;

			for (int y = topTile; y <= bottomTile; ++y)
			{
				for (int x = leftTile; x <= rightTile; ++x)
				{
					if ((x > 0 && x < 79 && y > 0 & y < 44) && Sprite.RotatedRect.Intersects(Level.tiles[y, x].SourceRect))
					{
						currentTileCollidingWith = Level.tiles[y, x];
						tileCollRect = Level.tiles[y, x].SourceRect;
					}
				}
			}
		}
	}
}