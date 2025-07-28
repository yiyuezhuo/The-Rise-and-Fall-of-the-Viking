using Unity.Properties;

namespace GameCore
{
    public partial class AreaReference
    {
        [CreateProperty]
        public string name
        {
            get => GetArea()?.name ?? "[Not Specified or Invalid]";
        }
    }
}