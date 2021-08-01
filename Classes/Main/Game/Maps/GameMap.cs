using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Midtown.Classes.Main.Game.MapElements;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Linq;

namespace Midtown.Classes.Main.Game.Maps
{
    public class GameMap : GameElement
    {
        private TiledMap _tiledMap;
        public string Name { get; }
        public int Width => _tiledMap.WidthInPixels;
        public int Height => _tiledMap.HeightInPixels;
        public int WidthInTiles => _tiledMap.Width;
        public int HeightInTiles => _tiledMap.Height;
        public TiledMapRenderer Renderer;
        private readonly string _file;
        public List<CollisionRectangle> Collisions;
        public List<TeleportationZone> TeleportationZones;

        public GameMap(MainGame game, GameplayScene scene, string file, string name) : base(game, scene)
        {
            this._file = file;
            this.Name = name;
        }

        public override void Initialize()
        {
            this.Collisions = new List<CollisionRectangle>();
            this.TeleportationZones = new List<TeleportationZone>();

            base.Initialize();
        }

        private void LoadCollisions()
        {
            foreach (TiledMapObjectLayer layer in _tiledMap.ObjectLayers)
            {
                if (layer.Name.Contains("collisions"))
                {
                    foreach (TiledMapObject layerObject in layer.Objects)
                    {
                        this.Collisions.Add(new CollisionRectangle(this, new RectangleF(layerObject.Position, layerObject.Size)));
                    }
                }
            }
        }

        private void LoadTeleporters()
        {
            foreach (TiledMapObjectLayer layer in _tiledMap.ObjectLayers)
            {
                if (layer.Name.Contains("teleporters"))
                {
                    foreach (TiledMapObject layerObject in layer.Objects)
                    {
                        this.TeleportationZones.Add(
                            new TeleportationZone(this,
                                new RectangleF(layerObject.Position, layerObject.Size),
                                ((GameplayScene)Scene).Maps.Single(map => map.Name == layerObject.Properties["destination-map"]),
                                new Vector2(float.Parse(layerObject.Properties["destination-x"]), float.Parse(layerObject.Properties["destination-y"])),
                                bool.Parse(layerObject.Properties["transition"])));
                    }
                }
            }
        }

        public override void Load()
        {
            _tiledMap = MainGame.Content.Load<TiledMap>(_file);
            Renderer = new TiledMapRenderer(Game.GraphicsDevice, _tiledMap);

            LoadCollisions();
            LoadTeleporters();

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            Renderer.Update(gameTime);

            base.Update(gameTime);
        }

        public void DrawLayersByName(GameTime gameTime, SpriteBatch spriteBatch, string name)
        {
            this.DrawLayersByName(Matrix.Identity, gameTime, spriteBatch, name);
        }

        public void DrawLayersByName(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch, string name)
        {
            foreach (TiledMapLayer layer in _tiledMap.Layers)
            {
                if (layer.Name.Contains(name))
                {
                    Renderer.Draw(layer, matrix);
                }
            }

            base.Draw(gameTime, spriteBatch);
        }

        public void DrawForeground(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.DrawForeground(Matrix.Identity, gameTime, spriteBatch);
        }

        public void DrawForeground(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawLayersByName(matrix, gameTime, spriteBatch, "foreground");
        }

        public void DrawMiddleground(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.DrawMiddleground(Matrix.Identity, gameTime, spriteBatch);
        }

        public void DrawMiddleground(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawLayersByName(matrix, gameTime, spriteBatch, "middleground");
        }

        public void DrawBackground(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.DrawBackground(Matrix.Identity, gameTime, spriteBatch);
        }

        public void DrawBackground(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawLayersByName(matrix, gameTime, spriteBatch, "background");
        }

        public void DrawReflections(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.DrawReflections(Matrix.Identity, gameTime, spriteBatch);
        }

        public void DrawReflections(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawLayersByName(matrix, gameTime, spriteBatch, "reflections");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.Draw(Matrix.Identity, gameTime, spriteBatch);
        }

        public override void Draw(Matrix matrix, GameTime gameTime, SpriteBatch spriteBatch)
        {
            Renderer.Draw(matrix);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
