using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBuilding : IINFO
{

    public int buildingHealth, buildingCurrentHealth;

    public abstract void setBuilding(StadtCentrum sc);
    public abstract void onDestoryBuilding(StadtCentrum sc);


}
