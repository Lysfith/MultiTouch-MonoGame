using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MultiTouch.GameLauncher
{
    public class MyGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private Texture2D _texture;
        private Texture2D _texturePostit;

        //Timer
        private Stopwatch _stopwatch;
        private double _updateTime;
        private double _drawTime;

        //Fps
        private int _frameCounter;
        private int _lastFrameCounter;
        private Stopwatch _stopwatchFps;
        private int _maxFrameCounter;
        private int _minFrameCounter;

        //Liste des images affichées
        private List<Sprite> _sprites;

        private TouchCollection _touches;

        private List<Color> _colors;

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public Random Random { get; private set; }

        public MyGame()
        {
            //ScreenWidth = 1366;
            //ScreenHeight = 768;
            ScreenWidth = 1920;
            ScreenHeight = 1080;
            Random = new Random();

            //Paramétrage de la fenêtre
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            //_graphics.PreferMultiSampling = true;
            //_graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _frameCounter = 0;
            _lastFrameCounter = 0;
            _minFrameCounter = int.MaxValue;
            _maxFrameCounter = 0;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _stopwatchFps = new Stopwatch();
            _stopwatchFps.Start();

            //On définit les mouvement possible en touch
            TouchPanel.EnabledGestures =
                GestureType.Hold |
                GestureType.Tap |
                GestureType.DoubleTap |
                GestureType.FreeDrag |
                GestureType.Flick |
                GestureType.Pinch;

            TouchPanel.EnableMouseTouchPoint = true;
            TouchPanel.EnableMouseGestures = true;

            _sprites = new List<Sprite>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Fonts/Arial-16");
            _texture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _texturePostit = Content.Load<Texture2D>("Textures/postit");

            Color[] tcolor = new Color[1];
            _texture.GetData<Color>(tcolor);
            tcolor[0] = new Color(255, 255, 255, 255);
            _texture.SetData<Color>(tcolor);

            _colors = new List<Color>()
            {
                Color.Red,
            };

            for (var i = 0; i < 10; i++)
            {
                _sprites.Add(new PostIt(_texturePostit, new Rectangle(i * 160, i * 70 + 100, 200, 200), new Color(255, 255, 165, 255), _font));
            }

            //On change la couleur du post-it pour le mettre blanc
            tcolor = new Color[_texturePostit.Width * _texturePostit.Height];
            _texturePostit.GetData<Color>(tcolor);

            Color colorTest = new Color(110, 118, 137, 153);

            for (var i = 0; i < tcolor.Length; i++)
            {
                var color = tcolor[i];
                if (color.R == colorTest.R
                    && color.G == colorTest.G
                    && color.B == colorTest.B)
                {
                    tcolor[i] = new Color(255, 255, 165, 255);
                }
            }

            _texturePostit.SetData<Color>(tcolor);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            //Calcul des fps
            if(_stopwatchFps.ElapsedMilliseconds > 1000)
            {
                if(_frameCounter > _maxFrameCounter)
                {
                    _maxFrameCounter = _frameCounter;
                }

                if (_frameCounter < _minFrameCounter)
                {
                    _minFrameCounter = _frameCounter;
                }

                _lastFrameCounter = _frameCounter;
                _frameCounter = 0;
                _stopwatchFps.Restart();
            }
            else
            {
                _frameCounter++;
            }

            _stopwatch.Restart();

            _touches = TouchPanel.GetState();

            //HandleTouchInput();

            _updateTime = _stopwatch.Elapsed.TotalMilliseconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _stopwatch.Restart();

            List<string> alreadyDraw = new List<string>();
            int cpt = 0;
            foreach (var touch1 in _touches)
            {
                var color = _colors[cpt % _colors.Count];

                foreach (var touch2 in _touches)
                {
                    if (touch1.Id != touch2.Id 
                        && !alreadyDraw.Contains(touch1.Id + "_" + touch2.Id)
                        && !alreadyDraw.Contains(touch2.Id + "_" + touch1.Id))
                    {
                        DrawLine(_spriteBatch, touch1.Position, touch2.Position, color);

                        alreadyDraw.Add(touch1.Id + "_" + touch2.Id);
                        alreadyDraw.Add(touch2.Id + "_" + touch1.Id);
                    }
                }

                cpt++;
            }

            foreach (Sprite sprite in _sprites)
            {
                sprite.HasMoved = false;
            }

            foreach (var touch in _touches)
            {
                var sprite = _sprites.Where(x => x.TouchId == touch.Id).FirstOrDefault();

                if(sprite != null)
                {
                    sprite.SetCenter(touch.Position);
                }
                else
                {
                    sprite = _sprites.Where(x => x.Rectangle.Contains(touch.Position)).FirstOrDefault();

                    if (sprite != null)
                    {
                        sprite.TouchId = touch.Id;
                        sprite.SetCenter(touch.Position);
                    }
                }
            }

            foreach (Sprite sprite in _sprites)
            {
                if(!sprite.HasMoved)
                {
                    sprite.TouchId = null;
                }
                sprite.Draw(_spriteBatch);
            }

            _drawTime = _stopwatch.ElapsedMilliseconds;

            _spriteBatch.DrawString(_font, "Update : " + _updateTime.ToString("0.000") + " ms", new Vector2(10, 30), Color.Yellow);
            _spriteBatch.DrawString(_font, "Draw : " + _drawTime.ToString("0.000") + " ms", new Vector2(10, 10), Color.Yellow);

            if (_minFrameCounter != int.MaxValue)
            {
                _spriteBatch.DrawString(_font,
                    "Fps : " + _lastFrameCounter.ToString("00.00") + ", Min : " + _minFrameCounter.ToString("00.00") + ", Max : " + _maxFrameCounter.ToString("00.00"),
                    new Vector2(10, 50), Color.Yellow);
            }
            else
            {
                _spriteBatch.DrawString(_font,
                    "Fps : " + _lastFrameCounter.ToString("00.00") + ", Max : " + _maxFrameCounter.ToString("00.00"),
                    new Vector2(10, 50), Color.Yellow);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(_texture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}
