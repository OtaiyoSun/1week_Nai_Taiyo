using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Part : MonoBehaviour
{
    public float mass;
    public bool isConstructMode;
    bool isPick;
    bool canConnect;


    void Awake()
    {
        if (GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = mass;
            rb.drag = 0.2f;
            rb.angularDrag = 0.1f;
            rb.gravityScale = 0;
            rb.isKinematic = false;

        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            myOnMouseDown();
        }

        if (isPick)
        {
            Picking();
            if (Input.GetMouseButtonUp(0))
            {
                if (canConnect)
                {
                    Connect();
                }
                else
                {
                    QuitPick();
                }
            }

            if (Input.GetMouseButton(1))
            {
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180 * Time.deltaTime);
            }
        }


    }

    void myOnMouseDown()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -10;

        // レイを飛ばす
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // レイキャストが自身に当たった場合
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (isConstructMode) isPick = true;
        }


    }

    void OnMouseDown()
    {
        //if (isConstructMode) isPick = true;
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPick)
        {
            if (collision.gameObject != gameObject && collision.gameObject.tag == "Part")
            {
                canConnect = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isPick)
        {
            if (collision.gameObject != gameObject && collision.gameObject.tag == "Part")
            {
                canConnect = false;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isPick)
        {
            if (collision.gameObject != gameObject && collision.gameObject.tag == "Part")
            {
                canConnect = true;
            }
        }
    }

    void Picking()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }

    public void StartConstructMode()
    {
        isConstructMode = true;
        GetComponent<Collider2D>().isTrigger = true;
        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }

    public void EndConstructMode()
    {
        isConstructMode = false;
        GetComponent<Collider2D>().isTrigger = false;
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().isKinematic = false;
    }

    void Connect()
    {
        Player player = GameManager.instance.player;
        player.AddPart(this);
        tag = "Part";
        //if (GetComponent<Rigidbody2D>()) Destroy(GetComponent<Rigidbody2D>()); //Rigidbody2Dを削除

        isPick = false;
    }

    void QuitPick()
    {

        transform.parent = GameManager.instance.floatingPartFolder;
        tag = "Untagged";

        if (!GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>(); //Rigidbody2Dを追加
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = 0.2f;
        rb.angularDrag = 0.1f;
        rb.gravityScale = 0;
        rb.isKinematic = true;


        isPick = false;
        canConnect = false;
    }

}
