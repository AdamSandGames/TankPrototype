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
    public class TankBarrel : Microsoft.Xna.Framework.DrawableGameComponent
    {
        VertexPositionNormalTexture[] pnverts;
        Texture2D texture;
        Effect PointDiffSpecTextureEffect;

        public TankTurret myParent { get; set; }
        public Tank myPaParent { get; set; }
        Vector3[] pts;
        Vector2[] tcs;
        public Matrix worldTrans { get; set; } = Matrix.Identity;
        public Matrix worldRot { get; set; } = Matrix.Identity;
        Matrix WorldM;
        Game1 myGame;
        public Vector3 Position { get; set; }
        public Vector3 bonePosition { get; set; } = Vector3.Forward * 1.5f;
        public Vector3 Direction { get; set; }
        public Matrix pivot { get; set; } = Matrix.Identity;
        public float vRotation { get; set; }
        int gunMode;
            // Mode 0: Craters
            // Mode 1: Excavate
            // Mode 2: Level
            // Mode 3: Build

        //Camera camera;
        //Vector4 lPos = Vector4.Zero;
        //Vector4 CamPos;
        //Vector4 Ambient;
        //Vector4 LightColor;
        //BasicEffect teffect;

        KeyboardState keyboardState;
        KeyboardState prevKeyboardState;
        float timeSinceFire = 3f;
        float timeDots = 0f;
        Matrix nozzleMatrix;
        Vector3 nozzlePosition;
        bool pdots = false;

        public TankBarrel(Game1 game, TankTurret parent)
            : base(game)
        {
            myGame = game;
            myParent = parent;
            myPaParent = parent.myParent;
            pnverts = new VertexPositionNormalTexture[36];

            //camera = cam;
            Position = parent.getPosition();
            Direction = parent.getDirection();
            vRotation = 0f;
            gunMode = 0;

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
            pts[2].Z = 0.0f;
            pts[3].X = -1.0f;
            pts[3].Y = 1.0f;
            pts[3].Z = 0.0f;
            pts[4].X = 1.0f;
            pts[4].Y = -1.0f;
            pts[4].Z = -1.0f;
            pts[5].X = 1.0f;
            pts[5].Y = 1.0f;
            pts[5].Z = -1.0f;
            pts[6].X = 1.0f;
            pts[6].Y = -1.0f;
            pts[6].Z = 0.0f;
            pts[7].X = 1.0f;
            pts[7].Y = 1.0f;
            pts[7].Z = 0.0f;

            for (int i = 0; i < 8; i++)
            {
                pts[i].Z *= 2.8f;
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
            //teffect = new BasicEffect(GraphicsDevice);
            //teffect.VertexColorEnabled = true;
            //teffect.World = myParent.getWorldMatrix();
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

            //Ambient.X = 0.2f; // ambient light
            //Ambient.Y = 10.0f; // specular exponent
            //Ambient.Z = 1.0f; // specular intensity
            //Ambient.W = 1.0f;

            //LightColor.X = 0.6f;
            //LightColor.Y = 0.4f;
            //LightColor.Z = 0.8f;
            //LightColor.W = 0.0f;
            //lPos.Y = 50f;
            

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (myGame.GameOn)
            {
                //CamPos = camera.getCamPos4();
                keyInput(gameTime);

                pivot = Matrix.CreateTranslation(bonePosition);

                Position = Vector3.Transform(myParent.Position, pivot);

                SetTranslation();

            }


            base.Update(gameTime);
        }

        public void SetTranslation()
        {
            worldRot = new Matrix(new Vector4(1, 0, 0, 0),
                                       new Vector4(0, (float)Math.Cos(vRotation), -(float)Math.Sin(vRotation), 0),
                                       new Vector4(0, (float)Math.Sin(vRotation), (float)Math.Cos(vRotation), 0),
                                       new Vector4(0, 0, 0, 1));

            worldTrans = myParent.getWorldMatrix(); // includes body rotation and position from tank
            WorldM = worldRot * pivot * worldTrans;
            

            Direction = WorldM.Forward;
            Direction.Normalize();
            

        }

        void keyInput(GameTime gameTime)
        {
            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.D1) && !prevKeyboardState.IsKeyDown(Keys.D1))
            {
                gunMode = 0;
                myGame.gunMode = "Craters";
            }
            if (keyboardState.IsKeyDown(Keys.D2) && !prevKeyboardState.IsKeyDown(Keys.D2))
            {
                gunMode = 1;
                myGame.gunMode = "Excavate";
            }
            if (keyboardState.IsKeyDown(Keys.D3) && !prevKeyboardState.IsKeyDown(Keys.D3))
            {
                gunMode = 2;
                myGame.gunMode = "Flatten";
            }
            if (keyboardState.IsKeyDown(Keys.D4) && !prevKeyboardState.IsKeyDown(Keys.D4))
            {
                gunMode = 3;
                myGame.gunMode = "Build";
            }

            float n = 0f;
            // Increase/Decrease Angle
            if (keyboardState.IsKeyDown(Keys.F) && vRotation <= 0.1f)
            {
                n += 1f;
            }
            if (keyboardState.IsKeyDown(Keys.R) && vRotation >= -1.0f)
            {
                n -= 1f;
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                n *= 1f / 3f; // Precision Controls
            }
            vRotation += 1f * n * (float)gameTime.ElapsedGameTime.TotalSeconds;

            timeSinceFire += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeDots += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceFire > 0.5f)
            {
                if (keyboardState.IsKeyDown(Keys.C) && myGame.Ammunition >= 1)
                {
                    Projectile p = new Projectile(myGame, this, gunMode);
                    myGame.Components.Add(p);
                    timeSinceFire = 0f;
                    myGame.gunFire.Play(myGame.gameVolume, 0.0f, 0.0f);
                }
                if (pdots == true && timeDots > 0.2f)// && keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    PredictionDot d = new PredictionDot(myGame, this);
                    myGame.Components.Add(d);
                    timeDots = 0f;
                }
            }
            
        }

        public override void Draw(GameTime gameTime)
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



            base.Draw(gameTime);
        }

        public Vector3 getPosition()
        {
            return Position;
        }
        public Vector3 getDirection()
        {
            return Direction;
        }
        public Matrix getWorldMatrix()
        {
            return WorldM;
        }
        public Matrix getNozMat()
        {
            return nozzleMatrix;
        }
        public Vector3 getNozPos()
        {
            return nozzlePosition;
        }
        public Matrix getFiringMatrix()
        {
            Matrix nozPivot = Matrix.CreateTranslation(Vector3.Forward * 2.9f);
            Matrix barTrans = WorldM;
            return nozPivot * barTrans;
        }
    }
}
