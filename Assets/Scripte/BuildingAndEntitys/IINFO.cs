using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;//new network tut

//change to network
public abstract class IINFO : NetworkBehaviour
{
    [System.Serializable]
    public class CreateInfo
    {
        public string Name;
        public Sprite icon;
        public float ProcessTime;
        public int Gold, Stone, Wood, Food;
    }

    public CreateInfo costInfo = new CreateInfo();

    //new (movment tut)
    public byte ID;


}
