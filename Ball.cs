using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Physics_Simulator
{
    public class Ball
    {
        public Vector2 position, velocity;
        public float restitution;
        public float friction;
        public float m;
        public float r;
        private Texture2D texture;
        public Ball(Vector2 startPosition, Vector2 startVelocity, float Restitution, float Friction, Texture2D tex, float mass, float radius)
        {
            texture = tex;
            position = startPosition;
            velocity = startVelocity;
            restitution = Restitution;
            friction = Friction;
            m = mass;
            r = radius;
            
        }
        public void update(GameTime gameTime, int screenWidth, int screenHeight)
        {
            if (velocity.LengthSquared() < 0.01f)
            {
                velocity = Vector2.Zero;
            }
            float diameter = 2 * r;
            velocity *= friction;
            if (position.X + diameter > screenWidth)
            {
                position.X = screenWidth - diameter;
                velocity.X *= -restitution;
            }
            if (position.X < 0)
            {
                position.X = 0;
                velocity.X *= -restitution;
            }
            if (position.Y + diameter > screenHeight)
            {
                position.Y = screenHeight - diameter;
                velocity.Y *= -restitution;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
                velocity.Y *= -restitution;
            }
            position += velocity;
        }
        public void Collision(Ball other, SoundEffect ImpactSoundInstance)
        {
            Vector2 c1 = this.position + new Vector2(this.r, this.r);
            Vector2 c2 = other.position + new Vector2(other.r, other.r);

            Vector2 delta = c2 - c1;
            float distance = delta.Length();
            float minDistance = this.r + other.r;
            float volume = 1f;

            if (distance <= 0.0001f) return;

            if (distance < minDistance)
            { 

                Vector2 normal = delta / distance;
                Vector2 tangent = new Vector2(-normal.Y, normal.X);

                float v1n = Vector2.Dot(this.velocity, normal);
                float v1t = Vector2.Dot(this.velocity, tangent);
                float v2n = Vector2.Dot(other.velocity, normal);
                float v2t = Vector2.Dot(other.velocity, tangent);

                float e = MathF.Min(this.restitution, other.restitution);

                if (v2n - v1n > 0)
                    return;

                float impulse = (-(1 + e) * (v2n - v1n)) / (1f / this.m + 1f / other.m);

                float v1nAfter = v1n - impulse / this.m;
                float v2nAfter = v2n + impulse / other.m;

                this.velocity = v1nAfter * normal + v1t * tangent;
                other.velocity = v2nAfter * normal + v2t * tangent;

                float overlap = minDistance - distance;

                float totalMass = this.m + other.m;
                float move1 = other.m / totalMass;
                float move2 = this.m / totalMass;

                c1 -= normal * overlap * move1;
                c2 += normal * overlap * move2;

                this.position = c1 - new Vector2(this.r, this.r);
                other.position = c2 - new Vector2(other.r, other.r);

                volume = Math.Clamp(Math.Abs(impulse) / 50f, 0f, 1f);
                ImpactSoundInstance.Play(volume, 0f, 0f);
            }
        }

        public bool IsCursorOnBall(Ball ball)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePosition = mouseState.Position;

            float dx = mousePosition.X - (ball.position.X + ball.r);
            float dy = mousePosition.Y - (ball.position.Y + ball.r);

            float distanceSquared = (dx * dx + dy * dy);
            return distanceSquared <= ball.r * ball.r;
        }
        public void DragBall(Ball ball)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePosition = mouseState.Position;
            Vector2 ballCenter = ball.position + new Vector2(ball.r, ball.r);
            Vector2 direction = new Vector2(mousePosition.X, mousePosition.Y) - ballCenter;
            ball.velocity = direction * 0.01f;
        }
        public void setScale(float scale)
        {
            scale = 1 / scale;
            r = r * scale;
            velocity = velocity * scale;
        }
        public void draw(SpriteBatch spriteBatch, Color colour)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 2 * (int)r, 2 * (int)r), colour);
        }
    }

}
