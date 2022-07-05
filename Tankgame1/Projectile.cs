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
    class Projectile : Microsoft.Xna.Framework.DrawableGameComponent
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
        Vector3 position;
        //Vector3 lastPosition;
        //Vector3 direction;
        Vector3 velocity;
        Vector3 launchDirection;
        Matrix launchMatrix;
        float spin;

        float gravity = 50f;
        int pType;

        //Camera camera;
        //Vector4 lPos = Vector4.Zero;
        //Vector4 CamPos;
        //Vector4 Ambient;
        //Vector4 LightColor;
        //BasicEffect teffect;

        public Projectile(Game1 game, TankBarrel parent, int type)
            : base(game)
        {
            myGame = (Game1)game;
            myParent = parent;
            pnverts = new VertexPositionNormalTexture[36];

            //camera = cam;

            // WorldM = worldRot * pivot * worldTrans;
            pType = type;
            launchMatrix = parent.getFiringMatrix();
            position = Vector3.Transform(Vector3.Zero, launchMatrix);
            launchDirection = launchMatrix.Forward;
            WorldM = launchMatrix;
            spin = 0f;
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
                pts[i].Z *= 0.2f;
                pts[i].X *= 0.2f;
                pts[i].Y *= 0.2f;
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
            myGame.LiveShots += 1;
            myGame.Ammunition -= pType == 3 ? 2 : 1;
            //myGame.Minerals -= myGame.shotCost;
            //Initial Velocity
            velocity = launchDirection * 50f + myParent.myPaParent.Velocity;
            //Vector3.Forward * 1.5f;
            //CamPos = camera.getCamPos4();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Textures/brick2_b");
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
                //CamPos = camera.getCamPos4();
                
                checkCollision();

                velocity += Vector3.Down * gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds; // add gravity
                SetTranslation();
            }

            base.Update(gameTime);
        }

        void checkCollision()
        {
            if (position.Y <= myGame.getElevation(position) && live)
            {
                switch (pType)
                {
                    case 0: // Crater
                        myGame.terrainImpactDeform(position, 16f, 1f, pType);
                        break;
                    case 1: // Excavate
                        myGame.terrainImpactDeform(position, 16f, 1f, pType);
                        break;
                    case 2: // Flatten
                        myGame.terrainImpactDeform(position, 16f, 1.5f, pType);
                        break;
                    case 3: // Build
                        myGame.terrainImpactDeform(position, 16f, 1f, pType);
                        break;
                    default:
                        break;
                }
                myGame.LiveShots -= 1;
                live = false;
                Enabled = false;
                myGame.Components.Remove(this);
            }
            
        }

        public void SetTranslation()
        {
            spin = 0.1f;
            worldRot *= new Matrix(new Vector4(1, 0, 0, 0),
                                       new Vector4(0, (float)Math.Cos(spin), -(float)Math.Sin(spin), 0),
                                       new Vector4(0, (float)Math.Sin(spin), (float)Math.Cos(spin), 0),
                                       new Vector4(0, 0, 0, 1));

            worldTrans = Matrix.CreateTranslation(position);
            WorldM = worldRot * worldTrans;
            //teffect.World = WorldM;

        }

        public override void Draw(GameTime gameTime)
        {
            if (live)
            {
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                //myGame.setCamera(teffect);

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
