using UnityEngine;
using UnityEngine.AddressableAssets;

// TODO: Use prefabs instead of addressables

[RequireComponent(typeof(SpriteRenderer))]
abstract class Projectile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Sprite Sprite { get; private set; }

    public abstract string SpriteAddress { get; }

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        // Load sprite synchronously for simplicity.
        Sprite = Addressables.LoadAssetAsync<Sprite>(SpriteAddress).WaitForCompletion();
        SpriteRenderer.sprite = Sprite;
    }
}
