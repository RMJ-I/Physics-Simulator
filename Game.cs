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

            balls = new List<Ball>();

            float ballRadius = 15f;
            float ballDiameter = ballRadius * 2f;

            balls.Add(new Ball(new Vector2(200, 240), new Vector2(0, 0), 0.9f, 0.996f, _texture, 1f, ballRadius));

            Vector2 rackStart = new Vector2(550, 240);
            int rows = 5;
            int index = 0;

            for (int row = 0; row < rows; row++)
            {
                float yOffset = -row * ballRadius;

                for (int col = 0; col <= row; col++)
                {
                    float x = rackStart.X + row * (ballDiameter * 0.87f);
                    float y = rackStart.Y + (col * ballDiameter) + yOffset;

                    float mass = 1f;
                    balls.Add(new Ball(new Vector2(x, y), Vector2.Zero, 0.9f, 0.996f, _texture, mass, ballRadius));
                    index++;
                }
            }

            balls[0].velocity = new Vector2(20f, 20f);
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


