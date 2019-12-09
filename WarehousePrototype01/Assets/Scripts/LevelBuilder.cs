using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private int xLevelSize;
    [SerializeField]
    private int yLevelSize;
    [SerializeField]
    private GameObject floorTilePrefab;
    [SerializeField]
    private GameObject StandartBoxPrefab;
    [SerializeField]
    private GameObject DoubleBoxPrefab;
    [SerializeField]
    private GameObject PlayerPrefab;
    [SerializeField]
    private GameObject BigWallPrefab;
    [SerializeField]
    private GameObject SmallWallPrefab;
    [SerializeField]
    private GameObject BigCornerWallPrefab;
    [SerializeField]
    private GameObject SmallCornerWallPrefab;
    [SerializeField]
    private GameObject BoxReciever;
    [SerializeField]
    private float tileGap;

    private GameObject[,] tilesArray;
    private GameObject[,] objectsArray;
    private GameObject playerObject;
    private GameObject playerHeadObject;
    private int boxesCount;
    private int recievedBoxesCount;
    private int xPlayerPos;
    private int yPlayerPos;
    private int xRecieverPos;
    private int yRecieverPos;

    void Start()
    {
        Physics.gravity *= -1;
        InitMap();
        SpawnFloorTiles();
        SpawnWalls();
        SpawnBoxes();
        SpawnPlayer();
        SpawnBoxReciever();

    }

    //MethodsForLevelBuilder
    private void InitMap()
    {
        tilesArray = new GameObject[xLevelSize, yLevelSize];
        boxesCount = xLevelSize * yLevelSize / 4;
        recievedBoxesCount = 0;
        objectsArray = new GameObject[xLevelSize, yLevelSize];
    }

    private Vector3 FindCellPosition(int x, int y)
    {
        return new Vector3(floorTilePrefab.transform.localScale.x * x + tileGap * x, 0, floorTilePrefab.transform.localScale.z * y + tileGap * y);
    }

    private bool IsThisCellEmpty(int x, int y) =>
        x >= 0 && y >= 0 && x < xLevelSize && y < yLevelSize && objectsArray[x, y] == null;

    private float GetPlayerRotation() => Mathf.Round(playerObject.transform.eulerAngles.y);

    private void SpawnFloorTiles()
    {
        for (int i = 0; i < xLevelSize; i++)
        {
            for (int j = 0; j < yLevelSize; j++)
            {
                Vector3 tilePosition = FindCellPosition(i, j);
                tilesArray[i, j] = Instantiate(floorTilePrefab, tilePosition, Quaternion.identity);
                tilesArray[i, j].name = $"tile[{i},{j}]";
            }
        }
    }

    private void SpawnWalls()
    {
        Instantiate(BigCornerWallPrefab, FindCellPosition(xLevelSize, yLevelSize), Quaternion.identity);
        Instantiate(SmallCornerWallPrefab, FindCellPosition(-1, -1), Quaternion.Euler(0, 270, 0));
        Instantiate(SmallCornerWallPrefab, FindCellPosition(xLevelSize, -1), Quaternion.Euler(0, 180, 0));
        Instantiate(SmallCornerWallPrefab, FindCellPosition(-1, yLevelSize), Quaternion.Euler(0, 0, 0));
        for (int i = 0; i < xLevelSize; i++)
        {
            for (int j = 0; j < yLevelSize; j++)
            {
                if (i == xLevelSize - 1)   Instantiate(BigWallPrefab,FindCellPosition(i+1, j),Quaternion.Euler(0,90,0));
                if (j == yLevelSize - 1)   Instantiate(BigWallPrefab, FindCellPosition(i, j+1), Quaternion.identity);
                if (i == 0) Instantiate(SmallWallPrefab, FindCellPosition(i - 1, j), Quaternion.Euler(0, 270, 0));
                if (j == 0) Instantiate(SmallWallPrefab, FindCellPosition(i, j - 1), Quaternion.Euler(0, 180, 0));
            }
        }
    }

    private void SpawnBoxes()
    {
        GameObject spawningObject;
        int spawnOnTileX;
        int spawnOnTileY;
        for (int i = 0; i < boxesCount; i++)
        {
            if (Random.value < 0.5f)
                spawningObject = StandartBoxPrefab;
            else
                spawningObject = DoubleBoxPrefab;
            do
            {
                spawnOnTileX = Random.Range(0, xLevelSize);
                spawnOnTileY = Random.Range(0, yLevelSize);
            } while (objectsArray[spawnOnTileX, spawnOnTileY] != null);
            Vector3 objPosition = FindCellPosition(spawnOnTileX, spawnOnTileY);
            objectsArray[spawnOnTileX,spawnOnTileY] = Instantiate(spawningObject, objPosition, GetRandomRotation());
            objectsArray[spawnOnTileX, spawnOnTileY].GetComponentInChildren<TextMesh>().text = GetBoxLabel(i+1);
            objectsArray[spawnOnTileX, spawnOnTileY].name = $"box[{spawnOnTileX},{spawnOnTileY}]";
        }
    }

    private void SpawnPlayer()
    {
        bool playerSpawned = false;
        for (int i = 0; i < xLevelSize; i++)
        {
            for (int j = 0; j < yLevelSize; j++)
            {
                if (objectsArray[i, j] == null)
                {
                    Vector3 playerPosition = FindCellPosition(i, j);
                    playerObject = Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                    xPlayerPos = i;
                    yPlayerPos = j;
                    playerSpawned = true;
                }
                if (playerSpawned) break;
            }
            if (playerSpawned) break;
        }
    }

    private void SpawnBoxReciever()
    {
        bool recieverSpawned = false;
        for (int i = xLevelSize - 1; i >= 0; i--)
        {
            for (int j = yLevelSize - 1; j >= 0; j--)
            {
                if (objectsArray[i, j] == null)
                {
                    Vector3 recieverPosition = FindCellPosition(i, j);
                    Instantiate(BoxReciever, recieverPosition, Quaternion.identity);
                    xRecieverPos = i;
                    yRecieverPos = j;
                    recieverSpawned = true;
                }
                if (recieverSpawned) break;
            }
            if (recieverSpawned) break;
        }
    }

    private (int,int) CellInFrontOfPlayer()
    {
        if (GetPlayerRotation() == 0.0f)
        {
            if(xPlayerPos+1 != xLevelSize)
            return (xPlayerPos + 1, yPlayerPos);
        }
        else if (GetPlayerRotation() == 90.0f)
        {
            if (yPlayerPos -1 != -1)
                return (xPlayerPos, yPlayerPos - 1);
        }
        else if (GetPlayerRotation() == 180.0f)
        {
            if (xPlayerPos -1 != -1)
                return (xPlayerPos - 1, yPlayerPos);
        }
        else if (GetPlayerRotation() == 270.0f)
        {
            if (yPlayerPos + 1 != yLevelSize)
                return (xPlayerPos, yPlayerPos + 1);
        }
        return (xPlayerPos, yPlayerPos);

    }

    public Vector3 MovePlayer()
    {
        if (IsThisCellEmpty(CellInFrontOfPlayer().Item1, CellInFrontOfPlayer().Item2))
        {
            xPlayerPos = CellInFrontOfPlayer().Item1;
            yPlayerPos = CellInFrontOfPlayer().Item2;

            if (RightBoxUnderTube(xPlayerPos,yPlayerPos))
                Invoke("BoxFlyAway", 0.2f);
            return FindCellPosition(xPlayerPos, yPlayerPos);
        }
        return FindCellPosition(xPlayerPos, yPlayerPos);
    }

    public Quaternion RotatePlayer(bool right)
    {
        float currentAngle = playerObject.transform.eulerAngles.y;
        if (right) return Quaternion.Euler(0, currentAngle + 90.0f, 0);
        else return Quaternion.Euler(0, currentAngle - 90.0f, 0);
    }

    private Quaternion GetRandomRotation()
    {
        int direction = Random.Range(0, 4);
        if (direction == 0) return Quaternion.Euler(0, 0, 0);
        else if (direction == 1) return Quaternion.Euler(0, 90, 0);
        else if (direction == 2) return Quaternion.Euler(0, 180, 0);
        return Quaternion.Euler(0, 270, 0);
    }

    private string GetBoxLabel(int number)
    {
        string label = $"{number}";
        if (number == 6 || number == 9 || number == 69 || number == 96)
            label += '.';
        return label;
    }

    private void BoxFlyAway()
    {
        playerHeadObject.transform.parent = null;
        Rigidbody headRB = playerHeadObject.AddComponent<Rigidbody>();
        recievedBoxesCount++;
        GameObject flewAwayBox = playerHeadObject;
        playerHeadObject = null;
        Destroy(flewAwayBox, 2f);
    }

    private bool RightBoxUnderTube(int x, int y)
    {   if (playerHeadObject == null) return false;
        return x == xRecieverPos
                    && y == yRecieverPos
                    && playerHeadObject.GetComponentInChildren<TextMesh>().text == GetBoxLabel(recievedBoxesCount + 1);
    }

    public void PickOrPut()
    {
        if (playerHeadObject == null) PickObject();
        else PutObject();
    }

    private void PickObject()
    {
        int xFwdTile = CellInFrontOfPlayer().Item1;
        int yFwdTile = CellInFrontOfPlayer().Item2;
        if (xFwdTile != xPlayerPos || yFwdTile != yPlayerPos)
            if (objectsArray[xFwdTile, yFwdTile] != null && playerHeadObject == null)
            {
                objectsArray[xFwdTile, yFwdTile].transform.position = playerObject.transform.position+ new Vector3(0,0.5f,0);
                objectsArray[xFwdTile, yFwdTile].transform.SetParent(playerObject.transform);
                playerHeadObject = objectsArray[xFwdTile, yFwdTile];
                objectsArray[xFwdTile, yFwdTile] = null;
            }
        if (RightBoxUnderTube(xPlayerPos, yPlayerPos))
        {
            BoxFlyAway();
        }
    }

    private void PutObject()
    {
        int xFwdTile = CellInFrontOfPlayer().Item1;
        int yFwdTile = CellInFrontOfPlayer().Item2;
        if (xFwdTile != xPlayerPos || yFwdTile != yPlayerPos)
            if ((objectsArray[xFwdTile, yFwdTile] == null || objectsArray[xFwdTile, yFwdTile].tag != "Box") && playerHeadObject != null)
            {
                playerHeadObject.transform.position = FindCellPosition(xFwdTile, yFwdTile);
                if (RightBoxUnderTube(xFwdTile,yFwdTile))
                {
                    BoxFlyAway();
                }
                else
                {
                    objectsArray[xFwdTile, yFwdTile] = playerHeadObject;
                    objectsArray[xFwdTile, yFwdTile].transform.parent = null;
                    playerHeadObject = null;
                }
                
                

            }
    }
}
