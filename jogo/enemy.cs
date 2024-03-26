using System.Collections;
using System.Collections.Generic;
using Unity Engine;

public class Enemy : MonoBehaviour 
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    
    [SerializeField] protected float damage;
    
    protected float recoilTimer;
    protected Rigidbody2D rb;
    
    // Start is called before the first frame update
  protected virtual void start() 
  {
    
  }
  
  protected virtual void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    player = PlayerController.Instance;
  }
  
  //Update is called once per frame
    protected virtual void Update()
    {
        if(health <=0)
        {
            Destroy(GameObject);
        }
        if(isRecoiling)
        {
            if(recoilTimer < recoilLenght)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if(!isRecoiling)
        {
            rb.Addforce(-_hitForce * recoilFactor * _hitDirection);
        }
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        if(_other.CompareTag("Player") && !PlayerController.Instance.pState.Invincible)
        {
            Attack();
        }
    }
    
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    
}