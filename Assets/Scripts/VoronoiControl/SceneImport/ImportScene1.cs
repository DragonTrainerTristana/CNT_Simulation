using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ImportScene1 : MonoBehaviour
{
    Renderer sp;

    public GameObject silicaParent;
    public GameObject voronoiParent;
    public string silicaPosFile;
    public string voronoiVertexPositionFile;
    public string voronoiDictFile;
    public int nSilica = 0;
    public int nVoronoiVertex = 0;
    public int nVoronoiSurface = 0;
    public List<Vector3> silicaLocations = new List<Vector3>();
    public List<Vector3> voronoiVertexLocations = new List<Vector3>();
    public Material m_vorVertex;
    public Material m_vorSurface;
    public Dictionary<string, List<int>> voronoiConnectivity;

    void Start()
    {


        var currentDirectory = Directory.GetCurrentDirectory();
        Debug.Log(currentDirectory);

        silicaParent = GameObject.Find("Silica");
        voronoiParent = GameObject.Find("Voronoi");
        
        //silicaPosFile = Application.dataPath + "/DataFiles/Option1/points1.csv";
        //voronoiVertexPositionFile = Application.dataPath + "/DataFiles/Option1/voronoi_vertices1.csv";
        //voronoiDictFile = Application.dataPath + "/DataFiles/Option1/voronoi_dict1.json";
        

        silicaPosFile = Application.dataPath + "/StreamingAssets/Option1/points1.csv";
        voronoiVertexPositionFile = Application.dataPath + "/StreamingAssets/Option1/voronoi_vertices1.csv";
        voronoiDictFile = Application.dataPath + "/StreamingAssets/Option1/voronoi_dict1.json";

        silicaLocations = parsePositions(silicaPosFile, out nSilica);
        voronoiVertexLocations = parsePositions(
            voronoiVertexPositionFile, out nVoronoiVertex);
        voronoiConnectivity = parseVoronoiEdges(voronoiDictFile, out nVoronoiSurface);

        int idx = 0;
        foreach (Vector3 v in silicaLocations)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sp = sphere.GetComponent<Renderer>();
            sp.material.color = Color.red;
            sphere.transform.position = v;
            sphere.name = "Silica" + idx.ToString();
            sphere.tag = "Silica";
            
            sphere.transform.SetParent(silicaParent.transform);
            idx++;
        }

        idx = 0;

        foreach (KeyValuePair<string, List<int>> entry in voronoiConnectivity)
        {

            GameObject surface = new GameObject();
            surface.name = "Surface_" + entry.Key;
            surface.tag = "VoronoiCell";
            surface.transform.SetParent(voronoiParent.transform);

            if (entry.Value.Contains(-1))
            {
                entry.Value.Remove(-1);
            }
            int nVertex = entry.Value.Count;
            if (nVertex <= 2)
            {
                continue;
            }
            Vector3[] voronoiVertices = new Vector3[nVertex];

            for (int i = 0; i < nVertex; i++)
            {
                int vertex_no = entry.Value[i];
                voronoiVertices[i] = voronoiVertexLocations[vertex_no];
            }

            MeshFilter meshfilter = surface.AddComponent<MeshFilter>();
            meshfilter.mesh = new Mesh();
            meshfilter.mesh.vertices = voronoiVertices;
            meshfilter.mesh.triangles = new int[] { 0, 1, 2 };
            MeshRenderer meshrenderer = surface.AddComponent<MeshRenderer>();
            meshrenderer.material = m_vorSurface;
        }
    }

    private List<Vector3> parsePositions(string filename, out int count)
    {
        using (StreamReader sr = new StreamReader(filename))
        {
            string line;
            count = 0;
            List<Vector3> listXYZ = new List<Vector3>();
            while ((line = sr.ReadLine()) != null)
            {
                count++;
                string[] coords = line.Split(',');
                Vector3 v = new Vector3(float.Parse(coords[0]),
                                        float.Parse(coords[1]),
                                        float.Parse(coords[2]));
                listXYZ.Add(v);
            }
            return listXYZ;
        }
    }

    private Dictionary<string, List<int>> parseVoronoiEdges(string filename, out int count)
    {
        using (StreamReader sr = new StreamReader(filename))
        {
            count = 0;
            string jsonText = sr.ReadLine();
            Dictionary<string, List<int>> connectivity = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonText);
            return connectivity;
        }
    }
}
