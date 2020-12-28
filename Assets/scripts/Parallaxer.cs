using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PollObject
    {
        public Transform transform;
        public bool inUse;
        public PollObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PollObject[] pollObjects;
    GameManager game;

     void Awake()
    {
        Configure();

    }
     void Start()
    {
        game = GameManager.Instance;
    }

     void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < pollObjects.Length; i++)
        {
            pollObjects[i].Dispose();
            pollObjects[i].transform.position = Vector3.one * 1000; 
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

     void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
       
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        pollObjects = new PollObject[poolSize];
        for (int i = 0; i < pollObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            pollObjects[i] = new PollObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect)/targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < pollObjects.Length; i++)
        {
            pollObjects[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
            //pollObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(pollObjects[i]);
        }
    }

    void CheckDisposeObject(PollObject pollObject)
    {
        if (pollObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect)/ targetAspect)
        {
            pollObject.Dispose();
            pollObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < pollObjects.Length; i++)
        {
            if (!pollObjects[i].inUse)
            {
                pollObjects[i].Use();
                return pollObjects[i].transform;
            }
        }
        return null;
    }
}
