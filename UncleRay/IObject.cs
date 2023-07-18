namespace UncleRay;

public interface IObject
{
    public bool Hit(Ray r, float minT, float maxT, out HitData hit);
}

public class ObjectList : List<IObject>, IObject
{
    public bool Hit(Ray r, float minT, float maxT, out HitData hit)
    {
        hit = default;
        bool haveHit = false;
        float closest = maxT;

        foreach(var obj in this)
        {
            if (obj.Hit(r, minT, closest, out HitData tempHit))
            {
                haveHit = true;
                hit = tempHit;
                closest = hit.T;

                // unordered list so don't break
            }
        }

        return haveHit;
    }
}
