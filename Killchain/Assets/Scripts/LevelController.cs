using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int startingEnemies = 4;
    public float spawnDelay = 3f;
    public int spawnDistance = 10;
    public GameObject playerPrefab;
    public ScoreScreen scoreScreen;
    public Text scoreText;

    private int score = 0;
    private Transform player;
    private Vector3[] posArray;
    private List<int> takenPositions = new List<int>();
    private List<Vector3> posList = new List<Vector3>();
    private List<int> coverList = new List<int>();
    private int[] cover;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the score at the top of the screen to 0
        score = 0;
        scoreText.text = score.ToString();

        // Generates a list of all possible positions and calculates their cover weights
        for (int x = -40; x < 0; x++)
        {
            for (int z = -40; z < 0; z++)
            {
                // If there is a position on the NavMesh within 0.1 of the selected position then it is valid
                if (NavMesh.SamplePosition(new Vector3(x, 0, z), out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    // Add position to the list
                    Vector3 pos = new Vector3(x, 0, z);
                    posList.Add(pos);

                    // Cover Calculation
                    // Checks each direction for an object between 0.5 and 1.5 units high
                    int coverCounter = 0;
                    Vector3 coverPos = pos + Vector3.up * 0.5f;
                    if (Physics.Raycast(coverPos, Vector3.right, 1) && !Physics.Raycast(coverPos + Vector3.up, Vector3.right, 1))
                    {
                        coverCounter += 1;
                    }
                    if (Physics.Raycast(coverPos, -Vector3.right, 1) && !Physics.Raycast(coverPos + Vector3.up, -Vector3.right, 1))
                    {
                        coverCounter += 1;
                    }
                    if (Physics.Raycast(coverPos, Vector3.forward, 1) && !Physics.Raycast(coverPos + Vector3.up, Vector3.forward, 1))
                    {
                        coverCounter += 1;
                    }
                    if (Physics.Raycast(coverPos, -Vector3.forward, 1) && !Physics.Raycast(coverPos + Vector3.up, -Vector3.right, 1))
                    {
                        coverCounter += 1;
                    }
                    coverList.Add(coverCounter);
                }
            }
        }

        // Converts both Lists to Arrays for ease of use in later code
        posArray = posList.ToArray();
        cover = coverList.ToArray();

        // Spawns the player in a random location on the map
        int randomPos = Random.Range(0, posArray.Length);
        player = Object.Instantiate(playerPrefab, posArray[randomPos] + Vector3.up, Quaternion.identity).transform;
        player.gameObject.name = "Player";

        // Spawns the starting enemies
        for (int i = 0; i < startingEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    public void EnemyKilled()
    {
        // Updates the player score
        score += 10;
        scoreText.text = score.ToString();

        // Spawns a new enemy on the map in 3 seconds
        Invoke("SpawnEnemy", spawnDelay);
    }

    void SpawnEnemy()
    {
        bool invalidPos = true;
        int randomPos = Random.Range(0, posArray.Length);

        // Generates a random position and checks it is valid
        // if it isnt valid then it checks a different random position
        // repeat until a valid position is found
        int count = 0;
        while (invalidPos)
        {
            // Conditions:
            // Is position taken
            // Is position within 'spawnDistance' units of player
            // Is there an object (i.e. another enemy) occupying that position
            // Can the player be seen from that location
            Vector3 toPlayer = (player.position - (posArray[randomPos] + Vector3.up));
            Physics.Raycast(posArray[randomPos] + Vector3.up, toPlayer, out RaycastHit hit, Mathf.Infinity);
            if (takenPositions.Contains(randomPos) || Vector3.Distance(posArray[randomPos] + Vector3.up, player.position) < spawnDistance || Physics.CheckSphere(posArray[randomPos] + Vector3.up, 0.75f) || hit.transform == player)
            {
                // if any of the above conditions are met, generate a new position to try
                randomPos = Random.Range(0, posArray.Length);
                count += 1;
            }
            else
            {
                // Once a valid position is found, spawn a random enemy in that location
                invalidPos = false;
                Object.Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], posArray[randomPos] + Vector3.up, Quaternion.identity);
            }

            // Makes sure that the loop doesn't get infinitely stuck
            // Quits the loop if no position is found after 100 random positions
            if (count > 100)
            {
                invalidPos = false;
                Invoke("SpawnEnemy", 0.1f);
            }
        }
    }

    public Vector3[] GetPositions()
    {
        // Returns the posArray to enemies
        return posArray;
    }

    public int[] GetCover()
    {
        // Returns the cover Array to enemies
        return cover;
    }

    public List<int> GetTaken()
    {
        // Returns the list of currently taken positions
        return takenPositions;
    }

    public bool ClaimPosition(int pos)
    {
        // Checks that the same positions isn't being claimed twice
        // Returns false if it is
        // Note: This should always return true
        if (!takenPositions.Contains(pos))
        {
            takenPositions.Add(pos);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReleasePosition(int pos)
    {
        // Removes the given position from the list of takenPositions
        takenPositions.Remove(pos);
    }

    public void EndRun()
    {
        // Pauses the game
        Time.timeScale = 0;
        // Releases the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Turns on the EndScreen
        scoreScreen.ActiveEndScreen(score);
    }
}
