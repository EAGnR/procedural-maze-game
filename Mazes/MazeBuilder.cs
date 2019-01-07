using System.Collections.Generic;
using UnityEngine;

namespace Mazes
{
    /// <summary>
    /// WIP
    /// </summary>
    public class MazeBuilder : MonoBehaviour
    {
        public List<GameObject> prefabs;
        public int rows;
        public int cols;

        private readonly float mazeTileOffset = 4.00068f;
        private readonly float ceilingHeight = 3.75f;
        private readonly float floorHeight = 0.00f;
        private readonly float wallHeight = 1.87478f;
        private readonly float wallZAngle = 90.0f;

        private enum PrefabType {CeilingPrefab, FloorPrefab, WallPrefab};

        private void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
         
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            MazeGenerator maze = new MazeGenerator(rows, cols);
            maze.GenerateMaze();

            int prefabIndex;
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            float wallYAngle;
            Transform transform = null;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    prefabIndex = (int) PrefabType.FloorPrefab;
                    position = new Vector3(position.x, floorHeight, position.z);
                    Instantiate(prefabs[prefabIndex], position, Quaternion.identity);

                    prefabIndex = (int) PrefabType.CeilingPrefab;
                    position = new Vector3(position.x, ceilingHeight, position.z);
                    //Instantiate(prefabs[prefabIndex], position, Quaternion.identity);

                    prefabIndex = (int) PrefabType.WallPrefab;
                    position = new Vector3(position.x, wallHeight, position.z);

                    if (maze.GetGridCell((i * cols) + j).leftWall)
                    {
                        wallYAngle = 0.0f;
                        transform = Instantiate(prefabs[prefabIndex], position, Quaternion.identity).transform;
                        transform.Rotate(transform.rotation.x, wallYAngle, wallZAngle);
                    }

                    if (maze.GetGridCell((i * cols) + j).aboveWall)
                    {
                        wallYAngle = 90.0f;
                        transform = Instantiate(prefabs[prefabIndex], position, Quaternion.identity).transform;
                        transform.Rotate(transform.rotation.x, wallYAngle, wallZAngle);
                    }

                    if (maze.GetGridCell((i * cols) + j).rightWall)
                    {
                        wallYAngle = 180.0f;
                        transform = Instantiate(prefabs[prefabIndex], position, Quaternion.identity).transform;
                        transform.Rotate(transform.rotation.x, wallYAngle, wallZAngle);
                    }

                    if (maze.GetGridCell((i * cols) + j).belowWall)
                    {
                        wallYAngle = 270.0f;
                        transform = Instantiate(prefabs[prefabIndex], position, Quaternion.identity).transform;
                        transform.Rotate(transform.rotation.x, wallYAngle, wallZAngle);
                    }
                    
                    position = new Vector3(position.x + mazeTileOffset, 0.0f, position.z);
                }

                position = new Vector3(0.0f, 0.0f, position.z - mazeTileOffset);
            }
        }
    }
}
