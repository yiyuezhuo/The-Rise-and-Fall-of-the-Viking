
namespace GameCore
{
    public class FromToAreaReferenceParameter
    {
        public AreaReference from;
        public AreaReference to;
    }

    public partial class ResourceAssignParameter : FromToAreaReferenceParameter
    {
        public int assignResource; // Controlled by user.
        public int assignResourceLimit; // Limit assignResource domain.
        public float modifierCoef = 1f;
    }
}