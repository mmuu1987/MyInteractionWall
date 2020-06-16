using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 组成立方体的运动类型
/// </summary>
public class CubeMotion : MotionInputMoveBase
{
   
    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();

        MotionType = MotionType.Cube;

        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);

        var sideCount = Mathf.FloorToInt(Mathf.Pow(ComputeBuffer.count, 1f / 3f));
        var count = sideCount * sideCount * sideCount;
        var dsideCount = sideCount * sideCount;

        var scale = 2.5f;

        var offset = -Vector3.one * 0.5f;
       // Debug.Log("init cubeInstance");
        //使图片组成立方体
        //顺便取出立方体靠近相机的那个面的四个角的坐标，拿出来，判断是否越出边界
        for (int x = 0; x < sideCount; x++)
        {
            var xoffset = x * dsideCount;
            for (int y = 0; y < sideCount; y++)
            {
                var yoffset = y * sideCount;
                for (int z = 0; z < sideCount; z++)
                {
                    var index = xoffset + yoffset + z;
                    Vector3 pos = new Vector3(x, y, z) * scale + new Vector3(-55f, -55f, -9f);
                    // var particle = new GPUParticle(Random.Range(0.5f, 1f), pos, Quaternion.identity, Vector3.one, Vector3.zero, Vector3.zero, Color.white);
                    datas[index].moveTarget = pos;
                    datas[index].moveDir = pos;//在这里的意义就是一个开关而已
                    datas[index].originalPos = pos;
                    datas[index].uvOffset = new Vector4(1f,1f,0f,0f);
                    datas[index].uv2Offset = new Vector4(1f, 1f, 0f, 0f);
                    datas[index].picIndex = index % TextureInstanced.Instance.TexArr.depth;
                    datas[index].bigIndex = index % TextureInstanced.Instance.TexArr.depth;
                    Vector4 posTemp = datas[index].position;
                    datas[index].position = new Vector4(posTemp.x, posTemp.y, posTemp.z, 1f);
                    if (index == 0)
                    {
                        LeftDownTip.position = pos;
                        _originalPosLeftDown = pos;
                    }



                    if (index == sideCount * (sideCount) - sideCount)
                    {
                        LeftUpTip.position = pos;
                        _originalPosLeftUp = pos;
                    }
                    if (index == sideCount * sideCount * (sideCount - 1))
                    {
                        RightDownTip.position = pos;
                        _originalPosRightDown = pos;

                    }
                    if (index == sideCount * sideCount * (sideCount) - sideCount)
                    {
                        RightUpTip.position = pos;
                        _originalPosRightUp = pos;

                    }

                }
            }
        }
        ComputeBuffer.SetData(datas);
        TextureInstanced.Instance.ChangeInstanceMat(null);
        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(1, 1, 1, 1));
       
    }
}



