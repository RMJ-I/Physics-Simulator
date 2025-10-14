using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Physics_Simulator
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<Ball> balls;
        SpriteFont _font;
        bool cursorOnBall;
        public SoundEffect ImpactSound;
        float scale = 10f;


        Texture2D _texture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("ball");
            _font = Content.Load<SpriteFont>("DefaultFont");
            ImpactSound = Content.Load<SoundEffect>("pool");

            balls = new List<Ball>
            {
                new Ball(new Vector2(700, 200), new Vector2(4, 8), 0.8f, 1F, _texture, 2f, 20f),
                new Ball(new Vector2(200, 50), new Vector2(-8, 7), 0.8f, 1F, _texture, 0.5f, 10f),
                new Ball(new Vector2(300, 300), new Vector2(15, 3), 0.8f, 1f, _texture, 8f, 30f),
                new Ball(new Vector2(100, 200), new Vector2(-6, 0), 0.9f, 1F, _texture, 2f, 20f),
                new Ball(new Vector2(300, 200), new Vector2(0, 0), 0.9f, 1F, _texture, 5f, 25f),
                new Ball(new Vector2(500, 200), new Vector2(3, 0), 0.9f, 1f, _texture, 1f, 15f)
            };
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].setScale(scale);
            }
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach (var b in balls)
            {
                b.update(gameTime, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }
            
            cursorOnBall = false;
            for (int i = 0; i < balls.Count; i++)
            {

                for (int j = i + 1; j < balls.Count; j++)
                {
                    balls[i].Collision(balls[j], ImpactSound);
                }
            }
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].IsCursorOnBall(balls[i]))
                {
                    cursorOnBall = true;
                }
            }
                // TODO: Add your update logic here

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].IsCursorOnBall(balls[i]))
                {
                    _spriteBatch.DrawString(_font, $"Speed: {Math.Round(balls[i].velocity.Length() * (float)Math.Pow(scale, 0.5), 2)}\nEnergy: {Math.Round(0.5f * (float)balls[i].m * ((float)balls[i].velocity.LengthSquared() * scale), 2)}", new Vector2(0, 0), Color.White);
                }
            }
            foreach (var b in balls)
            b.draw(_spriteBatch);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

