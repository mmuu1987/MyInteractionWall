using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMShader2 : MonoBehaviour
{

    public Transform target;

    public ComputeShader comshader;

    public Material mat;

    public ComputeBuffer combuffer;

    private int number = 131072;//512 * 8 * 8 * 8;

    private int kernel;


	// Use this for initialization
	void Start () {
        combuffer = new ComputeBuffer(number,12);

	    kernel = comshader.FindKernel("CSMain");
	}
	
	// Update is called once per frame
	void Update () {
		comshader.SetFloat("x",target.position.x);

        comshader.SetFloat("y", target.position.y);

        comshader.SetFloat("z", target.position.z);
	}

    private void OnRenderObject()
    {
        comshader.SetBuffer(kernel,"Result",combuffer);

        comshader.Dispatch(kernel,25,25,25);

        mat.SetBuffer("points",combuffer);

        mat.SetPass(0);

        Graphics.DrawProcedural(MeshTopology.Points, number);


    }

    private void OnDestroy()
    {
        combuffer.Release();
    }
}
