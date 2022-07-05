using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


// Adam Sanders

namespace Tankgame1
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        Game1 myGame;
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Direction { get; set; } = Vector3.Forward; 
        public Vector3 Up { get; } = Vector3.Up;

        public float FieldOfView { get; set; } = MathHelper.PiOver4;
        public float FocalLength { get; set; } = 1f;
        public float AspectRatio { get; set; }
        public float FisheyFactor { get; set; } = 1f;
        float fishStorage = -1f;
        float nearPlane = 2f;
        float farPlane = 800f;


        public Camera(Game1 game, Vector3 pos, Vector3 target, Vector3 up)
                : base(game)
        {
            myGame = game;
            Position = pos;
            Up = up;
            Direction = target - pos;
            FisheyFactor = 1f;
            fishStorage = -1f;
            AspectRatio = (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height;
            FieldOfView = MathHelper.PiOver2;
            FocalLength = 1f;
            renewCameraProj();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public Vector4 getCamPos4()
        {
            return new Vector4(Position, 1f);
        }
        public Vector4 getCamDir4()
        {
            return new Vector4(Direction, 1f);
        }
        public Vector4 getViewVector4()
        {
            Vector3 v = Vector3.Transform(Direction - Position, Matrix.CreateRotationY(0));
            return new Vector4(v, 1f);
        }
        public void setView()
        {
            View = Matrix.CreateLookAt(Position, Position + Direction, Up);
        }
        public float getFishFactor()
        {
            return FisheyFactor;
        }
        public void toggleLens()
        {
            //myGame.fishEye = f == f ? f : myGame.fishEye;
            if (myGame.fishEye == 1)
            { // Turns off fisheye
                fishStorage = FisheyFactor;
                FisheyFactor = 1;
                myGame.fishEye = 0;
                FieldOfView = MathHelper.PiOver2;
                renewCameraProj();
            } // Shader changes to default
            else
            { // Turns on fisheye
                FisheyFactor = fishStorage;
                myGame.fishEye = 1;
                FieldOfView = MathHelper.PiOver2;
                renewCameraProj();
            } // Shader changes to fisheye
        }
        public void renewCameraProj()
        {
            View = Matrix.CreateLookAt(Position, Position + Direction, Up);
            //float k = MathHelper.PiOver2;
            //if (myGame.fishEye == 1)
            //    k = MathHelper.Pi;
            FieldOfView = Math.Clamp(FieldOfView, MathHelper.Pi / 8, myGame.floatLerp(MathHelper.Pi, MathHelper.PiOver2, myGame.CustomNormalize(FisheyFactor, -1, 1)));
            FocalLength = Math.Clamp(FocalLength, 0.2f, 5f);
            switch (myGame.fishEye)
            {
                case 0:
                    Projection = ArtisinalProjectionMatrix(FieldOfView, AspectRatio, nearPlane, farPlane, FocalLength);
                    break;
                case 1:
                    Projection = ArtisinalProjectionMatrix(FieldOfView, AspectRatio, nearPlane, farPlane, FocalLength);
                    break;
                default:
                    Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, nearPlane, farPlane);
                    break;
            }
            //MathHelper.PiOver4
            //FieldOfView
        }
        public void resetVals()
        {
            myGame.worldShader = myGame.shaderLightNormal;
            myGame.fishEye = 0;
            myGame.camMode = 1;
            FieldOfView = MathHelper.PiOver4;
            FocalLength = 1f;
            FisheyFactor = -1f;
            AspectRatio = (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height;
            renewCameraProj();
        }

        Matrix ArtisinalProjectionMatrix(float fov = MathHelper.PiOver2, float aspRatio = 16/9,
                                         float nearPlane = 1f, float farPlane = 300f, float focalLength = 1f)
        {
            Matrix fish = Matrix.CreatePerspectiveFieldOfView(fov, aspRatio, nearPlane, farPlane);
            fish.M11 = ptgui11Fisheye(fov / 2, focalLength) / aspRatio;
            fish.M22 = ptgui11Fisheye(fov / 2, focalLength);
            //switch (myGame.fishEye)
            //{
            //    case 0: // Normal View
            //        //fish = new Matrix(ptgui11Fisheye(fov / 2, focalLength) / aspRatio, 0, 0, 0,
            //        //                    0, ptgui11Fisheye(fov / 2, focalLength), 0, 0,
            //        //                    0, 0, -(farPlane / (farPlane - nearPlane)),            /*2.2,  1.1, 1.00001  */            -1,
            //        //                    0, 0, -(farPlane / (farPlane - nearPlane)) * nearPlane,/*26.4, 2.2, 0.100001  */             0);
            //        fish = new Matrix((focalLength * (float)Math.Tan(fov / 2)) / aspRatio, 0, 0, 0,
            //                            0, (focalLength * (float)Math.Tan(fov / 2)), 0, 0,
            //                            0, 0, -(farPlane / (farPlane - nearPlane)), -1,
            //                            0, 0, -(farPlane / (farPlane - nearPlane)) * nearPlane, 0);
            //        break;
            //    case 1: // Fishey (focalLength / (float)Math.Sin(fov / 2)) * 1 / aspRatio
            //        fish = new Matrix(ptgui11Fisheye(fov / 2, focalLength) / aspRatio, 0, 0, 0,
            //                            0, ptgui11Fisheye(fov / 2, focalLength), 0, 0,
            //                            0, 0, -(farPlane / (farPlane - nearPlane)),            /*2.2,  1.1, 1.00001  */            -1,
            //                            0, 0, -(farPlane / (farPlane - nearPlane)) * nearPlane,/*26.4, 2.2, 0.100001  */             0);
            //        //fish = Matrix.CreateOrthographic(90 * aspRatio / focalLength, 90 / focalLength, nearPlane, farPlane);

            //        //fish = new Matrix(ptgui11Fisheye(FisheyFactor, fov / aspRatio, focalLength), 0, 0, 0,
            //        //                    0, ptgui11Fisheye(FisheyFactor, fov, focalLength), 0, 0,
            //        //                    0, 0, -(farPlane / (farPlane - nearPlane)),            /*2.2,  1.1, 1.00001  */            -1,
            //        //                    0, 0, -(farPlane / (farPlane - nearPlane)) * nearPlane,/*26.4, 2.2, 0.100001  */             0);
            //        // ptgui11Fisheye(FisheyFactor, fov, focalLength);
            //        break;
            //    default:
            //        fish = Matrix.CreatePerspectiveFieldOfView(fov, aspRatio, nearPlane, farPlane);
            //        break;
            //}
            return fish;
        }

        float ptgui11Fisheye(float thetaFov = MathHelper.PiOver2/2, float focalL = 1f)
        {
            //thetaFov = myGame.floatLerp(thetaFov / 2f, thetaFov, myGame.CustomNormalize(FisheyFactor, -1, 1));
            float n = FisheyFactor * thetaFov;
            FisheyFactor = Math.Clamp(FisheyFactor, -1, 1);
            float r = FisheyFactor == 0 ? focalL * thetaFov :
                    /*FisheyFactor != 0*/ (focalL / FisheyFactor) * (FisheyFactor < 0 ? (float)Math.Sin(n) :
                                                                   /*FisheyFactor > 0*/ (float)Math.Tan(n));
            
            return r * myGame.floatLerp(1f, 1f, myGame.CustomNormalize(FisheyFactor, -1, 1));
            //float r = thetaFov * focalL;
            //FisheyFactor = Math.Clamp(FisheyFactor, -1, 1);
            //if (FisheyFactor < 0)
            //{
            //    r = focalL * ((float)Math.Sin(FisheyFactor * thetaFov)) / FisheyFactor;
            //}
            //else if (FisheyFactor > 0)
            //{
            //    r = focalL * ((float)Math.Tan(FisheyFactor * thetaFov)) / FisheyFactor;
            //}
            //return r;
        }

        // Projection Math
        // Vector3(x, y, z) => ... => screen(x, y)
        // FocalLength  = HalfScreenDimension(1) / Tan(FOV / 2)
        // FOV          = Tan-1(HalfScreenDimension(1)/FL) * 2
        // Aspect       = screenWidth/screenHeight
        //
        // Where P is a point position in the world and vp is the view projection of that point.
        // P.X * 1 / tan(fov/2) = vp.X
        // P.Y * 1 / tan(fov/2) = vp.Y
        // P.Z / (farplane-nearplane) - nearplane = vp.Z(Depth0-1)
        //                 -P.Z = vp.W
        // vp(x, y, z, w)
        // vp.x/vp.w range{-1, 1}
        // vp.y/vp.w range{-1, 1}
        // vp.z/vp.w range{ 0, 1}
        //
        // Projection Matrix where n is the near plane and f is the far plane
        // { 1/tan(fov/2),       0       ,      0     ,     0    }
        // {    0        , 1 / tan(fov/2),      0     ,     0    }
        // {    0        ,       0       ,   f/(f-n)  ,     1    }
        // {    0        ,       0       , -nf/(f-n)  ,     0    }

    }
}
