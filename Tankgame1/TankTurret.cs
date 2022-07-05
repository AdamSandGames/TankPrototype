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
    public class TankTurret : Microsoft.Xna.Framework.DrawableGameComponent
    {

        VertexPositionNormalTexture[] pnverts;
        Texture2D texture;
        Effect PointDiffSpecTextureEffect;

        public Tank myParent { get; set; }
        public TankBarrel myBarrel { get; set; }
        Vector3[] pts;
        Vector2[] tcs;
        public Matrix worldTrans { get; set; } = Matrix.Identity;
        public Matrix worldRot { get; set; } = Matrix.Identity;
        Matrix WorldM;
        Game1 myGame;
        public Vector3 Position { get; set; }
        public Vector3 bonePosition { get; set; } = Vector3.Up * 2f;
        public Vector3 Direction { get; set; }
        public Matrix pivot { get; set; } = Matrix.Identity;
        public float yRotation { get; set; }

        //Camera camera;
        //Vector4 lPos = Vector4.Zero;
        //Vector4 CamPos;
        //Vector4 Ambient;
        //Vector4 LightColor;
        //BasicEffect teffect;

        KeyboardState keyboardState;

        public TankTurret(Game1 game, Tank parent)
            : base(game)
        {
            myGame = (Game1)game;
            myParent = parent;
            pnverts = new VertexPositionNormalTexture[36];

            //camera = cam;
            Position = parent.getPosition();
            Direction = parent.getDirection();
            yRotation = 0f;

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
                pts[i].Z *= 1.5f;
                pts[i].X *= 1.0f;
                pts[i].Y *= 0.7f;
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
            myBarrel = new TankBarrel(myGame, this);
            myGame.Components.Add(myBarrel);
            myParent.myBarrel = this.myBarrel;

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
                //Position = myParent.Position + (myParent.rotUp * 2);

                pivot = Matrix.CreateTranslation(bonePosition);

                Position = Vector3.Transform(myParent.Position, pivot);

                SetTranslation();

            }


            base.Update(gameTime);
        }

        public void SetTranslation()
        {
            worldRot = new Matrix(new Vector4((float)Math.Cos(yRotation), 0, -(float)Math.Sin(yRotation), 0),
                                       new Vector4(0, 1, 0, 0),
                                       new Vector4((float)Math.Sin(yRotation), 0, (float)Math.Cos(yRotation), 0),
                                       new Vector4(0, 0, 0, 1));

            
            worldTrans = myParent.getWorldMatrix(); // includes normal rotation and world trans from tank
            WorldM = worldRot * pivot * worldTrans;
            //teffect.World = WorldM;
            Direction = WorldM.Forward;

        }

        void keyInput(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            float n = 0f;
            // Turning
            if (keyboardState.IsKeyDown(Keys.E))
            {
                n += -1f;
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                n += 1f;
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                n *= 1f / 3f; // Precision Controls
            }
            yRotation += 2f * n * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (yRotation > MathHelper.Pi)
            {
                yRotation = -MathHelper.TwoPi + yRotation;
            }
            else if (yRotation < -MathHelper.Pi)
            {
                yRotation = MathHelper.TwoPi - yRotation;
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
        
    }
}
