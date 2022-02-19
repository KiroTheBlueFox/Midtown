using Microsoft.Xna.Framework.Content;

namespace Midtown.Classes.Utils
{
    public abstract class Registerable
    {
        public string ID { get; protected set; }
        public virtual void Initialize() { }
        public virtual void Load(ContentManager Content) { }
    }
}
