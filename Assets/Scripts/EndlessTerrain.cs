using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
	const float viewerThresholdUpdate = 25f; 
	Vector2 viewerPosOld;
	const float sqrViewerThresholdUpdate = viewerThresholdUpdate * viewerThresholdUpdate;

	public LODInfo[] detailLevels;
	public static float maxViewDst;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInView;

    Dictionary<Vector2, TerrainChunk> terrainChunk = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> lastVisible = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;
        chunksVisibleInView = Mathf.RoundToInt(maxViewDst / chunkSize);

		UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
		if((viewerPosOld - viewerPosition).sqrMagnitude > sqrViewerThresholdUpdate)
		{
			viewerPosOld = viewerPosition;
			UpdateVisibleChunks();
		}
        
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < lastVisible.Count; i++)
        {
            lastVisible[i].SetVisible(false);
        }
        lastVisible.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInView; yOffset <= chunksVisibleInView; yOffset++)
        {
            for (int xOffset = -chunksVisibleInView; xOffset <= chunksVisibleInView; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunk.ContainsKey(viewedChunkCoord))
                {
                    terrainChunk[viewedChunkCoord].UpdateChunk();
                }
                else
                {
					terrainChunk.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 pos;
        Bounds bounds;

        MeshRenderer renderer;
        MeshFilter filter;

		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;

		MapData mapData;
		bool mapDataReceived;
		int prevLODIndex = -1;

		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material mat)
        {
			this.detailLevels = detailLevels;

            pos = coord * size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);

            meshObject = new GameObject("Terrain Chunk");
            renderer = meshObject.AddComponent<MeshRenderer>();
            filter = meshObject.AddComponent<MeshFilter>();
            renderer.material = mat;

            meshObject.transform.position = posV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

			lodMeshes = new LODMesh[detailLevels.Length];
			for(int i=0; i<detailLevels.Length; i++)
			{
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateChunk);
			}

            mapGenerator.RequestMapData(pos, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colors, MapGenerator.MAP_CHUNK_SIZE, MapGenerator.MAP_CHUNK_SIZE);
			renderer.material.mainTexture = texture;

			UpdateChunk();
        }


        public void UpdateChunk()
        {
			if(mapDataReceived)
			{
	            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
	            bool visible = viewerDstFromNearestEdge <= maxViewDst;

				if(visible)
				{
					int lodIndex = 0;

					for(int i=0; i< detailLevels.Length - 1; i++)
					{
						if(viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold) 
						{
							lodIndex = i + 1;
						} else
						{
							break;
						}
					}
					if(lodIndex != prevLODIndex)
					{
						LODMesh lodMesh = lodMeshes[lodIndex];
						if(lodMesh.hasMesh)
						{
							prevLODIndex = lodIndex;
							filter.mesh = lodMesh.mesh;
						}
						else if (!lodMesh.hasRequestedMesh)
						{
							lodMesh.RequestMesh(mapData);
						}
					}
					lastVisible.Add(this);
				}
				SetVisible(visible);
			}
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

	class LODMesh{
		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;
		System.Action updateCb;

		public LODMesh(int lod, System.Action cb)
		{
			this.lod = lod;	
			this.updateCb = cb;
		}

		void OnMeshDataReceived(MeshData meshData)
		{
			mesh = meshData.CreateMesh ();
			hasMesh = true;

			updateCb();
		}

		public void RequestMesh(MapData mapData)
		{
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData (mapData, lod, OnMeshDataReceived);
		}
	}

	[System.Serializable]
	public struct LODInfo{
		public int lod;
		public float visibleDstThreshold;
	}
}
