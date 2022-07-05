using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace Tankgame1
{

    public class Sphere : Microsoft.Xna.Framework.DrawableGameComponent
    {

        VertexPositionColor[] verts;
        int nlat, nlong;  // number of line lattitude and longitude
        BasicEffect ceffect;
        Vector3[] pts;
        float[] lts;
        float radius;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;
        Game1 myGame;
        Vector3 position;
        float gravity = -0.5f;
        Vector3 speed = new Vector3(0, 0, 0);
        Vector3 normal = new Vector3(0, 0, 0);
        static public Boolean bounce = false; // static: one variable shared by all instances of sphere


        public Sphere(Game game)
             : base(game)
        {

            myGame = (Game1)game;
            nlat = 10;
            nlong = 20;
            radius = 1.0f;

            verts = new VertexPositionColor[nlat * nlong * 6];
            int iv = 0;

            pts = new Vector3[4];
            lts = new float[4];
            Color[] clrs = new Color[4];


            for (int il = 0; il < 4; il++)
            {  // grey for now
                clrs[il] = new Color(.5f, .5f, .5f);
            }


            for (int i = 0; i < nlat; i++)
            {
                for (int j = 0; j < nlong; j++)
                {

                    pts[0].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[0].Y = (float)(Math.Cos(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[0].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));

                    pts[1].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[1].Y = (float)(Math.Cos(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[1].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));

                    pts[2].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[2].Y = (float)(Math.Cos(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[2].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));

                    pts[3].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[3].Y = (float)(Math.Cos(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[3].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));


                    verts[iv++] = new VertexPositionColor(pts[0], clrs[0]);
                    verts[iv++] = new VertexPositionColor(pts[1], clrs[1]);
                    verts[iv++] = new VertexPositionColor(pts[2], clrs[2]);
                    verts[iv++] = new VertexPositionColor(pts[2], clrs[2]);
                    verts[iv++] = new VertexPositionColor(pts[1], clrs[1]);
                    verts[iv++] = new VertexPositionColor(pts[3], clrs[3]);
                }
            }
        }

        public void SetTranslation(Vector3 trns)
        {  // set sphere location
            position = trns;
            worldTrans = Matrix.CreateTranslation(position);
            ceffect.World = worldRot * worldTrans;
        }


        public void SetLightDir(Vector3 ldir)
        {  // calculate lightmaps - call this only once, not every frame

            Color ncol = new Color(0, 0, 0, 255);
            float lt;

            for (int il = 0; il < nlat * nlong * 6; il++)
            {  // asssumes radius is one
                lt = Vector3.Dot(ldir, verts[il].Position);
                if (lt < 0.0f) lt = 0.0f;
                ncol.R = ncol.G = ncol.B = (byte)(lt * 255);

                verts[il].Color = ncol;
            }

        }


        public override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization code here
            ceffect = new BasicEffect(GraphicsDevice);
            ceffect.VertexColorEnabled = true;
            ceffect.World = worldRot * worldTrans;

        }


        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (true)
            {
                if (position.Y - radius < Game1.mHeights[(int)(Math.Floor(position.X) + Game1.gDim / 2), (int)(Math.Floor(position.Z) + Game1.gDim / 2)])
                {
                    speed = Vector3.Reflect(speed, Game1.normals[(int)(Math.Floor(position.X) + Game1.gDim / 2), (int)(Math.Floor(position.Z) + Game1.gDim / 2)]) * 0.9f;
                }
            }
            position += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            speed.Y += gravity;
            SetTranslation(position);

            

            base.Update(gameTime);
        }



        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            myGame.setCamera(ceffect);

            foreach (EffectPass pass in ceffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                        (PrimitiveType.TriangleList, verts, 0, nlat * nlong * 2);
            }


            base.Draw(gameTime);
        }

        public Vector3 getPosition()
        {
            return position;
        }
    }
}
