using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AllenSnapSimple : MonoBehaviour
{
    public XRGrabInteractable allenKey;
    public Transform attachPoint;   // di kepala baut
    public Transform bolt;          // baut utama
    public Vector3 axisLocal = Vector3.forward; // arah baut

    bool snapped;
    IXRInteractor hand;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == allenKey.gameObject)
        {
            // Snap kunci L ke baut
            snapped = true;
            allenKey.transform.SetPositionAndRotation(attachPoint.position, attachPoint.rotation);
            allenKey.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == allenKey.gameObject)
        {
            snapped = false;
            allenKey.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void Update()
    {
        if (!snapped) return;

        // Cek siapa yang pegang
        if (allenKey.interactorsSelecting.Count > 0)
        {
            hand = allenKey.interactorsSelecting[0];
            Vector3 worldAxis = bolt.TransformDirection(axisLocal.normalized);

            // Ambil rotasi tangan searah sumbu baut
            float angle = Vector3.SignedAngle(bolt.forward, hand.transform.forward, worldAxis);

            // Terapkan rotasi ke kunci L (posisi tetap di attachPoint)
            allenKey.transform.position = attachPoint.position;
            allenKey.transform.rotation = Quaternion.AngleAxis(angle, worldAxis) * bolt.rotation;
        }
    }
}