#ifndef _PARTICLE_COMPUTE_COMMON_INCLUDED_
#define _PARTICLE_COMPUTE_COMMON_INCLUDED_

#define THREAD_X 8
#define THREAD_Y 1
#define THREAD_Z 1

RWStructuredBuffer<PosAndDir> positionBuffer;
RWStructuredBuffer<float4> colorBuffer;
//边界检测buff
RWStructuredBuffer<float4> boundaryBuffer;

float x;

float y;

float z;

float RadianRatio =  57.29578;
   float GetCross(float2 p1, float2 p2, float2 p)
    {
        return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
    }
    //计算一个点是否在矩形里  2d
    bool ContainsQuadrangle(float2 leftDownP2, float2 leftUpP1, float2 rightDownP3, float2 rightUpP4, float2 p)
    {

        float value1 = GetCross(leftUpP1, leftDownP2, p);

        float value2 = GetCross(rightDownP3, rightUpP4, p);

        if (value1 * value2 < 0) return false;

        float value3 = GetCross(leftDownP2, rightDownP3, p);

        float value4 = GetCross(rightUpP4, leftUpP1, p);

        if (value3 * value4 < 0) return false;

        return true;
    }



#endif // _PARTICLE_COMPUTE_COMMON_INCLUDED_
