
namespace GameCore
{
    public class FromToAreaReferenceParameter
    {
        public AreaReference from;
        public AreaReference to;
    }

    public class ResourceAssignParameter : FromToAreaReferenceParameter
    {
        public int assignResource; // Controlled by user.
        public int assignResourceLimit; // Limit assignResource domain.
    }
}