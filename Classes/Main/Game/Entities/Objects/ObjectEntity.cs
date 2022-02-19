using Microsoft.Xna.Framework;
using Midtown.Classes.Main.Game.Maps;
using Midtown.Classes.Utils;
using MonoGame.Extended;

namespace Midtown.Classes.Main.Game.Entities.Objects
{
    public class ObjectEntity : Entity
    {
        private AnimatedTexture2D sprite;
        private readonly string texturePath;

        public ObjectEntity(GameScreen scene, GameMap map, RectangleF bounds, Vector2 textureOffset, string texturePath) : base(scene, map, bounds, textureOffset, null, null, true, true)
        {
            this.texturePath = texturePath;
            HasShadow = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            sprite = new AnimatedTexture2D(Game, Screen, texturePath+".json", texturePath);
        }

        public override void Load()
        {
            base.Load();

            sprite.Load();
            sprite.Play("animation");
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sprite.Draw(gameTime, PixelPosition + TextureOffset);
        }

        public override void DrawReflection(GameTime gameTime)
        {
            sprite.DrawReflection(gameTime, PixelPosition + TextureOffset);
        }
    }
}
