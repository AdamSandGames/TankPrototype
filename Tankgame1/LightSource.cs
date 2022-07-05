using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tankgame1
{
    struct DiffuseLight
    {
        public DiffuseLight(Color col, float intensity = 1f)
        {
            Color4 = col.ToVector4();
            Intensity = intensity;
        }
        public Vector4 Color4 { get; set; }
        public float Intensity { get; set; }
    }
    struct AmbientLight
    {
        public AmbientLight(Color col, float intensity = 1f)
        {
            Color4 = col.ToVector4();
            Intensity = intensity;
        }
        public Vector4 Color4 { get; set; }
        public float Intensity { get; set; }
    }
    struct SpecularLight
    {
        public SpecularLight(float shine, Color col, float intensity = 1f)
        {
            Shine = shine;
            Color4 = col.ToVector4();
            Intensity = intensity;
        }
        public float Shine { get; set; }
        public Vector4 Color4 { get; set; }
        public float Intensity { get; set; }
    }
    class LightSource : Microsoft.Xna.Framework.GameComponent
    {
        Game1 myGame;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;
        Matrix WorldM = Matrix.Identity;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Color LightColor { get; set; }
        public DiffuseLight Diffuse { get; set; }
        public AmbientLight Ambient { get; set; }
        public SpecularLight Specular { get; set; }
        public Fog Fog { get; set; }

        Vector3 orbitOffset;
        Vector3 orbit;
        //float Yaw, Pitch, Roll;   // Angles 
        float Distance;
        float tTime;

        public LightSource(Game game, Vector3 pos)
            : base(game)
        {
            myGame = (Game1)game;
            Position = pos;

            Direction = Vector3.Down;
            LightColor = Color.White;
            Diffuse = new DiffuseLight(LightColor, 1);
            Ambient = new AmbientLight(LightColor, 1);
            Specular = new SpecularLight(1, LightColor, 1);

            orbitOffset = Vector3.Zero;
            //Yaw = 0;
            //Pitch = 0;
            //Roll = 0;
            //Distance = 0;
            //tTime = 0;
        }
        public LightSource(Game game, Vector3 pos, Vector3 dir, Color col,
            DiffuseLight diff, AmbientLight amb, SpecularLight spec, Fog fog, Vector3 orbital)
            : base(game)
        {
            myGame = (Game1)game;
            LightColor = col;
            Diffuse = diff;
            Ambient = amb;
            Specular = spec;
            Fog = fog;

            Distance = orbital.Length();
            if (Distance > 0f)
            {
                Position = pos + orbital;
                Direction = Position;
                Direction.Normalize();
                orbitOffset = orbital;
                orbit = pos;
                tTime = 0;
            }
            else
            {
                Position = pos;
                Direction = dir;
                Direction.Normalize();
                orbitOffset = Vector3.Zero;
                orbit = pos;
                tTime = 0;
            }
            //Yaw = 0;
            //Pitch = 0;
            //Roll = 0;
        }
        public override void Initialize()
        {
            worldTrans = Matrix.CreateTranslation(Position);
            WorldM = worldRot * worldTrans;
            Direction.Normalize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Distance > 1f && myGame.GameOn)
            {
                tTime += (float)gameTime.ElapsedGameTime.TotalSeconds / 0.5f;
                //this.velocity.Y = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 20.0f)));
                //if (velocity.Y < 0)
                //    velocity.Y *= -1.0f;
                orbitOffset.X = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 40.0f)));
                orbitOffset.Y = (float)(Math.Sin(MathHelper.TwoPi * (tTime / 40.0f))); //0f;
                orbitOffset.Z = 0f; // (float)(Math.Sin(MathHelper.TwoPi * (tTime / 40.0f)));
                orbitOffset.Normalize();
                Position = orbit + orbitOffset * Distance;
                Direction = Position;
                Direction.Normalize();
                //Debug.WriteLine("Orbit "+tTime);
            }
            
            /*
            if (orbit != orbit)
            {   // Yes Orbit Exists, and yes I'm using this property again lol
                
                if (false)
                {
                    Yaw = 0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Pitch = velocity.Length() * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Roll = 0f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Vector3 OrbitOffset = Position - orbit;

                    Matrix Rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);

                    Vector3.Transform(ref OrbitOffset, ref Rotation, out OrbitOffset);

                    Position = orbit + OrbitOffset;  // Final position of the rotated object

                }
                else
                {
                    tTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                    this.velocity.Y = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 20.0f)));
                    if (velocity.Y < 0)
                        velocity.Y *= -1.0f;
                    velocity.Z = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 40.0f)));
                    velocity.X = (float)(Math.Sin(MathHelper.TwoPi * (tTime / 40.0f)));
                    velocity.Normalize();
                    Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

            }
            else
            {   // No Orbit Exists
                Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            */

            base.Update(gameTime);
        }
        public Matrix getWorldMatrix()
        {
            return WorldM;
        }
        //public void setVelocity(Vector3 s)
        //{
        //    velocity = s;
        //}
        //public void setOrbit(Vector3 o)
        //{
        //    orbit = o;
        //}
        public Vector4 getLightPos4()
        {
            return new Vector4(Position, 1f);
        }
        public Vector4 getLightDir4()
        {
            Vector4 v = new Vector4(Direction, 0f);
            v.Normalize();
            return v;
        }
        public Vector4 getLightColor4()
        {
            return LightColor.ToVector4();
        }

    }
}
