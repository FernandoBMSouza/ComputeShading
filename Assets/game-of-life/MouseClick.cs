using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    private void OnMouseDown() {
        if(this.GetComponent<MeshRenderer>().material.color == Color.yellow)
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        } 
        else
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }
    }
}
