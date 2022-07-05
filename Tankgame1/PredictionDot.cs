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
    class PredictionDot : Microsoft.Xna.Framework.DrawableGameComponent
    {
        bool live = true;

        VertexPositionNormalTexture[] pnverts;
        Texture2D texture;
        Effect PointDiffSpecTextureEffect;

        TankBarrel myParent { get; set; }
        Vector3[] pts;
        Vector2[] tcs;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;
        Matrix WorldM;
        Game1 myGame;
        public Vector3 Position { get; set; }

        Vector3 bonePosition;
        Vector3 travelPosition;
        public Matrix pivot { get; set; } = Matrix.Identity;
        //Vector3 lastPosition;
        //Vector3 direction;
        Vector3 velocity;
        Vector3 launchDirection;
        Matrix launchMatrix;

        float gravity = 50f;

        //Camera camera;
        //Vector4 lPos = Vector4.Zero;
        //Vector4 CamPos;
        //Vector4 Ambient;
        //Vector4 LightColor;
        //BasicEffect teffect;

        public PredictionDot(Game1 game, TankBarrel parent)
            : base(game)
        {
            myGame = (Game1)game;
            myParent = parent;
            pnverts = new VertexPositionNormalTexture[36];

            launchMatrix = parent.getFiringMatrix();
            bonePosition = Vector3.Transform(Vector3.Zero, launchMatrix);
            pivot = Matrix.CreateTranslation(bonePosition);
            Position = Vector3.Transform(myParent.Position, pivot);
            launchDirection = launchMatrix.Forward;
            WorldM = launchMatrix;
            worldRot = parent.worldRot * parent.myParent.worldRot * parent.myPaParent.worldRot;

            int iv = 0;
            pts = new Vector3[8];
            Vector3[] nms = new Vector3[8];
            tcs = new Vector2[8];


            pts[0].X = -1.0f;
            pts[0].Y = -1.0f;
            pts[0].Z = -1.0f;
            pts[1].X = -1.0f;
            pts[1].Y = 1.0f;
            pts[1].Z = -1.0f;
            pts[2].X = -1.0f;
            pts[2].Y = -1.0f;
            pts[2].Z = 1.0f;
            pts[3].X = -1.0f;
            pts[3].Y = 1.0f;
            pts[3].Z = 1.0f;
            pts[4].X = 1.0f;
            pts[4].Y = -1.0f;
            pts[4].Z = -1.0f;
            pts[5].X = 1.0f;
            pts[5].Y = 1.0f;
            pts[5].Z = -1.0f;
            pts[6].X = 1.0f;
            pts[6].Y = -1.0f;
            pts[6].Z = 1.0f;
            pts[7].X = 1.0f;
            pts[7].Y = 1.0f;
            pts[7].Z = 1.0f;

            for (int i = 0; i < 8; i++)
            {
                pts[i].Z *= 0.1f;
                pts[i].X *= 0.1f;
                pts[i].Y *= 0.1f;
            }

            nms[0].X = 0.0f;
            nms[0].Y = 1.0f;
            nms[0].Z = 0.0f;
            nms[1].X = 0.0f;
            nms[1].Y = -1.0f;
            nms[1].Z = 0.0f;
            nms[2].X = 1.0f;
            nms[2].Y = 0.0f;
            nms[2].Z = 0.0f;
            nms[3].X = -1.0f;
            nms[3].Y = 0.0f;
            nms[3].Z = 0.0f;
            nms[4].X = 0.0f;
            nms[4].Y = 0.0f;
            nms[4].Z = 1.0f;
            nms[5].X = 0.0f;
            nms[5].Y = 0.0f;
            nms[5].Z = -1.0f;

            tcs[0].X = 0.0f;
            tcs[0].Y = 0.0f;
            tcs[1].X = 1.0f;
            tcs[1].Y = 0.0f;
            tcs[2].X = 0.0f;
            tcs[2].Y = 1.0f;
            tcs[3].X = 1.0f;
            tcs[3].Y = 1.0f;

            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[3], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[3], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[1], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[1], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[0], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[0], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[5], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[5], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[4], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[4], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[2], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[2], tcs[2]);
        }

        public override void Initialize()
        {
            live = true;
            velocity = launchDirection * 50f;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Texture2D r = new Texture2D(GraphicsDevice, 8, 8);
            Color[] c = new Color[8 * 8];
            for(int i = 0; i < (8*8); i++)
            {
                c[i] = Color.Red;
            }
            r.SetData(c);

            texture = r;//Game.Content.Load<Texture2D>("Textures/brick2_b");
            PointDiffSpecTextureEffect = Game.Content.Load<Effect>("Effects/DiffuseTexture"); //("Effects/DiffSpecPointTexture");
            if (PointDiffSpecTextureEffect != null)
            {
                Console.WriteLine("PointDiff Check Pass");
            }



            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (live && myGame.GameOn)
            {
                checkCollision();
                velocity += Vector3.Down * gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                bonePosition += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds; // add gravity
                pivot = Matrix.CreateTranslation(bonePosition);
                Position = Vector3.Transform(myParent.Position, pivot);
                SetTranslation();
            }

            base.Update(gameTime);
        }

        void checkCollision()
        {
            if (bonePosition.Y <= myGame.getElevation(bonePosition) && live)
            {
                live = false;
                Enabled = false;
                myGame.Components.Remove(this);
            }

        }

        public void SetTranslation()
        {
            //worldTrans = Matrix.CreateTranslation(bonePosition);
            worldTrans = myParent.getFiringMatrix(); // includes body rotation and position from tank
            WorldM = worldRot * pivot * worldTrans;

        }

        public override void Draw(GameTime gameTime)
        {
            if (live)
            {
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                if (myGame.worldShader != null)
                {
                    // Console.WriteLine("PointDiff Draw Pass");
                    myGame.shaderAssignment(WorldM, texture);

                    foreach (EffectPass pass in myGame.worldShader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                               (PrimitiveType.TriangleList, pnverts, 0, 12);
                    }
                }
                else
                {
                    //LoadContent();
                }

            }

            base.Draw(gameTime);
        }
    }
}

