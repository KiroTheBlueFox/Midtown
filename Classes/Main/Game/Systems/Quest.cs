namespace Midtown.Classes.Main.Game.Systems
{
    public class Quest
    {
        public readonly string id;
        public readonly string Name;
        public readonly string Description;
        public readonly int Reward;

        public Quest(string id, string Name, string Description, int Reward)
        {
            this.id = id;
            this.Name = Name;
            this.Description = Description;
            this.Reward = Reward;
        }

        public Quest(QuestJson values)
        {
            this.id = values.id;
            this.Name = values.Name;
            this.Description = values.Description;
            this.Reward = values.Reward;
        }
    }

    public class QuestJson
    {
        public string Name;
        public string Description;
        public int Reward;
        public string id;
    }
}
