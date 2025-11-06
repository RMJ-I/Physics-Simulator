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
        Ball StartingBall;
        SpriteFont _font;
        bool cursorOnBall, StartingCursorOnBall;
        public SoundEffect ImpactSound;
        float scale = 1f;
        Rectangle box;
        Texture2D _box;
        bool Collision = true;
        bool Dragging = false;
        MouseState previousMouseState;
        int currentBall = 0;

        Texture2D _texture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            box = new Rectangle(650, 0, 150, 480);
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _box = Content.Load<Texture2D>("rectangle");
            _texture = Content.Load<Texture2D>("ball2");
            _font = Content.Load<SpriteFont>("DefaultFont");
            ImpactSound = Content.Load<SoundEffect>("pool");

            balls = new List<Ball>();

            float ballRadius = 15f;
            float ballDiameter = ballRadius * 2f;
            float StartingBallRadius = 25f;
            float StartingBallDiameter = StartingBallRadius * 2f;

            float mass = 0.16f;
            Vector2 ballPosition = new Vector2(box.X + box.Width/2 - (int)StartingBallRadius, 
                box.Height - (int)(box.Height / 1.2 - (int)StartingBallRadius));
            balls.Add(new Ball(new Vector2(ballPosition.X, ballPosition.Y), Vector2.Zero, 0.9f, 1f, _texture, mass, StartingBallRadius));
            Vector2 rackStart = new Vector2(550, 240 - ballRadius);
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            cursorOnBall = false;
            StartingCursorOnBall = false;
            if (Collision)
            {
                for (int i = 1; i < balls.Count; i++)
                {
                    balls[i].update(gameTime, GraphicsDevice.Viewport.Width - box.Width, GraphicsDevice.Viewport.Height);
                    for (int j = i + 1; j < balls.Count; j++)
                    {
                        balls[i].Collision(balls[j], ImpactSound);
                    }
                }
            }
            Collision = true;
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].IsCursorOnBall(balls[i]))
                {
                    cursorOnBall = true;
                }
            }
            if (balls[0].IsCursorOnBall(balls[0]))
                StartingCursorOnBall = true;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && StartingCursorOnBall && previousMouseState.LeftButton == ButtonState.Released)
            {
                Dragging = true;
            }
            if (Dragging && previousMouseState.LeftButton == ButtonState.Released)
            {
                Collision = false;
                float Mx = Mouse.GetState().Position.X;
                float My = Mouse.GetState().Position.Y;
                balls.Add(new Ball(new Vector2(Mx - balls[0].r, My - balls[0].r), balls[0].velocity, balls[0].restitution, balls[0].friction, _texture, balls[0].m, balls[0].r));
                currentBall++;
            }
            if (Dragging)
            {
                float Mx = Mouse.GetState().Position.X;
                float My = Mouse.GetState().Position.Y;
                balls[currentBall].position = new Vector2(Mx - balls[0].r, My - balls[0].r);
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed && Dragging)
            {
                Dragging = false;
                Collision = true;
                if (balls[currentBall].position.X - 2 * balls[0].r >= box.X)
                {
                    balls.RemoveAt(currentBall);
                }
            }
            // TODO: Add your update logic here
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_box, box, Color.DarkGray);
            for (int i = 1; i < balls.Count; i++)
            {
                if (balls[i].IsCursorOnBall(balls[i]))
                {
                    _spriteBatch.DrawString(_font, $"Speed: {Math.Round(balls[i].velocity.Length() * scale, 2)}\nEnergy: {Math.Round(0.5f * (float)balls[i].m * ((float)balls[i].velocity.LengthSquared() * scale * scale), 2)}", new Vector2(0, 0), Color.White);
                }
                balls[i].draw(_spriteBatch, Color.DarkBlue);
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
