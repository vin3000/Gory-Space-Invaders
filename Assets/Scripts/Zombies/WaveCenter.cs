using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEditor.UIElements;
using UnityEngine;

public class WaveCenter : MonoBehaviour
{
    
    /* Till game manager!!!
     * 
     * waves appear in dynamic squares relative to difficulty (8*8 is SUPER HARD)
     * Waves kan instantiatas vid 1 av 4 olika summoners
     * kan summona individuella zombies
     * framkallar specifik m�ngd av waves (eller framkallar x wave sv�righet varje y sekunder tills timer �r slut)
     * framkalla bossar var 5 wave
     * framkalla m�nga bossar sista wave
     * 
     * Variabler;
     * wave number
     * time between spawns
     * special spawns (lista)
     * storlek baserad p� sv�righet/prefab lista??
     * 
     * antal spawns/timer
     * 
     */
    
    private Vector3 initialPosition;
    public int row;
    public int col;
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
        int a = UnityEngine.Random.Range(0, difficulty + 1);
        Debug.Log(a);
        specZombiePrefab = specZombiePrefabs[a];
        
        initialPosition = transform.position;
        CreateInvaderGrid();
        m_Collider = GetComponent<BoxCollider2D>();
        m_Collider.size = new Vector2(row, col);
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
            Vector2 centerOffset = new Vector2(-width * 0.25f, -height * 0.25f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (r) + centerOffset.y, 0f);

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
                position.x += c;
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
        }
        else
        {
            position = UnityEngine.Random.Range(1, row - 1);
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
