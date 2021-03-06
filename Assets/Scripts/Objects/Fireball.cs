using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private BreakableTiles _tile;

    public Transform breakPoint;
    private Rigidbody2D rb2d;

    public StateMachina stateMachina;
    public float speed;
    public float lifeTime;
    private Vector2 spawnPoint;
    public int direction=1;
    public float gravityTimer=1;
    private float gravitYcounter=1;
    private Vector2 workSpace;
    public Vector2 angle;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (rb2d.gravityScale==0)
            {
                //rb2d.velocity = new Vector2(speed * direction, 0f);
                SetVelocity(speed * direction, angle, direction);
            }
            else
            {
                rb2d.velocity = new Vector2(speed * direction, rb2d.velocity.y);
                if (direction>0)
                    transform.Rotate(0, 0, -3);
                else
                    transform.Rotate(0, 0, 3);
            }
        }
        else
            rb2d.velocity = new Vector2(0f, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy == true)
        {
            gravitYcounter -= Time.deltaTime;
            if (gravitYcounter < 0)
                gravitYcounter = 0;
            if (gravitYcounter==0)
                rb2d.gravityScale = 4;
        }
        

    }
    private void OnEnable()
    {
        gravitYcounter = gravityTimer;
        rb2d.gravityScale = 0;
        angle = stateMachina.dashAngle;
        if (stateMachina.horizontalMove==0 && stateMachina.verticalMove!=0)
        {
            angle.x = 0f;
        }
        direction = stateMachina.direction;
        if (angle.y == 1)
        {
            if (angle.x == 1)
                transform.rotation = Quaternion.Euler(0, 0, 135);
            else if (angle.x == -1)
                transform.rotation = Quaternion.Euler(0, 0, -135);
            else
                transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (angle.y == -1)
        {
            if (angle.x == 1)
                transform.rotation = Quaternion.Euler(0, 0, 45);
            else if (angle.x == -1)
                transform.rotation = Quaternion.Euler(0, 0, -45);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if (direction == 1)
                transform.rotation = Quaternion.Euler(0, 0, 90);
            else
                transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        spawnPoint = transform.position;
        StartCoroutine("Destroy");
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        transform.position = spawnPoint;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
            FindObjectOfType<AudioManager>().PlaySound("Fireball");
            gameObject.SetActive(false);
        }
        if (collision.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
        if (collision.CompareTag("Breakable"))
        {
            var worldPoint = new Vector3Int(Mathf.FloorToInt(breakPoint.transform.position.x), Mathf.FloorToInt(breakPoint.transform.position.y), 0);

            var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

            if (tiles.TryGetValue(worldPoint, out _tile))
            {
                //print("Tile " + _tile.Name + _tile.LocalPlace);
                _tile.TilemapMember.SetTile(_tile.LocalPlace, null);
            }
            gameObject.SetActive(false);

        }
    }
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity * direction);
        rb2d.velocity = workSpace;
    }
}
