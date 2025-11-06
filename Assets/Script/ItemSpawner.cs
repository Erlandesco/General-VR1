using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab yang akan di-spawn")]
    public GameObject prefab;

    [Header("Transform titik spawn (opsional). Kosong = pakai transform ini.")]
    public Transform spawnTransform;

    [Header("Spawn Control")]
    public float spawnDelayOnGrab = 0.1f;        // delay singkat setelah di-grab
    public float areaClearRadius = 0.2f;         // radius cek area kosong
    public LayerMask areaBlockLayers = ~0;       // layer yang dianggap menghalangi (default: semua)
    public float areaClearTimeout = 0.5f;        // maksimal nunggu area kosong

    [Header("Grace Collision Ignore")]
    public float ignoreCollisionDuration = 0.35f; // durasi abaikan tabrakan antara objek lama-baru
    public bool setKinematicGrace = true;         // jadikan kinematic sebentar
    public float kinematicDuration = 0.25f;

    [Header("Opsional: Temp Layer NoCollide")]
    public bool useTempNoCollideLayer = false;
    public string tempNoCollideLayerName = "NoCollide"; // pastikan layer ini ada di Project Settings
    public float tempLayerDuration = 0.35f;

    [Header("Pengaman")]
    public float registerSnapRadius = 0.3f; // jarak max supaya dianggap "yang di anchor"

    [HideInInspector] public GameObject currentOccupant;

    Transform SpawnPose => spawnTransform ? spawnTransform : transform;

    void Awake()
    {
        if (!prefab)
        {
            Debug.LogWarning($"[RespawableItem] Prefab belum di-assign pada {name}");
        }
    }

    public void RegisterIfAtAnchor(GameObject obj)
    {
        if (currentOccupant == null)
        {
            float d = Vector3.Distance(obj.transform.position, SpawnPose.position);
            if (d <= registerSnapRadius)
                currentOccupant = obj;
        }
    }

    public void RequestImmediateReplacement(GameObject requester)
    {
        // hanya spawn pengganti jika yang di-grab adalah occupant (atau belum ada occupant)
        if (currentOccupant == null || requester == currentOccupant)
        {
            StartCoroutine(SpawnReplacementSafe(requester));
        }
    }

    IEnumerator SpawnReplacementSafe(GameObject requester)
    {
        // 1) Delay singkat setelah ambil
        if (spawnDelayOnGrab > 0f)
            yield return new WaitForSeconds(spawnDelayOnGrab);

        // 2) Tunggu area kosong (hindari tabrakan awal)
        float t = 0f;
        while (true)
        {
            if (IsAreaClear())
                break;
            if (t >= areaClearTimeout) break;
            t += Time.deltaTime;
            yield return null;
        }

        // 3) Spawn
        var pose = SpawnPose;
        GameObject g = Instantiate(prefab, pose.position, pose.rotation);
        currentOccupant = g;

        // pastikan spawnable tahu anchor-nya
        var spawnable = g.GetComponent<RespawnableItem>();
        if (spawnable) spawnable.spawnerAnchor = this;

        // 4) Terapkan grace anti-benturan
        StartCoroutine(ApplyGraceCollision(g, requester));
    }

    bool IsAreaClear()
    {
        var pos = SpawnPose.position;
        // Cek collider apapun dalam radius (kecuali trigger?) — kita pakai OverlapSphere default
        Collider[] hits = Physics.OverlapSphere(pos, areaClearRadius, areaBlockLayers, QueryTriggerInteraction.Ignore);
        // Area dianggap "clear" kalau tidak ada collider apapun di radius
        return hits == null || hits.Length == 0;
    }

    IEnumerator ApplyGraceCollision(GameObject spawned, GameObject requester)
    {
        if (!spawned) yield break;

        // Kumpulkan collider
        var spawnedCols = GetAllColliders(spawned);
        var requesterCols = GetAllColliders(requester);

        // IgnoreCollision dua arah
        foreach (var a in spawnedCols)
            foreach (var b in requesterCols)
                if (a && b) Physics.IgnoreCollision(a, b, true);

        // Opsional: jadikan kinematic sebentar untuk redam impuls
        List<Rigidbody> spawnedRBs = new List<Rigidbody>();
        if (setKinematicGrace)
        {
            foreach (var rb in spawned.GetComponentsInChildren<Rigidbody>(true))
            {
                spawnedRBs.Add(rb);
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Opsional: pindah layer ke "NoCollide" sebentar
        List<(Transform, int)> originalLayers = null;
        int tempLayer = -1;
        if (useTempNoCollideLayer)
        {
            tempLayer = LayerMask.NameToLayer(tempNoCollideLayerName);
            if (tempLayer == -1)
                Debug.LogWarning($"[VRObjectSpawnerAnchor] Layer '{tempNoCollideLayerName}' tidak ditemukan.");
            else
            {
                originalLayers = new List<(Transform, int)>();
                foreach (var tr in spawned.GetComponentsInChildren<Transform>(true))
                {
                    originalLayers.Add((tr, tr.gameObject.layer));
                    tr.gameObject.layer = tempLayer;
                }
            }
        }

        // Durasi grace (pakai yang terpanjang di antara opsi)
        float grace = Mathf.Max(ignoreCollisionDuration, setKinematicGrace ? kinematicDuration : 0f);
        grace = Mathf.Max(grace, useTempNoCollideLayer && tempLayer != -1 ? tempLayerDuration : 0f);

        if (grace > 0f) yield return new WaitForSeconds(grace);

        // Kembalikan isKinematic
        if (setKinematicGrace)
            foreach (var rb in spawnedRBs) if (rb) rb.isKinematic = false;

        // Kembalikan layer
        if (originalLayers != null)
            foreach (var pair in originalLayers)
                if (pair.Item1) pair.Item1.gameObject.layer = pair.Item2;

        // Kembalikan IgnoreCollision
        foreach (var a in spawnedCols)
            foreach (var b in requesterCols)
                if (a && b) Physics.IgnoreCollision(a, b, false);
    }

    static List<Collider> GetAllColliders(GameObject root)
    {
        var list = new List<Collider>();
        if (!root) return list;
        root.GetComponentsInChildren(true, list);
        return list;
    }
}
