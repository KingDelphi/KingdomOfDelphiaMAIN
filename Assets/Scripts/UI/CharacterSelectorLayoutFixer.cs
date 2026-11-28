using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reacomoda las siluetas "Character (N)" en una cuadrícula de N columnas,
/// mantiene los personajes coloreados sobre su silueta, centra y escala el
/// contenedor, y crea un panel gris detrás.
/// Agregar a: CharacterSelectorCanvas
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CharacterSelectorLayoutFixer : MonoBehaviour
{
    [Header("Cuadrícula")]
    [Tooltip("Columnas por fila (16 recomendado para ~123 personajes en 16:9)")]
    [SerializeField] private int columns = 16;
    [SerializeField] private float cellSize = 100f;
    [SerializeField] private float spacing = 6f;

    [Header("Tamaño máximo del panel (% de la pantalla)")]
    [Range(0.3f, 1f)] [SerializeField] private float maxWidthPercent = 0.92f;
    [Range(0.3f, 1f)] [SerializeField] private float maxHeightPercent = 0.85f;

    [Header("Escala")]
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private float windowPadding = 20f;

    [Header("Color del panel")]
    [SerializeField] private Color windowColor = new Color(0.15f, 0.15f, 0.15f, 0.75f);

    private const string BackdropName = "CharacterSelectorBackdrop";
    private const float PollInterval = 0.25f;

    private RectTransform rt;
    private RectTransform parentRt;
    private RectTransform backdrop;
    private float timer;
    private Vector2 lastParentSize;
    private int lastChildCount = -1;
    private Rect lastBounds;
    private bool didRelayout;

    private void OnEnable()
    {
        timer = 0f;
        lastChildCount = -1;
        if (backdrop != null) backdrop.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (backdrop != null) backdrop.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (backdrop != null) Destroy(backdrop.gameObject);
    }

    private void LateUpdate()
    {
        timer -= Time.unscaledDeltaTime;
        if (timer > 0f) return;
        timer = PollInterval;
        Apply();
    }

    [ContextMenu("Aplicar layout")]
    public void Apply()
    {
        if (!Application.isPlaying) return;

        rt = (RectTransform)transform;
        parentRt = transform.parent as RectTransform;
        if (parentRt == null) return;

        Vector2 parentSize = parentRt.rect.size;
        if (parentSize.x <= 1f || parentSize.y <= 1f) return;

        if (TryGetComponent(out Image oldImg) && oldImg.sprite == null) Destroy(oldImg);
        if (TryGetComponent(out LayoutGroup lg)) lg.enabled = false;
        if (TryGetComponent(out ContentSizeFitter csf)) csf.enabled = false;

        // Reacomodar siluetas (una sola vez)
        RelayoutOnce();

        if (!TryMeasureChildren(out Rect bounds)) return;

        bool changed = parentSize != lastParentSize
                    || transform.childCount != lastChildCount
                    || bounds != lastBounds;
        if (!changed) return;

        lastParentSize = parentSize;
        lastChildCount = transform.childCount;
        lastBounds = bounds;

        float availW = parentSize.x * maxWidthPercent - windowPadding * 2f;
        float availH = parentSize.y * maxHeightPercent - windowPadding * 2f;
        float s = Mathf.Min(availW / bounds.width, availH / bounds.height);
        s = Mathf.Clamp(s, 0.01f, maxScale);

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = new Vector3(s, s, 1f);
        rt.anchoredPosition = -bounds.center * s;

        EnsureBackdrop();
        backdrop.sizeDelta = new Vector2(bounds.width * s + windowPadding * 2f,
                                         bounds.height * s + windowPadding * 2f);
        backdrop.anchoredPosition = Vector2.zero;
        backdrop.gameObject.SetActive(isActiveAndEnabled);
    }

    // ---------- Reacomodo de la cuadrícula ----------

    private struct Shadow
    {
        public int id;
        public RectTransform rect;
        public Vector2 oldCenter;
        public Vector2 oldSize;
    }

    private struct Linked
    {
        public RectTransform rect;
        public int shadowIndex;   // índice en la lista ordenada
        public Vector2 offset;    // desplazamiento / tamaño de silueta
        public Vector2 sizeRatio; // tamaño / tamaño de silueta
    }

    private void RelayoutOnce()
    {
        if (didRelayout) return;

        var shadows = new List<Shadow>();
        var others = new List<RectTransform>();

        foreach (Transform t in transform)
        {
            if (!(t is RectTransform r)) continue;

            if (t.name.StartsWith("Character (") &&
                int.TryParse(t.name.Replace("Character (", "").Replace(")", ""), out int id))
            {
                Rect lr = GetLocalRect(r);
                shadows.Add(new Shadow { id = id, rect = r, oldCenter = lr.center, oldSize = lr.size });
            }
            else if (t.name != BackdropName)
            {
                others.Add(r);
            }
        }

        if (shadows.Count == 0) return;

        shadows.Sort((a, b) => a.id.CompareTo(b.id));

        // Asociar cada personaje coloreado con la silueta más cercana (antes de mover nada)
        var links = new List<Linked>();
        foreach (RectTransform o in others)
        {
            Rect orr = GetLocalRect(o);
            int best = -1;
            float bestDist = float.MaxValue;
            for (int i = 0; i < shadows.Count; i++)
            {
                float d = Vector2.Distance(orr.center, shadows[i].oldCenter);
                if (d < bestDist) { bestDist = d; best = i; }
            }

            if (best < 0) continue;
            Shadow sh = shadows[best];
            if (sh.oldSize.x <= 0.01f || sh.oldSize.y <= 0.01f) continue;
            if (bestDist > sh.oldSize.x) continue; // no está sobre ninguna silueta: no tocar

            links.Add(new Linked
            {
                rect = o,
                shadowIndex = best,
                offset = new Vector2((orr.center.x - sh.oldCenter.x) / sh.oldSize.x,
                                     (orr.center.y - sh.oldCenter.y) / sh.oldSize.y),
                sizeRatio = new Vector2(orr.width / sh.oldSize.x, orr.height / sh.oldSize.y)
            });
        }

        // Nueva cuadrícula
        int count = shadows.Count;
        int cols = Mathf.Clamp(columns, 1, count);
        int rows = Mathf.CeilToInt(count / (float)cols);
        float gridW = cols * cellSize + (cols - 1) * spacing;
        float gridH = rows * cellSize + (rows - 1) * spacing;

        var newCenters = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            float x = -gridW * 0.5f + cellSize * 0.5f + col * (cellSize + spacing);
            float y = gridH * 0.5f - cellSize * 0.5f - row * (cellSize + spacing);
            newCenters[i] = new Vector2(x, y);

            RectTransform r = shadows[i].rect;
            SetCentered(r, newCenters[i], new Vector2(cellSize, cellSize));
        }

        foreach (Linked l in links)
        {
            Vector2 center = newCenters[l.shadowIndex] + l.offset * cellSize;
            Vector2 size = l.sizeRatio * cellSize;
            SetCentered(l.rect, center, size);
        }

        Debug.Log($"[CharacterSelectorLayoutFixer] {count} siluetas en {cols} columnas x {rows} filas. " +
                  $"Personajes coloreados enlazados: {links.Count}/{others.Count}");

        didRelayout = true;
    }

    private static void SetCentered(RectTransform r, Vector2 center, Vector2 size)
    {
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.localScale = Vector3.one;
        r.sizeDelta = size;
        r.anchoredPosition = center;
    }

    /// <summary>Rectángulo de un hijo en el espacio local de este objeto (incluye su escala).</summary>
    private Rect GetLocalRect(RectTransform r)
    {
        var corners = new Vector3[4];
        r.GetWorldCorners(corners);
        Vector3 p0 = transform.InverseTransformPoint(corners[0]);
        Vector3 p2 = transform.InverseTransformPoint(corners[2]);
        return Rect.MinMaxRect(Mathf.Min(p0.x, p2.x), Mathf.Min(p0.y, p2.y),
                               Mathf.Max(p0.x, p2.x), Mathf.Max(p0.y, p2.y));
    }

    // ---------- Medición, panel ----------

    private bool TryMeasureChildren(out Rect result)
    {
        result = default;
        bool has = false;
        float minX = 0, minY = 0, maxX = 0, maxY = 0;
        var corners = new Vector3[4];

        foreach (Transform t in transform)
        {
            if (!t.gameObject.activeSelf) continue;
            if (!(t is RectTransform c)) continue;

            c.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++)
            {
                Vector3 p = transform.InverseTransformPoint(corners[i]);
                if (!has) { minX = maxX = p.x; minY = maxY = p.y; has = true; }
                else
                {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                }
            }
        }

        if (!has || maxX - minX < 1f || maxY - minY < 1f) return false;
        result = Rect.MinMaxRect(minX, minY, maxX, maxY);
        return true;
    }

    private void EnsureBackdrop()
    {
        if (backdrop != null) return;

        Transform existing = parentRt.Find(BackdropName);
        if (existing != null)
        {
            backdrop = (RectTransform)existing;
        }
        else
        {
            var go = new GameObject(BackdropName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            backdrop = (RectTransform)go.transform;
            backdrop.SetParent(parentRt, false);
        }

        backdrop.anchorMin = backdrop.anchorMax = new Vector2(0.5f, 0.5f);
        backdrop.pivot = new Vector2(0.5f, 0.5f);
        backdrop.localScale = Vector3.one;

        var img = backdrop.GetComponent<Image>();
        img.sprite = null;
        img.color = windowColor;
        img.raycastTarget = true;

        backdrop.SetSiblingIndex(transform.GetSiblingIndex());
    }
}