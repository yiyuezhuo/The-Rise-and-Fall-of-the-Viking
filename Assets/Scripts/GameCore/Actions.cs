
using System.Linq;

namespace GameCore
{
    public class FromToAreaReferenceParameter
    {
        public AreaReference from;
        public AreaReference to;

        public int GetDistance()
        {
            var fromArea = from.GetArea();
            var path = fromArea.paths.First(p => p.toAreaObjectId == to.objectId);
            return path.cost;
        }
    }

    public partial class ResourceAssignParameter : FromToAreaReferenceParameter
    {
        public int assignResource; // Controlled by user.
        public int assignResourceLimit; // Limit assignResource domain.
        public float modifierCoef = 1f;
    }
}