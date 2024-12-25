using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [SerializeField] public Transform floatingPartFolder;
    [SerializeField] float constructRange;

    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] float orthoSizeConstruct;
    [SerializeField] float orthoSizeNormal;

    List<Part> partsConstructingList = new List<Part>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void StartConstructMode()
    {
        foreach (Part part in floatingPartFolder.GetComponentsInChildren<Part>())
        {
            if ((part.transform.position - player.transform.position).magnitude < constructRange)
            {
                SetConstructingParts(part);
            }
        }

        vCamera.m_Lens.OrthographicSize = orthoSizeConstruct;
        player.StartConstructMode();
    }

    public void EndConstructMode()
    {
        foreach (Part part in partsConstructingList)
        {
            part.EndConstructMode();
        }
        partsConstructingList.Clear();

        vCamera.m_Lens.OrthographicSize = orthoSizeNormal;
        player.EndConstructMode();
    }

    public void SetConstructingParts(Part part)
    {
        partsConstructingList.Add(part);
        part.StartConstructMode();
    }
}
