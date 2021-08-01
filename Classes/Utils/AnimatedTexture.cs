using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main;
using Midtown.Classes.Main.Game.Entities;
using MonoGame.Extended;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Midtown.Classes.Utils
{
    public class AnimatedTexture : GameElement
    {
        private readonly string spriteFactoryPath, texturePath;
        private string animationName;
        public string AnimationName { get => animationName; set => Play(value); }
        private JObject spriteFactory;
        private double animationTime;
        private int numberOfFrames, columns, lines, currentFrame;
        private readonly Entity entity;

        public AnimatedTexture(MainGame game, Scene scene, string spriteFactoryPath, string texturePath, Entity entity) : base(game, scene)
        {
            this.spriteFactoryPath = spriteFactoryPath;
            this.texturePath = texturePath;
            animationTime = 0;
            this.entity = entity;
        }

        public override void Load()
        {
            var jsonPath = Path.Combine(Game.Content.RootDirectory, spriteFactoryPath);
            using (var stream = TitleContainer.OpenStream(jsonPath))
            {
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                spriteFactory = JObject.Parse(json);
            }
            CurrentTexture = Game.Content.Load<Texture2D>(texturePath);
            columns = CurrentTexture.Width / spriteFactory["textureAtlas"]["regionWidth"].ToObject<int>();
            lines = CurrentTexture.Height / spriteFactory["textureAtlas"]["regionHeight"].ToObject<int>();
        }

        public void Play(string animationName)
        {
            if (animationName != this.animationName)
            {
                numberOfFrames = spriteFactory["cycles"][animationName]["frames"].ToObject<int[]>().Length;
                currentFrame = 0;
                animationTime = 0;
                this.animationName = animationName;
            }
        }

        public override void Update(GameTime gameTime)
        {
            animationTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTime >= spriteFactory["cycles"][animationName]["frameDuration"].ToObject<double>())
            {
                animationTime = 0;
                currentFrame += 1;
                if (currentFrame >= numberOfFrames)
                {
                    if (spriteFactory["cycles"][animationName]["isLooping"].ToObject<bool>())
                        currentFrame = 0;
                    else
                        currentFrame = numberOfFrames - 1;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int frame = spriteFactory["cycles"][animationName]["frames"].ToObject<int[]>()[currentFrame];
            int x = frame % columns;
            int y = frame / columns;

            spriteBatch.Draw(CurrentTexture, entity.Bounds.Position,
                new Rectangle(x * spriteFactory["textureAtlas"]["regionWidth"].ToObject<int>(),
                    y * spriteFactory["textureAtlas"]["regionHeight"].ToObject<int>(),
                    spriteFactory["textureAtlas"]["regionWidth"].ToObject<int>(),
                    spriteFactory["textureAtlas"]["regionHeight"].ToObject<int>()), Color.White);

            base.Draw(gameTime, spriteBatch);
        }

        public void DrawReflection(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int frame = spriteFactory["cycles"][animationName]["frames"].ToObject<int[]>()[currentFrame];
            int x = frame % columns;
            int y = frame / columns;

            spriteBatch.Draw(CurrentTexture, ((RectangleF)entity.Bounds).BottomLeft,
                new Rectangle(x * spriteFactory["textureAtlas"]["regionWidth"].ToObject<int>(),
                    y * spriteFactory["textureAtlas"]["regionHeight"].ToObject<int>(),
                    spriteFactory["textureAtlas"]["regionWidth"].ToObject<int>(),
                    spriteFactory["textureAtlas"]["regionHeight"].ToObject<int>()), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0f);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
