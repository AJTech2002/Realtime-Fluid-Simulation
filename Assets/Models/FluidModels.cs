using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidSimulation 
{
    [System.Serializable]
    public struct FGridTile {
        [Range(0f,1f)]
        public float density;
        public Vector3 velocity;
    }
    
}
