using System.Collections.Generic;

public class DisplayableDream
{
    public DreamOption dream;
    public List<InterestCategory> orderedElements;
    public bool isSelected = false;

    public DisplayableDream(DreamOption dream, List<InterestCategory> ordered)
    {
        this.dream = dream;
        this.orderedElements = ordered;
    }
}
