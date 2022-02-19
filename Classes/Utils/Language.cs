﻿
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Midtown.Classes.Utils
{
    public class Language
    {
        public MainGame Game { get; private set; }
        public string Name { get; private set; }
        public Dictionary<string, string> Langs { get; private set; }
        public string this[string lang]
        {
            get
            {
                if (Langs.ContainsKey(lang))
                    return Langs[lang];
                else
                    return lang;
            }
        }
        private string filePath;

        public Language(MainGame game, string name, string filePath)
        {
            Game = game;
            Name = name;
            this.filePath = filePath;
        }

        public void Load()
        {
            var paramsPath = Path.Combine(Game.Content.RootDirectory, filePath + ".json");
            using var stream = TitleContainer.OpenStream(paramsPath);
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            JObject langJSON = JObject.Parse(json);

            Langs = new Dictionary<string, string>();
            foreach (var langValue in langJSON)
            {
                Langs[langValue.Key] = langValue.Value.ToString();
            }
        }
    }
}
