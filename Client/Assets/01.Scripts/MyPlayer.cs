using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    void Start()
    {
        StartCoroutine(CoSendPacket());
    }

    Vector3 tar;

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 p = Input.mousePosition;
            Ray cast = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(cast, out hit))
            {
                tar = hit.point;
                Debug.Log(tar);
            }
        }

        base.Update();
    }

    IEnumerator CoSendPacket()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.25f);

            Move movePacket = new Move();
            movePacket.posX = tar.x;
            movePacket.posY = 0;
            movePacket.posZ = tar.z;

            NetworkManager.Instance.Send(movePacket.Write());
        }
    }
}
