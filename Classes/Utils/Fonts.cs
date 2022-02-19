using FontStashSharp;
using System.Collections.Generic;
using System.IO;

namespace Midtown.Classes.Utils
{
    public class Fonts
    {
        public Dictionary<string, FontSystem> FontSystems { get; private set; }
        public static readonly string Normal = "Normal";
        public static readonly string Bold = "Bold";
        public static readonly string NormalSmall = "NormalSmall";
        public static readonly string BoldSmall = "BoldSmall";
        private byte[] NormalFile, BoldFile, NormalSmallFile, BoldSmallFile;
        private Dictionary<string, byte[]> FontFiles;

        public MainGame Game { get; }

        public Fonts(MainGame game)
        {
            Game = game;

            FontSystems = new Dictionary<string, FontSystem>();
            FontFiles = new Dictionary<string, byte[]>();
        }

        public void Load()
        {
            NormalFile = File.ReadAllBytes(Game.Content.RootDirectory + @"/Fonts/PixelOperator.ttf");
            BoldFile = File.ReadAllBytes(Game.Content.RootDirectory + @"/Fonts/PixelOperator-Bold.ttf");
            NormalSmallFile = File.ReadAllBytes(Game.Content.RootDirectory + @"/Fonts/PixelOperator8.ttf");
            BoldSmallFile = File.ReadAllBytes(Game.Content.RootDirectory + @"/Fonts/PixelOperator8-Bold.ttf");

            FontFiles.Add(Normal, NormalFile);
            FontFiles.Add(Bold, BoldFile);
            FontFiles.Add(NormalSmall, NormalSmallFile);
            FontFiles.Add(BoldSmall, BoldSmallFile);

            FontSystems[Normal] = new FontSystem();
            FontSystems[Normal].AddFont(NormalFile);

            FontSystems[Bold] = new FontSystem();
            FontSystems[Bold].AddFont(BoldFile);

            FontSystems[NormalSmall] = new FontSystem();
            FontSystems[NormalSmall].AddFont(NormalSmallFile);

            FontSystems[BoldSmall] = new FontSystem();
            FontSystems[BoldSmall].AddFont(BoldSmallFile);
        }

        public FontSystem this[string name]
        {
            get => FontSystems[name];
        }

        public FontSystem this[string name, FontSystemSettings settings]
        {
            get {
                FontSystem font = new FontSystem(settings);
                font.AddFont(FontFiles[name]);
                return font;
            }
        }
    }
}
