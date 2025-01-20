using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Map Generation")]
    [SerializeField] private GameObject meshObject;  // Objekt obsahující mesh pro generování
    public Texture2D spawnTexture;

    [Header("Data")]
    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;
    public Material terrainMaterial;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;  // Transform hráče, který bude spawnovat objekty v jeho okolí

    [Header("Stone")]
    [SerializeField] private GameObject stonePrefab;  // Prefab kamene, který chceme spawnovat
    [SerializeField] private float stoneSpawnRadius = 10f;  // Radius pro spawn kamene kolem hráče
   
    [Header("Shipwrecks")]
    public GameObject[] shipwreckPrefabs;  // Prefaby vraků lodí
    public int shipwreckCount = 10;  // Počet vraků k vygenerování
        
    [Header("Objects")]
    public GameObject[] natureObjects;  // Přírodní objekty (stromy, rudy)
    public GameObject[] otherObjects;   // Ostatní objekty (obelisky, bedny)
    public float natureObjectChance = 0.8f;  // Pravděpodobnost generování přírodních objektů
    public float otherObjectChance = 0.05f;  // Pravděpodobnost generování jiných objektů

    [Header("LOD preview")]
    [Range(0, 6)]
    public int editorPreviewLOD;

    private GameObject resourcesContainer;  // Kontejner pro všechny vygenerované objekty

    public bool autoUpdate;
    private float[,] falloffMap;

    void Start()
    {

        GenerateObjects(this);
    }

    public int mapChunkSize
    {
        get { return terrainData.useFlatShading ? 95 : 239; }
    }


    public MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

        if (terrainData.useFalloff)
        {
            if (falloffMap == null)
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);

            for (int y = 0; y < mapChunkSize + 2; y++)
            {
                for (int x = 0; x < mapChunkSize + 2; x++)
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
            }
        }

        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        return new MapData(noiseMap);
    }

    public void GenerateObjects(MapGenerator mapGenerator)
    {
        // Vytvoř nový kontejner pro objekty
        if (resourcesContainer != null)
        {
            DestroyImmediate(resourcesContainer);  // Zničení starého kontejneru
        }
        resourcesContainer = new GameObject("ResourcesContainer");

        // Generování objektů (přírodní a jiné objekty)
        GenerateObjectsOnTerrain();
        SpawnStoneNearPlayer();
        GenerateShipwrecks();
    }

    // Generování objektů na základě terénu
    private void GenerateObjectsOnTerrain()
    {
        float minDistanceForNatureObjects = 2f;
        float minDistanceForOtherObjects = 20f;

        List<Vector3> spawnedPositions = new List<Vector3>();  // Seznam pozic pro již vygenerované objekty
        int natureObjectsSpawned = 0;
        int otherObjectsSpawned = 0;

        int maxObjects = 1600;
        int maxAttempts = 5000;

        for (int attempt = 0; attempt < maxAttempts && (natureObjectsSpawned + otherObjectsSpawned) < maxObjects; attempt++)
        {
            // Náhodně vyber vrchol
            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.sharedMesh;
            int randomIndex = Random.Range(0, mesh.vertices.Length);
            Vector3 worldPosition = meshObject.transform.TransformPoint(mesh.vertices[randomIndex]);

            // Normalizuj výšku podle rozsahu
            float normalizedHeight = Mathf.InverseLerp(0f, 100f, worldPosition.y);

            if (normalizedHeight > 0.2f)  // Pokud je výška vhodná
            {
                GameObject objectToSpawn = null;

                if (otherObjectsSpawned < maxObjects && Random.Range(0f, 1f) < otherObjectChance)
                {
                    if (IsPositionFarEnough(worldPosition, spawnedPositions, minDistanceForOtherObjects))
                    {
                        objectToSpawn = otherObjects[Random.Range(0, otherObjects.Length)];
                        otherObjectsSpawned++;
                    }
                }
                else if (natureObjectsSpawned < maxObjects)
                {
                    if (IsPositionFarEnough(worldPosition, spawnedPositions, minDistanceForNatureObjects))
                    {
                        objectToSpawn = natureObjects[Random.Range(0, natureObjects.Length)];
                        natureObjectsSpawned++;
                    }
                }

                if (objectToSpawn != null)
                {
                    // Generování náhodné rotace kolem osy Y
                    float randomYRotation = Random.Range(0f, 360f);  // Náhodná rotace
                    Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);  // Rotace pouze na ose Y

                    GameObject newObj = Instantiate(objectToSpawn, worldPosition, randomRotation);
                    newObj.transform.parent = resourcesContainer.transform;
                    spawnedPositions.Add(worldPosition);  // Přidáme pozici do seznamu
                }
            }
        }

        Debug.Log($"Generováno {natureObjectsSpawned} přírodních objektů a {otherObjectsSpawned} jiných objektů.");
    }
    private void GenerateShipwrecks()
    {
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        Vector3[] vertices = mesh.vertices;
        int shipwrecksSpawned = 0;

        // Získání vrstvy vody z TextureData
        TextureData.Layer waterLayer = GetWaterLayer();
        if (waterLayer == null)
        {
            Debug.LogError("Vrstva vody nebyla nalezena!");
            return;
        }

        float waterStartHeight = waterLayer.startHeight;

        for (int i = 0; i < vertices.Length && shipwrecksSpawned < shipwreckCount; i++)
        {
            // Náhodně vybereme vrchol z meshe
            int randomIndex = Random.Range(0, vertices.Length);
            Vector3 localVertex = vertices[randomIndex];

            // Převedeme lokální pozici vrcholu na světovou
            Vector3 worldPosition = meshObject.transform.TransformPoint(localVertex);

            // Získáme výšku terénu na této pozici
            float terrainHeight = GetTerrainHeightAtPosition(worldPosition);

            // Normalizovaná výška pro kontrolu vody
            float normalizedHeight = Mathf.InverseLerp(textureData.savedMinHeight, textureData.savedMaxHeight, terrainHeight);

            // Pokud je výška nad vrstvou vody, spawne se vrak
            if (normalizedHeight >= waterStartHeight)
            {
                GameObject shipwreckPrefab = shipwreckPrefabs[Random.Range(0, shipwreckPrefabs.Length)];
                GameObject newShipwreck = Instantiate(shipwreckPrefab, new Vector3(worldPosition.x, terrainHeight, worldPosition.z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
                newShipwreck.transform.parent = resourcesContainer.transform;
                shipwrecksSpawned++;
            }
        }

        Debug.Log($"Generováno {shipwrecksSpawned} vraků lodí.");
    }


    // Získá vrstvu vody z TextureData
    private TextureData.Layer GetWaterLayer()
    {
        foreach (var layer in textureData.layers)
        {
            if (layer.texture.name == "Water")  // Textura vody
            {
                return layer;
            }
        }
        return null;  // Vrstva vody nebyla nalezena
    }

    private void SpawnStoneNearPlayer()
    {
        if (stonePrefab != null && playerTransform != null)
        {
            // Náhodná pozice kolem hráče (v okruhu kolem hráče)
            Vector3 randomPosition = playerTransform.position + new Vector3(Random.Range(-stoneSpawnRadius, stoneSpawnRadius), 0f, Random.Range(-stoneSpawnRadius, stoneSpawnRadius));

            // Zajištění, že kámen spawnuje na zemi (správná výška)
            randomPosition.y = GetTerrainHeightAtPosition(randomPosition);  // Nastaví správnou výšku podle terénu

            // Spawnuje kámen
            GameObject stone = Instantiate(stonePrefab, randomPosition, Quaternion.identity);
            stone.transform.parent = resourcesContainer.transform;  // Nastavení rodiče
            Debug.Log($"Kámen spawnuje na pozici: {randomPosition}");
        }
        else
        {
            Debug.LogError("Prefab kamene nebo transform hráče není přiřazen v Inspectoru!");
        }
    }
    private float GetTerrainHeightAtPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point.y;  // Vrátíme výšku terénu
        }
        return position.y;  // Pokud není terén, vrátíme původní Y hodnotu
    }

    // Kontrola, zda je pozice dostatečně vzdálená od ostatních
    private bool IsPositionFarEnough(Vector3 position, List<Vector3> existingPositions, float minDistance)
    {
        foreach (Vector3 existingPosition in existingPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minDistance)
            {
                return false;  // Příliš blízko jiného objektu
            }
        }
        return true;  // Pozice je dostatečně vzdálená
    }

    void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            GenerateObjects(this);
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }
}

public struct MapData
{
    public readonly float[,] heightMap;
    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
