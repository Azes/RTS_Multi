using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : IBuilding
{
    public int populationSpace;

    public override void setBuilding(StadtCentrum s)
    {

        s.MaxPopulation += populationSpace;
    }

    public override void onDestoryBuilding(StadtCentrum sc)
    {

        sc.MaxPopulation -= populationSpace;

    }

}
