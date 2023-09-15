using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
public class DrawGiz : MonoBehaviour
{
    [System.Serializable]
    public class dimentions
    {
        public Vector3 position;
        public Vector3 offset;
        public Vector3 direction;
        public float range;
        public float angle;
    }

    
    public Color color;
    public dimentions _dimentions;
    public enum type
    {
        Rect,Sphere, Disc, Arc
    };

    public type Type;


    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Handles.color = color;


        switch (Type)
        {
            case type.Rect:

                Gizmos.DrawCube(transform.position + _dimentions.offset, new Vector3(_dimentions.range, _dimentions.range, _dimentions.range));
                break;

            case type.Sphere:
                Gizmos.DrawSphere(transform.position + _dimentions.offset, _dimentions.range);
                break;

            case type.Disc:
                if(_dimentions.direction != null)
                Handles.DrawSolidDisc(transform.position + _dimentions.offset, _dimentions.direction, _dimentions.range);
                break;
            case type.Arc:
                if (_dimentions.direction != null)
                    Handles.DrawSolidArc(transform.position + _dimentions.offset, _dimentions.direction, _dimentions.position, _dimentions.angle, _dimentions.range);

                    break;
        }


    }

}
#endif

