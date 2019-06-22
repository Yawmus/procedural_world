using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void Start()
    {
        GameObject.Find("Mesh").SetActive(false);
    }

    public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

    public void DrawMesh(MeshData mesh, Texture2D texture)
    {
        meshFilter.sharedMesh = mesh.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
