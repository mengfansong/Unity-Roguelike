using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



#region WallType
[System.Serializable]
public class WallType
{
    public GameObject wallU, wallD, wallL, wallR,
                                    wallUD, wallUL, wallUR, wallUDL, wallUDR, wallUDLR, wallULR,
                                    wallDL, wallDR, wallDLR,
                                    wallLR;
}
#endregion





public class RoomGenerator : MonoBehaviour
{
    public enum Direction
    {
        up,
        down,
        left,
        right
    };

    public Direction direction;

    [Header("房间信息")]
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;
    private GameObject endRoom;

    [Header("位置控制")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;

    public LayerMask roomLayer;

    public List<Room> rooms = new List<Room>();

    public int maxStep;     //最远房间的步数

    List<GameObject> farRooms = new List<GameObject>();
    List<GameObject> lessFarRooms = new List<GameObject>();
    List<GameObject> oneWayRooms = new List<GameObject>();

    public WallType wallType;


    private bool mapActive = false;
    public GameObject map;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < roomNumber; i++)
        {
            rooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>());
            Debug.Log(i);
            //改变point位置
            ChangePointPosition();

            map.SetActive(mapActive);
        }

        rooms[0].GetComponent<SpriteRenderer>().color = startColor;

        endRoom = rooms[0].gameObject;
        foreach (var room in rooms)
        {
            /*if (room.transform.position.sqrMagnitude>endRoom.transform.position.sqrMagnitude)
            {
                endRoom = room.gameObject;
            }*/

            SetupRoom(room, room.transform.position);
        }


        FindEndRoom();
        endRoom.GetComponent<SpriteRenderer>().color = endColor;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

        if (Input.GetKeyDown(KeyCode.M))
        {
            mapActive = !mapActive;
            
            map.SetActive(mapActive);
        }

    }

    public void ChangePointPosition()
    {
        do
        {
            direction = (Direction)Random.Range(0, 4);
            switch (direction)
            {
                case Direction.up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));
    }


    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.UpdateRoom(xOffset, yOffset);

        switch (newRoom.doorNum)
        {
            case 1:
                if (newRoom.roomUp)
                {
                    Instantiate(wallType.wallU, roomPosition, Quaternion.identity);                    
                }
                if (newRoom.roomDown)
                {
                    Instantiate(wallType.wallD, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomLeft)
                {
                    Instantiate(wallType.wallL, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomRight)
                {
                    Instantiate(wallType.wallR, roomPosition, Quaternion.identity);
                }
                break;
            case 2:
                if (newRoom.roomUp && newRoom.roomDown)
                {
                    Instantiate(wallType.wallUD, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomUp && newRoom.roomLeft)
                {
                    Instantiate(wallType.wallUL, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomUp && newRoom.roomRight)
                {
                    Instantiate(wallType.wallUR, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomDown && newRoom.roomLeft)
                {
                    Instantiate(wallType.wallDL, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomDown && newRoom.roomRight)
                {
                    Instantiate(wallType.wallDR, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallLR, roomPosition, Quaternion.identity);
                }
                break;
            case 3:
                if (newRoom.roomUp && newRoom.roomDown&&newRoom.roomLeft)
                {
                    Instantiate(wallType.wallUDL, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomUp && newRoom.roomDown && newRoom.roomRight)
                {
                    Instantiate(wallType.wallUDR, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomUp && newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallULR, roomPosition, Quaternion.identity);
                }
                if (newRoom.roomDown && newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallDLR, roomPosition, Quaternion.identity);
                }
                break;
            case 4:
                Instantiate(wallType.wallUDLR, roomPosition, Quaternion.identity);
                break;

        }
    }


    public void FindEndRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepToStart > maxStep)
            {
                maxStep = rooms[i].stepToStart;
            }
        }

        foreach (var room in rooms)
        {
            if (room.stepToStart == maxStep)
            {
                farRooms.Add(room.gameObject);
                //Debug.Log("farroom" + room.doorNum);
                if (room.doorNum == 1)
                {
                    oneWayRooms.Add(room.gameObject);
                    //Debug.Log(oneWayRooms.Count);
                }
            }
            if (room.stepToStart == (maxStep - 1))
            {
                lessFarRooms.Add(room.gameObject);
                //Debug.Log("lessfarroom" + room.doorNum);
                if (room.doorNum == 1)
                {
                    oneWayRooms.Add(room.gameObject);
                    //Debug.Log(oneWayRooms.Count);
                }
            }
        }

        //for (int i = 0; i < farRooms.Count; i++)
        //{
        //    if (farRooms[i].GetComponent<Room>().doorNum == 1)
        //    {
        //        oneWayRooms.Add(farRooms[i]);
        //        Debug.Log("第二套的最远加入");
        //    }
        //}

        //for (int i = 0; i < lessFarRooms.Count; i++)
        //{
        //    if (lessFarRooms[i].GetComponent<Room>().doorNum == 1)
        //    {
        //        oneWayRooms.Add(lessFarRooms[i]);
        //        Debug.Log("第二套的次远加入");
        //    }
        //}

        if (oneWayRooms.Count != 0)             //存在一个合适的单出入口房间
        {
            Debug.Log("one way rooms' number: " + oneWayRooms.Count);
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {                                          //没有合适的房间
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }

    }
}
