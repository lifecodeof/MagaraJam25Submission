using UnityEngine;

abstract class Enemy : MonoBehaviour
{
    public virtual bool FollowsPlayer { get; protected set; } = true;
    public virtual int Damage { get; protected set; } = 1;
    public virtual float Speed { get; protected set; } = 1f;

    private GameObject player;

    public virtual void Start()
    {
        player = Helpers.FindRequired<PlayerInputManager>().gameObject;
    }

    public virtual void Update()
    {
        if (FollowsPlayer)
        {
            var direction = (player.transform.position - transform.position).normalized;
            transform.Translate(Speed * Time.deltaTime * direction);
        }
    }
}
