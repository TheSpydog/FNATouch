using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace FNATouch
{
	public static class VirtualGamepad
	{
		class VirtualButton
		{
			internal Rectangle Bounds;
			internal bool Active = true;
			internal ButtonState PreviousState = ButtonState.Released;
			internal ButtonState State = ButtonState.Released;
		}

		static Dictionary<string, VirtualButton> buttons = new Dictionary<string, VirtualButton>();
		static Texture2D pixel;

		// Modifiers

		public static void AddButton(string action, int x, int y, int width, int height)
		{
			buttons.Add(action, new VirtualButton
			{
				Bounds = new Rectangle(x, y, width, height)
			});
		}

		public static void RemoveButton(string action)
		{
			buttons.Remove(action);
		}

		public static void SetActive(string action, bool active)
		{
			buttons[action].Active = active;
			if (!active)
			{
				// Reset
				buttons[action].PreviousState = ButtonState.Released;
				buttons[action].State = ButtonState.Released;
			}
		}

		// Life Cycle

		public static void Initialize(GraphicsDevice graphicsDevice)
		{
			pixel = new Texture2D(graphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });
		}

		public static void Update()
		{
			TouchCollection touches = TouchPanel.GetState();

			foreach (VirtualButton button in buttons.Values)
			{
				if (button.Active)
				{
					button.PreviousState = button.State;
					button.State = ButtonState.Released;

					foreach (TouchLocation touch in touches)
					{
						if (button.Bounds.Contains((int)touch.Position.X, (int)touch.Position.Y))
						{
							button.State = ButtonState.Pressed;
							break;
						}
					}
				}
			}
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (VirtualButton button in buttons.Values)
			{
				if (button.Active)
				{
					Color drawColor = Color.White;
					if (button.State == ButtonState.Pressed)
						drawColor = Color.Red;

					spriteBatch.Draw(pixel, button.Bounds, drawColor);
				}
			}
		}

		// Info Fetching

		public static bool IsActive(string action)
		{
			return buttons[action].Active;
		}

		public static bool IsDown(string action)
		{
			return buttons[action].State == ButtonState.Pressed;
		}

		public static bool IsUp(string action)
		{
			return buttons[action].State == ButtonState.Released;
		}

		public static bool JustPressed(string action)
		{
			return buttons[action].State == ButtonState.Pressed &&
				buttons[action].PreviousState == ButtonState.Released;
		}

		public static bool JustReleased(string action)
		{
			return buttons[action].State == ButtonState.Released &&
				buttons[action].PreviousState == ButtonState.Pressed;
		}
	}
}
