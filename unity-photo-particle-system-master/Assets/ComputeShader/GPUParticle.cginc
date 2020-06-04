#ifndef _PARTICLE_INCLUDED_
#define _PARTICLE_INCLUDED_

  struct PosAndDir
{
        float4 position;
        float4 velocity;
		float3 initialVelocity;
        float4 originalPos;
		float3 moveTarget;
		float3 moveDir;
		float2 indexRC;
		int picIndex;
        int bigIndex;
        float4 uvOffset; 
	    float4 uv2Offset; 
};



#endif // _PARTICLE_INCLUDED_
