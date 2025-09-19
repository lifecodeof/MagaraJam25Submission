using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

class DamageIndicator : MonoBehaviour
{
    private static readonly float speed = 1f;
    private static readonly float fadeSpeed = 0.5f;
    private static readonly int fontSize = 10;
    private static readonly float outlineWidth = 0.2f;
    private static readonly float maxDistanceSquared = 2f;

    private static readonly ConditionalWeakTable<GameObject, DamageIndicator> indicators = new();
    private static readonly Material fontMaterial =
        Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Outline");

    private TextMeshPro textMesh;
    private float opacity = 1f;

    private static Color GetColor(Damage damage) => damage switch
    {
        Damage.Physical => Color.yellow,
        _ => Color.red,
    };

    public static void IndicateDamage(GameObject obj, Damage damage)
    {
        if (indicators.TryGetValue(obj, out var existing))
        {
            if (existing == null)
                indicators.Remove(obj);
            else
            {
                existing.AddDamage(damage);
                if (
                    Vector3.SqrMagnitude(
                        existing.transform.position - obj.transform.position
                    ) > maxDistanceSquared
                ) existing.transform.position = obj.transform.position;

                return;
            }
        }

        var go = new GameObject("DamageIndicator");
        go.transform.position = obj.transform.position;
        var indicator = go.AddComponent<DamageIndicator>();
        var text = indicator.textMesh = go.AddComponent<TextMeshPro>();
        text.renderer.sortingLayerName = "Indicator";
        text.fontMaterial = fontMaterial;
        text.text = damage.Amount.ToString();
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.color = GetColor(damage);
        text.outlineColor = Color.black;
        text.outlineWidth = outlineWidth;
        indicators.Add(obj, indicator);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
        opacity -= Time.deltaTime * fadeSpeed;
        if (opacity <= 0f) Destroy(gameObject);
        var color = textMesh.color;
        color.a = opacity;
        textMesh.color = color;
    }

    void AddDamage(Damage damage)
    {
        textMesh.text = (int.Parse(textMesh.text) + damage.Amount).ToString();
        textMesh.color = GetColor(damage);
        opacity = 1f;
    }
}
