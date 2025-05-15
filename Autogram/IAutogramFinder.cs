namespace Autogram
{
    public interface IAutogramFinder
    {
        Status Iterate();
        void Reset(bool resetRandom = true);
    }
}