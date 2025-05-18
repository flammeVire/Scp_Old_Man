using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineOnCenter : MonoBehaviour
{
    [Header("Outline Settings")]
    public Material outlineMaterial;         // Le mat�riau avec l'outline
    public Material defaultMaterial;         // Le mat�riau de base sans outline
    [Range(0f, 0.5f)]
    public float centerTolerance = 0.1f;     // Plus c�est petit, plus c�est pr�cis (0.1 = 10% du centre)

    private Renderer targetRenderer;
    private Camera mainCam;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();

        // R�cup�re la cam�ra principale automatiquement
        mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogWarning("Aucune cam�ra trouv�e avec le tag 'MainCamera'. V�rifie ta sc�ne.");
        }
    }

    void Update()
    {
        if (mainCam == null) return;

        // Calcule la position de l�objet dans l�espace viewport
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);

        // V�rifie si l�objet est bien visible (devant la cam�ra)
        if (viewportPos.z < 0f)
        {
            SetMaterial(defaultMaterial);
            return;
        }

        // Distance au centre de l��cran (viewport va de (0,0) � (1,1))
        float dx = Mathf.Abs(viewportPos.x - 0.5f);
        float dy = Mathf.Abs(viewportPos.y - 0.5f);

        bool isCentered = dx < centerTolerance && dy < centerTolerance;

        // Active ou d�sactive le mat�riau d�outline
        if (isCentered)
        {
            SetMaterial(outlineMaterial);
        }
        else
        {
            SetMaterial(defaultMaterial);
        }
    }

    private void SetMaterial(Material mat)
    {
        if (targetRenderer.sharedMaterial != mat)
        {
            targetRenderer.sharedMaterial = mat;
        }
    }
}