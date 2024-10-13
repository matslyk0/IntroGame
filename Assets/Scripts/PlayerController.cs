using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerController : MonoBehaviour {

    GameObject[] PickUpsArray;
    GameObject closestPickup;
    
    public Vector2 moveValue;
    private Vector3 oldPosition;
    private Vector3 velocity;
    private List<GameObject> PickUps;

    public float speed;
    private float distance = 100;
    private float closestDistance = 100;
    private int count;
    private int numPickups = 5;
    private string debugMode = "Normal";

    private LineRenderer lineRenderer;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI playerPosition;
    public TextMeshProUGUI playerVelocity;
    public TextMeshProUGUI playerDistance;

    void Start() {
        PickUpsArray = GameObject.FindGameObjectsWithTag("PickUp");
        PickUps = PickUpsArray.ToList();
        count = 0;
        oldPosition = transform.position;
        winText.text = "";
        playerPosition.text = "0";
        playerVelocity.text = "0";
        playerDistance.text = "0";
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        SetCountText();
        SetStateText();

    }
    
    void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            switch (debugMode) {
                case "Normal":
                    debugMode = "Distance";
                    break;
                case "Distance":
                    debugMode = "Vision";
                    break;
                case "Vision":
                    debugMode = "Normal";
                    break;
            }
        }
        
        if (debugMode.Equals("Distance")) {
            velocity = ((transform.position - oldPosition) / Time.deltaTime);
            FindClosestPickup();
            oldPosition = transform.position;
        }

        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);

        SetStateText();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "PickUp") {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
            PickUps.Remove(other.gameObject);
        }
    }

    private void SetCountText() {
        scoreText.text = "Score: " + count.ToString();
        if(count >= numPickups) {
            winText.text = "You win!";
        }
    }

    private void SetStateText() {
        playerPosition.text = "Position: " + transform.position.ToString();
        playerVelocity.text = "Velocity: " 
                             + velocity.ToString()
                             + " As scalar: "
                             + velocity.magnitude.ToString("0.00")
                             + " Player Speed: "
                             + speed;
        
    }

    private void FindClosestPickup() {
        foreach (GameObject pickup in PickUps) {
            distance = Vector3.Distance(transform.position, pickup.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPickup = pickup;
            }
            pickup.GetComponent<Renderer>().material.color = Color.white;
        }
        
        closestPickup.GetComponent<Renderer>().material.color = Color.blue;
        playerDistance.text = "Distance to pickup: " + closestDistance.ToString("0.00");

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, closestPickup.transform.position);

        distance = 100;
        closestDistance = 100;

    }

}
