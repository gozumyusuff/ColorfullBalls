using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private Vector3 startMousePos, startBallPos;
    private bool moveTheBall;
    [Range(0f, 1f)] public float maxSpeed;
    [Range(0f, 1f)] public float camSpeed;
    [Range(0f, 50f)] public float pathSpeed;
    private float velocity, camVelocity_x, camVelocity_y;
    public Transform path;
    public ParticleSystem CollideParticle;
    public ParticleSystem airEffect;

    private Camera mainCam;
    private Rigidbody rb;
    private Collider _collider;
    private Renderer BallRenderer;
    private Transform ball;
    public Transform ballMesh;
    public GameObject Shield;


    public Material[] Colors;

    public int currentScore = 0;
    public bool isTouchedBlue = false;
    public bool isTouchedGreen = false;

    private Vector3 initialScale;

    public int blueCount = 0;
    public int greenCount = 0;

    bool isShieldActive = false;
   

    void Start()
    {
        ball = transform;
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        BallRenderer = ballMesh.GetComponent<Renderer>();
        ColEnter();
        initialScale = transform.localScale;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && MenuManager.MenuManagerInstance.GameState)
        {
            moveTheBall = true;


            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray, out var distance))
            {
                startMousePos = ray.GetPoint(distance);
                startBallPos = ball.position;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }

        if (moveTheBall)
        {
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray, out var distance))
            {
                Vector3 mouseNewPos = ray.GetPoint(distance);
                Vector3 MouseNewPos = mouseNewPos - startMousePos;
                Vector3 DesireBallPos = MouseNewPos + startBallPos;

                DesireBallPos.x = Mathf.Clamp(DesireBallPos.x, -1.55f, 1.55f);
                ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x, DesireBallPos.x, ref velocity, maxSpeed), ball.position.y, ball.position.z);
            }
        }



        if (MenuManager.MenuManagerInstance.GameState)
        {
            var pathNewPos = path.position;
            path.position = new Vector3(pathNewPos.x, pathNewPos.y, Mathf.MoveTowards(pathNewPos.z, pathNewPos.z + 10, pathSpeed * Time.deltaTime));
        }




    }
    private void LateUpdate()
    {
        var cameraNewPos = mainCam.transform.position;
        if (rb.isKinematic)
            mainCam.transform.position = new Vector3(Mathf.SmoothDamp(cameraNewPos.x, ball.transform.position.x, ref camVelocity_x, camSpeed)
                , Mathf.SmoothDamp(cameraNewPos.y, ball.transform.position.y + 3f, ref camVelocity_y, camSpeed), cameraNewPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("obstacle"))
        {
            if (!isShieldActive)
            {
                gameObject.SetActive(false);
                MenuManager.MenuManagerInstance.GameState = false;
                MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
                MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose";
                if (PlayerPrefs.GetInt("score") > currentScore)
                {
                    return;
                }
                PlayerPrefs.SetInt("score", currentScore);

            }
        }       


        if (other.CompareTag("Ball"))
        {

            if (other.GetComponent<Renderer>().sharedMaterial == Colors[0])
            {
                Debug.Log("blue entered");
                //ballMesh.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
                isTouchedBlue = true;
                blueCount++;
                greenCount = 0;
                
               

                if (blueCount > 1)
                {
                    gameObject.SetActive(false);
                    MenuManager.MenuManagerInstance.GameState = false;
                    MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
                    MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose";
                }
            }
            else if (other.GetComponent<Renderer>().sharedMaterial == Colors[1])
            {
                Debug.Log("green entered");
                //ballMesh.localScale += new Vector3(0.3f, 0.3f, 0.3f);
                isTouchedGreen = true;
                greenCount++; 
                blueCount = 0;
                if (greenCount > 1)
                {
                    gameObject.SetActive(false);
                    MenuManager.MenuManagerInstance.GameState = false;
                    MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
                    MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose";
                }
            }

            BallRenderer.material = other.GetComponent<Renderer>().material;
            var NewParticle = Instantiate(CollideParticle, transform.position, Quaternion.identity);
            NewParticle.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;

            other.gameObject.SetActive(false);
            currentScore += 5;
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<TextMeshProUGUI>().text = "" + currentScore;

        }
        if (other.CompareTag("Shield"))
        {
            Debug.Log("shield entered");
            //ballMesh.localScale = initialScale;
            //blueCount = 0;
            //greenCount = 0;
            StartCoroutine(Countdown());
        }

    }

    IEnumerator Countdown()
    {
        isShieldActive = true;
        Shield.SetActive(true);
        yield return new WaitForSeconds(3);
        isShieldActive = false;
        Shield.SetActive(false);
    }




    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("path"))
        {
            Debug.Log("exit");
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f, 9f, 0f);
            pathSpeed = pathSpeed * 2;

            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed = 10f;

        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("path"))
        {
            ColEnter();
        }

    }

    void ColEnter()
    {
        Debug.Log("enter");
        rb.isKinematic = _collider.isTrigger = true;
        rb.velocity = new Vector3(0f, 0f, 0f);

        pathSpeed = 28f;

        var airEffectMain = airEffect.main;
        airEffectMain.simulationSpeed = 4f;
    }

}
