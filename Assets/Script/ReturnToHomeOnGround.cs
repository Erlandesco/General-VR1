using System.Collections;
using UnityEngine;

/// <summary>
/// No-respawn: saat menyentuh Ground (atau jatuh di bawah Y), kembalikan objek ke home pose.
/// Home pose bisa diisi manual (homeTransform) atau otomatis pakai posisi/rotasi saat Start.
///
//— set isKinematic sebentar + reset velocity agar tidak memantul lagi.
/// </summary>
[DisallowMultipleComponent]
public class ReturnToHomeOnGround : MonoBehaviour
{
    [Header("Home Pose")]
    public Transform homeTransform;              // kosong = rekam dari posisi awal
    public bool recordCurrentAsHomeOnStart = true;

    [Header("Triggers")]
    public bool enableBelowY = true;
    public float yThreshold = -5f;
    public bool enableGroundCollision = true;
    public string groundTag = "Ground";          // pastikan tag Ground ada

    [Header("Reset Options")]
    public bool resetRotation = true;            // ikut pulihkan rotasi
    public bool resetScale = false;              // jarang perlu, default off

    [Header("Stabilizer")]
    public float kinematicDuration = 0.2f;       // tahan kinematic sebentar
    public float postTeleportYOffset = 0.0f;     // offset kecil jika perlu (mis. 0.01f)

    Rigidbody _rb;
    Vector3 _homePos;
    Quaternion _homeRot;
    Vector3 _homeScale;
    bool _homeRecorded;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (homeTransform)
        {
            CacheHomeFrom(homeTransform.position, homeTransform.rotation, homeTransform.localScale);
        }
        else if (recordCurrentAsHomeOnStart)
        {
            CacheHomeFrom(transform.position, transform.rotation, transform.localScale);
        }
    }

    void Update()
    {
        if (enableBelowY && transform.position.y < 0f)
            TeleportHome();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!enableGroundCollision) return;
        if (!string.IsNullOrEmpty(groundTag) && collision.gameObject.CompareTag(groundTag))
            TeleportHome();
    }

    public void SetHomeToCurrent()
    {
        CacheHomeFrom(transform.position, transform.rotation, transform.localScale);
    }

    public void SetHomeFromTransform(Transform t)
    {
        if (!t) return;
        CacheHomeFrom(t.position, t.rotation, t.localScale);
    }

    void CacheHomeFrom(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        _homePos = pos;
        _homeRot = rot;
        _homeScale = scale;
        _homeRecorded = true;
    }

    void TeleportHome()
    {
        if (!_homeRecorded)
        {
            // fallback: rekam dulu bila belum
            CacheHomeFrom(transform.position, transform.rotation, transform.localScale);
        }

        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        // Matikan fisika sejenak
        if (_rb)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // Set pose
        Vector3 targetPos = _homePos + new Vector3(0f, postTeleportYOffset, 0f);
        transform.position = targetPos;
        if (resetRotation) transform.rotation = _homeRot;
        if (resetScale) transform.localScale = _homeScale;

        // Tunggu sedikit agar kontak ground stabil, lalu hidupkan fisika lagi
        if (kinematicDuration > 0f)
            yield return new WaitForSeconds(kinematicDuration);

        if (_rb)
            _rb.isKinematic = false;
    }
}
