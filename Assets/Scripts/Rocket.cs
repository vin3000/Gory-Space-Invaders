using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Rocket : Projectile
{
    public float damage;
    public float explosionDamage;
    public Explosion explosionParticle;
    private ParticleSystem partSys;

    private void Awake()
    {
        direction = Vector3.up;
        partSys = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    void CheckCollision(Collider2D collision)
    {
        Bunker bunker = collision.gameObject.GetComponent<Bunker>();

        if(bunker == null) //Om det inte �r en bunker vi tr�ffat s� ska skottet f�rsvinna.
        {
            Explosion explosion = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            explosion.explosionDamage = explosionDamage;
            Destroy(gameObject);
            Destroy(explosion.GetComponent<CircleCollider2D>());
            print(partSys.main.duration);
            Destroy(explosion, partSys.main.duration);
        }
    }
}
