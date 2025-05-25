#ifndef ERASER_BEAM_INCLUDED
#define ERASER_BEAM_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
    float3 _EraserOrigin;
    float3 _EraserDirection; // Should be normalized
    float  _EraserAngle;     // Radians
    float  _EraserRange;
    float  _EraserSmoothness; // Optional falloff softness (0 = hard edge)
CBUFFER_END

inline half ComputeEraserBeam(float3 worldPos)
{
    float3 toPix = worldPos - _EraserOrigin;
    float dist = length(toPix);
    float3 dir = toPix / max(dist, 0.00001);

    float angleCos = dot(dir, _EraserDirection);
    float angleThreshold = cos(_EraserAngle * 0.5);

    float inCone = saturate((angleCos - angleThreshold) / max(0.00001, _EraserSmoothness));
    float inRange = saturate((_EraserRange - dist) / max(0.00001, _EraserSmoothness));

    return saturate(inCone * inRange);
}

#endif // ERASER_BEAM_INCLUDED
