using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCenterBigTest : MonoBehaviour
{
    private Vector3 initialPosition;

    private int row;
    private int col;
    public int[] rowRange = new int[2]; //smallest, largest
    public int[] colRange = new int[2];

    public int difficulty; //0->3
    /* 0 - easy, 1-5
     * 1 - medium, 6-10
     * 2 - hard, 11-15
     * 3 - supah zombie mode - 16-20
     */
    public Zombies mainZombiePrefab; //the regular little guys
    public bool hasSpecial; //ska den ha en speciel? t.ex gun troops ska ej ha annat.
    public Zombies[] specZombiePrefabs = new Zombies[4]; //whatever spawns in the special spot (0-Expl, 1-*//* + expl, 2- *//* + infest, 3- *//*one extra explode, to make it easier to kill bosses)
    Zombies specZombiePrefab;
    public float speed;
    BoxCollider2D m_Collider;

    void Awake() //en speciel zombie per wave
    {
        row = UnityEngine.Random.Range(rowRange[0], rowRange[1] + 1);
        col = UnityEngine.Random.Range(colRange[0], colRange[1] + 1);
        print($"row = {row}");
        print($"col = {col}");

        int a = UnityEngine.Random.Range(0, difficulty + 1);
        Debug.Log(a);
        specZombiePrefab = specZombiePrefabs[a];

        initialPosition = transform.position;
        CreateInvaderGrid();
        m_Collider = GetComponent<BoxCollider2D>();
        m_Collider.size = new Vector2(2*col, 2*row);
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.down; //move
        int zombCount = GetZombieCount();
        if (zombCount == 0)
        {
            Destroy(gameObject);
        }
    }
    void CreateInvaderGrid()
    {
        float rand = UnityEngine.Random.value;
        bool spawnSpecial = false;
        if (rand < 0.5) //beror p� difficulty?? och wave size????
        {
            spawnSpecial = true;
        }

        float randRow = SetSpecPosition(row);
        float randCol = SetSpecPosition(col);
        Debug.Log($"RandRow = {randRow}  randCol = {randCol}");
        for (int r = 0; r < row; r++)
        {
            float width = 2f * (col - 1);
            float height = 2f * (row - 1);

            //f�r att centerar invaders
            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, 2*r + centerOffset.y, 0f);

            for (int c = 0; c < col; c++)
            {
                Zombies tempZombie;
                if (hasSpecial && spawnSpecial && r == randRow && c == randCol)
                {
                    tempZombie = Instantiate(specZombiePrefab, transform);
                }
                else
                {
                    tempZombie = Instantiate(mainZombiePrefab, transform);
                }
                Vector3 position = rowPosition;
                position.x += 2*c;
                tempZombie.transform.localPosition = position;
                tempZombie.speed = 0;
            }
        }
    }
    private int SetSpecPosition(int a) //returns a position in the wave (for column or row)
    {
        int position;
        if (a == 1 || a == 2)
        {
            position = 0;
            print("to small :)");
        }
        else
        {
            position = UnityEngine.Random.Range(1, row - 1);
            print($"i put it as {position}");
        }
        return position;
    }
    public int GetZombieCount()
    {
        int nr = 0;

        foreach (Transform Zombie in transform)
        {
            if (Zombie.gameObject.activeSelf)
                nr++;
        }

        Debug.Log(nr);
        return nr;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            return;
        }
        else
        {
            Destroy(m_Collider);
            print("I tried to kill myself");
        }
    }
}
