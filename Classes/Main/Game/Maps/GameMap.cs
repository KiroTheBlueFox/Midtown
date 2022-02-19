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
        public float TileSize => (_tiledMap.TileHeight + _tiledMap.TileWidth) / 2f;
        public Tile[,] Tiles;
        public TiledMapRenderer Renderer;
        private readonly string _file;
        public List<CollisionRectangle> Collisions;
        public List<TeleportationZone> TeleportationZones;

        public GameMap(MainGame game, GameScreen scene, string file, string name) : base(game, scene)
        {
            _file = file;
            Name = name;
        }

        public override void Initialize()
        {
            Collisions = new List<CollisionRectangle>();
            TeleportationZones = new List<TeleportationZone>();

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
                        Collisions.Add(new CollisionRectangle(this, new RectangleF(layerObject.Position, layerObject.Size)));
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
                        TeleportationZones.Add(
                            new TeleportationZone(this,
                                new RectangleF(layerObject.Position, layerObject.Size),
                                ((GameScreen)Screen).Maps.Single(map => map.Name == layerObject.Properties["destination-map"]),
                                new Vector2(float.Parse(layerObject.Properties["destination-x"]), float.Parse(layerObject.Properties["destination-y"])),
                                bool.Parse(layerObject.Properties["transition"])));
                    }
                }
            }
        }

        public override void Load()
        {
            _tiledMap = Game.Content.Load<TiledMap>(_file);
            Renderer = new TiledMapRenderer(Game.GraphicsDevice, _tiledMap);
            Tiles = new Tile[_tiledMap.Width,_tiledMap.Height];

            LoadCollisions();
            LoadTeleporters();

            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            Renderer.Update(gameTime);

            base.Update(gameTime);
        }

        public void DrawLayersByName(GameTime gameTime, string name)
        {
            DrawLayersByName(Matrix.Identity, gameTime, name);
        }

        public void DrawLayersByName(Matrix matrix, GameTime gameTime, string name)
        {
            foreach (TiledMapLayer layer in _tiledMap.Layers)
            {
                if (layer.Name.Contains(name))
                {
                    Renderer.Draw(layer, matrix);
                }
            }

            base.Draw(gameTime);
        }

        public void DrawForeground(GameTime gameTime)
        {
            DrawForeground(Matrix.Identity, gameTime);
        }

        public void DrawForeground(Matrix matrix, GameTime gameTime)
        {
            DrawLayersByName(matrix, gameTime, "foreground");
        }

        public void DrawMiddleground(GameTime gameTime)
        {
            DrawMiddleground(Matrix.Identity, gameTime);
        }

        public void DrawMiddleground(Matrix matrix, GameTime gameTime)
        {
            DrawLayersByName(matrix, gameTime, "middleground");
        }

        public void DrawBackground(GameTime gameTime)
        {
            DrawBackground(Matrix.Identity, gameTime);
        }

        public void DrawBackground(Matrix matrix, GameTime gameTime)
        {
            DrawLayersByName(matrix, gameTime, "background");
        }

        public void DrawReflections(GameTime gameTime)
        {
            DrawReflections(Matrix.Identity, gameTime);
        }

        public void DrawReflections(Matrix matrix, GameTime gameTime)
        {
            DrawLayersByName(matrix, gameTime, "reflections");
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(Matrix.Identity, gameTime);
        }

        public override void Draw(Matrix matrix, GameTime gameTime)
        {
            Renderer.Draw(matrix);

            base.Draw(gameTime);
        }
    }
}
