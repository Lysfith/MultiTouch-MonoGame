using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiTouch.GameLauncher
{
    public class PostIt : Sprite
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }

        public PostIt(Texture2D texture, Rectangle rectangle, Color color, SpriteFont font, string text = null)
            :base(texture, rectangle, color)
        {
            Font = font;
            Text = LoremNET.Lorem.Words(1, 3);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, Color);

            if(!string.IsNullOrEmpty(Text))
            {
                var mesuringString = Font.MeasureString(Text);

                spriteBatch.DrawString(Font, Text, new Vector2(
                    Rectangle.X + Rectangle.Width * 0.5f - mesuringString.X * 0.5f,
                    Rectangle.Y + Rectangle.Height * 0.5f - mesuringString.Y * 0.5f - 10
                    ),
                    Color.Gray);
            }
        }

        public void SetText(string text)
        {
            Text = text;
        }
    }
}
