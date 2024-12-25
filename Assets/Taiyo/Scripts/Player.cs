using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float frictionRatio;
    [SerializeField] float frictionAngularRatio;
    [SerializeField] float angleCorrectionForce;

    [SerializeField] Transform partFolder;


    Rigidbody2D rb;

    Vector2 inputVec;
    float deadZone = 0.1f;

    bool inputAction;
    bool inputActionDown;

    List<Part> partsList = new List<Part>();

    Vector2 velocityPrev;
    float angularVelocityPrev;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        SetParts();

        SetMass();
    }

    // Update is called once per frame
    void Update()
    {
        SetInput();


    }

    void FixedUpdate()
    {
        DoPower();
        CorrectDirection();
        DoHoldAction();
        DoPushAction();


        Friction();

        ResetInput();
    }

    void SetInput()
    {
        inputVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.Space))
        {
            if (!inputAction) inputActionDown = true;
            inputAction = true;
        }
        else
        {
            inputAction = false;
        }
    }

    void ResetInput()
    {
        inputActionDown = false;
    }

    public void SetParts()
    {
        partsList.Clear();
        //子オブジェクトのPartを取得
        foreach (Part part in partFolder.GetComponentsInChildren<Part>())
        {
            partsList.Add(part);

        }

        SetMass();
        SetAngleCorrectionForce();
    }

    void SetMass()
    {
        //各部品の質量を合計してプレイヤーの質量にする
        float mass = 0;
        Vector2 massCenter = Vector2.zero;
        foreach (Part part in partsList)
        {
            mass += part.mass;
            massCenter += (Vector2)part.transform.localPosition * part.mass;
        }
        rb.mass = mass;

        //重心を設定
        rb.centerOfMass = massCenter / mass;
    }

    void SetAngleCorrectionForce()
    {
        float force = 0;
        foreach (Part part in partsList)
        {
            if (part is Part_Power)
            {
                Part_Power part_Power = (Part_Power)part;
                force += part_Power.inputEffectRatio * 10f;
            }

        }
        angleCorrectionForce = force;
    }

    public void AddPart(Part part)
    {
        part.transform.SetParent(partFolder);
        SetParts();
        SetMass();
        SetAngleCorrectionForce();
    }

    public void StartConstructMode()
    {
        velocityPrev = rb.velocity;
        angularVelocityPrev = rb.angularVelocity;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (Part part in partFolder.GetComponentsInChildren<Part>())
        {
            GameManager.instance.SetConstructingParts(part);
        }
    }

    public void EndConstructMode()
    {
        foreach (Part part in partsList)
        {
            //部品のRigidbody2Dを削除
            if (part.GetComponent<Rigidbody2D>()) Destroy(part.GetComponent<Rigidbody2D>());
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = velocityPrev;
        rb.angularVelocity = angularVelocityPrev;
    }

    void Friction()
    {
        rb.velocity *= frictionRatio;
        rb.angularVelocity *= frictionAngularRatio;
    }

    //動力パーツの力を加える
    void DoPower()
    {
        if (inputVec.magnitude > deadZone) //入力がある場合
        {
            foreach (Part part in partsList)
            {
                //動力パーツなら力を加える
                if (part is Part_Power)
                {
                    Part_Power part_Power = (Part_Power)part;
                    part_Power.AddPower(rb, inputVec);
                }
            }
        }
    }

    //入力方向に船体の向きを補正する
    void CorrectDirection()
    {
        if (inputVec.magnitude > deadZone)
        {
            //入力方向
            float inputAngle = Mathf.Atan2(inputVec.y, inputVec.x) * Mathf.Rad2Deg;
            if (inputAngle < 0) inputAngle += 360; //0~360に変換

            //現在の船体の向き
            float angleNow = (transform.rotation.eulerAngles.z + 90f) % 360;

            float diff = inputAngle - angleNow;
            if (diff > 180)
            {
                diff -= 360;
            }
            else if (diff < -180)
            {
                diff += 360;
            }

            //if (Mathf.Abs(diff) < 30) diff = math.sign(diff) * 30; //補正する力は30~180の係数で変化
            if (Mathf.Abs(diff) < 10) diff = 0;

            rb.AddTorque(math.sign(diff) * rb.mass * angleCorrectionForce);
        }
    }

    void DoHoldAction()
    {
        if (inputAction)
        {
            foreach (Part part in partsList)
            {
                if (part is Part_Action)
                {
                    Part_Action part_Action = (Part_Action)part;
                    part_Action.HoldAction();
                }
            }
        }
    }

    void DoPushAction()
    {
        if (inputActionDown)
        {
            foreach (Part part in partsList)
            {
                if (part is Part_Action)
                {
                    Part_Action part_Action = (Part_Action)part;
                    part_Action.PushAction();
                }
            }
        }
    }

}
