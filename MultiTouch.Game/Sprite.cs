using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiTouch.GameLauncher
{
    public class Sprite
    {
        public Texture2D Texture { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public Color Color { get; private set; }
        public Vector2 Center { get; private set; }
        public Vector2 Velocity { get; private set; }
        public int? TouchId { get; set; }
        public bool HasMoved { get; set; }

        public Sprite(Texture2D texture, Rectangle rectangle, Color color)
        {
            Texture = texture;
            Rectangle = rectangle;
            Color = color;
            Center = Vector2.Zero;
            Velocity = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color);
        }

        public void ChangeColor(Random random)
        {
            Color = new Color(
                (byte)random.Next(255),
                (byte)random.Next(255),
                (byte)random.Next(255),
                (byte)255);
        }

        public void SetCenter(Vector2 position)
        {
            Center = position;
            Rectangle = new Rectangle(
                (int)(position.X - Rectangle.Width * 0.5f),
                (int)(position.Y - Rectangle.Height * 0.5f),
                Rectangle.Width,
                Rectangle.Height);
        }

        public void SetDelta(Vector2 delta)
        {
            SetCenter(new Vector2(Center.X + delta.X, Center.Y + delta.Y));
        }

        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
        }
    }
}
