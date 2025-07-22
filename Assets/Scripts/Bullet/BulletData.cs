using UnityEngine;
[CreateAssetMenu(menuName = "Bullet Data")]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public float damage;
    public bool healsOnHit;
    public float lifetime;
    public Color tracerColor;
    public float tracerWidth;
    public GameObject tracerPrefab;
    public Sprite icon;
    // 다양한 특성들!
}
