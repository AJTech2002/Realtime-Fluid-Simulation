using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidSimulation
{
    public class FluidSolver
    {
        private static int g (int x, int y, int size) {
            x = Mathf.Clamp(x, 0, size - 1);
            y = Mathf.Clamp(y, 0, size - 1);
            return x * size + y;
        }

        public static void Diffuse (FGridTile[] grid, int size, float dt, float diffusionRate) {
            float a = dt * diffusionRate * size * size;

            float[] newDensValues = new float[grid.Length];
            Vector3[] newVelocityValues = new Vector3[grid.Length];
            
            // Gauss Seidel Solver with 20 Iterations
            for (int k = 0; k < 20; k++) {
                for (int i = 1; i < size-1; i++) {
                    for (int j = 1; j < size-1; j++) {
                        int current = g(i, j, size);

                        int left = g(i-1, j, size);
                        int right = g(i+1, j, size);
                        int down = g(i, j-1, size);
                        int up = g(i, j+1, size);

                        newDensValues[current] = (grid[current].density + a * (newDensValues[left] + newDensValues[right] + newDensValues[up] + newDensValues[down])/4) / (1 + a);
                        newVelocityValues[current] = (grid[current].velocity + a * (newVelocityValues[left] + newVelocityValues[right] + newVelocityValues[up] + newVelocityValues[down])/4) / (1 +  a);
                    }
                }

                
                SetBndsDensity(grid, size, 0);
                SetBndsHorizontalVelocity(grid, size, 1);
                SetBndsVerticalVelocity(grid, size, 2);
            }

            for (int i = 0; i < newDensValues.Length; i++) {
                grid[i].density = newDensValues[i];
                grid[i].velocity = newVelocityValues[i];
            }

        }
        
        public static void SetBnds (float[] grid, int size, int b) {
            int N = size - 2;

            for (int i = 1; i < size - 1; i++) {
                //Set Wall Values to their nearest neighbour


                grid[g(0, i, size)] = b == 1 ? -grid[g(1, i, size)] : grid[g(1, i, size)];
                grid[g(N+1, i, size)] = b == 1 ? -grid[g(N, i, size)] : grid[g(N, i, size)];
                grid[g(i,0, size)] = b == 2 ? -grid[g(i, 1, size)] : grid[g(i, 1, size)];
                grid[g(i, N+1, size)] = b == 2 ? -grid[g(i, N, size)] : grid[g(i, N, size)];
                

            }
            
            // Set Corners to the average of two nearby squares
            grid[g(0, 0, size)] = 0.5f * (grid[g(1, 0,size)] + grid[g(0, 1,size)]);
            grid[g(0, N+1, size)] = 0.5f * (grid[g(1, N+1,size)] + grid[g(0, N,size)]);
            grid[g(N+1, 0, size)] = 0.5f * (grid[g(N, 0,size)] + grid[g(N+1, 1,size)]);
            grid[g(N+1, N+1, size)] = 0.5f * (grid[g(N, N+1,size)] + grid[g(N+1, N,size)]);
        }

        public static void SetBndsDensity (FGridTile[] grid, int size, int b) {

            for (int i = 1; i < size - 1; i++) {
                //Set Wall Values to their nearest neighbour
                grid[g(0, i, size)].density = b == 1 ? -grid[g(1, i, size)].density : grid[g(1, i, size)].density;
                grid[g(size-1, i, size)].density = b == 1 ? -grid[g(size-2, i, size)].density : grid[g(size-2, i, size)].density;
                grid[g(i,0, size)].density = b == 2 ? -grid[g(i, 1, size)].density : grid[g(i, 1, size)].density;
                grid[g(i, size-1, size)].density = b == 2 ? -grid[g(i, size-2, size)].density : grid[g(i, size-2, size)].density;
                

            }
            
            // Set Corners to the average of two nearby squares
            // grid[g(0, 0, size)].density = 0.5f * (grid[g(1, 0,size)].density + grid[g(0, 1,size)].density);
            // grid[g(0, size-1, size)].density = 0.5f * (grid[g(1, size-1,size)].density + grid[g(0, size-2,size)].density);
            // grid[g(size-1, 0, size)].density = 0.5f * (grid[g(size-2, 0,size)].density + grid[g(size-1, 1,size)].density);
            // grid[g(size-1, size-1, size)].density = 0.5f * (grid[g(size-2, size-1,size)].density + grid[g(size-1, size-2,size)].density);
        }

        public static void SetBndsHorizontalVelocity (FGridTile[] grid, int size, int b) {

            for (int i = 1; i < size - 1; i++) {
                //Set Wall Values to their nearest neighbour
                grid[g(0, i, size)].velocity.x = b == 1 ? -grid[g(1, i, size)].velocity.x : grid[g(1, i, size)].velocity.x;
                grid[g(size-1, i, size)].velocity.x = b == 1 ? -grid[g(size-2, i, size)].velocity.x : grid[g(size-2, i, size)].velocity.x;
                grid[g(i,0, size)].velocity.x = b == 2 ? -grid[g(i, 1, size)].velocity.x : grid[g(i, 1, size)].velocity.x;
                grid[g(i, size-1, size)].velocity.x = b == 2 ? -grid[g(i, size-2, size)].velocity.x : grid[g(i, size-2, size)].velocity.x;
                

            }
            
            // Set Corners to the average of two nearby squares
            grid[g(0, 0, size)].velocity.x = 0.5f * (grid[g(1, 0,size)].velocity.x + grid[g(0, 1,size)].velocity.x);
            grid[g(0, size-1, size)].velocity.x = 0.5f * (grid[g(1, size-1,size)].velocity.x + grid[g(0, size-2,size)].velocity.x);
            grid[g(size-1, 0, size)].velocity.x = 0.5f * (grid[g(size-2, 0,size)].velocity.x + grid[g(size-1, 1,size)].velocity.x);
            grid[g(size-1, size-1, size)].velocity.x = 0.5f * (grid[g(size-2, size-1,size)].velocity.x + grid[g(size-1, size-2,size)].velocity.x);
        }

        public static void SetBndsVerticalVelocity (FGridTile[] grid, int size, int b) {

            for (int i = 1; i < size - 1; i++) {
                //Set Wall Values to their nearest neighbour
                grid[g(0, i, size)].velocity.y = b == 1 ? -grid[g(1, i, size)].velocity.y : grid[g(1, i, size)].velocity.y;
                grid[g(size-1, i, size)].velocity.y = b == 1 ? -grid[g(size-2, i, size)].velocity.y : grid[g(size-2, i, size)].velocity.y;
                grid[g(i,0, size)].velocity.y = b == 2 ? -grid[g(i, 1, size)].velocity.y : grid[g(i, 1, size)].velocity.y;
                grid[g(i, size-1, size)].velocity.y = b == 2 ? -grid[g(i, size-2, size)].velocity.y : grid[g(i, size-2, size)].velocity.y;
                

            }
            
            // Set Corners to the average of two nearby squares
            grid[g(0, 0, size)].velocity.y = 0.5f * (grid[g(1, 0,size)].velocity.y + grid[g(0, 1,size)].velocity.y);
            grid[g(0, size-1, size)].velocity.y = 0.5f * (grid[g(1, size-1,size)].velocity.y + grid[g(0, size-2,size)].velocity.y);
            grid[g(size-1, 0, size)].velocity.y = 0.5f * (grid[g(size-2, 0,size)].velocity.y + grid[g(size-1, 1,size)].velocity.y);
            grid[g(size-1, size-1, size)].velocity.y = 0.5f * (grid[g(size-2, size-1,size)].velocity.y + grid[g(size-1, size-2,size)].velocity.y);
        }


        public static void Advect (FGridTile[] grid, int size, float dt) {
            
            float dt0 = dt * size;

            for (int i = 1; i < size - 1; i++)
            {
                for (int j = 1; j < size - 1; j++)
                {
                    int current = g(i, j, size);

                    float x = i - dt0 * grid[current].velocity.x;
                    float y = j - dt0 * grid[current].velocity.y;

                    if (x < 0.5f) x = 0.5f; if (y < 0.5f) y = 0.5f;
                    if (x > size + 0.5f) x = size + 0.5f; if (y > size + 0.5f) y = size + 0.5f;
                  
                    // Neighbours of where the velocity backtrace ended
                    int i0 = (int)x;
                    int i1 = i0 + 1;
                    int j0 = (int)y;
                    int j1 = j0 + 1;

                    // Finding fractionally how much of each of the neighbours to affect
                    float s1 = x - i0;
                    float s0 = 1 - s1;
                    float t1 = y - j0;
                    float t0 = 1 - t1;

                    float left = s0 * (t0 * grid[g(i0, j0, size)].density + t1 * grid[g(i0, j1, size)].density);
                    float right = s1 * (t0 * grid[g(i1, j0, size)].density + t1 * grid[g(i1, j1, size)].density);
                    grid[current].density = left + right;

                    Vector3 leftV = s0 * (t0 * grid[g(i0, j0, size)].velocity + t1 * grid[g(i0, j1, size)].velocity);
                    Vector3 rightV = s1 * (t0 * grid[g(i1, j0, size)].velocity + t1 * grid[g(i1, j1, size)].velocity);
                    grid[current].velocity = leftV + rightV;
                }
            }

            SetBndsDensity(grid, size, 0);
            SetBndsHorizontalVelocity(grid, size, 1);
            SetBndsVerticalVelocity(grid, size, 2);
        }

        public static void ClearDivergence(FGridTile[] grid, int size, float dt)
        {
            float h = 1.0f / size;

            float[] p = new float[size * size];            
            float[] divs = new float[size * size];

            for (int i = 1; i < size - 1; i++)
            {
                for (int j = 1; j < size - 1; j++)
                {
                    int left = g(i-1, j, size);
                    int right = g(i+1, j, size);
                    int down = g(i, j-1, size);
                    int up = g(i, j+1, size);
                    
                    // Calculate current divergence
                    float div = h * -0.5f *(grid[right].velocity.x - grid[left].velocity.x +  grid[up].velocity.y - grid[down].velocity.y);
                    divs[g(i, j,size)] = div;
                }
            }

            SetBnds(divs, size, 0);
            SetBnds(p, size, 0);

            // Gauss Seidel Solver with 20 Iterations
            for (int k = 0; k < 20; k++)
            {
                for (int i = 1; i < size - 1; i++)
                {
                    for (int j = 1; j < size - 1; j++)
                    {
                        // Not sure what this part is doing

                        p[g(i, j, size)] = (divs[g(i, j, size)] + p[g(i - 1, j, size)] + p[g(i + 1, j, size)] + p[g(i, j - 1, size)] + p[g(i, j + 1, size)])/4;
                    }
                }

                SetBnds(p, size, 0);
            }
            
            for (int i = 1; i < size - 1; i++)
            {
                for (int j = 1; j < size - 1; j++)
                {
                    // Not sure what this part is doing
                    Vector3 curVel = grid[g(i, j,size)].velocity;
                    Vector3 dif = new Vector3();
                    dif.x = 0.5f * (p[g(i + 1, j,size)] - p[g(i - 1, j, size)]) / h;
                    dif.y = 0.5f * (p[g(i, j+1,size)] - p[g(i, j-1, size)]) / h;
                 
                    grid[g(i, j, size)].velocity -= dif;
                }
            }

            SetBndsHorizontalVelocity(grid, size, 1);
            SetBndsVerticalVelocity(grid, size, 2);
        }
    }
}
