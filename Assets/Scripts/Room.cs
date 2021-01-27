using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{

    public GameObject doorUp, doorDown, doorLeft, doorRight;
    public bool roomUp, roomDown, roomLeft, roomRight;

    public int stepToStart;

    public Text text;

    public int doorNum;


    // Start is called before the first frame update
    void Start()
    {
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoom(float x,float y)
    {
        //float x = this.GetComponentInParent<RoomGenerator>().xOffset;
        //float y = this.GetComponentInParent<RoomGenerator>().yOffset;
        stepToStart = (int)(Mathf.Abs(transform.position.x / x) + Mathf.Abs(this.transform.position.y / y));
        text.text = stepToStart.ToString();

        if (roomUp) { doorNum++; }
        if (roomDown) { doorNum++; }
        if (roomLeft) { doorNum++; }
        if (roomRight) { doorNum++; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.instance.ChangeTarget(transform);
        }
    }




}
