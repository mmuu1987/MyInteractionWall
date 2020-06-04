using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransformationUtilities : MonoBehaviour
{
    public static CameraTransformationUtilities Instance { get; private set; }
    protected CameraTransformationUtilities() { }

    private void Start()
    {
        Instance = this;
    }

    /// <param name="projectionMatrix">投影矩阵</param>
    /// <param name="viewMatrix">视点矩阵</param>
    /// <param name="projection">列表形式的投影矩阵</param>
    /// <param name="view">列表形式的视点矩阵</param>
    /// <param name="p">要投影的点的引用</param>
    /// <param name="pl">要投影的点引用列表</param>
    /// <param name="width">屏幕宽</param>
    /// <param name="height">屏幕高</param>
    /// <param name="rowBase">列表是否以行优先进行矩阵元素的存储</param>
    /// <param name="flipLeftToRight">水平翻转</param>
    /// <param name="flipUpsideDown">上下翻转</param>

    public void UnprojectScreenToWorld(
        Matrix4x4 projectionMatrix,
        Matrix4x4 viewMatrix,
        ref Vector3 p)
    {
        // 世界坐标->相机坐标， 相机坐标->屏幕坐标 分别需要 viewMatrix和projectionMatrix
        Matrix4x4 PV = projectionMatrix * viewMatrix;

        // 逆矩阵即为从屏幕到世界的坐标变换，需指定像素深度
        p = PV.inverse.MultiplyPoint(p);
    }

    public void UnprojectScreenToWorld(
        List<float> projection,
        List<float> view,
        ref Vector3 p,
        bool rowBase = true
    )
    {
        Matrix4x4 projectionMatrix = ConstructMatirx(projection, rowBase);
        Matrix4x4 viewMatrix = ConstructMatirx(view, rowBase);
        UnprojectScreenToWorld(projectionMatrix, viewMatrix, ref p);
    }

    public void UnprojectScreenToWorld(
        List<float> projection,
        List<float> view,
        ref Vector3 p,
        float width,
        float height,
        bool rowBase = true,
        bool flipLeftToRight = false,
        bool flipUpsideDown = false
        )
    {
        Matrix4x4 projectionMatrix = ConstructMatirx(projection, rowBase);
        Matrix4x4 viewMatrix = ConstructMatirx(view, rowBase);
        ScreenSpaceTransform(ref p, width, height, flipLeftToRight, flipUpsideDown);
        UnprojectScreenToWorld(projectionMatrix, viewMatrix, ref p);
    }

    public void UnProjectScreenToWorld(
        Matrix4x4 projectionMatrix,
        Matrix4x4 viewMatrix,
        ref List<Vector3> pl
        )
    {
        Matrix4x4 PV = projectionMatrix * viewMatrix;
        for (int i = 0; i < pl.Count; i++)
        {
            pl[i] = PV.inverse.MultiplyPoint(pl[i]);
        }
    }

    public void UnProjectScreenToWorld(
        List<float> projection,
        List<float> view,
        ref List<Vector3> pl,
        float width,
        float height,
        bool rowBase = true,
        bool flipLeftToRight = false,
        bool flipUpsideDown = false
        )
    {
        Matrix4x4 projectionMatrix = ConstructMatirx(projection, rowBase);
        Matrix4x4 viewMatrix = ConstructMatirx(view, rowBase);
        Matrix4x4 PV = projectionMatrix * viewMatrix;
        ScreenSpaceTransform(ref pl, width, height, flipLeftToRight, flipUpsideDown);
        for (int i = 0; i < pl.Count; i++)
        {
            pl[i] = PV.inverse.MultiplyPoint(pl[i]);
        }
    }

    /// <summary>
    /// 获得相机的世界坐标，遵从 Direct3D 标准的左手笛卡尔坐标系。
    /// </summary>
    /// <param name="viewMatrix">视点矩阵</param>
    /// <returns></returns>
    public Vector3 GetCameraPosition(Matrix4x4 viewMatrix)
    {
        Matrix4x4 invV = viewMatrix.inverse;
        return new Vector3(invV.m30, invV.m31, invV.m32);
    }

    /// <summary>
    /// 获得相机的世界坐标，遵从 Direct3D 标准的左手笛卡尔坐标系。
    /// </summary>
    /// <param name="view">视点矩阵元素列表</param>
    /// <param name="rowBase">列表是否以行优先进行矩阵元素的存储</param>
    /// <returns></returns>
    public Vector3 GetCameraPosition(List<float> view, bool rowBase = true)
    {
        Matrix4x4 viewMatrix = ConstructMatirx(view, rowBase);
        Matrix4x4 invV = viewMatrix.inverse;
        return new Vector3(invV.m30, invV.m31, invV.m32);
    }

    /// <summary>
    /// 获得相机的视线方向，遵从 Direct3D 标准的左手笛卡尔坐标系。
    /// </summary>
    /// <param name="viewMatrix">视点矩阵</param>
    /// <returns></returns>
    public Vector3 GetCameraViewDirection(Matrix4x4 viewMatrix)
    {
        Matrix4x4 invV = viewMatrix.inverse;
        return new Vector3(invV.m20, invV.m21, invV.m22);
    }

    /// <summary>
    /// 获得相机的视线方向，遵从 Direct3D 标准的左手笛卡尔坐标系。
    /// </summary>
    /// <param name="view">视点矩阵元素列表</param>
    /// <param name="rowBase">列表是否以行优先进行矩阵元素的存储</param>
    /// <returns></returns>
    public Vector3 GetCameraViewDirection(List<float> view, bool rowBase = true)
    {
        Matrix4x4 viewMatrix = ConstructMatirx(view, rowBase);
        Matrix4x4 invV = viewMatrix.inverse;
        return new Vector3(invV.m20, invV.m21, invV.m22);
    }

    /// <summary>
    /// 将屏幕点坐标转换成以屏幕中心为原点，
    /// 右和上分别为x,y正方向，
    /// -1到1的坐标
    /// </summary>
    /// <param name="screen">屏幕宽或高</param>
    /// <param name="p">像素坐标（指定宽则为横坐标，指定高则为纵坐标）</param>
    /// <param name="flip">是否翻转（Unity通常情况下会将纵坐标翻转）</param>
    /// <returns></returns>
    public void ScreenSpaceTransform(ref Vector3 p, float width, float height, bool flipLeftToRight = false, bool flipUpSideDown = false)
    {
        p.Set((p.x / width * 2 - 1) * (flipLeftToRight ? -1 : 1),
                    (p.y / height * 2 - 1) * (flipUpSideDown ? -1 : 1),
                    p.z);
    }

    public void ScreenSpaceTransform(ref List<Vector3> pl, float width, float height, bool flipLeftToRight = false, bool flipUpSideDown = false)
    {
        for (int i = 0; i < pl.Count; i++)
        {
            pl[i].Set((pl[i].x / width * 2 - 1) * (flipLeftToRight ? -1 : 1),
                    (pl[i].y / height * 2 - 1) * (flipUpSideDown ? -1 : 1),
                    pl[i].z);
        }
    }

    /// <summary>
    /// 从列表中构造Matirx4x4矩阵
    /// </summary>
    /// <param name="elements">矩阵元素列表</param>
    /// <param name="rowBase">列表是否以行优先进行矩阵元素的存储</param>
    /// <returns></returns>
    public Matrix4x4 ConstructMatirx(List<float> elements, bool rowBase = true)
    {
        if (elements.Count != 16) return Matrix4x4.identity;
        Matrix4x4 mat = new Matrix4x4();
        if (rowBase)
        {
            mat.m00 = elements[0];
            mat.m01 = elements[1];
            mat.m02 = elements[2];
            mat.m03 = elements[3];
            mat.m10 = elements[4];
            mat.m11 = elements[5];
            mat.m12 = elements[6];
            mat.m13 = elements[7];
            mat.m20 = elements[8];
            mat.m21 = elements[9];
            mat.m22 = elements[10];
            mat.m23 = elements[11];
            mat.m30 = elements[12];
            mat.m31 = elements[13];
            mat.m32 = elements[14];
            mat.m33 = elements[15];
        }
        else
        {
            mat.m00 = elements[0];
            mat.m10 = elements[1];
            mat.m20 = elements[2];
            mat.m30 = elements[3];
            mat.m01 = elements[4];
            mat.m11 = elements[5];
            mat.m21 = elements[6];
            mat.m31 = elements[7];
            mat.m02 = elements[8];
            mat.m12 = elements[9];
            mat.m22 = elements[10];
            mat.m32 = elements[11];
            mat.m03 = elements[12];
            mat.m11 = elements[13];
            mat.m23 = elements[14];
            mat.m33 = elements[15];
        }
        return mat;
    }
}