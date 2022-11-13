using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidSimulation
{
    public class FluidVisualiser : MonoBehaviour
    {
        [Header("Selection")]
        public float mousePower;
        public float mouseVelocityPower;
        public float distanceThreshold;

        [Header("Physics")]
        public float gravityRate = 9.8f;
        public float diffusionRate;

        [Header("Grid")]
        public int size;
        public FGridTile[] grid;

        private void Awake() {
            grid = new FGridTile[size*size];

       

        }

        private int getGridTile (int x, int y) {
            return x * size + y;
        }

        Vector3 lastMousePosition = Vector3.zero;

        private void Update() {

            // Appply Gravity
            // for (int y = 1; y < size-1; y++)
            // {
            //     for (int x = 1; x < size-1; x++)
            //     {
            //         grid[getGridTile(x, y)].velocity += Vector3.down * gravityRate * Time.deltaTime; // Multiply by dt later for velocity
            //         grid[getGridTile(x, y)].velocity = Vector3.ClampMagnitude(grid[getGridTile(x, y)].velocity, 1);
            //     }
            // }

            FluidSolver.SetBndsHorizontalVelocity(grid, size, 1);
            FluidSolver.SetBndsVerticalVelocity(grid, size, 2);

            if (Input.GetMouseButton(0)) {
                for (int x = 0; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        // Add Fluid to Point
                        Vector3 origin = Vector3.right * x / 2 + Vector3.up * y / 2;
                        if (Vector2.Distance(Camera.main.WorldToScreenPoint(origin), Input.mousePosition) <= distanceThreshold) {
                            grid[getGridTile(x, y)].density += mousePower * Time.deltaTime;
                            // grid[getGridTile(x, y)].density = Mathf.Clamp(grid[getGridTile(x, y)].density, 0f, 1f);
                        
                            if (lastMousePosition != Vector3.zero) {
                                grid[getGridTile(x, y)].velocity += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0)*Time.deltaTime*mouseVelocityPower;
                                grid[getGridTile(x, y)].velocity = Vector3.ClampMagnitude(grid[getGridTile(x, y)].velocity, 1);
                            }
                        }

                        lastMousePosition = Input.mousePosition;

                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) lastMousePosition = Vector2.zero;
            //SetBnds();
            FluidSolver.Diffuse(grid, size, Time.deltaTime, diffusionRate);

            FluidSolver.ClearDivergence(grid, size, Time.deltaTime);
            FluidSolver.Advect(grid, size, Time.deltaTime);
  
            FluidSolver.ClearDivergence(grid, size, Time.deltaTime);
      
        }


        private void OnDrawGizmos() {
            if (grid != null && grid.Length > 0)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        Vector3 origin = Vector3.right * x / 2 + Vector3.up * y / 2;
                        Gizmos.color = Color.Lerp(new Color(0,0,0,0), Color.cyan, grid[getGridTile(x,y)].density);
                        Gizmos.DrawWireCube(origin, Vector3.one / 2.2f);
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawRay(origin,  (grid[getGridTile(x, y)].velocity.normalized*0.3f));
                    }
                }
            }
        }

    }
}
