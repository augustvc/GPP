using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Cloth
{
    static class Cloth
    {
        static float width = 10;
        static float height = 10;

        static int xRes = 70;
        static int yRes = 70;

        static Vector3[] positions = new Vector3[xRes * yRes];
        static Vector3[] velocities = new Vector3[xRes * yRes];

        static Cloth()
        {
            for(int x = 0; x < xRes; x++)
            {
                for (int y = 0; y < yRes; y++) {
                    positions[x + y * xRes] = new Vector3(((float)x / (float)(xRes)) * width, 0, ((float)y / (float)yRes) * height);
                    velocities[x + y * xRes] = new Vector3(0, 0, 0);
                }
            }
        }

        static float restLength = width / (float)xRes;
        static float restDiagonal = (float)Math.Sqrt(2 * restLength * restLength);
        static float structuralCoeff = 80f * xRes / width * yRes / height;
        static float shearCoeff = 3f * xRes / width * yRes / height;

        public static void Integrate(float dt, Vector3 blowDirection)
        {
            for(int x = 0; x < xRes; x++)
            {
                for(int y = 0; y < yRes; y++)
                {
                    if(y == 0)
                    {
                        //static vertex
                        continue;
                    }

                    int index = x + y * xRes;

                    Vector3 pos = positions[index];

                    velocities[index] += new Vector3(0, -10f, 0) * dt + blowDirection * 15f * dt;

                    List<int> lateralNeighbours = getLateralNeighbourIndices(index);
                    
                    for(int i = 0; i < lateralNeighbours.Count; i++)
                    {
                        Vector3 neighbourPos = positions[lateralNeighbours[i]];
                        float distDiff = (neighbourPos - pos).Length - restLength;
                        Vector3 dir = (neighbourPos - pos).Normalized();
                        Vector3 force = dir * distDiff * structuralCoeff;

                        velocities[index] += force * dt;
                    }

                    List<int> diagonalNeighbours = getDiagonalNeighbourIndices(index);

                    for(int i = 0; i < diagonalNeighbours.Count; i++)
                    {
                        Vector3 neighbourPos = positions[diagonalNeighbours[i]];
                        float distDiff = (neighbourPos - pos).Length - restLength;
                        Vector3 dir = (neighbourPos - pos).Normalized();
                        Vector3 force = dir * distDiff * shearCoeff;

                        velocities[index] += force * dt;
                    }

                    velocities[index] = velocities[index] * (1f - 0.4f * dt);
                }
            }

            for(int i = 0; i < positions.Length; i++)
            {
                positions[i] += velocities[i] * dt;
            }
        }

        public static float[] exportVertices()
        {
            float[] vertices = new float[(xRes - 1) * (yRes - 1) * 48];
            int i = 0;
            for(int x = 1; x < xRes; x++)
            {
                for (int y = 1; y < yRes; y++)
                {
                    int[] ids = {
                        id(x - 1, y - 1),
                        id(x, y),
                        id(x - 1, y),
                        id(x - 1, y - 1),
                        id(x, y),
                        id(x, y - 1)
                    };

                    Vector3 normal1 = Vector3.Cross(positions[ids[2]] - positions[ids[0]], positions[ids[1]] - positions[ids[0]]).Normalized();
                    Vector3 normal2 = -Vector3.Cross(positions[ids[5]] - positions[ids[0]], positions[ids[1]] - positions[ids[0]]).Normalized();
                    
                    for (int j = 0; j < ids.Length; j++)
                    {
                        Vector3 pos = positions[ids[j]];
                        for (int k = 0; k < 3; k++)
                        {
                            vertices[i++] = pos[k];
                        }

                        //tex coords:
                        vertices[i++] = ((float)ids[j]%xRes) / (xRes - 1);
                        vertices[i++] = 1f - (float)(ids[j]/xRes) / (float)(yRes - 1);

                        Vector3 normal = normal1;
                        if(j >= 3)
                        {
                            normal = normal2;
                        }

                        for(int k = 0; k < 3; k++)
                        {
                            vertices[i++] = normal[k];
                        }
                    }
                }
            }

            return vertices;
        }

        static int id(int x, int y)
        {
            return x + y * xRes;
        }

        static List<int> getLateralNeighbourIndices(int index)
        {
            int x = index % xRes;
            int y = index / xRes;
            List<int> neighbours = new List<int>();

            if(x > 0)
            {
                neighbours.Add(x - 1 + y * xRes);
            }
            if(x < xRes - 1)
            {
                neighbours.Add(x + 1 + y * xRes);
            }
            if(y > 0)
            {
                neighbours.Add(x + (y - 1) * xRes);
            }
            if(y < yRes - 1)
            {
                neighbours.Add(x + (y + 1) * xRes);
            }

            return neighbours;
        }

        static List<int> getDiagonalNeighbourIndices(int index)
        {
            int x = index % xRes;
            int y = index / xRes;
            List<int> neighbours = new List<int>();

            if (x > 0 && y > 0)
            {
                neighbours.Add(x - 1 + (y - 1) * xRes);
            }
            if (x < xRes - 1 && y < yRes - 1)
            {
                neighbours.Add(x + 1 + (y + 1) * xRes);
            }
            if(x > 0 && y < yRes - 1)
            {
                neighbours.Add(x - 1 + (y + 1) * xRes);
            }
            if (x < xRes - 1 && y > 0)
            {
                neighbours.Add(x + 1 + (y - 1) * xRes);
            }
            return neighbours;
        }
    }
}
