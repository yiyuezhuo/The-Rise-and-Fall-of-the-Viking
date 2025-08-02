using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    // The instance of CoreManager can be referenced by UITK dataSrouce.
    public class CoreManager
    {
        public GameState state = new();
        static CoreManager _instance = new();


        // Temp Settings
        public bool disableReshuffle = false;

        public static CoreManager Instance
        {
            get => _instance;
        }

        public EntityManager entityManager = new();

        public void LoadFromXml(string xml)
        {
            state = XmlUtils.FromXML<GameState>(xml);

            state.ResetAndRegisterAll();

            state.EnsureSetup();
        }
    }
}